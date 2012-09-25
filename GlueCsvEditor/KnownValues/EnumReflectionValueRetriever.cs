using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace GlueCsvEditor.KnownValues
{
    public class EnumReflectionValueRetriever : IKnownValueRetriever
    {
        public IEnumerable<string> GetKnownValues(string fullTypeName)
        {
            if (string.IsNullOrWhiteSpace(fullTypeName))
                return new string[0];

            // Use reflection to retrieve the specified enum
            var type = AppDomain.CurrentDomain
                                .GetAssemblies()
                                .SelectMany(x => x.GetTypes())
                                .Where(x => x.FullName.Equals(fullTypeName.Trim(), StringComparison.OrdinalIgnoreCase))
                                .Where(x => x.IsEnum)
                                .FirstOrDefault();

            if (type == null)
                return new string[0];

            // Get all the enum values
            return type.GetMembers(BindingFlags.Public | BindingFlags.Static)
                       .Select(x => x.Name)
                       .ToList();
        }
    }
}
