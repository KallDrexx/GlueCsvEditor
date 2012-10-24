using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GlueCsvEditor.Data;

namespace GlueCsvEditor.Controls
{
    public partial class EntityDesigner : UserControl
    {
        protected CsvData _csvData;
        protected bool _dataLoading;

        public EntityDesigner(CsvData csvData)
        {
            _csvData = csvData;

            InitializeComponent();
        }

        public void Reload()
        {
            _dataLoading = true;
            lstRecords.Enabled = false;

            if (_csvData.RecordIdentifiers == null)
            {
                MessageBox.Show("Entity designer can only be used when 1 (and only 1) column is marked as required", "Error");
                return;
            }

            lstRecords.Enabled = true;
            lstRecords.DataSource = _csvData.RecordIdentifiers.OrderBy(x => x.Key).ToList();
            lstRecords.DisplayMember = "Key";
            lstRecords.ValueMember = "Value";
            _dataLoading = false;
        }

        private void EntityDesigner_Load(object sender, EventArgs e)
        {
            this.Dock = DockStyle.Fill;
        }

        private void lstRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_dataLoading)
                return;

            if (lstRecords.SelectedIndex >= 0)
                MessageBox.Show("Record #" + lstRecords.SelectedValue);
        }

    }
}
