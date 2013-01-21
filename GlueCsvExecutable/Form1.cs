using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GlueCsvEditor.Controls;

namespace GlueCsvExecutable
{
    public partial class Form1 : Form
    {
        EditorMain mEditorMain;

        public Form1()
        {
            InitializeComponent();

        }

        private void openCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "*.csv|*.csv";

            var result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ShowCsv(openFileDialog.FileName);
            }

        }

        private void ShowCsv(string fileName)
        {
            if (mEditorMain != null)
            {
                throw new NotImplementedException();
            }

            mEditorMain = new EditorMain(fileName, ',');
            this.Controls.Add(mEditorMain);
            mEditorMain.Dock = DockStyle.Fill;
            mEditorMain.BringToFront();
        }
    }
}
