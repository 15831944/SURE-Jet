using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURE_Jet
{
    public class Cut
    {
        public bool switched = false;
        public Vector p1;
        public Vector p2;
        public String info;
        public Cut(Vector P1, Vector P2, String i, bool b)
        {
            p1 = P1;
            p2 = P2;
            info = i;
            switched = b;
        }
        public Cut(Vector P1, Vector P2, String i)
        {
            p1 = P1;
            p2 = P2;
            info = i;
        }
        public override string ToString()
        {
            return "Start: " + p1.ToString() + " End: " + p2.ToString() + " Info: " + info + " switched: " + switched.ToString(); 
        }
        public Cut Copy()
        {
            return new Cut(p1, p2, info, switched);
        }
    }
}
