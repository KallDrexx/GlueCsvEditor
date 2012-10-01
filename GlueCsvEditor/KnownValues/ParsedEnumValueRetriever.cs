using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Glue;
using FlatRedBall.Glue.Parsing;

namespace GlueCsvEditor.KnownValues
{
    public class ParsedEnumValueRetriever : IKnownValueRetriever
    {
        public IEnumerable<string> GetKnownValues(string fullTypeName)
        {
            // Parse all custom code and find the matching enumeration
            var items = ProjectManager.ProjectBase.Where(x => x.Name == "Compile");
            string baseDirectory = ProjectManager.ProjectBase.Directory;

            foreach (var item in items)
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
