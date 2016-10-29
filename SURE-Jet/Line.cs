using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SURE_Jet
{
    class Line
    {
        public double yIntercept;
        private double s;
        public double slope
        {
            get
            {
                if (Math.Abs(s) > Math.Pow(10, 10))
                    s = Convert.ToDouble("Infinity");
                return s;
            }
            set
            {
                s = value;
            }
        }
        public double xIntercept;
        public double X1;
        public double Y1;
        public double X2;
        public double Y2;

        public Line() { }
        public Line(double x1, double y1, double slope3)
        {
            
            slope = slope3;
            if(!slopeIsVertical(slope3))
            yIntercept = y1 - slope * x1;

            if (slopeIsVertical(slope3))
                xIntercept = x1;
        }
        public Line(double x1, double y1, double x2, double y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            //standard case
            slope = (y2 - y1) / (x2 - x1);

            if (!slopeIsVertical(slope))
            yIntercept = y1 - slope * x1;
            if (x2 - x1 == 0)
            {
                xIntercept = x1;
            }
            if (y2 - y1 == 0)
            {
                yIntercept = y1;
            }
        }
        public Vector getIntersection(Line l2)
        {
            Vector a = new Vector();
            //standard case, neither slope = 0 or - 1
            if (!slopeIsZeroOrInfinity(slope) && !slopeIsZeroOrInfinity(l2.slope))
            {
                a.x = (l2.yIntercept - yIntercept) / (slope - l2.slope);
                a.y = a.x * slope + yIntercept;
            }
            else if (slopeIsVertical(slope) && l2.slope == 0)
            {
                a.y = l2.yIntercept;
                a.x = xIntercept;
            }
            else if (slope == 0 && slopeIsVertical(l2.slope))
            {
                a.y = yIntercept;
                a.x = l2.xIntercept;
            }
            else if (slopeIsVertical(slope) && !slopeIsZeroOrInfinity(l2.slope))
            {
                a.x = xIntercept;
                a.y = l2.slope * a.x + l2.yIntercept;
            }
            else if (slopeIsVertical(l2.slope) && !slopeIsZeroOrInfinity(slope))
            {
                a.x = l2.xIntercept;
                a.y = slope * a.x + yIntercept;
            }
            else if (slope == 0 && !slopeIsZeroOrInfinity(l2.slope))
            {
                a.y = yIntercept;
                a.x = (a.y - l2.yIntercept) / l2.slope;
            }
            else if (l2.slope == 0 && !slopeIsZeroOrInfinity(slope))
            {
                a.y = l2.yIntercept;
                a.x = (a.y - yIntercept) / slope;
            }
            else
                //MessageBox.Show("This intersection is Unimplemented: " + l2.ToString() + " " + this.ToString());

            if (l2.CompareTo(this) == 0)
            {
                return null;
            }
            return a;
        }

        private int CompareTo(Line line)
        {
            if (this.slopeIsVertical(this.slope) && line.slopeIsVertical(line.slope) && this.xIntercept == line.xIntercept)
                return 0;
            if (this.slope == line.slope && this.yIntercept == line.yIntercept)
                return 0;
            return 1;
        }
        public bool slopeIsZeroOrInfinity(double slopeLocal)
        {
            return slopeLocal == 0 || slopeLocal.ToString() == "Infinity" || slopeLocal.ToString() == "-Infinity";
        }
        public bool slopeIsVertical(double s)
        {
            return s.ToString() == "Infinity" || s.ToString() == "-Infinity" || Math.Abs(s) > Math.Pow(10,12);
        }
        public override string ToString()
        {
            return "Line: Slope: " + slope.ToString() + " Yint:" + yIntercept.ToString() + " Xint: " + xIntercept.ToString();
        }

        internal int CompareSlope(double p)
        {
            if (slopeIsVertical(slope) && slopeIsVertical(p))
            {
                return 0;
            }
            if (slope == p)
            {
                return 0;
            }

            return -1;
        }
    }
}
