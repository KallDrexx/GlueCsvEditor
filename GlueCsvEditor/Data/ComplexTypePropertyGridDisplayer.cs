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

        public ComplexTypeUpdatedDelegate ComplexTypeUpdatedHandler { get; set; }

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
                string propertyName = complexTypeDetails.Properties[x].Name;
                if (!string.IsNullOrWhiteSpace(complexTypeDetails.Properties[x].Type))
                    propertyName = string.Concat(propertyName, " (", complexTypeDetails.Properties[x].Type, ")");
                
                // Setup events
                MemberChangeEventHandler setter = (sender, args) => 
                {
                    complexTypeDetails.Properties[count].Value = args.Value as string;
                    if (ComplexTypeUpdatedHandler != null)
                        ComplexTypeUpdatedHandler((mInstance as ComplexTypeDetails).ToString());
                };

                Func<object> getter = () => { return complexTypeDetails.Properties[count].Value; };
                IncludeMember(propertyName, typeof(string), setter, getter);
            }
        }



        public delegate void ComplexTypeUpdatedDelegate(string complexTypeString);
    }
}
