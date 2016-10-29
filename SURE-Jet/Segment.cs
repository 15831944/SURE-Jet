using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SURE_Jet
{
    class Segment
    {
        public double x1;
        public double y1;
        public double x2;
        public double y2;
        public Segment(double X1, double Y1, double X2, double Y2)
        {
            x1 = X1;
            y1 = Y1;
            x2 = X2;
            y2 = Y2;
        }
        public void addPerpendicular(double r)
        {
            double xx = x2 - x1;
            double yy = y2 - y1;
            Vector v1 = new Vector(x2 - x1, y2 - y1);
            v1.addPerpendiclar(r);


            
            x2 = x1 + v1.x;
            y2 = y1 + v1.y;
            x1 = x1 + (v1.x - (xx));
            y1 = y1 + (v1.y - (yy));

        }

        internal void print(String name)
        {
            MessageBox.Show(name + " (" + x1.ToString() + ", " + y1.ToString() + ")" + " to " + "(" + x2.ToString() + ", " + y2.ToString() + ")"); 
        }

        internal void removeFromEnd(double lengthDeleted)
        {
            Vector v = new Vector(x2 - x1, y2 - y1);
            Vector diff = v.addMagnitude(-lengthDeleted);
            double xdiff = diff.x - v.x;
            double ydiff = diff.y - v.y;
            if (xdiff.ToString() == "NaN")
                return;
            x2 = x2 + xdiff;
            y2 = y2 + ydiff;

        }

        internal void removeFromBeginning(double lengthDeleted)
        {
            Vector v = new Vector(x2 - x1, y2 - y1);
            Vector diff = v.addMagnitude(lengthDeleted);
            double xdiff = diff.x - v.x;
            double ydiff = diff.y - v.y;
            if (xdiff.ToString() == "NaN")
                return;
            x1 = x1 + xdiff;
            y1 = y1 + ydiff;
        }

        internal Segment Copy()
        {
            return new Segment(x1, y1, x2, y2);
        }
        public override string ToString()
        {
            return "(" + x1.ToString() + "," + y1.ToString() + ")" + " to " + "(" + x2.ToString() + "," + y2.ToString() + ")";
        }

        public Line Line
        {
            get
            {
                return new Line(x1, y1, x2, y2);
            }
        }

    }
}
