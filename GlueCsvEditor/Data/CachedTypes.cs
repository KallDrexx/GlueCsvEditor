using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Glue.Parsing;
using System.Threading.Tasks;
using FlatRedBall.Glue;
using FlatRedBall.Glue.SaveClasses;
using FlatRedBall.Glue.Plugins;
using FlatRedBall.Glue.Elements;

namespace GlueCsvEditor.Data
{
    public class CachedTypes
    {
        protected readonly object _cacheLock = new object();
        protected bool _cacheReady;

        // Cached values
        protected List<ParsedEnum> _parsedProjectEnums;
        protected List<ParsedClass> _parsedPrjectClasses;
        protected List<EntitySave> _entities;
        protected List<ScreenSave> _screens;

        public bool IsCacheReady
        {
            get
            {
                lock (_cacheLock)
                {
                    return _cacheReady;
                }
            }
        }

        public List<ParsedEnum> ProjectEnums 
        { 
            get 
            {
                if (!IsCacheReady)
                    return new List<ParsedEnum>();

                return _parsedProjectEnums; 
            } 
        }

        public List<ParsedClass> ProjectClasses
        {
            get
            {
                if (!IsCacheReady)
                    return new List<ParsedClass>();

                return _parsedPrjectClasses;
            }
        }

        public CachedTypes()
        {
            StartCacheTask();
        }

        protected void StartCacheTask()
        {
            new Task(() =>
            {
                lock (_cacheLock)
                {
                    _cacheReady = false;
                }

                try
                {
                    // Save all the entity screens and 
                    _entities = ObjectFinder.Self.GlueProject.Entities;
                    _screens = ObjectFinder.Self.GlueProject.Screens;

                    // Go through all the code in the project and generate a list of enums and classes
                    var items = ProjectManager.ProjectBase.Where(x => x.Name == "Compile");
                    string baseDirectory = ProjectManager.ProjectBase.Directory;

                    _parsedPrjectClasses = new List<ParsedClass>();
                    _parsedProjectEnums = new List<ParsedEnum>();

                    foreach (var item in items)
                    {
                        var file = new ParsedFile(baseDirectory + item.Include);
                        foreach (var ns in file.Namespaces)
                        {
                            _parsedProjectEnums.AddRange(ns.Enums);
                            _parsedPrjectClasses.AddRange(ns.Classes);
                        }
                    }
                }
                catch (Exception ex)
                {
                    PluginManager.ReceiveOutput(
                        string.Concat(
                            "Exception occurred while caching project types: ",
                            ex.GetType(),
                            ":",
                            ex.Message));

                    return;
                }

                lock (_cacheLock)
                {
                    _cacheReady = true;
                }

            }).Start();
        }
    }
}
