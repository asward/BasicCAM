using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicCAM.WASM.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                           System.AttributeTargets.Struct)
    ]
    public class DescriptionAttribute : System.Attribute
    {
        private string description;

        public DescriptionAttribute(string description)
        {
            this.description = description;
        }
    }
}