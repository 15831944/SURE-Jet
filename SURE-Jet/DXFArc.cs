using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURE_Jet
{
    public class DXFArc
    {
        double xmidAbs;// = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[0]);
        double ymidAbs;// = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[1]);
        double radius;// = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[2]);
        double angle1;// = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[3]) * Math.PI / 180.0;
        double angle2;// = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[4]) * Math.PI / 180.0;
        public DXFArc(double x, double y, double r, double a1, double a2){
            xmidAbs = x;
            ymidAbs = y;
            radius = r;
            angle1 = a1;
            angle2 = a2;
        }
        public override string ToString()
        {
            return "dxfarc: xmid: " + xmidAbs.ToString() + " ymid: " + ymidAbs.ToString() + " radius: " + radius.ToString() + " angle1: " + angle1.ToString() + " angle2: " + angle2.ToString();
        }
    }
}
