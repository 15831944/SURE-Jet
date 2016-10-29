using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxTrioPCLib;
using System.IO;

namespace SURE_Jet
{
    public class Offset
    {
        public Offset(AxTrioPC ax)
        {
            readFromFile();
        }
        private double x;
        public double X
        {
            get{
                return x;
            }
            set{
                x = value;
                writeToFile();
            }
        }

        private double y;
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                writeToFile();
            }
        }

        private void writeToFile()
        {
            String file = "X:" + x.ToString() + "\n" + "Y:" + y.ToString();
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "Offset" + ".txt", file);
        }

        public void readFromFile()
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "Offset" + ".txt"))
            {
                String file = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "Offset" + ".txt");
                x = Double.Parse(file.Split('\n')[0].Split(':')[1]);
                y = Double.Parse(file.Split('\n')[1].Split(':')[1]);
            }
        }
        

        public void reset()
        {
            X = 0;
            Y = 0;
        }
    }
}
