using BasicCAM.Core.Geometry;
using System;
using System.Collections.Generic;

using System.Text;
using static BasicCAM.Core.Geometry.Axes;

namespace BasicCAM.Core.Geometry
{
    public static class Quadrants
    {

        public enum QUADRANT
        {
            I = 0,
            II = 1,
            III = 2,
            IV = 3
        }
        public static QUADRANT Next(this QUADRANT quad)
        {
            switch (quad)
            {
                case (QUADRANT.I):
                    return QUADRANT.II;

                case (QUADRANT.II):
                    return QUADRANT.III;

                case (QUADRANT.III):
                    return QUADRANT.IV;

                case (QUADRANT.IV):
                    return QUADRANT.I;
            }
            throw new NotImplementedException();
        }
        public static QUADRANT Previous(this QUADRANT quad)
        {
            switch (quad)
            {
                case (QUADRANT.I):
                    return QUADRANT.IV;

                case (QUADRANT.IV):
                    return QUADRANT.III;

                case (QUADRANT.III):
                    return QUADRANT.II;

                case (QUADRANT.II):
                    return QUADRANT.I;
            }
            throw new NotImplementedException();
        }
        public static AXES PreviousAxis(this QUADRANT quad)
        {
            switch (quad)
            {
                case (QUADRANT.I):
                    return AXES.X_POS;

                case (QUADRANT.II):
                    return AXES.Y_POS;

                case (QUADRANT.III):
                    return AXES.X_NEG;

                case (QUADRANT.IV):
                    return AXES.Y_NEG;
            }
            throw new NotImplementedException();
        }
        public static AXES NextAxis(this QUADRANT quad)
        {
            switch (quad)
            {
                case (QUADRANT.I):
                    return AXES.Y_NEG;

                case (QUADRANT.II):
                    return AXES.X_NEG;

                case (QUADRANT.III):
                    return AXES.Y_NEG;

                case (QUADRANT.IV):
                    return AXES.X_POS;
            }
            throw new NotImplementedException();
        }

        public static QUADRANT FindQuadrant(Point p, Point origin = new Point())
        {
            Vector v = origin.To(p);

            if (v.X >= 0 && v.Y > 0)
                return QUADRANT.I;
            if (v.X < 0 && v.Y >= 0)
                return QUADRANT.II;
            if (v.X <= 0 && v.Y < 0)
                return QUADRANT.III;
            return QUADRANT.IV;
        }
    }
}
