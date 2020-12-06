using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicCAM.WASM.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                           System.AttributeTargets.Struct)
    ]
    public class DisplayNameAttribute : System.Attribute
    {
        public string Name { get; }

        public DisplayNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}