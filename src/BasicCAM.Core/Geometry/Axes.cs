using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Core.Geometry
{

    public static class Axes
    {
        public enum AXES
        {
            X_POS = 0,
            Y_POS = 1,
            X_NEG = 2,
            Y_NEG = 3
        }
        public static AXES Next(this AXES axis)
        {
            switch (axis)
            {
                case (AXES.X_POS):
                    return AXES.Y_POS;

                case (AXES.Y_POS):
                    return AXES.X_NEG;

                case (AXES.X_NEG):
                    return AXES.Y_NEG;

                case (AXES.Y_NEG):
                    return AXES.X_POS;
            }
            throw new NotImplementedException();
        }
        public static AXES Previous(this AXES axis)
        {
            switch (axis)
            {
                case (AXES.X_POS):
                    return AXES.Y_NEG;

                case (AXES.Y_NEG):
                    return AXES.X_NEG;

                case (AXES.X_NEG):
                    return AXES.Y_POS;

                case (AXES.Y_POS):
                    return AXES.X_POS;
            }
            throw new NotImplementedException();
        }
    }
}
