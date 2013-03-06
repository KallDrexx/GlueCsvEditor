using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GlueCsvEditor.Data;
using System.Reflection;
using System.Threading;
using FlatRedBall.Glue.Parsing;

namespace GlueCsvEditor.Controls
{

    public enum LayoutState
    {
        OnlyDataGrid,
        ShowPropertyGrid,
        ShowMultiLineText,
        ExpandedShowNothing
    }

    public partial class GridView : UserControl
    {
        #region Member Variables

        private int _lastColumnIndex = -1;
        private int _currentColumnIndex;
        private int _currentRowIndex;
        private readonly CsvData _data;
        private IEnumerable<string> _knownTypes;
        private int _originalGridTop;
        private int _originalHeight;
        private bool _stringColumnSelected;
        private readonly List<Keys> _downArrowKeys;
        private readonly CachedTypes _cachedTypes;
        private LayoutState mCurrentLayoutStateButPleaseUseThePropertyInstead;
        #endregion

        #region Properties

        LayoutState CurrentLayoutState
        {
            get
            {
                return mCurrentLayoutStateButPleaseUseThePropertyInstead;
            }
            set
            {
                var oldCurrentState =
                    mCurrentLayoutStateButPleaseUseThePropertyInstead;


                mCurrentLayoutStateButPleaseUseThePropertyInstead = value;

                if (mCurrentLayoutStateButPleaseUseThePropertyInstead == LayoutState.OnlyDataGrid)
                {
                    // Hide the top
                    LeftSideSplitContainer.Panel1MinSize = 0;
                    LeftSideSplitContainer.IsSplitterFixed = true;
                    LeftSideSplitContainer.SplitterDistance = 0;

                }
                else
                {
                    // Show the top
                    LeftSideSplitContainer.Panel1MinSize = 30;
                    LeftSideSplitContainer.IsSplitterFixed = false;
                    if (oldCurrentState == LayoutState.OnlyDataGrid)
                    {
                        LeftSideSplitContainer.SplitterDistance = LeftSideSplitContainer.Height / 2;
                    }
                }


                // Show the right controls
                RefreshAlternativeEditControlVisibility();

                // Refresh the display
                switch (mCurrentLayoutStateButPleaseUseThePropertyInstead)
                {
                    case LayoutState.ShowMultiLineText:
                        RefreshMultiLineDisplay();
                        break;
                    case LayoutState.ShowPropertyGrid:
                        break;

                }
            }
        }




        private int DataLoadingCount { get; set; }

        private bool DataLoading
        {
            get
            {
                return DataLoadingCount != 0;
            }
        }

        public bool IgnoreNextFileChange { get; set; }

        #endregion

        #region Public Methods

        public GridView(CsvData data, CachedTypes cachedTypes)
        {
            DataLoadingCount = 0;
            _data = data;
            _cachedTypes = cachedTypes;
            _downArrowKeys = new List<Keys>();
            _knownTypes = new string[0];

            InitializeComponent();
        }

        public void ReloadCsvDisplay()
        {
            // Clear the right editor side
            txtHeaderName.Text = string.Empty;
            txtHeaderType.Text = string.Empty;
            chkIsList.Checked = false;
            chkIsList.Checked = false;

            SuspendLayout();
            DataLoadingCount++;

            // Reset the current column count so we are sure the CellEnter event
            //  so we can guarantee that the cell displays are updated
            _currentColumnIndex = -1;
            _currentRowIndex = -1;

            // Add the CSV headers to the datagrid
            var headers = _data.GetHeaderText();

            dgrEditor.Columns.Clear();
            foreach (var header in headers)
                dgrEditor.Columns.Add(header, header);

            // Add the records
            dgrEditor.RowCount = _data.GetRecordCount();

            RefreshRowHeaders();
            ResumeLayout();

            // Auto-focus on the first cell
            if (headers.Count > 0 && dgrEditor.Rows.Count > 0)
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

            DataLoadingCount--;
        }

        public void CachedTypesReady()
        {
            // Make sure this call is done in the control's thread
            Invoke((MethodInvoker)delegate
            {
                lstFilteredTypes.Enabled = true;
                cmbCelldata.Enabled = true;
                btnShowComplexProperties.Enabled = true;

                // Load all the known types
                _knownTypes = _cachedTypes.KnownTypes;
                FilterKnownTypes();
                UpdateCellDisplays(true);
            });
        }

        #endregion

        #region Form Events

        private void GridView_Load(object sender, EventArgs e)
        {
            Dock = DockStyle.Fill;
            SetDoubleBuffered(true);
            ReloadCsvDisplay();

            // Disable any controls that rely on the cached types
            btnShowComplexProperties.Enabled = false;
            lstFilteredTypes.Enabled = false;
            cmbCelldata.Enabled = false;

            CurrentLayoutState = LayoutState.OnlyDataGrid;
        }

