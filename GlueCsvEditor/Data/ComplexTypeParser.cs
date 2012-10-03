using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Instructions.Reflection;

namespace GlueCsvEditor.Data
{
    public class ComplexTypeParser
    {
        public string TypeName { get; protected set; }
        public string Namespace { get; protected set; }
        public string ConstructorValues { get; protected set; }
        public PropertyCollection Properties { get; protected set; }

        protected ComplexTypeParser()
        {
            Properties = new PropertyCollection();
        }

        public static ComplexTypeParser ParseValue(string value)
        {
            var result = new ComplexTypeParser();

            // Figure out if the type is in the format of 
            //   new Namespace.Type() or 
            //   new Namespace.Type { Property1 = "abcd" }
            value = (value ?? string.Empty).Trim();
            if (!value.StartsWith("new ", StringComparison.OrdinalIgnoreCase))
                return null;

            // Get indexes for braces/parenthesis
            int openParen = value.IndexOf("(");
            int closeParen = value.IndexOf(")");
            int openBrace = value.IndexOf("{");
            int closeBrace = value.IndexOf("}");
            int secondSpace = value.IndexOf(" ", 5);

            // Make sure it either has parenthesis or braces
            if (!(openParen >= 0 && closeParen >= 0) && !(openBrace >= 0 && closeBrace >= 0))
                return null;

            // Make sure braces/parenthesis aren't in the wrong order
            if (openParen > closeParen || openBrace > closeBrace)
                return null;

            // Figure out where the type string ends
            int typeStringEndIndex;
            if ((openParen >= 0 && openParen > secondSpace))
                typeStringEndIndex = secondSpace;
            else if (openBrace >= 0 && openParen == -1 && openBrace > secondSpace)
                typeStringEndIndex = secondSpace;

            else if (openParen >= 0)
                typeStringEndIndex = openParen;
            else
                typeStringEndIndex = openBrace;

            // Isolate the type
            string isolatedTypeString = value.Substring(3, typeStringEndIndex - 3);
            if (isolatedTypeString.Contains("."))
            {
                result.Namespace = isolatedTypeString.Remove(isolatedTypeString.LastIndexOf("."));
                result.TypeName = isolatedTypeString.Substring(isolatedTypeString.LastIndexOf(".") + 1);
            }
            else
            {
                result.Namespace = string.Empty;
                result.TypeName = isolatedTypeString.Trim();
            }

            // If any constructor values are found, save them
            if (openParen >= 0 && closeParen - openParen > 1)
                result.ConstructorValues = value.Substring(openParen + 1, closeParen - openParen);
            else
                result.ConstructorValues = string.Empty;

            // Figure out the properties
            if (openBrace >= 0)
            {
                string propertiesString = value.Substring(openBrace + 1, closeBrace - openBrace);
                var propertyDefinitions = propertiesString.Split(new char[] { ',' });
                foreach (var propertyDefinition in propertyDefinitions)
                {
                    var pair = propertyDefinition.Split(new char[] { '=' });
                    if (pair.Length != 2)
                        continue; // not a valid property definition

                    result.Properties.Add(pair[0].Trim(), pair[1].Trim());
                }
            }

            return result;
        }
    }
}
