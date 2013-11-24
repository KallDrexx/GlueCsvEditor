using System;
using System.ComponentModel.Composition;
using System.Reflection;
using FlatRedBall.Glue;
using FlatRedBall.Glue.Plugins.ExportedInterfaces;
using FlatRedBall.Glue.Plugins.Interfaces;
using FlatRedBall.Glue.SaveClasses;
using FlatRedBall.Glue.Plugins;
using GlueCsvEditor.Controls;
using System.Windows.Forms;
using FlatRedBall.Glue.Controls;

namespace GlueCsvEditor
{
    [Export(typeof(PluginBase))]
    public class CsvEditorPlugin : PluginBase
    {
        #region Fields

        private EditorMain _editor;
        private TabControl _tabContainer;
        private PluginTab _tab;
        private string _currentCsv;

        #endregion

        [Import("GlueProjectSave")]
        public GlueProjectSave GlueProjectSave
        {
            get;
            set;
        }

        [Import("GlueCommands")]
        public IGlueCommands GlueCommands
        {
            get;
            set;
        }
		
		[Import("GlueState")]
		public IGlueState GlueState
		{
		    get;
		    set;
        }

        public override Version Version
        {
            get 
            { 
                return Assembly.GetAssembly(typeof (CsvEditorPlugin))
                               .GetName()
                               .Version; 
            }
        }

        public override string FriendlyName { get { return "  Csv Editor"; } }

        public override bool ShutDown(PluginShutDownReason reason)
        {
            if (_tab != null)
                _tabContainer.Controls.Remove(_tab);

            _tabContainer = null;
            _tab = null;
            _editor = null;

            return true;
        }

        public override void StartUp()
        {
            // Initialize the handlers I need
            InitializeCenterTabHandler = InitializeTab;
            ReactToItemSelectHandler = ReactToItemSelect;
            ReactToFileChangeHandler = ReactToFileChange;
        }

        private void InitializeTab(TabControl tabControl)
        {
            _tabContainer = tabControl;
        }

        private void ReactToItemSelect(TreeNode selectedTreeNode)
        {
            if (_tab != null)
            {
                _editor.SaveEditorSettings();
            }

            // Determine if a csv was selected
            if (IsCsv(selectedTreeNode.Tag))
            {
                if (_tab == null)
                {
                    CreateNewCsvControl(selectedTreeNode);
                }
                else
                {
                    LoadFile(selectedTreeNode);
                }
            }
            else
            {
                // Close the existing tab
                if (_tab != null)
                {
                    _editor = null;

                    _tabContainer.Controls.Remove(_tab);
                    _tab = null;
                    _currentCsv = null;
                }
            }
        }

        private void CreateNewCsvControl(TreeNode selectedTreeNode)
        {
            bool succeeded = false;

            try
            {
                _tab = new PluginTab { Text = "CSV Editor" };

                _editor = new EditorMain();


                _tab.Controls.Add(_editor);
                _tabContainer.Controls.Add(_tab);


                succeeded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create a CSV runtime representation: " + ex.Message, "Error");
            }

            if (succeeded)
            {
                LoadFile(selectedTreeNode);
            }
        }

        private void LoadFile(TreeNode selectedTreeNode)
        {
            var csvRfs = selectedTreeNode.Tag as ReferencedFileSave;
            if (csvRfs == null)
                throw new InvalidOperationException("Node is CSV but did not cast as a ReferencedFileSave");



            char delimiter;
            _currentCsv = ProjectManager.MakeAbsolute(csvRfs.Name, true);


            switch (csvRfs.CsvDelimiter)
            {
                case AvailableDelimiters.Pipe:
                    delimiter = '|';
                    break;

                case AvailableDelimiters.Tab:
                    delimiter = '\t';
                    break;

                default:
                    delimiter = ',';
                    break;
            }


            _editor.LoadCsv(_currentCsv, delimiter);

            int indexToSelect = _tabContainer.Controls.IndexOf(_tab);
            _tabContainer.SelectTab(indexToSelect);
        }

        private void ReactToFileChange(string filename)
        {
            if (filename.Equals(_currentCsv, StringComparison.OrdinalIgnoreCase))
            {
                PluginManager.ReceiveOutput("CSV Editor: Loading file because of external change " + filename);
                _editor.SaveEditorSettings();
                _editor.NotifyOfCsvUpdate();
            }
        }

        private bool IsCsv(object obj)
        {
            var csv = obj as ReferencedFileSave;
            if (csv == null)
                return false;

            return csv.IsCsvOrTreatedAsCsv;
        }
    }
}
