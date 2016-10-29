using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

namespace SURE_Jet
{
    class Circle
    {
        public double radius;
        public Vector center;
        public Circle(Vector cent, double r)
        {
            center = cent;
            radius = r;
        }
        public override string ToString()
        {
            return "Center: (" + center.x.ToString() + "," + center.y.ToString() + ") Radius: " + radius.ToString();
        }

        internal Vector[] getIntersections(Line previousLine)
        {
            Vector[] intersections = new Vector[2];
            double A = Math.Pow(previousLine.slope,2) + 1;
            double B = -2 * center.x + 2 * previousLine.slope * (previousLine.yIntercept - center.y);
            double C = Math.Pow(center.x, 2) + Math.Pow((previousLine.yIntercept - center.y), 2) - radius * radius;
            double discriminant = B * B - 4 * A * C;
            
            if (Math.Abs(discriminant) < Math.Pow(10, -2))
                discriminant = 0;
            if (discriminant < 0){
                //MessageBox.Show("Not found Intersections between " + previousLine.ToString() + " and " + this.ToString());

              // MessageBox.Show("No intersection between circle and line.");
               //MessageBox.Show(discriminant.ToString());
               //MessageBox.Show(this.ToString() + previousLine.ToString());
                return intersections;
            }
            double x1 = (-B + Math.Sqrt(discriminant)) / (2*A);
            double x2 = (-B - Math.Sqrt(discriminant)) / (2*A);

            Vector v1 = new Vector(x1, previousLine.slope * x1 + previousLine.yIntercept);
            Vector v2 = new Vector(x2, previousLine.slope * x2 + previousLine.yIntercept);

            if (previousLine.slopeIsVertical(previousLine.slope))
            {
                x1 = previousLine.xIntercept;
                x2 = previousLine.xIntercept;
                double a = 1;
                double b = -2*center.y;
                double c = center.y*center.y + (x1 - center.x)*(x1 - center.x) - radius*radius;
                double d = Math.Sqrt(b * b - 4 * a * c);
                if (Math.Abs(b * b - 4 * a * c) < Math.Pow(10, -8))
                    d = 0;
                double y1 = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                double y2 = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                if (d == 0)
                {
                    y1 = (-b) / (2 * a);
                    y2 = (-b) / (2 * a);
                }
                if (d.ToString() == "NaN")
                {
                    v1 = null;
                    v2 = null;
                    //y1 = (-b) / (2 * a);
                    //y2 = (-b) / (2 * a);
                }
                else
                {
                    v1 = new Vector(x1, y1);
                    v2 = new Vector(x2, y2);
                }
            }
            intersections[0] = v1;
            intersections[1] = v2;
            //MessageBox.Show("Intersections between " + previousLine.ToString() + " and " + this.ToString() + " Is " + v1.ToString() + " and " + v2.ToString());
            return intersections;
        }
        internal Vector[] getIntersections(Circle circ)
        {
            double d = Math.Sqrt(Math.Pow(center.x - circ.center.x, 2) + Math.Pow(center.y - circ.center.y, 2));
            double l = (radius * radius - circ.radius * circ.radius + d * d) / (2 * d);
            double h = Math.Sqrt(radius * radius - l * l);
            if (radius * radius - l * l < Math.Pow(10, -10))
                h = 0;
            //if (radius + circ.radius < d)
            //{
            //    MessageBox.Show("No intersection");
            //    return new Vector[] { null, null };
            //}
            double x1 = l/d*(circ.center.x - center.x) + h/d*(circ.center.y - center.y) + center.x;
            double y1 = l/d*(circ.center.y - center.y) - h/d*(circ.center.x - center.x) + center.y;
            double x2 = l/d*(circ.center.x - center.x) - h/d*(circ.center.y - center.y) + center.x;
            double y2 = l/d*(circ.center.y - center.y) + h/d*(circ.center.x - center.x) + center.y;

            Vector[] inter = new Vector[2];
            inter[0] = new Vector(x1, y1);
            inter[1] = new Vector(x2, y2);
            //MessageBox.Show("Intersection between :" + circ.ToString() + " and " + this.ToString() + " is " + inter[0].ToString() + " and " + inter[1].ToString());
            return inter;

        }

    }
}
