using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURE_Jet
{
    public class MotionPath
    {
        public String motionCommands = "";
        public int onLine = 0;
        //should these be NaN??
        public List<Vector> previousPositions = new List<Vector>();

        public MotionPath(string theFile, int p)
        {
            this.motionCommands = theFile;
            this.onLine = p;
        }
        public bool isDone
        {
            get
            {
                return onLine >= motionCommands.Split('\n').Length;
            }
        }
        public override string ToString()
        {
            return "Motion Path:\n" + onLine.ToString() + "\n" + motionCommands;
        }
    }
}
