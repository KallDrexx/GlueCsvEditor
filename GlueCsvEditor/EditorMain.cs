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

        public EditorMain(IGlueCommands glueCommands, IGlueState glueState, string csvPath)
        {
            _glueCommands = glueCommands;
            _gluState = glueState;
            _csvPath = csvPath;

            InitializeComponent();
        }

        private void EditorMain_Load(object sender, EventArgs e)
        {
            // Serialize the csv
            var csv = CsvFileManager.CsvDeserializeToRuntime(_csvPath);

            // Add the CSV headers to the datagrid
            for (int x = 0; x < csv.Headers.Length; x++)
            {
                dgrEditor.Columns.Add(csv.Headers[x].Name, string.Concat(csv.Headers[x].Name, " (", csv.Headers[x].MemberTypes.ToString(), ")"));
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
    }
}