        private void txtHeaderName_TextChanged(object sender, EventArgs e)
        {
            UpdateColumnDetails();
        }

        private void txtHeaderType_TextChanged(object sender, EventArgs e)
        {
            if (DataLoading)
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
            DataLoadingCount++;

            _data.UpdateValue(e.RowIndex, e.ColumnIndex, e.Value as string);
            cmbCelldata.Text = e.Value as string;
            SaveCsv();

            // If this was an update to a value in the first column, update the header cell text
            if (e.ColumnIndex == 0)
                dgrEditor.Rows[e.RowIndex].HeaderCell.Value = e.Value as string;

            // Since this is set in BeginEdit we need to end it here

            // Update the display
            UpdateCellDisplays(true);

            DataLoadingCount--;
        }

        private void dgrEditor_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            _lastColumnIndex = _currentColumnIndex;
            _currentColumnIndex = e.ColumnIndex;
            _currentRowIndex = e.RowIndex;

            // If no arrow keys are currently being pressed, update the cell displays
            if (_downArrowKeys.Count == 0)
                UpdateCellDisplays();
        }

        private void dgrEditor_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
        }

        private void dgrEditor_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            if (DataLoading)
                return;

            _data.AddColumn(e.Column.Index);
            SaveCsv();

            // If the new column is the first column, blank out all the header cell values
            if (e.Column.Index == 0)
                foreach (DataGridViewRow row in dgrEditor.Rows)
                    row.HeaderCell.Value = string.Empty;

            // Update the header text for the new column
            // Update the column header
            var header = _data.GetHeaderText()[e.Column.Index];
            dgrEditor.Columns[e.Column.Index].HeaderText = header;
        }

        private void dgrEditor_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            if (DataLoading)
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

        private void dgrEditor_KeyDown(object sender, KeyEventArgs e)
        {
            var arrowKeys = new[] { Keys.Down, Keys.Up, Keys.Left, Keys.Right };

            if (e.KeyCode == Keys.V && e.Control)
            {
                var data = Clipboard.GetData(DataFormats.Text).ToString();
                var cells = data.Split('\t');
                if (dgrEditor.CurrentRow != null)
                    for (var i = 0; i < cells.Length; i++)
                        dgrEditor[_currentColumnIndex + i, dgrEditor.CurrentRow.Index].Value = cells[i];
            }

            else if (e.KeyCode == Keys.X && e.Control)
            {
                if (dgrEditor.CurrentCell != null)
                {
                    Clipboard.SetText(dgrEditor.CurrentCell.Value.ToString());
                    dgrEditor.CurrentCell.Value = string.Empty;
                }
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
                // Clear the value of all selected cells
                var cells = dgrEditor.SelectedCells;
                for (int x = 0; x < cells.Count; x++)
                {
                    cells[x].Value = string.Empty;
                }

                e.Handled = true;
            }

            // If an arrow key is pushed, note it down
            else if (arrowKeys.Contains(e.KeyCode))
            {
                if (!_downArrowKeys.Contains(e.KeyCode))
                    _downArrowKeys.Add(e.KeyCode);
            }
        }

        private void dgrEditor_KeyUp(object sender, KeyEventArgs e)
        {
            var arrowKeys = new[] { Keys.Down, Keys.Up, Keys.Left, Keys.Right };
            if (arrowKeys.Contains(e.KeyCode))
            {
                if (_downArrowKeys.Contains(e.KeyCode))
                    _downArrowKeys.Remove(e.KeyCode);

                if (_downArrowKeys.Count == 0)
                    UpdateCellDisplays();
            }
        }

        private void dgrEditor_Leave(object sender, EventArgs e)
        {
            // Clear out all the known pressed keys
            _downArrowKeys.Clear();
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            _data.AddRow(_currentRowIndex + 1);
            SaveCsv();

            dgrEditor.RowCount = _data.GetRecordCount();
            RefreshRowHeaders();
            UpdateCellDisplays(true);
            dgrEditor.Invalidate();
        }

        private void btnPrependRow_Click(object sender, EventArgs e)
        {
            if (DataLoading)
                return;

            _data.AddRow(_currentRowIndex);
            SaveCsv();

            dgrEditor.RowCount = _data.GetRecordCount();
            RefreshRowHeaders();
            UpdateCellDisplays(true);
            dgrEditor.Invalidate();
        }

        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            string message;

            // Find some identifying information for the row to present in
            //   a confirmation box
            var headers = Enumerable.Range(1, _data.GetHeaderText().Count).Select(x => _data.GetHeaderDetails(x - 1)).ToList();
            var values = Enumerable.Range(1, headers.Count).Select(x => _data.GetValue(_currentRowIndex, x - 1)).ToList();

            // First check if all the values are empty
            if (values.All(string.IsNullOrWhiteSpace))
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
                dgrEditor.Invalidate();
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
            if (DataLoading)
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

            if (CurrentLayoutState != LayoutState.OnlyDataGrid)
            {
                CurrentLayoutState = LayoutState.OnlyDataGrid;
            }
            else
            {
                // If we currently have a string column selected, don't show the property grid
                //   instead show the textbox for text editing
                if (_stringColumnSelected)
                {
                    CurrentLayoutState = LayoutState.ShowMultiLineText;
                }
                else
                {
                    CurrentLayoutState = LayoutState.ShowPropertyGrid;
                    
                }
            }
        }

        private void txtMultilineEditor_Leave(object sender, EventArgs e)
        {
            txtMultilineEditor.Visible = false;
            ResetDataGridSizing();
        }

        private void txtMultilineEditor_TextChanged(object sender, EventArgs e)
        {
            if (DataLoading)
                return;

            _data.UpdateValue(_currentRowIndex, _currentColumnIndex, txtMultilineEditor.Text);
            SaveCsv();
        }

        private void cmbCelldata_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (_currentRowIndex < (dgrEditor.RowCount - 1))
                    dgrEditor.CurrentCell = dgrEditor[_currentColumnIndex, _currentRowIndex + 1];
            }
        }

        #endregion

        #region Internal Methods

        private bool IsComplexType(string typeAsString)
        {
            Type type = TypeManager.GetTypeFromString(typeAsString);

            if (type == null || type.IsPrimitive == false || type.IsEnum == false)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void SaveCsv()
        {
            IgnoreNextFileChange = true;
            _data.SaveCsv();
        }

        private void RefreshRowHeaders()
        {
            // Add the first value of each record to the row header text
            if (_data.GetHeaderText().Count > 0)
                for (int x = 0; x < _data.GetRecordCount(); x++)
                    dgrEditor.Rows[x].HeaderCell.Value = _data.GetValue(x, 0);
        }

        private void UpdateColumnDetails()
        {
            if (DataLoading)
                return;

            if (_currentColumnIndex != -1)
            {
                _data.SetHeader(_currentColumnIndex, txtHeaderName.Text, txtHeaderType.Text, chkIsRequired.Checked, chkIsList.Checked);
                SaveCsv();

                // Update the column header
                var header = _data.GetHeaderText()[_currentColumnIndex];
                dgrEditor.Columns[_currentColumnIndex].HeaderText = header;
            }
        }

        private void GoToNextSearchMatch(bool reverse = false)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                return;

            var cell = _data.FindNextValue(txtSearch.Text, _currentRowIndex, _currentColumnIndex, true, reverse);
            if (cell == null)
                return;

            dgrEditor.CurrentCell = dgrEditor[cell.ColumnIndex, cell.RowIndex];
        }

        private void SetupCellKnownValuesComboBox()
        {
            cmbCelldata.Items.Clear();
            if (_currentColumnIndex != -1)
            {
                var knownValues = _data.GetKnownValues(_currentColumnIndex);
                foreach (string value in knownValues.Where(x => !string.IsNullOrWhiteSpace(x)))
                    cmbCelldata.Items.Add(value);
            }
        }

        object mFilterKnownTypesLock = new object();
        private void FilterKnownTypes()
        {
            // This can't be in the ThreadPool because we can't access winforms stuff from other threads
            string trimmed = txtHeaderType.Text.Trim();
            ThreadPool.QueueUserWorkItem(stateObject =>
            {
                lock(mFilterKnownTypesLock)
                {

                    var dataSource = _knownTypes.Where(x => x.IndexOf(trimmed, StringComparison.OrdinalIgnoreCase) >= 0)
                               .ToList();
                
                    lstFilteredTypes.Invoke((MethodInvoker)(() =>
                        {
                            lstFilteredTypes.DataSource = dataSource;
                            lstFilteredTypes.ClearSelected();
                        }
                        
                     ));
                }
            });
        }

        private void UpdatePropertiesDisplay(string type, string value)
        {
            if (string.IsNullOrWhiteSpace(type))
                return;

            var ns = string.Empty;
            if (type.Contains(".") && type.Last() != '.')
            {
                ns = type.Remove(type.LastIndexOf('.'));
                type = type.Substring(type.LastIndexOf('.') + 1);
            }

            // Get property information for the type
            var knownProperties = _data.GetKnownProperties(_currentColumnIndex);
            var complexTypeProperties = knownProperties as ComplexTypeProperty[] ?? knownProperties.ToArray();
            var complexType = ComplexCsvTypeDetails.ParseValue(value);
            
            btnShowComplexProperties.Visible = true;

            if (!complexTypeProperties.Any() && complexType == null)
            {
                btnShowComplexProperties.Visible = false;
                return;
            }

            // If the complex type coudln't be parsed from the current value, create one manually
            if (complexType == null)
            {
                complexType = new ComplexCsvTypeDetails { Namespace = ns, TypeName = type };
            }

            complexType.DefaultType = type;

            // Go through all the properties and add any "known ones" that weren't part of the parsed set
            foreach (var prop in complexTypeProperties)
            {
                var tp = complexType.Properties.FirstOrDefault(x => x.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));

                if (tp == null)
                    complexType.Properties.Add(new ComplexTypeProperty { Name = prop.Name, Type = prop.Type });

                // Otherwise if the property exists then update the type
                else
                    tp.Type = prop.Type;
            }
            
            // Setup pgrid displayer
            // Note: While the displayer variable is never used, this cannot be deleted.
            //   The propertygrid property assignment triggers the property grid to show the complex properties
            //   correctly.  Without this code, the property grid will be empty
