using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Glue;
using FlatRedBall.Glue.Parsing;
using Microsoft.Build.BuildEngine;

namespace GlueCsvEditor.KnownValues
{
    public class InterfaceImplementationsValueRetriever : IKnownValueRetriever
    {
        protected IEnumerable<BuildItem> _cachedBuildItems;

        public InterfaceImplementationsValueRetriever(IEnumerable<BuildItem> cachedBuildItems)
        {
            _cachedBuildItems = cachedBuildItems ?? new BuildItem[0];
        }

        public IEnumerable<string> GetKnownValues(string fullTypeName)
        {
            // Parse all custom code and find all classes that are implementations of an interface
            var results = new List<string>();
            string baseDirectory = ProjectManager.ProjectBase.Directory;
            string typeName = fullTypeName.Substring(fullTypeName.LastIndexOf(".") + 1);

            foreach (var item in _cachedBuildItems)
            {
                var file = new ParsedFile(baseDirectory + item.Include);
                foreach (var ns in file.Namespaces)
                {
                    foreach (var cls in ns.Classes)
                    {
                        bool isImplementation =
                            cls.ParentClassesAndInterfaces
                               .Where(x => x.IsInterface)
                               .Any(x => x.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
                               //.Any(x => x.Name.Equals(fullTypeName, StringComparison.OrdinalIgnoreCase));

                        if (!isImplementation)
                            continue;

                        results.Add(string.Concat("new ", cls.Namespace, ".", cls.Name, "()"));
                    }
                }
            }

            return results;
        }
    }
}
