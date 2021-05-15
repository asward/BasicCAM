using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Core.Exceptions
{
    public class SegmentCreationException : Exception
    {
        public SegmentCreationException(string message) : base(message)
        {
        }
    }
}
