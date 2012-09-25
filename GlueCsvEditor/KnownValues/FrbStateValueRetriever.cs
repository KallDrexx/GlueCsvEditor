using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Glue.Elements;

namespace GlueCsvEditor.KnownValues
{
    public class FrbStateValueRetriever : IKnownValueRetriever
    {
        public IEnumerable<string> GetKnownValues(string fullTypeName)
        {
            // Try and get the FRB IElement for the fully qualified type name
            if (fullTypeName.IndexOf('.') < 0)
                return new string[0];

            fullTypeName = fullTypeName.Trim();

            string elementName = fullTypeName.Remove(fullTypeName.LastIndexOf('.'));
            string stateTypeName = fullTypeName.Substring(fullTypeName.LastIndexOf('.') + 1);

            // Convert the element name to GlueProject SaveClass format
            if (!elementName.Contains("Entities") && !elementName.Contains("Screens"))
                return new string[0];

            bool isEntity = elementName.Contains("Entities");
            elementName = elementName.Substring(elementName.LastIndexOf('.') + 1);
            if (isEntity)
                elementName = "Entities/" + elementName;
            else
                elementName = "Screens/" + elementName;

            var element = ObjectFinder.Self.GetIElement(elementName);
            if (element == null)
                return new string[0];

            // First see if this is a uncategorized
            if (stateTypeName == "VariableState")
            {
                // Loop through the element's uncategorized state values
                return element.States
                              .Select(x => x.Name)
                              .OrderBy(x => x)
                              .ToList();
            }

            // Otherwise see if the state category with the same name exists
            var category = element.StateCategoryList.FirstOrDefault(x => x.Name.Equals(stateTypeName, StringComparison.OrdinalIgnoreCase));
            if (category == null)
                return new string[0];

            return category.States
                           .Select(x => x.Name)
                           .OrderBy(x => x)
                           .ToList();
        }
    }
}
