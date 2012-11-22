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
using System.Reflection;

namespace GlueCsvEditor.Controls
{
    public partial class EditorMain : UserControl
    {
        protected CsvData _csvData;
        protected GridView _gridView;
        protected CachedTypes _cachedTypes;

        public EditorMain(IGlueCommands glueCommands, IGlueState glueState, string csvPath, char delimiter)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;

            // Load all the data
            _cachedTypes = new CachedTypes(CachedTypesReadyHandler);
            _csvData = new CsvData(csvPath, _cachedTypes, delimiter);
        }

        public void NotifyOfCsvUpdate()
        {
            if (_gridView.IgnoreNextFileChange)
                _gridView.IgnoreNextFileChange = false;
            else
                ReloadCsv();
        }

        private void EditorMain_Load(object sender, EventArgs e)
        {

            _gridView = new GridView(_csvData, _cachedTypes);
            Controls.Add(_gridView);

            // Victor Chelaru November 22, 2012
            // I don't think we need this because
            // it causes a double-load of the CSV.
            // This makes the CSV plugin a little slower
            // and makes debugging more difficult.
            //ReloadCsv();
        }

        protected void ReloadCsv()
        {
            lock (this)
            {
                _csvData.Reload();
                _gridView.ReloadCsvDisplay();
            }
        }

        protected void CachedTypesReadyHandler()
        {
            _gridView.CachedTypesReady();
        }
    }
}
