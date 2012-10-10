using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Glue.GuiDisplay;
using System.ComponentModel;

namespace GlueCsvEditor.Data
{
    public class ComplexTypePropertyGridDisplayer : PropertyGridDisplayer
    {
        protected CsvData _csvData;

        public override object Instance
        {
            get
            {
                return base.Instance;
            }
            set
            {
                mInstance = value;
                UpdateDisplayedFields(value as ComplexCsvTypeDetails);
                base.Instance = value;
            }
        }

        public ComplexTypeUpdatedDelegate ComplexTypeUpdatedHandler { get; set; }

        public ComplexTypePropertyGridDisplayer(CsvData csvData)
        {
            _csvData = csvData;
        }

        protected void UpdateDisplayedFields(ComplexCsvTypeDetails complexTypeDetails)
        {
            if (complexTypeDetails == null)
                return;

            ResetToDefault();
            
            // Write excludes
            ExcludeMember("ConstructorValues");
            ExcludeMember("Properties");

            var propertyCategory = new CategoryAttribute("Properties");

            // Add properties
            for (int x = 0; x < complexTypeDetails.Properties.Count; x++)
            {
                int count = x; // Required for delegates to evaluate properly
                string propertyName = complexTypeDetails.Properties[x].Name;
                if (!string.IsNullOrWhiteSpace(complexTypeDetails.Properties[x].Type))
                    propertyName = string.Concat(propertyName, " (", complexTypeDetails.Properties[x].Type, ")");
                
                // Setup events
                Func<object> getter = () => { return complexTypeDetails.Properties[count].Value; };
                MemberChangeEventHandler setter = (sender, args) => 
                {
                    complexTypeDetails.Properties[count].Value = args.Value as string;
                    if (ComplexTypeUpdatedHandler != null)
                        ComplexTypeUpdatedHandler((mInstance as ComplexCsvTypeDetails).ToString());
                };

                // Setup type converter
                var knownValues = _csvData.GetKnownValuesForType(complexTypeDetails.Properties[x].Type);
                TypeConverter converter = null;
                if (knownValues.Count() > 0)
                    converter = new AvailableKnownValuesTypeConverter(knownValues);

                IncludeMember(propertyName, typeof(string), setter, getter, converter, new Attribute[] { propertyCategory });
            }
        }

        public delegate void ComplexTypeUpdatedDelegate(string complexTypeString);
    }
}
