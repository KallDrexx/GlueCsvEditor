using System;
using System.Windows.Forms;
using GlueCsvEditor.Data;

namespace GlueCsvEditor.Controls
{
    public partial class EditorMain : UserControl
    {
        private CsvData _csvData;
        private GridView _gridView;
        private CachedTypes _cachedTypes;

        public EditorMain(string csvPath, char delimiter)
        {
            InitializeComponent();

            // Load all the data
            
            LoadCsv(csvPath, delimiter);
        }

        private void LoadCsv(string csvPath, char delimiter)
        {
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

        public void SaveEditorSettings()
        {
            _gridView.SaveEditorSettings();
        }

        private void EditorMain_Load(object sender, EventArgs e)
        {
            Dock = DockStyle.Fill;

            _gridView = new GridView(_csvData, _cachedTypes);
            Controls.Add(_gridView);

            // Victor Chelaru November 22, 2012
            // I don't think we need this because
            // it causes a double-load of the CSV.
            // This makes the CSV plugin a little slower
            // and makes debugging more difficult.
            //ReloadCsv();
        }

        private void ReloadCsv()
        {
            lock (this)
            {
                _csvData.Reload();
                _gridView.ReloadCsvDisplay();
            }
        }

        private void CachedTypesReadyHandler()
        {
            // Don't let this crash Glue:
            if (_gridView != null)
            {
                _gridView.CachedTypesReady();
            }
        }
    }
}
