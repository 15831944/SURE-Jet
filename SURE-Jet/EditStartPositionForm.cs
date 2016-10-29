using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SURE_Jet
{
    public partial class EditStartPositionForm : Form
    {
        TrioWrapper Trio;
        public EditStartPositionForm(TrioWrapper t)
        {
            InitializeComponent();
            Trio = t;
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE" + ".txt"))
            {
                String theFile = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE" + ".txt");

                String[] lines = theFile.Split('\n');
                hScrollBar1.Maximum = lines.Length;
                hScrollBar1.Minimum = 0;
            }
            this.Width = 740;
            this.Height = 492;
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            
            Clipboard.SetText(addOffsetToText(getClippedCode()));
        }

        private String addOffsetToText(String text)
        {
            String clip = text;
            String results = "";
            for (int i = 0; i < clip.Split('\n').Length; i++)
            {
                String line = clip.Split('\n')[i];
                if (line.StartsWith("MOVEABS"))//lineStartsWith(line, "MOVEABS"))
                {
                    double xVal = Convert.ToDouble(line.Split('(')[1].Split(',')[0]) + Trio.Offset.X;
                    double yVal = Convert.ToDouble(line.Split('(')[1].Split(',')[1].Split(')')[0]) + Trio.Offset.Y;
                    results += "MOVEABS(" + xVal + "," + yVal + ")\n";
                }
                else if (line.StartsWith("MOVECIRC"))//lineStartsWith(line, "MOVECIRC"))
                {
                    double xend = Convert.ToDouble(line.Split('(')[1].Split(',')[0]);
                    double yend = Convert.ToDouble(line.Split('(')[1].Split(',')[1]);
                    double xmid = Convert.ToDouble(line.Split('(')[1].Split(',')[2]);
                    double ymid = Convert.ToDouble(line.Split('(')[1].Split(',')[3]);
                    short direction = Convert.ToInt16(line.Split('(')[1].Split(',')[4].Split(')')[0]);
                    results += line + '\n';
                }
                else
                {
                    results += line + '\n';
                }
            }
            return results;
            //lastAbsolutePath = results;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE" + ".txt"))
            {
                
                String remainingCode = getClippedCode();
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE" + ".txt", remainingCode);
            }
            this.Close();
        }

        private String getClippedCode()
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE" + ".txt"))
            {
                String theFile = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE" + ".txt");
                //Regex Gcode = new Regex("moveabs", RegexOptions.IgnoreCase);
                String[] lines = theFile.Split('\n');

                //must start at a move abs becuase movecirc is relative to current location
                int startLine = hScrollBar1.Value;
                for (int i = startLine; i >= 0; i--)
                {
                    if (lines[i].StartsWith("MOVEABS"))
                    {
                        startLine = i;
                        break;
                    }
                }


                String remainingCode = "OFF\n";
                remainingCode += lines[startLine] + "\n";

                if (lines[startLine + 1].StartsWith("OFF"))
                {

                }
                else
                {
                    for (int i = startLine; i >= 0; i--)
                    {
                        if (lines[i] == "ON")
                        {
                            remainingCode += "ON\n";
                            break;
                        }
                        else if (lines[i] == "OFF")
                        {
                            break;
                        }

                    }
                }



                for (int i = startLine + 1; i < lines.Length; i++)
                {
                    remainingCode += lines[i] + "\n";
                }

                return remainingCode;
                //Clipboard.SetText(remainingCode);
                //enableAllButtons(false);

            }
            else
            {
                MessageBox.Show("No G-Code on file");
                return "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