#pragma warning disable 168
            var displayer = new ComplexTypePropertyGridDisplayer(_data)
            {
                ComplexTypeUpdatedHandler = ComplexTypeUpdated,
                Instance = complexType,
                PropertyGrid = pgrPropertyEditor
            };
#pragma warning restore 168
        }

        private void ComplexTypeUpdated(string complexTypeString)
        {
            dgrEditor[_currentColumnIndex, _currentRowIndex].Value = complexTypeString;
        }

        private void UpdateCellDisplays(bool forceUpdate = false)
        {
            DataLoadingCount++;
            if (_currentColumnIndex != -1)
            {
                // Update the selected header information
                var header = _data.GetHeaderDetails(_currentColumnIndex);

                // Only update the header details if we changed columns
                if (_lastColumnIndex != _currentColumnIndex || forceUpdate)
                {
                    txtHeaderName.Text = header.Name;
                    txtHeaderType.Text = header.Type;
                    chkIsList.Checked = header.IsList;
                    chkIsRequired.Checked = header.IsRequired;
                    FilterKnownTypes();
                }

                // Setup the combobox
                var value = _data.GetValue(_currentRowIndex, _currentColumnIndex);
                cmbCelldata.Text = value;

                _stringColumnSelected = (header.Type == "string");

                bool isComplexType = IsComplexType(header.Type);

                if (!_stringColumnSelected &&
                    isComplexType == false)
                {
                    if (CurrentLayoutState != LayoutState.OnlyDataGrid)
                    {
                        CurrentLayoutState = LayoutState.ExpandedShowNothing;
                    }
                }
                else
                {
                    if (CurrentLayoutState != LayoutState.OnlyDataGrid)
                    {
                        if (_stringColumnSelected)
                        {
                            CurrentLayoutState = LayoutState.ShowMultiLineText;
                        }
                        else
                        {
                            CurrentLayoutState = LayoutState.ShowPropertyGrid;
                        }
                    }

                    if (!_stringColumnSelected)
                    {
                        UpdatePropertiesDisplay(header.Type, value);
                    }
                }
            }
            DataLoadingCount--;
        }
        private void RefreshAlternativeEditControlVisibility()
        {
            switch (mCurrentLayoutStateButPleaseUseThePropertyInstead)
            {
                case LayoutState.ShowMultiLineText:
                    pgrPropertyEditor.Visible = false;

                    txtMultilineEditor.Visible = true;
                    break;
                case LayoutState.ShowPropertyGrid:
                    pgrPropertyEditor.Visible = true;
                    txtMultilineEditor.Visible = false;

                    break;
                case LayoutState.ExpandedShowNothing:
                    pgrPropertyEditor.Visible = false;
                    txtMultilineEditor.Visible = false;

                    break;
            }
        }
        private void RefreshMultiLineDisplay()
        {
            DataLoadingCount++;
            txtMultilineEditor.Text = _data.GetValue(_currentRowIndex, _currentColumnIndex);
            DataLoadingCount--;
        }

        private void SetDoubleBuffered(bool setting)
        {
            Type dgvType = dgrEditor.GetType();
            var pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgrEditor, setting, null);
        }

        private void ResetDataGridSizing()
        {
            // Reset the property grid sizing
            dgrEditor.Top = _originalGridTop;
            dgrEditor.Height = _originalHeight;
        }

        #endregion
    }
}
