using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FlatRedBall.Glue.Plugins.ExportedInterfaces;
using System.IO;
using FlatRedBall.IO.Csv;
using FlatRedBall.Glue.Plugins;
using System.Diagnostics;
using GlueCsvEditor.Data;
using FlatRedBall.Glue.Elements;
using FlatRedBall.Glue.SaveClasses;
using FlatRedBall.Glue;
using FlatRedBall.Glue.Parsing;
using Microsoft.Build.BuildEngine;

namespace GlueCsvEditor.Controls
{
    public partial class EditorMain : UserControl
    {
        #region Member Variables

        protected IGlueCommands _glueCommands;
        protected IGlueState _gluState;
        protected int _currentColumnIndex = 0;
        protected int _currentRowIndex = 0;
        protected bool _dataLoading;
        protected bool _currentlyEditing;
        protected bool _ignoreNextFileChange;
        protected CsvData _data;
        protected IEnumerable<string> _knownTypes;
        protected int _originalGridTop;
        protected int _originalHeight;

        #endregion

        #region Public Methods

        public EditorMain(IGlueCommands glueCommands, IGlueState glueState, string csvPath, char delimiter)
        {
            _glueCommands = glueCommands;
            _gluState = glueState;

            InitializeComponent();
            this.Dock = DockStyle.Fill;

            // Load all the data
            _data = new CsvData(csvPath, delimiter);
        }

        public void NotifyOfCsvUpdate()
        {
            if (_ignoreNextFileChange)
                _ignoreNextFileChange = false;
            else
                LoadCsv();
        }

        #endregion

        #region Form Events

        private void EditorMain_Load(object sender, EventArgs e)
        {
            LoadCsv();
        }

        private void txtHeaderName_TextChanged(object sender, EventArgs e)
        {
            UpdateColumnDetails();
        }

        private void txtHeaderType_TextChanged(object sender, EventArgs e)
        {
            if (_dataLoading)
                return;

            UpdateColumnDetails();
            SetupCellKnownValuesComboBox();
            FilterKnownTypes();
        }

        private void chkIsRequired_CheckedChanged(object sender, EventArgs e)
        {
            UpdateColumnDetails();
        }

        private void chkIsList_CheckedChanged(object sender, EventArgs e)
        {
            UpdateColumnDetails();
        }

        private void dgrEditor_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            _dataLoading = true;

            _data.UpdateValue(e.RowIndex, e.ColumnIndex, e.Value as string);
            cmbCelldata.Text = e.Value as string;
            SaveCsv();

            // If this was an update to a value in the first column, update the header cell text
            if (e.ColumnIndex == 0)
                dgrEditor.Rows[e.RowIndex].HeaderCell.Value = e.Value as string;

            // Since this is set in BeginEdit we need to end it here
            _currentlyEditing = false;

            _dataLoading = false;
        }

        private void dgrEditor_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            _currentColumnIndex = e.ColumnIndex;
            _currentRowIndex = e.RowIndex;

            _dataLoading = true;

            // Update the selected header
            var header = _data.GetHeaderDetails(_currentColumnIndex);
            txtHeaderName.Text = header.Name;
            txtHeaderType.Text = header.Type;
            chkIsList.Checked = header.IsList;
            chkIsRequired.Checked = header.IsRequired;

            // Setup the combobox
            string value = _data.GetValue(_currentRowIndex, _currentColumnIndex);
            cmbCelldata.Text = value;
            FilterKnownTypes();

            UpdatePropertiesDisplay(header.Type, value);

