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

        public EditorMain(IGlueCommands glueCommands, IGlueState glueState, string csvPath)
        {
            _glueCommands = glueCommands;
            _gluState = glueState;
            _csvPath = csvPath;

            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }

        private void EditorMain_Load(object sender, EventArgs e)
        {
            _dataLoading = true;

            // Serialize the csv
            _csv = CsvFileManager.CsvDeserializeToRuntime(_csvPath);
            _csv.RemoveHeaderWhitespaceAndDetermineIfRequired();            

            // Add the CSV headers to the datagrid
            for (int x = 0; x < _csv.Headers.Length; x++)
            {
                int index = dgrEditor.Columns.Add(_csv.Headers[x].Name, _csv.Headers[x].OriginalText);
                dgrEditor.Columns[index].Tag = _csv.Headers[x];
            }

            // Add the records
            for (int x = 0; x < _csv.Records.Count; x++)
            {
                _dataLoading = true; // Required because CellEnter will set it to false
                int rowIndex = dgrEditor.Rows.Add();
                for (int y = 0; y < _csv.Records[x].Length; y++)
                    dgrEditor.Rows[rowIndex].Cells[y].Value = _csv.Records[x][y];
            }

            _dataLoading = false;
        }

        private void dgrEditor_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Update the value of the specified record in the RCR
            if (e.RowIndex >= _csv.Records.Count || e.RowIndex < 0)
                throw new InvalidOperationException("Row index out of range");

            if (e.ColumnIndex >= _csv.Records[e.RowIndex].Length || e.ColumnIndex < 0)
                throw new InvalidOperationException("Column index out of range");

            _csv.Records[e.RowIndex][e.ColumnIndex] = dgrEditor[e.ColumnIndex, e.RowIndex].Value as string;
            SaveCsv();
        }

        private void dgrEditor_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            _currentColumnIndex = e.ColumnIndex;
            _dataLoading = true;

            // Update the selected header
            txtHeaderName.Text = string.Empty;
            txtHeaderType.Text = string.Empty;

            var header = (CsvHeader)dgrEditor.Columns[e.ColumnIndex].Tag;
            string type = CsvHeader.GetClassNameFromHeader(header.Name) ?? "string";
            
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

        protected void SaveCsv()
        {
            CsvFileManager.Serialize(_csv, _csvPath);
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
            var header = (CsvHeader)dgrEditor.Columns[_currentColumnIndex].Tag;
            header.OriginalText = text.ToString();
            header.Name = text.ToString();
            header.IsRequired = chkIsRequired.Checked;

            dgrEditor.Columns[_currentColumnIndex].Tag = header;
            dgrEditor.Columns[_currentColumnIndex].HeaderText = text.ToString();
            _csv.Headers[_currentColumnIndex] = header;

            // Save after every change
            SaveCsv();
        }
    }
}
