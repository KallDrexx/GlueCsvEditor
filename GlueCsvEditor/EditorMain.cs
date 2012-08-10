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
        protected RuntimeCsvRepresentation _csv;

        public EditorMain(IGlueCommands glueCommands, IGlueState glueState, RuntimeCsvRepresentation csv)
        {
            _glueCommands = glueCommands;
            _gluState = glueState;
            _csv = csv;

            InitializeComponent();
        }

        private void EditorMain_Load(object sender, EventArgs e)
        {
            // Add the CSV headers to the datagrid
            for (int x = 0; x < _csv.Headers.Length; x++)
                dgrEditor.Columns.Add(_csv.Headers[x].Name, string.Concat(_csv.Headers[x].Name, " (", _csv.Headers[x].MemberTypes.ToString(), ")"));

            // Add the records
            for (int x = 0; x < _csv.Records.Count; x++)
            {
                dgrEditor.Rows.Add();
                for (int y = 0; y < _csv.Records[x].Length; y++)
                    dgrEditor.Rows[x].Cells[y].Value = _csv.Records[x][y];
            }
        }
    }
}
