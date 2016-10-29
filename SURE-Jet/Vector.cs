using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SURE_Jet
{
    public class Vector
    {
        public double x;
        public double y;
        public Vector() { }
        public Vector(double xx, double yy)
        {
            x = xx;
            y = yy;
        }
        public double Magnitude
        {
            get
            {
                return Math.Sqrt(Math.Pow(x,2) + Math.Pow(y,2));
            }
        }
        public double angle
        {
            get
            {
                if (x == 0)
                {
                    if (y > 0)
                        return Math.PI / 2;
                    return 3 * Math.PI / 2;
                }
                else if (y == 0)
                {
                    if (x > 0)
                        return 0;
                    return Math.PI;
                }
                else if (x > 0 && y > 0)
                {
                    return Math.Atan(y / x);
                }
                else if (x < 0 && y < 0)
                {
                    return Math.PI + Math.Atan(y / x);
                }
                else if (x > 0 && y < 0)
                {
                    return Math.Atan(y / x);
                }
                else if (x < 0 && y > 0)
                {
                    return Math.PI + Math.Atan(y / x);
                }
                else
                {
                    MessageBox.Show("Error in polar vector class");
                    return 0;
                }
            }
        }
        public Vector addMagnitudeInSameDirection(double r)
        {
            Vector v = new Vector(x, y);
            double angle = v.angle;
            double newMagnitude = v.Magnitude + r;
            double newMagnitude2 = v.Magnitude - r;
            if (Math.Max(Math.Abs(newMagnitude), Math.Abs(newMagnitude2)) == Math.Abs(newMagnitude))
                return getVectorFromMagnitudeAndAngle(newMagnitude, angle);
            return getVectorFromMagnitudeAndAngle(newMagnitude2, angle);

        }
        public Vector addMagnitudeInReverseDirection(double r)
        {
            Vector v = new Vector(x, y);
            double angle = v.angle;
            double newMagnitude = v.Magnitude + r;
            double newMagnitude2 = v.Magnitude - r;
            if (Math.Min(Math.Abs(newMagnitude), Math.Abs(newMagnitude2)) == Math.Abs(newMagnitude))
                return getVectorFromMagnitudeAndAngle(newMagnitude, angle);
            return getVectorFromMagnitudeAndAngle(newMagnitude2, angle);
        }
        public Vector addMagnitude(double r)
        {
            return getVectorFromMagnitudeAndAngle(Magnitude + r, angle);
        }
        public Vector decreaseByMagnitude(double r)
        {
            return null;
        }
        public Vector getVectorFromMagnitudeAndAngle(double mag, double angle)
        {
            return new Vector(mag * Math.Cos(angle), mag * Math.Sin(angle));
        }


        internal void addPerpendiclar(double cutterCompensation)
        {
            if (x >= 0 && y >= 0)
                addVector(getVectorFromMagnitudeAndAngle(cutterCompensation, this.angle + Math.PI / 2));
            else if (x > 0 && y < 0)
                addVector(getVectorFromMagnitudeAndAngle(cutterCompensation, this.angle + Math.PI / 2));
            else if (x < 0 && y < 0)
                addVector(getVectorFromMagnitudeAndAngle(cutterCompensation, this.angle + Math.PI / 2));
            else if (x < 0 && y > 0)
                addVector(getVectorFromMagnitudeAndAngle(cutterCompensation, this.angle + Math.PI / 2));
            else if(x == 0 && y > 0)
                addVector(getVectorFromMagnitudeAndAngle(cutterCompensation, this.angle + Math.PI / 2));
            else if(x == 0 && y < 0)
                addVector(getVectorFromMagnitudeAndAngle(cutterCompensation, this.angle + Math.PI / 2));
            else if(y == 0 && x > 0)
                addVector(getVectorFromMagnitudeAndAngle(cutterCompensation, this.angle + Math.PI / 2));
            else if(y == 0 && x < 0)
                addVector(getVectorFromMagnitudeAndAngle(cutterCompensation, this.angle + Math.PI / 2));
            else
                MessageBox.Show("Unimplemented: " + x.ToString() + "," + y.ToString());


        }
        public void addVector(Vector v2)
        {
            x = x + v2.x;
            y = y + v2.y;
        }

        internal string print()
        {
            return "(" + x.ToString() + "," + y.ToString() + ")";
        }

        internal double getDistance(Vector vector)
        {
            return Math.Sqrt(Math.Pow(x - vector.x, 2) + Math.Pow(y - vector.y, 2));
        }
        public override string ToString()
        {
            return "(" + x.ToString() + "," + y.ToString() + ")";
        }

        internal bool IsNotANumber()
        {
            return x.ToString() == "NaN" || y.ToString() == "NaN";
        }

        internal Vector Copy()
        {
            return new Vector(x, y);
        }

        internal int CompareTo(Vector centerThree)
        {
            if (this.getDistance(centerThree) < Math.Pow(10, -4))
                return 0;
            return 1;
            double diffX = Math.Abs(centerThree.x - this.x);
            double diffY = Math.Abs(centerThree.y - this.y);
            if (diffX < Math.Pow(10,-10) && diffY < Math.Pow(10,-10))
                return 0;
            return -1;
        }
        public double getAngleFrom3Points(Vector a, Vector b, Vector c)
        {
            return Math.Acos((Math.Pow(a.getDistance(b),2) + Math.Pow(a.getDistance(c),2) - Math.Pow(b.getDistance(c),2)) / (2 * a.getDistance(b) * a.getDistance(c)));
        }
    }
}
