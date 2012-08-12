using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using FlatRedBall.Glue;
using FlatRedBall.Glue.Plugins.ExportedInterfaces;
using FlatRedBall.Glue.Plugins.Interfaces;
using FlatRedBall.Glue.SaveClasses;
using FlatRedBall.Glue.Plugins;
using GlueCsvEditor.Controls;
using System.Windows.Forms;
using FlatRedBall.Glue.Controls;
using System.IO;
using FlatRedBall.IO.Csv;

namespace GlueCsvEditor
{
    [Export(typeof(ICenterTab))]
    [Export(typeof(ITreeItemSelect))]
    public class CsvEditorPlugin : IPlugin, ICenterTab, ITreeItemSelect
    {
        protected EditorMain _editor;
        protected TabControl _tabContainer;
        protected PluginTab _tab;

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

        public string FriendlyName
        {
            get { return "Csv Editor"; }
        }

        public bool ShutDown(PluginShutDownReason reason)
        {
            if (_tab != null)
                _tabContainer.Controls.Remove(_tab);

            _tabContainer = null;
            _tab = null;
            _editor = null;

            return true;
        }

        public void StartUp()
        {
            // Do anything your plugin needs to do when it first starts up
        }

        public Version Version
        {
            get { return new Version(1, 0); }
        }

        public void InitializeTab(TabControl tabControl)
        {
            _tabContainer = tabControl;
        }

        protected void OnClosedByUser(object sender)
        {
            PluginManager.ShutDownPlugin(this);
        }

        public void ReactToItemSelect(TreeNode selectedTreeNode)
        {
            // Close the existing tab
            if (_tab != null)
            {
                _tabContainer.Controls.Remove(_tab);
                _editor = null;
                _tab = null;
            }

            // Determine if a csv was selected
            if (IsCsv(selectedTreeNode.Tag))
            {
                var csv = selectedTreeNode.Tag as ReferencedFileSave;
                string path = ProjectManager.MakeAbsolute(csv.Name, true);

                try
                {
                    _tab = new PluginTab();
                    _tab.Text = "CSV Editor";

                    _editor = new EditorMain(GlueCommands, GlueState, path);
                    _tab.Controls.Add(_editor);
                    _tabContainer.Controls.Add(_tab);
                    _tabContainer.SelectTab(_tabContainer.Controls.Count - 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to create a CSV runtime representation: " + ex.Message, "Error");
                }
            }
        }

        protected bool IsCsv(object obj)
        {
            var csv = obj as ReferencedFileSave;
            if (csv == null)
                return false;

            return csv.IsCsvOrTreatedAsCsv;
        }
    }
}
