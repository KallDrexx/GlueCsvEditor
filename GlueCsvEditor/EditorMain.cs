﻿using System;
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
            // Serialize the csv
            var csv = CsvFileManager.CsvDeserializeToRuntime(_csvPath);

            // Add the CSV headers to the datagrid
            for (int x = 0; x < csv.Headers.Length; x++)
            {
                dgrEditor.Columns.Add(csv.Headers[x].Name, csv.Headers[x].Name);
                dgrEditor.Columns[x].Tag = csv.Headers[x];
            }

            // Add the records
            for (int x = 0; x < csv.Records.Count; x++)
            {
                dgrEditor.Rows.Add();
                for (int y = 0; y < csv.Records[x].Length; y++)
                {
                    dgrEditor.Rows[x].Cells[y].Value = csv.Records[x][y];
                }
            }
        }

        private void dgrEditor_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
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

            if (header.Name.Contains("("))
            {
                txtHeaderName.Text = header.Name.Substring(0, header.Name.IndexOf('('));

                if (header.Name.Contains(')'))
                {
                    int startIndex = header.Name.IndexOf('(');
                    int endIndex = header.Name.IndexOf(')');
                    txtHeaderType.Text = header.Name.Substring(startIndex + 1, endIndex - startIndex - 1);
                }
            }
            else
            {
                txtHeaderName.Text = header.Name;
            }

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

        protected void SaveCsv()
        {
            
        }

        protected void UpdateColumnDetails()
        {
            if (_dataLoading)
                return;

            if (dgrEditor.Columns.Count <= _currentColumnIndex || _currentColumnIndex < 0)
                return; // column is out of bounds

            var header = (CsvHeader)dgrEditor.Columns[_currentColumnIndex].Tag;
            header.Name = string.Concat(txtHeaderName.Text.Trim(), " (", txtHeaderType.Text.Trim(), ")");
            dgrEditor.Columns[_currentColumnIndex].Tag = header;

            // Update the column display
            dgrEditor.Columns[_currentColumnIndex].HeaderText = header.Name;
        }
    }
}