            _dataLoading = false;
        }

        private void dgrEditor_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            _currentlyEditing = true;
        }

        private void dgrEditor_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            if (_dataLoading)
                return;

            // If the new column is the first column, blank out all the header cell values
            if (e.Column.Index == 0)
                foreach (DataGridViewRow row in dgrEditor.Rows)
                    row.HeaderCell.Value = string.Empty;

            _data.AddColumn(e.Column.Index);
            SaveCsv();
        }

        private void dgrEditor_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            if (_dataLoading)
                return;

            _data.RemoveColumn(e.Column.Index);
            SaveCsv();

            // If the first column was removed, update the header cell values
            if (e.Column.Index == 0)
                if (_data.GetHeaderText().Count > 0)
                    for (int x = 0; x < _data.GetRecordCount(); x++)
                        dgrEditor.Rows[x].HeaderCell.Value = _data.GetValue(x, 0);
        }

        private void dgrEditor_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            e.Value = _data.GetValue(e.RowIndex, e.ColumnIndex);
        }

        private void dgrEditor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.V && e.Control)
            {
                string data = Clipboard.GetData(DataFormats.Text).ToString();
                string[] cells = data.Split('\t');
                for (int i = 0; i < cells.Length; i++)
                    dgrEditor[_currentColumnIndex + i, dgrEditor.CurrentRow.Index].Value = cells[i];
            }
            else if (e.KeyCode == Keys.F3)
            {
                if (e.Shift)
                    GoToNextSearchMatch(true);
                else
                    GoToNextSearchMatch();

                e.Handled = true;
            }

            else if (e.KeyCode == Keys.Delete)
            {
                // Clear the whole cell's value
                dgrEditor[_currentColumnIndex, _currentRowIndex].Value = string.Empty;
                e.Handled = true;
            }
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            if (_dataLoading)
                return;

            _data.AddRow(_currentRowIndex + 1);
            SaveCsv();

            dgrEditor.RowCount = _data.GetRecordCount();
            RefreshRowHeaders();
        }

        private void btnPrependRow_Click(object sender, EventArgs e)
        {
            if (_dataLoading)
                return;

            _data.AddRow(_currentRowIndex);
            SaveCsv();

            dgrEditor.RowCount = _data.GetRecordCount();
            RefreshRowHeaders();
        }

        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            string message;

            // Find some identifying information for the row to present in
            //   a confirmation box
            var headers = Enumerable.Range(1, _data.GetHeaderText().Count).Select(x => _data.GetHeaderDetails(x - 1)).ToList();
            var values = Enumerable.Range(1, headers.Count).Select(x => _data.GetValue(_currentRowIndex, x - 1)).ToList();

            // First check if all the values are empty
            if (values.All(x => string.IsNullOrWhiteSpace(x)))
            {
                message = string.Format("Delete row #{0} (empty row)?", _currentRowIndex);
            }
            else if (headers.Any(x => x.IsRequired))
            {
                var requiredHeader = headers.FirstOrDefault(x => x.IsRequired);
                message = string.Format("Delete row #{0} ({1})?", _currentRowIndex, values[headers.IndexOf(requiredHeader)]);
            }
            else
            {
                // Get the first non-empty value
                var val = values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                message = string.Format("Delete row #{0} ({1})?", _currentRowIndex, val);
            }

            var result = MessageBox.Show(message, "Confirm Row Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                _data.RemoveRow(_currentRowIndex);
                SaveCsv();

                dgrEditor.RowCount = _data.GetRecordCount();
                RefreshRowHeaders();
            }
        }

        private void btnAddColumn_Click(object sender, EventArgs e)
        {
            dgrEditor.Columns.Add(string.Empty, "");
            txtHeaderName.Enabled = true;
            txtHeaderType.Enabled = true;
            chkIsList.Enabled = true;
            chkIsRequired.Enabled = true;
            btnRemove.Enabled = true;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            string message = string.Format("Are you sure you want to remove the '{0}' column?", _data.GetHeaderText()[_currentColumnIndex]);
            var result = MessageBox.Show(message, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result != DialogResult.Yes)
                return;

            // Delete the current column
            dgrEditor.Columns.RemoveAt(_currentColumnIndex);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                return;

            var cell = _data.FindNextValue(txtSearch.Text, _currentRowIndex, _currentColumnIndex);
            if (cell == null)
                return;

            dgrEditor.CurrentCell = dgrEditor[cell.ColumnIndex, cell.RowIndex];
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
            {
                GoToNextSearchMatch();
                e.Handled = true;
            }

            else if (e.KeyCode == Keys.Up)
            {
                GoToNextSearchMatch(true);
                e.Handled = true;
            }

            else if (e.KeyCode == Keys.F3)
            {
                if (e.Shift)
                    GoToNextSearchMatch(true);
                else
                    GoToNextSearchMatch();

                e.Handled = true;
            }
        }

        private void btnFindNext_Click(object sender, EventArgs e)
        {
            GoToNextSearchMatch();
        }

        private void cmbCelldata_TextChanged(object sender, EventArgs e)
        {
            if (_dataLoading)
                return;

            _data.UpdateValue(_currentRowIndex, _currentColumnIndex, cmbCelldata.Text);
            SaveCsv();

            // Update the value in the datagrid
            dgrEditor.InvalidateCell(_currentColumnIndex, _currentRowIndex);
        }

        private void cmbCelldata_DropDown(object sender, EventArgs e)
        {
            SetupCellKnownValuesComboBox();
        }

        private void lstFilteredTypes_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstFilteredTypes.SelectedIndex != -1)
            {
                var rect = lstFilteredTypes.GetItemRectangle(lstFilteredTypes.SelectedIndex);
                if (rect.Contains(e.Location))
                {
                    txtHeaderType.Text = lstFilteredTypes.SelectedValue as string;
                }
            }
        }

        private void btnShowComplexProperties_Click(object sender, EventArgs e)
        {
            pgrPropertyEditor.Visible = !pgrPropertyEditor.Visible;
            if (pgrPropertyEditor.Visible)
            {
                // Resize the data grid
                _originalGridTop = dgrEditor.Top;
                _originalHeight = dgrEditor.Height;
                dgrEditor.Height = dgrEditor.Bottom - pgrPropertyEditor.Bottom;
                dgrEditor.Top = pgrPropertyEditor.Bottom;
                pgrPropertyEditor.Focus();
            }
        }

        private void pgrPropertyEditor_Leave(object sender, EventArgs e)
        {
            pgrPropertyEditor.Visible = false;

            // Reset the property grid sizing
            dgrEditor.Top = _originalGridTop;
            dgrEditor.Height = _originalHeight;
        }

        #endregion

        #region Internal Methods

        protected void LoadCsv()
        {
            // Clear the right editor side
            txtHeaderName.Text = string.Empty;
            txtHeaderType.Text = string.Empty;
            chkIsList.Checked = false;
            chkIsList.Checked = false;

            this.SuspendLayout();
            _dataLoading = true;

            // Reload the CSV
            _data.Reload();

            // Add the CSV headers to the datagrid
            var headers = _data.GetHeaderText();

            dgrEditor.Columns.Clear();
            for (int x = 0; x < headers.Count; x++)
            {
                //var column = new DataGridViewComboBoxColumn();
                //column.HeaderText = headers[x];
                //column.Name = headers[x];
                //dgrEditor.Columns.Add(column);
                dgrEditor.Columns.Add(headers[x], headers[x]);
            }

            // Add the records
            dgrEditor.RowCount = _data.GetRecordCount();

            RefreshRowHeaders();
            this.ResumeLayout();

            // Load all the known types
            _knownTypes = GetKnownTypes();

            // Auto-focus on the first cell
            if (headers.Count > 0)
            {
                txtHeaderName.Enabled = true;
                txtHeaderType.Enabled = true;
                chkIsList.Enabled = true;
                chkIsRequired.Enabled = true;
                btnRemove.Enabled = true;
                dgrEditor.CurrentCell = dgrEditor[0, 0];
            }
            else
            {
                // Since we have no headers, disable all the right side controls
                txtHeaderName.Enabled = false;
                txtHeaderType.Enabled = false;
                chkIsList.Enabled = false;
                chkIsRequired.Enabled = false;
                btnRemove.Enabled = false;
            }

            _dataLoading = false;
        }

        protected void SaveCsv()
        {
            _ignoreNextFileChange = true;
            _data.SaveCsv();
        }

        protected void RefreshRowHeaders()
        {
            // Add the first value of each record to the row header text
            if (_data.GetHeaderText().Count > 0)
                for (int x = 0; x < _data.GetRecordCount(); x++)
                    dgrEditor.Rows[x].HeaderCell.Value = _data.GetValue(x, 0);
        }

        protected void UpdateColumnDetails()
        {
            if (_dataLoading)
                return;

            _data.SetHeader(_currentColumnIndex, txtHeaderName.Text, txtHeaderType.Text, chkIsRequired.Checked, chkIsList.Checked);
            SaveCsv();

            // Update the column header
            var header = _data.GetHeaderText()[_currentColumnIndex];
            dgrEditor.Columns[_currentColumnIndex].HeaderText = header;
        }

        protected void GoToNextSearchMatch(bool reverse = false)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                return;

            var cell = _data.FindNextValue(txtSearch.Text, _currentRowIndex, _currentColumnIndex, true, reverse);
            if (cell == null)
                return;

            dgrEditor.CurrentCell = dgrEditor[cell.ColumnIndex, cell.RowIndex];
        }

        protected string[] GetKnownTypes()
        {
            // Instantiate the types list with basic value types
            var types = new List<string>()
            {
                "bool", "double", "float", "int", "Matrix", "string", "Texture2D", "Vector2", "Vector3",
                "Vector4", "Color"
            };

            // Get all enumerations via reflection
            var enums = AppDomain.CurrentDomain
                                 .GetAssemblies()
                                 .SelectMany(x => x.GetTypes())
                                 .Where(x => x.IsEnum)
                                 .Select(x => x.FullName)
                                 .ToArray();

            types.AddRange(enums);

            // Add all FRB states
            var entityStates = ObjectFinder.Self.GlueProject.Entities.SelectMany(x => GetGlueStateNamespaces(x)).ToList();
            var screenStates = ObjectFinder.Self.GlueProject.Screens.SelectMany(x => GetGlueStateNamespaces(x)).ToList();
            types.AddRange(entityStates);
            types.AddRange(screenStates);

            // Get project types
            var projectTypes = GetProjectTypes();
            types.AddRange(projectTypes);

            return types.Distinct().OrderBy(x => x).ToArray();
        }

        protected IEnumerable<string> GetGlueStateNamespaces(EntitySave entity)
        {
            string ns = string.Concat(ProjectManager.ProjectNamespace, 
                                      ".",
                                      entity.Name.Replace("\\", "."),
                                      ".");

            var states = new List<string>() { ns + "VariableState" };
            states.AddRange(entity.StateCategoryList
                                  .Where(x => !x.SharesVariablesWithOtherCategories)
                                  .Select(x => ns + x.Name)
                                  .ToArray());

            return states;
        }

        protected IEnumerable<string> GetGlueStateNamespaces(ScreenSave entity)
        {
            string ns = string.Concat(ProjectManager.ProjectNamespace,
                                      ".",
                                      entity.Name.Replace("\\", "."),
                                      ".");

            var states = new List<string>() { ns + "VariableState" };
            states.AddRange(entity.StateCategoryList
                                  .Where(x => !x.SharesVariablesWithOtherCategories)
                                  .Select(x => ns + x.Name)
                                  .ToArray());

            return states;
        }

        protected IEnumerable<string> GetProjectTypes()
        {
            var results = new List<string>();
            var items = ProjectManager.ProjectBase.Where(x => x.Name == "Compile");
            string baseDirectory = ProjectManager.ProjectBase.Directory;

            foreach (var item in items)
            {
                var file = new ParsedFile(baseDirectory + item.Include);
                foreach (var ns in file.Namespaces)
                {
                    // Add all the classes in the namespace
                    foreach (var cls in ns.Classes)
                        results.Add(string.Concat(cls.Namespace, ".", cls.Name));

                    // Add enums
                    foreach (var enm in ns.Enums)
                        results.Add(string.Concat(enm.Namespace, ".", enm.Name));
                }
            }

            return results;
        }

        protected void SetupCellKnownValuesComboBox()
        {
            cmbCelldata.Items.Clear();
            var knownValues = _data.GetKnownValues(_currentColumnIndex);
            foreach (string value in knownValues.Where(x => !string.IsNullOrWhiteSpace(x)))
                cmbCelldata.Items.Add(value);
        }

        protected void FilterKnownTypes()
        {
            lstFilteredTypes.DataSource =
                _knownTypes.Where(x => x.IndexOf(txtHeaderType.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0)
                           .ToList();

            lstFilteredTypes.ClearSelected();
        }

        protected void UpdatePropertiesDisplay(string type, string value)
        {
            if (string.IsNullOrWhiteSpace(type))
                return;

            string ns = string.Empty;
            if (type.Contains(".") && type.Last() != '.')
            {
                ns = type.Remove(type.LastIndexOf('.'));
                type = type.Substring(type.LastIndexOf('.') + 1);
            }

            // Get property information for the type
            var knownProperties = _data.GetKnownProperties(_currentColumnIndex);
            var complexType = ComplexTypeDetails.ParseValue(value);
            btnShowComplexProperties.Visible = true;

            if (knownProperties.Count() == 0 && complexType == null)
            {
                btnShowComplexProperties.Visible = false;
                return;
            }

            // If the complex type coudln't be parsed from the current value, create one manually
            if (complexType == null)
                complexType = new ComplexTypeDetails { Namespace = ns, TypeName = type };

            // Go through all the properties and add any "known ones" that weren't part of the parsed set
            foreach (var prop in knownProperties)
            {
                var tp = complexType.Properties
                                    .Where(x => x.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase))
                                    .FirstOrDefault();

                if (tp == null)
                    complexType.Properties.Add(new ComplexTypeProperty { Name = prop.Name, Type = prop.Type });

                // Otherwise if the property exists then update the type
                else
                    tp.Type = prop.Type;
            }

            // Setup pgrid displayer
            var displayer = new ComplexTypePropertyGridDisplayer(_data);
            displayer.ComplexTypeUpdatedHandler = ComplexTypeUpdated;
            displayer.Instance = complexType;
            displayer.PropertyGrid = pgrPropertyEditor;
        }

        protected void ComplexTypeUpdated(string complexTypeString)
        {
            dgrEditor[_currentColumnIndex, _currentRowIndex].Value = complexTypeString;
        }

        #endregion
    }
}
