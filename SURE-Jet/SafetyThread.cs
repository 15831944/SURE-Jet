using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AxTrioPCLib;

namespace SURE_Jet
{
    class SafetyThread
    {
        AxTrioPC trio;
        public SafetyThread(AxTrioPC ax)
        {
            trio = ax;
        }
        public void stop()
        {
            trio.RapidStop();
        }
    }
}
