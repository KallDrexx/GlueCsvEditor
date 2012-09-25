using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace GlueCsvEditor.KnownValues
{
    public class EnumReflectionValueRetriever : IKnownValueRetriever
    {
        protected static object _threadLock = new object();
        protected static Dictionary<string, IEnumerable<string>> _cachedTypeValues;

        public IEnumerable<string> GetKnownValues(string fullTypeName)
        {
            if (string.IsNullOrWhiteSpace(fullTypeName))
                return new string[0];

            // Lock to prevent multiple threads from accessing the type dictionary at the same time
            lock (_threadLock)
            {
                // If the dictionary hasn't been instantiated yet, set it up
                if (_cachedTypeValues == null)
                    _cachedTypeValues = new Dictionary<string, IEnumerable<string>>();

                // Check if this type's value has already been cached
                if (!_cachedTypeValues.ContainsKey(fullTypeName))
                    CacheTypeValues(fullTypeName);

                return _cachedTypeValues[fullTypeName];
            }
        }

        protected void CacheTypeValues(string fullTypeName)
        {
            IEnumerable<string> foundValues;

            // Use reflection to retrieve the specified enum
            var type = AppDomain.CurrentDomain
                                .GetAssemblies()
                                .SelectMany(x => x.GetTypes())
                                .Where(x => x.FullName.Equals(fullTypeName.Trim(), StringComparison.OrdinalIgnoreCase))
                                .Where(x => x.IsEnum)
                                .FirstOrDefault();

            if (type == null)
            {
                foundValues = new string[0];
            }
            else
            {
                // Get all the enum values
                foundValues = type.GetMembers(BindingFlags.Public | BindingFlags.Static)
                                   .Select(x => x.Name)
                                   .ToList();
            }

            if (_cachedTypeValues.ContainsKey(fullTypeName))
                _cachedTypeValues[fullTypeName] = foundValues;
            else
                _cachedTypeValues.Add(fullTypeName, foundValues);
        }
    }
}
