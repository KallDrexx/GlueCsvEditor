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
            UpdateColumnDetails();
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
            cmbCelldata.Text = _data.GetValue(_currentRowIndex, _currentColumnIndex);

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

        #endregion
    }
}
