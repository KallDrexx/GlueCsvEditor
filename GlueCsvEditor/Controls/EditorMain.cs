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

namespace GlueCsvEditor.Controls
{
    public partial class EditorMain : UserControl
    {
        protected IGlueCommands _glueCommands;
        protected IGlueState _gluState;
        protected string _csvPath;
        protected int _currentColumnIndex = -1;
        protected bool _dataLoading;
        protected RuntimeCsvRepresentation _csv;
        protected char _delimiter;
        protected bool _currentlyEditing;

        public EditorMain(IGlueCommands glueCommands, IGlueState glueState, string csvPath, char delimiter)
        {
            _glueCommands = glueCommands;
            _gluState = glueState;
            _csvPath = csvPath;
            _delimiter = delimiter;

            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }

        public void NotifyOfCsvUpdate()
        {
            string message = "Warning: This CSV has been updated externally.  Do you want to reload?";
            var result = MessageBox.Show(message, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
                LoadCsv();
        }

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
            // Update the value of the specified record in the RCR
            if (e.RowIndex >= _csv.Records.Count || e.RowIndex < 0)
                throw new InvalidOperationException("Row index out of range");

            if (e.ColumnIndex >= _csv.Records[e.RowIndex].Length || e.ColumnIndex < 0)
                throw new InvalidOperationException("Column index out of range");

            _csv.Records[e.RowIndex][e.ColumnIndex] = e.Value as string;

            // If this was an update to a value in the first column, update the header cell text
            if (e.ColumnIndex == 0)
                dgrEditor.Rows[e.RowIndex].HeaderCell.Value = e.Value as string;

            SaveCsv();
            _currentlyEditing = false;
        }

        private void dgrEditor_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            _currentColumnIndex = e.ColumnIndex;
            _dataLoading = true;

            // Update the selected header
            txtHeaderName.Text = string.Empty;
            txtHeaderType.Text = string.Empty;

            var header = _csv.Headers[e.ColumnIndex];
            string type = CsvHeader.GetClassNameFromHeader(header.OriginalText) ?? "string";

            int typeDataIndex = header.Name.IndexOf("(");
            if (typeDataIndex < 0)
                typeDataIndex = header.Name.Length;

            // Strip out the List<and > values
            if (type.Contains("List<"))
            {
                chkIsList.Checked = true;
                type = type.Replace("List<", "");
                if (type.Contains(">"))
                    type = type.Remove(type.LastIndexOf(">"), 1);
            }
            else
            {
                chkIsList.Checked = false;
            }

            txtHeaderName.Text = header.Name.Substring(0, typeDataIndex);
            txtHeaderType.Text = type;
            chkIsRequired.Checked = header.IsRequired;

            _dataLoading = false;
        }

