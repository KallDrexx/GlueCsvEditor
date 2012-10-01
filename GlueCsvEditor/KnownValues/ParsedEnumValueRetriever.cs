using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Glue;
using FlatRedBall.Glue.Parsing;
using Microsoft.Build.BuildEngine;

namespace GlueCsvEditor.KnownValues
{
    public class ParsedEnumValueRetriever : IKnownValueRetriever
    {
        protected IEnumerable<BuildItem> _buildItemsWithEnums;

        public ParsedEnumValueRetriever(IEnumerable<BuildItem> buildItemsWithEnums)
        {
            _buildItemsWithEnums = buildItemsWithEnums ?? new BuildItem[0];
        }

        public IEnumerable<string> GetKnownValues(string fullTypeName)
        {
            // Parse all custom code and find the matching enumeration
            string baseDirectory = ProjectManager.ProjectBase.Directory;

            // Only search through build items known to have enumerations
            foreach (var item in _buildItemsWithEnums)
            {
                var file = new ParsedFile(baseDirectory + item.Include);
                foreach (var ns in file.Namespaces)
                {
                    foreach (var enm in ns.Enums)
                    {
                        var enumFullType = string.Concat(enm.Namespace, ".", enm.Name);
                        if (!enumFullType.Equals(fullTypeName, StringComparison.OrdinalIgnoreCase))
                            continue;

                        // return all the values for the enum
                        return enm.Values;
                    }
                }
            }

            return new string[0];
        }
    }
}
