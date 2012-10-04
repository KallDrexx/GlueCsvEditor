using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Glue.GuiDisplay;

namespace GlueCsvEditor.Data
{
    public class ComplexTypePropertyGridDisplayer : PropertyGridDisplayer
    {
        public override object Instance
        {
            get
            {
                return base.Instance;
            }
            set
            {
                mInstance = value;
                UpdateDisplayedFields(value as ComplexTypeDetails);
                base.Instance = value;
            }
        }

        protected void UpdateDisplayedFields(ComplexTypeDetails complexTypeDetails)
        {
            if (complexTypeDetails == null)
                return;

            ResetToDefault();
            
            // Write excludes
            ExcludeMember("ConstructorValues");
            ExcludeMember("Properties");

            // Add properties
            for (int x = 0; x < complexTypeDetails.Properties.Count; x++)
            {
                int count = x; // Required for delegates to evaluate properly
                string propertyName = complexTypeDetails.Properties.Keys.ElementAt(x);
                
                // Setup events
                MemberChangeEventHandler setter = (sender, args) => { complexTypeDetails.Properties[propertyName] = args.Value as string; };
                Func<object> getter = () => { return complexTypeDetails.Properties[propertyName]; };
                IncludeMember(propertyName, typeof(string), setter, getter);
            }
        }
    }
}