        private void dgrEditor_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            _currentlyEditing = true;
        }

        private void dgrEditor_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (_dataLoading)
                return;

            _csv.Records.Add(new string[dgrEditor.Columns.Count]);
            SaveCsv();
        }

        private void dgrEditor_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (_dataLoading)
                return;

            _csv.Records.RemoveAt(e.RowIndex);
            SaveCsv();
        }

        private void dgrEditor_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            if (_dataLoading)
                return;

            // Add this column to the RCR
            var headers = new List<CsvHeader>(_csv.Headers);
            headers.Insert(e.Column.Index, new CsvHeader { Name = string.Empty, OriginalText = string.Empty });
            _csv.Headers = headers.ToArray();

            // Add the column to all the records
            for (int x = 0; x < _csv.Records.Count; x++)
            {
                var values = new List<string>(_csv.Records[x]);
                values.Insert(e.Column.Index, string.Empty);
                _csv.Records[x] = values.ToArray();
            }

            // If the new column is the first column, blank out all the header cell values
            if (e.Column.Index == 0)
                foreach (DataGridViewRow row in dgrEditor.Rows)
                    row.HeaderCell.Value = string.Empty;

            SaveCsv();
        }

        private void dgrEditor_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            if (_dataLoading)
                return;

            // Remove this column to the RCR
            var headers = new List<CsvHeader>(_csv.Headers);
            headers.RemoveAt(e.Column.Index);
            _csv.Headers = headers.ToArray();

            // Remove the column to all the records
            for (int x = 0; x < _csv.Records.Count; x++)
            {
                var values = new List<string>(_csv.Records[x]);
                values.RemoveAt(e.Column.Index);
                _csv.Records[x] = values.ToArray();
            }

            // If the first column was removed, update the header cell values
            if (e.Column.Index == 0)
                if (_csv.Headers.Length > 0)
                    for (int x = 0; x < _csv.Records.Count; x++)
                        dgrEditor.Rows[x].HeaderCell.Value = _csv.Records[x][0];

            SaveCsv();
        }

        private void dgrEditor_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            // Make sure the row/column is valid
            if (e.RowIndex >= _csv.Records.Count)
                return;

            if (e.ColumnIndex >= _csv.Headers.Length)
                return;

            e.Value = _csv.Records[e.RowIndex][e.ColumnIndex];
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
        }

        private void btnAddColumn_Click(object sender, EventArgs e)
        {
            dgrEditor.Columns.Add(string.Empty, "");
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            string message = string.Format("Are you sure you want to remove the '{0}' column?",
                _csv.Headers[_currentColumnIndex].OriginalText);
            var result = MessageBox.Show(message, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result != DialogResult.Yes)
                return;

            // Delete the current column
            dgrEditor.Columns.RemoveAt(_currentColumnIndex);
        }

        protected void LoadCsv()
        {
            this.SuspendLayout();
            _dataLoading = true;

            // Serialize the csv
            CsvFileManager.Delimiter = _delimiter;
            _csv = CsvFileManager.CsvDeserializeToRuntime(_csvPath);
            _csv.RemoveHeaderWhitespaceAndDetermineIfRequired();

            // Add the CSV headers to the datagrid
            dgrEditor.Columns.Clear();
            for (int x = 0; x < _csv.Headers.Length; x++)
                dgrEditor.Columns.Add(_csv.Headers[x].Name, _csv.Headers[x].OriginalText);

            // Add the records
            dgrEditor.RowCount = _csv.Records.Count;

            // Add the first value of each record to the row text
            if (_csv.Headers.Length > 0)
                for (int x = 0; x < _csv.Records.Count; x++)
                    dgrEditor.Rows[x].HeaderCell.Value = _csv.Records[x][0];

            this.ResumeLayout();
            _dataLoading = false;
        }

        protected void SaveCsv()
        {
            CsvFileManager.Delimiter = _delimiter;
            try { CsvFileManager.Serialize(_csv, _csvPath); }
            catch (Exception ex)
            {
                string message = string.Format("Error saving CSV: {0} - {1}", ex.GetType(), ex.Message);
                MessageBox.Show(message, "Error Saving CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected void UpdateColumnDetails()
        {
            if (_dataLoading)
                return;

            if (dgrEditor.Columns.Count <= _currentColumnIndex || _currentColumnIndex < 0)
                return; // column is out of bounds

            // Form the new text value
            var text = new StringBuilder();
            text.Append(txtHeaderName.Text.Trim());
            text.Append(" (");

            if (chkIsList.Checked)
                text.Append("List<");

            text.Append(txtHeaderType.Text.Trim());

            if (chkIsList.Checked)
                text.Append(">");

            if (chkIsRequired.Checked)
                text.Append(", required");

            text.Append(")");

            // Update the header details
            var header = _csv.Headers[_currentColumnIndex];
            header.OriginalText = text.ToString();
            header.Name = text.ToString();
            header.IsRequired = chkIsRequired.Checked;

            dgrEditor.Columns[_currentColumnIndex].HeaderText = text.ToString();
            _csv.Headers[_currentColumnIndex] = header;

            // Save after every change
            SaveCsv();
        }
    }
}
