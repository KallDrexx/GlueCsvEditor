using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Instructions.Reflection;
using System.ComponentModel;

namespace GlueCsvEditor.Data
{
    public class ComplexCsvTypeDetails
    {
        protected const char PROPERTY_COLLECTION_START_CHAR = '(';
        protected const char PROPERTY_COLLECTION_END_CHAR = ')';

        [Category("General Information")]
        public string TypeName { get; set; }

        [Category("General Information")]
        public string Namespace { get; set; }

        public List<ComplexTypeProperty> Properties { get; protected set; }

        public ComplexCsvTypeDetails()
        {
            Properties = new List<ComplexTypeProperty>();
        }

        public static ComplexCsvTypeDetails ParseValue(string value)
        {
            var result = new ComplexCsvTypeDetails();

            // Figure out if the type is in the format of 
            //   new Namespace.Type( Property1 = abc, Property2 = cdef )
            value = (value ?? string.Empty).Trim();
            if (!value.StartsWith("new ", StringComparison.OrdinalIgnoreCase))
                return null;

            // Get indexes for braces/parenthesis
            int start = value.IndexOf(PROPERTY_COLLECTION_START_CHAR);
            int end = value.LastIndexOf(PROPERTY_COLLECTION_END_CHAR);

            if (start < 0 || end < 0)
                return null; // Invalid format

            // Isolate the type string
            string isolatedTypeString = value.Substring(3, start - 3);
            if (isolatedTypeString.Contains("."))
            {
                result.Namespace = isolatedTypeString.Remove(isolatedTypeString.LastIndexOf(".")).Trim();
                result.TypeName = isolatedTypeString.Substring(isolatedTypeString.LastIndexOf(".") + 1).Trim();
            }
            else
            {
                result.Namespace = string.Empty;
                result.TypeName = isolatedTypeString.Trim();
            }

            // Figure out the properties
            string propertiesString = value.Substring(start + 1, end - start - 1);
            var propertyDefinitions = propertiesString.Split(new char[] { ',' });
            foreach (var propertyDefinition in propertyDefinitions)
            {
                var pair = propertyDefinition.Split(new char[] { '=' });
                if (pair.Length != 2)
                    continue; // not a valid property definition

                result.Properties.Add(new ComplexTypeProperty
                {
                    Name = pair[0].Trim(),
                    Value = pair[1].Trim()
                });
            }

            return result;
        }

        public override string ToString()
        {
            var output = new StringBuilder("new ");

            if (!string.IsNullOrWhiteSpace(Namespace))
            {
                output.Append(Namespace);
                output.Append(".");
            }

            output.Append(TypeName);

            // Get a list of all properties that do not hav empty values
            var filledProperties = Properties.Where(x => !string.IsNullOrWhiteSpace(x.Value))
                                             .ToList();

            output.Append(PROPERTY_COLLECTION_START_CHAR);
            output.Append(" ");

            // Add defined properties
            if (filledProperties.Count > 0)
            {
                for (int x = 0; x < filledProperties.Count; x++)
                {
                    var prop = filledProperties[x];

                    if (x > 0)
                        output.Append(", ");

                    output.Append(prop.Name);
                    output.Append(" = ");
                    output.Append(prop.Value);
                }
            }

            output.Append(" ");
            output.Append(PROPERTY_COLLECTION_END_CHAR);

            return output.ToString();
        }
    }
}
