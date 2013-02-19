using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model
{
    [OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
    [OslcResourceShape(title = "OSLC Allowed Values Resource Shape", describes = new string[] { OslcConstants.TYPE_ALLOWED_VALUES })]
    class AllowedValues
    {
        private List<string> allowedValues = new List<string>();

	    public AllowedValues():base() 
        {
	    }

	    public void AddAllowedValue(string allowedValue) 
        {
            this.allowedValues.Add(allowedValue);
        }

	    [OslcDescription("Value allowed for a property")]
        [OslcName("allowedValue")]
        [OslcOccurs(Occurs.OneOrMany)]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "allowedValue")]
        [OslcReadOnly]
        [OslcTitle("Allowed Values")]
        public string[] GetAllowedValues() 
        {
            return allowedValues.ToArray<string>();
        }

	    public void SetAllowedValues(string[] allowedValues) 
        {
	        this.allowedValues.Clear();
	        if (allowedValues != null) {
	            this.allowedValues.AddAll(allowedValues.ToList<string>());
	        }
	    }
    }
}
