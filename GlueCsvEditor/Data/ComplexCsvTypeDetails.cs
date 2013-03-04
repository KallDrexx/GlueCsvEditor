using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GlueCsvEditor.Data
{
    public class ComplexCsvTypeDetails
    {
        private const char PropertyCollectionStartChar = '(';
        private const char PropertyCollectionEndChar = ')';

        [Category("General Information")]
        public string TypeName { get; set; }

        [Category("General Information")]
        public string Namespace { get; set; }

        [Category("General Information")]
        public bool IsShorthandFormat { get; set; }

        public List<ComplexTypeProperty> Properties { get; private set; }

        public ComplexCsvTypeDetails()
        {
            Properties = new List<ComplexTypeProperty>();
        }

        public static ComplexCsvTypeDetails ParseValue(string value)
        {
            return ParseLonghandTypeFormat(value) 
                ?? ParseShorthandTypeFormat(value);
        }

        private static ComplexCsvTypeDetails ParseLonghandTypeFormat(string value)
        {
            var result = new ComplexCsvTypeDetails();

            // Figure out if the type is in the format of 
            //   new Namespace.Type( Property1 = abc, Property2 = cdef )
            value = (value ?? string.Empty).Trim();
            if (!value.StartsWith("new ", StringComparison.OrdinalIgnoreCase))
                return null;

            // Get indexes for braces/parenthesis
            int start = value.IndexOf(PropertyCollectionStartChar);
            int end = value.LastIndexOf(PropertyCollectionEndChar);

            if (start < 0 || end < 0)
                return null; // Invalid format

            // Isolate the type string
            string isolatedTypeString = value.Substring(3, start - 3);
            if (isolatedTypeString.Contains("."))
            {
                result.Namespace = isolatedTypeString.Remove(isolatedTypeString.LastIndexOf(".", System.StringComparison.Ordinal)).Trim();
                result.TypeName = isolatedTypeString.Substring(isolatedTypeString.LastIndexOf(".", System.StringComparison.Ordinal) + 1).Trim();
            }
            else
            {
                result.Namespace = string.Empty;
                result.TypeName = isolatedTypeString.Trim();
            }

            // Figure out the properties
            string propertiesString = value.Substring(start + 1, end - start - 1);
            var propertyDefinitions = propertiesString.Split(new char[] {','});
            foreach (var propertyDefinition in propertyDefinitions)
            {
                var pair = propertyDefinition.Split(new char[] {'='});
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

        private static ComplexCsvTypeDetails ParseShorthandTypeFormat(string value)
        {
            var result = new ComplexCsvTypeDetails {IsShorthandFormat = true};

            // Parse type specified in "property = value, property2 = value2" format
            if (string.IsNullOrWhiteSpace(value) || !value.Contains("="))
                return null;

            var properties = value.Split(',');
            foreach (var property in properties)
            {
                // Invalid propery definitions are ignored
                var parts = property.Split('=');
                if (parts.Length != 2)
                    continue;

                // First part is the property name
                if (parts[0].Trim() == string.Empty)
                    continue;

                result.Properties.Add(new ComplexTypeProperty
                {
                    Name = parts[0].Trim(),
                    Value = parts[1].Trim()
                });
            }

            return result;
        }

        public override string ToString()
        {
            return IsShorthandFormat 
                ? ToShorthandString() 
                : ToLonghandString();
        }

        private string ToShorthandString()
        {
            var output = new StringBuilder();

            var filledProperties = Properties.Where(x => !string.IsNullOrWhiteSpace(x.Value))
                                             .ToList();

            for (int x = 0; x < filledProperties.Count; x++)
            {
                var prop = filledProperties[x];

                if (x > 0)
                    output.Append(", ");

                output.Append(prop.Name);
                output.Append(" = ");
                output.Append(prop.Value);
            }

            return output.ToString();
        }

        private string ToLonghandString()
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

            output.Append(PropertyCollectionStartChar);
            output.Append(" ");

            // Add defined properties
            if (filledProperties.Any())
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
            output.Append(PropertyCollectionEndChar);

            return output.ToString();
        }
    }
}
