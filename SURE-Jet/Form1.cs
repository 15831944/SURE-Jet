using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
//using AxTrioPCLib;
using System.Threading;
using SlimDX;
using MathNet;
//using Autodesk.AutoCAD.Geometry;


namespace SURE_Jet
{
    public partial class Form1 : Form
    {
        protected short m_iPortID;
        Offset Offset;
        TrioWrapper Trio;
        bool checkChanging = false;
        System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Orange);
        System.Drawing.Graphics speedGraphics;
        System.Drawing.Graphics speed2Graphics;
        bool workerActive = false;
        String lastAbsolutePath = "";
        bool stopButtonPressed = false;
        SlimDX.XInput.Controller controller = new SlimDX.XInput.Controller(SlimDX.XInput.UserIndex.One);
        bool manualModeController = true;
        //String polygonString = "";
        String triangle = "(5,5)\n(5,15)\n(20,5)\n(5,5)\n(5,7)\n";
        String goofy = "(5,5)\n(5,20)\n(5,10)\n(30,10)\n(20,10)\n(0,0)";
        String testPoly = "(3,0)\n(0,2)\n(6,4)\n(0,10)\n(6,14)\n(0,20)\n(6,23)\n(0,30)\n(6,35)\n(0,37)\n(6,40)\n(0,42)\n(6,44)\n(0,46)\n(6,48)\n(0,50)";
        String zigzag = "(3,0)\n(0,2)\n(6,4)\n(0,10)\n(6,14)\n(0,20)\n(6,23)\n(0,30)\n(6,35)\n(0,37)\n(6,40)\n(0,42)\n(6,44)\n(0,46)\n(6,48)\n(0,50)";
        MotionPath currentPath;
        bool pauseMode = false;
        double analogJoyStickRange = 25767.0;
        private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();

        public Form1()
        {
            InitializeComponent();
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL");
            }
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet");
            }
            this.Size = new Size(1024, 719);
            this.axTrioPC.Size = new Size(closeButton.Width,closeButton.Height / 2);
            this.axTrioPC.Location = new Point(closeButton.Location.X, closeButton.Location.Y + closeButton.Height + 10);
            Offset = new Offset(axTrioPC);
            this.CenterToScreen();
            Trio = new TrioWrapper(axTrioPC, Offset);
            cutterdiamlabel.Text = "Cutter Diameter (in): " + Trio.cutterDiameter.ToString();
            leadInLengthLabel.Text = "Lead-In Length (in): " + Trio.leadInLength.ToString();
            pierceDelayLabel.Text = "Pierce Delay (s): " + Trio.PierceDelay.ToString();
            RefreshValues();
            
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker3.WorkerReportsProgress = true;
            backgroundWorker3.WorkerSupportsCancellation = true;
            speedGraphics = speedPB.CreateGraphics();
            speed2Graphics = speed2PB.CreateGraphics();
            double middle = speedPB.Width / 2;
            double speedPercentage = (middle) / speedPB.Width * 2;//goes to 200%
            //Trio.PercentageSpeedOverride = speedPercentage;
            //Trio.PercentageSpeedOverride2 = 1;
            repaintSpeedBoxes();
            speedOverrideLabel.Text = "Speed Override: " + (Math.Round(Trio.PercentageSpeedOverride, 2) * 100).ToString() + "%";
            feedSpeedTB.Text = Trio.feedSpeed.ToString();
            rapidSpeedTB.Text = Trio.rapidSpeed.ToString();
            //polygonTextBox.Text = testPoly;
            //backgroundWorker3.RunWorkerAsync(); //this is the safety thread. MUST be running at all times
           //updatePolygonListBox();
            showDeveloperFeatures(false);

            _timer.Interval = 300;
            _timer.Tick += TimerTick;
            _timer.Enabled = true;
            _timer.Stop();
            //_timer.Start();
        }
        void TimerTick(object sender, EventArgs e)
        {
            Console.WriteLine("Running timer.");
            ////read the inputs. 1 should be 0, 2 should be 1, and 3 should be 0.
            ////input 0 caused the machine to set the speed to 0, not the watchdog. Fucking weird couldn't figure it out.
            //int input2 = Trio.digitalRead(2);
            //int input3 = Trio.digitalRead(3);
            //int input4 = Trio.digitalRead(4);

            //if (input2 != 0 || input3 != 1 || input4 != 0)
            //{
            //    //stop, hit the boundaries
            //    Trio.axTrioPC.RapidStop();
            //    Trio.turnOffWatchDogAndServos(true);
            //    Console.WriteLine("STOP! inputs: " + input2.ToString() + input3.ToString() + input4.ToString());
            //    Trio.Cancelled = true;
            //    backgroundWorker1.CancelAsync();
            //    manualModeController = false;
            //    backgroundWorker2.CancelAsync();
            //    Trio.turnOffWatchDogAndServos(true);
            //    Trio.WaterOn = false;
            //    Trio.AbrasiveOn = false;
            //    _timer.Stop();
            //    MessageBox.Show("An endstop was hit. This is not supposed to happen.");
               

            //}
            Console.WriteLine("Before");
            double xmpos = Trio.getVariable("X", "MPOS");
            double yMpos = Trio.getVariable("Y", "MPOS");
            Console.WriteLine("after");
            Console.WriteLine(xmpos);
            if(xmpos == -1.0)
            {
                _timer.Stop();
                MessageBox.Show("Communication failed and DRO has stopped.");
                
            }

            xCurrentLocation.Text = "X: " + Math.Round(xmpos - Trio.Offset.X, 3).ToString();
            yCurrentLocation.Text = "Y: " + Math.Round((yMpos - Trio.Offset.Y), 3).ToString();
            xAbsoluteLocation.Text = "X: " + Math.Round(xmpos, 3).ToString();
            yAbsoluteLocation.Text = "Y: " + Math.Round(yMpos, 3).ToString();
        }

        private void showDeveloperFeatures(bool p)
        {
            button1.Visible = p;
            button7.Visible = p;
            resetHome.Visible = p;
            button8.Visible = p;
            button9.Visible = p;
            button15.Visible = p;
            //GCodeButton.Visible = p;
            button10.Visible = p;
            button11.Visible = p;
            button24.Visible = p;
            button23.Visible = p;
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            
            short iPortID = -1,
                iPortType = -1;

            openButton.Enabled = false;

            Trio.axTrioPC.HostAddress = "192.168.000.250";
            iPortType = 2;
            iPortID = 3240;
              

            if ((iPortType != -1) && (iPortID != -1))
                Trio.axTrioPC.Open(iPortType, iPortID);

            if (Trio.axTrioPC.IsOpen(iPortID))
            {
                //_timer.Start();
                closeButton.Enabled = true;
                m_iPortID = iPortID;
                Trio.axTrioPC.Refresh();
                RefreshValues();
                //Trio.setSpeed(true,1);
                repaintSpeedBoxes();
                
                Trio.setDefaultValues();
                Trio.turnOnWatchDogAndServos(true);

                //if (checkLimitsCB.Checked)
                //{
                    //if (Math.Abs(Trio.getVariable("X", "FS_LIMIT") - Trio.xLimitUpper) > .1)
                    //{
                        double currentXEncoder = Trio.getVariable("X", "ENCODER");
                        double Xposition = Math.Abs(currentXEncoder - Trio.XEncoderZero) / Trio.Units * Trio.YtoXEncoderRatio;
                        double currentYEncoder = Trio.getVariable("Y", "ENCODER");
                        double YPosition = Math.Abs(currentYEncoder - Trio.YEncoderZero) / Trio.Units;// / Trio.YtoXEncoderRatio;//might be *
                        Trio.setMeasuredPosition("X", "Y", Xposition, YPosition);
                        Trio.setLimits();
                        Trio.axTrioPC.Run("ENDSTOP");


                        //Trio limits are not set. Find absolute zero.
                        //MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                        //DialogResult result;

                        //// Displays the MessageBox.

                        //result = MessageBox.Show("The limits on the Trio are not correctly set. " +
                        //    "The machine must find the absolute zero and set its limits."
                        //    , "Machine needs to be configured", buttons);

                        //if (result == System.Windows.Forms.DialogResult.OK)
                        //{
                        //    setAbsoluteZeroAndLimits();
                        //    MessageBox.Show("Done");
                        //}
                        //else
                        //{
                        //    Trio.turnOffWatchDogAndServos(true);
                        //    Trio.axTrioPC.Close();
                        //    _timer.Stop();
                        //    RefreshValues();
                        //    return;
                        //}

                   // }
                //}


                //Trio.setLimits();
                

                //Trio.Offset.X = Trio.getVariable("X", "MPOS");
                //Trio.Offset.Y = Trio.getVariable("Y", "MPOS");
                Trio.turnOffJog(true);

                RefreshValues();

                    
            }
            else
            {
                openButton.Enabled = true;
                m_iPortID = 0;
            }
            updateSpeedLabels();
            _timer.Start();
        }

        private void RefreshValues()
        {
            offsetXLabel.Text = Math.Round(Offset.X,3).ToString();
            offsetYLabel.Text = Math.Round(Offset.Y,3).ToString();
            runningLabel.Text = workerActive.ToString();
            runningLabel.Refresh();
            if (Trio.axTrioPC.IsOpen(m_iPortID) && !workerActive)
            {
                enableAllButtons(true);
            }
            else
            {
                enableAllButtons(false);
            }
        }

        private void enableAllButtons(bool p)
        {
            makeCircleButton.Enabled = p;
            makeRectangleButton.Enabled = p;
            makeLineButton.Enabled = p;
            runGCodeButton.Enabled = p;
            offsetButton.Enabled = p;
            setZeroButton.Enabled = p;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(this.Width.ToString() + " " + this.Height.ToString());
            if (Trio.axTrioPC.IsOpen(0))
            {
                closeButton.Enabled = false;
                Trio.axTrioPC.Close(0);
                openButton.Enabled = true;
                Trio.axTrioPC.Refresh();
            }
            RefreshValues();
        }

        private void makeCircleButton_Click(object sender, EventArgs e)
        {
            if (!checkCircleInputs())
            {
                MessageBox.Show("Invalid circle input.");
                return;
            }
            writeCircleFile();


            enableAllButtons(false);
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                updateLogFile("CIRCLE");
                currentPath = new MotionPath(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "CIRCLE CODE" + ".txt"), 0);
                backgroundWorker1.RunWorkerAsync(currentPath);
            }
            Clipboard.SetText(addOffsetToText(Clipboard.GetText()));
        }

        private void updateLogFile(string p)
        {
            String exist = "";
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "LOGFILE" + ".txt"))
            {
                exist = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "LOGFILE" + ".txt");
            }
            exist += "¥:" + DateTime.Now.ToString() + "\n";
            if (p == "CIRCLE")
            {
                exist += ":CIRCLE\nCenter:" + circleDiameterTB.Text + "\nX:" + circleXLocationTB.Text + "\nY:" + circleYLocationTB.Text + "\nCutInside:" + cutInsideCircleCB.Checked.ToString() + "\n";
            }
            if (p == "RECTANGLE")
            {
                exist += ":RECTANGLE\nX:" + recXLocationTB.Text + "\nY:" + recYLocationTB.Text + "\nHeight:" + recHeightTB.Text + "\nWidth:" + recWidthTB.Text + "\nCutInside:" + cutInsideRectangleCB.Checked.ToString() + "\n";
            }
            if (p == "POLYLINE")
            {
                exist += ":POLYLINE:Radius:" + filletRadiusTB.Text + ":LeftSide:" + leftOfLineCB.Checked.ToString() + ":Middle:" + centerOfLineCB.Checked.ToString() + "\n" + polygonTextBox.Text;
            }
            if (p == "GCODE")
            {
                exist += ":GCODE:\n"; 
            }
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "LOGFILE" + ".txt", exist);
        }

        
        private bool isMoving()
        {
            axTrioPC.Base(1, 1);
            double speed;
           
            axTrioPC.GetVariable("MSPEED", out speed);
            axTrioPC.Base(1, 2);
            double speed2;
            axTrioPC.GetVariable("MSPEED", out speed2);

            return Math.Abs(speed) > .5 || Math.Abs(speed2) > .5;

        }

        private bool checkCircleInputs()
        {
            try
            {
                double xLocation = Convert.ToDouble(circleXLocationTB.Text);
                double yLocation = Convert.ToDouble(circleYLocationTB.Text);
                double radius = Convert.ToDouble(circleDiameterTB.Text);
            }
            catch (Exception e)
            {
                return false;
            }
            if (!cutInsideCircleCB.Checked && !cutOutsideCircleCB.Checked)
            {
                return false;
            }
            return true;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.Height.ToString() + "  " + this.Width.ToString());

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!checkRectangleInputs())
            {
                MessageBox.Show("Invalid rectangle input");
                return;
            }
            writeRectangleFile();

            enableAllButtons(false);
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                updateLogFile("RECTANGLE");
                currentPath = new MotionPath(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "RECTANGLE CODE" + ".txt"), 0);
                backgroundWorker1.RunWorkerAsync(currentPath);
            }
            Clipboard.SetText(addOffsetToText(Clipboard.GetText()));
        }

        private bool checkRectangleInputs()
        {
            try
            {
                double xLocation = Convert.ToDouble(recXLocationTB.Text);
                double yLocation = Convert.ToDouble(recYLocationTB.Text);
                double height = Convert.ToDouble(recHeightTB.Text);
                double width = Convert.ToDouble(recWidthTB.Text);
            }
            catch
            {
                return false;
            }
            if (!(cutInsideRectangleCB.Checked || cutOutsideRectangleCB.Checked))
            {
                return false;
            }
            
            return true;
        }

        private void GCodeButton_Click(object sender, EventArgs e)
        {
            //cases:
            //line to line any angle: Go to intersection of the lines offset by the radius.
            //line to arc: Use vectors, add the radius
            //arc to line: 
            String TrioBasicCode = "";
            String currentSpeed = "";
            String theFile = "";
            OpenFileDialog file = new OpenFileDialog();
            file.FileName = "";
            file.Title = "Open A Text document.";
            file.Filter = "(*.txt)|*.txt";
            DialogResult result = file.ShowDialog();
            if (result == DialogResult.OK)
            {
                System.IO.StreamReader OpenFile = new System.IO.StreamReader(file.FileName);
                theFile = OpenFile.ReadToEnd();
                OpenFile.Close();

                //Read X Y Z coordinates from the opened file:

                Regex Gcode = new Regex("[nfgxyzfijm][+-]?[0-9]*\\.?[0-9]*", RegexOptions.IgnoreCase);
                MatchCollection m = Gcode.Matches(theFile);

                int g_code = 0;
                int m_code = 0;
                double x_code = 0, y_code = 0, i_code, j_code, f_code;
                double xpos = 0;
                double ypos = 0;

                String[] lines = theFile.Split('\n');
                string lastCommand = "";
                int lastArcDirection = -1;
                for (int i = 1; i < lines.Length; i++)
                {
                    //Console.WriteLine("Processing Line: " + i.ToString());
                    String line = lines[i];
                    g_code = -1;
                    x_code = 0;
                    y_code = 0;
                    i_code = 0;
                    j_code = 0;
                    m_code = 0;
                    f_code = 0;
                    if ((line.Length > 0 && line.Substring(0, 1) == "("))
                        continue; //comments

                    m = Gcode.Matches(lines[i]);
                    bool xValChanged = false;
                    bool yValChanged = false;
                    




                    foreach (Match n in m)
                    {
                        if (n.Value.StartsWith("G"))
                        {
                            g_code = Convert.ToInt32(ExtractNumbers(n.Value));
                            if (g_code == 40)
                                TrioBasicCode += "CUTTER COMPENSATION OFF\n";
                            if (g_code == 41)
                                TrioBasicCode += "CUTTER COMPENSATION LEFT\n";
                            if (g_code == 42)
                                TrioBasicCode += "CUTTER COMPENSATION RIGHT\n";
                            if (g_code == 0)
                            {
                                //rapid speed
                                if (currentSpeed != "RAPID")
                                {
                                    currentSpeed = "RAPID";
                                    TrioBasicCode += "SPEED = RAPID\n";
                                }
                            }
                            if(g_code == 1){
                                //feed speed
                                if(currentSpeed != "FEED"){
                                    currentSpeed = "FEED";
                                    TrioBasicCode += "SPEED = FEED\n";
                                }
                            }
                            if (g_code == 28)
                            {
                                TrioBasicCode += "MOVEABS(0,0)\n";
                            }

                        }

                        if (n.Value.StartsWith("X"))
                        {
                            x_code = Convert.ToDouble(ExtractNumbers(n.Value));
                            xValChanged = true;
                        }

                        if (n.Value.StartsWith("Y"))
                        {
                            y_code = Convert.ToDouble(ExtractNumbers(n.Value));
                            yValChanged = true;
                        }

                        if (n.Value.StartsWith("I"))
                        {
                            i_code = Convert.ToDouble(ExtractNumbers(n.Value));
                        }

                        if (n.Value.StartsWith("J"))
                        {
                            j_code = Convert.ToDouble(ExtractNumbers(n.Value));
                        }

                        if (n.Value.StartsWith("Z"))
                        {
                            MessageBox.Show("Z Coordinate Detected at line: " + i.ToString());
                        }
                        if (n.Value.StartsWith("M"))
                        {
                            m_code = Convert.ToInt32(ExtractNumbers(n.Value));
                        }
                        if (n.Value.StartsWith("F"))
                        {
                            String num = ExtractNumbers(n.Value);
                            f_code = Convert.ToDouble(num);
                        }
                    }




                    string nextCommand = "";
                    string onCommand = "";
                    switch (m_code)
                    {
                        case 70:
                            onCommand = "ON";
                            break;
                        case 71:
                            onCommand = "OFF";
                            break;
                        default:
                            break;
                    }
                    switch (g_code)
                    {
                        case 0:
                            //Rapid Position, moveABS, adjust speed
                            if (xValChanged)
                                xpos = x_code;
                            if (yValChanged)
                                ypos = y_code;
                            if (xValChanged || yValChanged)
                            {
                                    nextCommand = "MOVEABS(" + xpos + "," + ypos + ")\n";
                            }
                            break;
                        case 1:
                            if (xValChanged)
                                xpos = x_code;
                            if (yValChanged)
                                ypos = y_code;
                            if (xValChanged || yValChanged)
                            {
                                    nextCommand = "MOVEABS(" + xpos + "," + ypos + ")\n";
                            }
                            break;
                        case 2:
                            //Clockwise circle
                            double xEndMoveCirc = 0;
                            if (xValChanged)
                            {
                                xEndMoveCirc = x_code - xpos;
                            }
                            double yEndMoveCirc = 0;
                            if (yValChanged)
                            {
                                yEndMoveCirc = y_code - ypos;
                            }

                            nextCommand = "MOVECIRC(" + xEndMoveCirc + "," + yEndMoveCirc + "," + i_code + "," + j_code + "," + "1)\n";
                            if (xValChanged)
                                xpos = x_code;
                            if (yValChanged)
                                ypos = y_code;
                            lastArcDirection = 1;
                            break;
                        case 3:
                            //Counter Clockwise circle
                            Console.WriteLine("Case 3 on line: " + i.ToString() + " : " + line);
                            double xEndMoveCirc1 = 0;
                            if (xValChanged)
                            {
                                xEndMoveCirc1 = x_code - xpos;
                            }
                            double yEndMoveCirc1 = 0;
                            if (yValChanged)
                            {
                                yEndMoveCirc1 = y_code - ypos;
                            }

                            nextCommand = "MOVECIRC(" + xEndMoveCirc1 + "," + yEndMoveCirc1 + "," + i_code + "," + j_code + "," + "0)\n";
                            if (xValChanged)
                                xpos = x_code;
                            if (yValChanged)
                                ypos = y_code;
                            lastArcDirection = 0;
                            break;
                        case 4:
                            //dwell command
                            nextCommand = "SLEEP(" + x_code.ToString() + ")\n";

                            break;
                        default:
                            Console.WriteLine("Default on line: " + i.ToString() + " ; " + line);
                            if (line.Contains("I") || line.Contains("J"))
                            {
                                //Console.WriteLine("Found I and J in line. Arc diretion: " + lastArcDirection.ToString());
                                //circle move in same direction as last commanded circle move.
                                if (lastArcDirection == 1)
                                {
                                    Console.WriteLine("one icode: " + i_code.ToString());
                                    Console.WriteLine("one jcode: " + j_code.ToString());
                                    double xEndMoveCirc3 = 0;
                                    if (xValChanged)
                                    {
                                        xEndMoveCirc3 = x_code - xpos;
                                    }
                                    double yEndMoveCirc3 = 0;
                                    if (yValChanged)
                                    {
                                        yEndMoveCirc3 = y_code - ypos;
                                    }

                                    nextCommand = "MOVECIRC(" + xEndMoveCirc3 + "," + yEndMoveCirc3 + "," + i_code + "," + j_code + "," + "1)\n";
                                    if (xValChanged)
                                        xpos = x_code;
                                    if (yValChanged)
                                        ypos = y_code;
                                    lastArcDirection = 1;
                                }
                                else if (lastArcDirection == 0)
                                {
                                    //Counter Clockwise circle
                                    double xEndMoveCirc4 = 0;
                                    if (xValChanged)
                                    {
                                        xEndMoveCirc4 = x_code - xpos;
                                    }
                                    double yEndMoveCirc4 = 0;
                                    if (yValChanged)
                                    {
                                        yEndMoveCirc4 = y_code - ypos;
                                    }

                                    nextCommand = "MOVECIRC(" + xEndMoveCirc4 + "," + yEndMoveCirc4 + "," + i_code + "," + j_code + "," + "0)\n";
                                    if (xValChanged)
                                        xpos = x_code;
                                    if (yValChanged)
                                        ypos = y_code;
                                    lastArcDirection = 0;
                                }
                            }
                            else
                            {
                                if (xValChanged)
                                    xpos = x_code;
                                if (yValChanged)
                                    ypos = y_code;
                                if (xValChanged || yValChanged)
                                    nextCommand = "MOVEABS(" + xpos + "," + ypos + ")\n";
                            }
                            break;

                    }
                    if (onCommand != "")
                    {
                        TrioBasicCode += onCommand + '\n';
                    }
                    if (f_code != 0.0)
                        TrioBasicCode += "FEEDSPEED =" + f_code / 60.0 + "\n";

                    if (nextCommand != "")
                    {
                        //if (lastCommand != nextCommand)
                            TrioBasicCode += nextCommand;
                        //MessageBox.Show("Last Command :" + lastCommand + " Next Command: " + nextCommand);

                        lastCommand = nextCommand;
                    }

                }

                //Clipboard.SetText(TrioBasicCode);
                Clipboard.SetText(addOffsetToText(TrioBasicCode));

                if (!withinBoundaries(Clipboard.GetText()))
                {
                    MessageBox.Show("Not Within Boundaries!");
                }
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE" + ".txt", TrioBasicCode);
               
                MessageBox.Show("Latest G-Code program is on file");
            }
        }

        private string ExtractNumbers(string p)
        {
            string b = string.Empty;
            for (int i = 0; i < p.Length; i++)
            {
                if (Char.IsDigit(p[i]) || p[i] == '.' || p[i] == '-')
                    b += p[i];
            }
            if (b.EndsWith("."))
                b = b.Substring(0, b.Length - 1);
            return b;
        }

        private void runGCodeButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE" + ".txt"))
            {
                String theFile = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE" + ".txt");
                //Regex Gcode = new Regex("moveabs", RegexOptions.IgnoreCase);
                Clipboard.SetText(theFile);
                enableAllButtons(false);
                if (backgroundWorker1.IsBusy != true)
                {
                    // Start the asynchronous operation.
                    updateLogFile("GCODE");
                    currentPath = new MotionPath(theFile, 0);
                    backgroundWorker1.RunWorkerAsync(currentPath);

                }
            }
            else
            {
                MessageBox.Show("No G-Code on file");
            }
            Clipboard.SetText(addOffsetToText(Clipboard.GetText()));
        }

        private void runBasicCode(MotionPath m)
        {
            Trio.turnOffJog(true);
            String theFile = m.motionCommands;
            theFile = addOffsetToText(theFile);
            if (!withinBoundaries(theFile)) //this doesnt have fucking offset inside it
            {
                MessageBox.Show("Not within boundaries, cancelling cut");
                return;
            }
            if(SureJetSettings.writeToConsole)
                Console.WriteLine("starting run basic code. Lines of code:" + m.motionCommands.Split('\n').Length.ToString());
            currentPath = m;
            //Trio.turnOnWatchDogAndServos(true);
            pauseMode = false;
            Trio.turnOnMerge();
            
            //bool cont = true;
            //if (m.onLine != 0)//resuming previous cut
            //{
            //    if (SureJetSettings.writeToConsole)
            //        Console.WriteLine("Resuming previous cut");
            //    Trio.Base("X");
            //    Thread.Sleep(10);
            //    if (currentPath.previousPositions.Count > 2)
            //    {
            //        double xVal = currentPath.previousPositions[currentPath.previousPositions.Count - 3].x;
            //        double yVal = currentPath.previousPositions[currentPath.previousPositions.Count - 3].y;
            //        Trio.moveAbs("X","Y",xVal, yVal,Trio.Offset.X,Trio.Offset.Y);//move was cancelled
            //    }
            //}
            //if (cont)
            //{
                bool firstMoveDone = m.onLine != 0;
                Trio.turnOffJog(true);

                for (int i = m.onLine; i < theFile.Split('\n').Length; i++)
                {
                    backgroundWorker1.ReportProgress((int)((double)i / (double)theFile.Split('\n').Length * 100.0));
                    //buffer one move ahead so as to keep the place... can't just add em all if i want to stop at any point then restart
                    //so don't wait on the first one and then wait on the next
                    String line = theFile.Split('\n')[i];
                    //Trio.Base("X");
                    Thread.Sleep(10);//This is important. Sometimes it doesn't happen in time. Sometimes.
                    //MatchCollection m = Gcode.Matches(theFile);
                    if (SureJetSettings.writeToConsole)
                        Console.WriteLine(i);
                    if (line.StartsWith("MOVEABS"))//lineStartsWith(line, "MOVEABS"))
                    {
                        //currentPath.onLine = i;//only update if its a moveabs command

                        double xVal = Convert.ToDouble(line.Split('(')[1].Split(',')[0]);
                        double yVal = Convert.ToDouble(line.Split('(')[1].Split(',')[1].Split(')')[0]);
                        Console.WriteLine("Sending moveabs command: " + xVal.ToString() + " , " + yVal.ToString() +
                            Trio.Offset.X.ToString() + " , " + Trio.Offset.Y);
                        //Trio.moveAbs("X", "Y", xVal, yVal, Trio.Offset.X, Trio.Offset.Y);//move was cancelled
                        Trio.moveAbs("X", "Y", xVal, yVal);//, 0, 0);
                        //currentPath.previousPositions.Add(new Vector(xVal, yVal));
                    }
                    else if (line.StartsWith("MOVECIRC"))//lineStartsWith(line, "MOVECIRC"))
                    {
                        
                        //currentPath.onLine = i;

                        double xend = Convert.ToDouble(line.Split('(')[1].Split(',')[0]);
                        double yend = Convert.ToDouble(line.Split('(')[1].Split(',')[1]);
                        double xmid = Convert.ToDouble(line.Split('(')[1].Split(',')[2]);
                        double ymid = Convert.ToDouble(line.Split('(')[1].Split(',')[3]);
                        short direction = Convert.ToInt16(line.Split('(')[1].Split(',')[4].Split(')')[0]);
                        Console.WriteLine("Sending: " + "MOVECIRC(" + xend.ToString() + "," + yend.ToString() + "," + xmid.ToString() + "," + ymid.ToString() + "," + direction.ToString() + ")");
                        Trio.MoveCircular(xend, yend, xmid, ymid, direction);//move was cancelled
                        //Vector prev = new Vector(0,0);
                        //if(currentPath.previousPositions.Count != 0)
                        //    prev = currentPath.previousPositions[currentPath.previousPositions.Count - 1];
                        //currentPath.previousPositions.Add(new Vector(prev.x + xend, prev.y + yend));
                    }
                    else if (line.StartsWith("ON"))//lineStartsWith(line, "ON"))
                    {
                        Console.WriteLine("Before Wait for end of Move.");
                        if (!Trio.waitForEndOfMove(true))
                        {//TIME
                            if (SureJetSettings.writeToConsole)
                                Console.WriteLine("Broke from on command at i:" + i.ToString());
                            break;
                        }
                        if (!dryModeCheckBox.Checked)
                        {
                            Trio.WaterOn = true; //listener
                        }
                        Console.WriteLine("After wait for end of move.");
                    }
                    else if (line.StartsWith("OFF"))//lineStartsWith(line, "OFF"))
                    {
                        if (!Trio.waitForEndOfMove(true))
                        { //TIME
                            if (SureJetSettings.writeToConsole)
                                Console.WriteLine("Broke from off command at: " + i.ToString());
                            break;
                        }
                        Trio.WaterOn = false;
                    }
                    else if (line.StartsWith("SPEED"))//lineStartsWith(line, "SPEED"))
                    {
                        if (!Trio.waitForEndOfMove(true))
                        {//TIME
                            if (SureJetSettings.writeToConsole)
                                Console.WriteLine("Broke from speed command at: " + i.ToString());
                            break;
                        }
                        if (line.Split('=')[1] == " RAPID")
                        {
                            Trio.SpeedMode = 2;
                            updateSpeedLabelColor();
                        }
                        if (line.Split('=')[1] == " FEED")
                        {
                            Trio.SpeedMode = 1;
                            updateSpeedLabelColor();
                        }
                    }
                    else if (line.StartsWith("MOVE TO HOME POSITION"))//lineStartsWith(line, "MOVE TO HOME POSITION"))
                    {
                        currentPath.onLine = i;

                        Trio.moveAbs("X", "Y", 0, 0);// Trio.Offset.X, Trio.Offset.Y);
                        currentPath.previousPositions.Add(new Vector(0, 0));
                    }
                    else if (line.StartsWith("SLEEP"))
                    {
                        MessageBox.Show("Click OK to continue");
                        //currentPath.onLine = i;
                        //double sleep = Double.Parse(line.Split('(')[1].Split(')')[0]);
                        //Thread.Sleep((int)sleep * 1000);
                    }
                    else if (line.StartsWith("FEEDSPEED"))
                    {
                        if (!Trio.waitForEndOfMove(true))
                        {//TIME
                            if (SureJetSettings.writeToConsole)
                                Console.WriteLine("Broke from speed command at: " + i.ToString());
                            break;
                        }
                        double speed = Convert.ToDouble(line.Split('=')[1]);
                        Trio.feedSpeed = speed;
                        Trio.PercentageSpeedOverride = Trio.PercentageSpeedOverride; //forces speed update
                        //if (line.Split('=')[1] == " RAPID")
                        //{
                        //    Trio.SpeedMode = 2;
                        //    updateSpeedLabelColor();
                        //}
                        //if (line.Split('=')[1] == " FEED")
                        //{
                        //    Trio.SpeedMode = 1;
                        //    updateSpeedLabelColor();
                        //}
                    }
                    Thread.Sleep(10);
                    if (!mergeModeCheckBox.Checked)
                    {
                        Trio.waitForEndOfMove(true);
                    }
                    else
                    {
                        while (Trio.getVariable("X", "MOVES_BUFFERED") >= 1)
                        {
                            if (Trio.Cancelled == true)
                            {
                                break;
                            }
                            Thread.Sleep(10);
                        }
                    }
                    
                    if (Trio.Cancelled == true)
                    {
                        break;
                    }//TIME
                    //    //wait
                    //    Thread.Sleep(10);
                    //    if (SureJetSettings.writeToConsole)
                    //    Console.Write(".");
                    //    if (Trio.Cancelled == true)
                    //    {
                    //        if (SureJetSettings.writeToConsole)
                    //        Console.WriteLine("Broke out at: " + i.ToString());
                    //        i = 10000000;//breaks out of for loop
                    //        break;
                    //    }
                    //}
                    //Console.WriteLine("MOVES_BUFFERED:" + Trio.getVariable("X", "MOVES_BUFFERED").ToString());
                    
                }
            //}
            //if (m.onLine > 1)
            //    m.onLine = m.onLine - 1;//This is for the pauseMode offset.
            //while (Trio.getVariable("X", "MOVES_BUFFERED") > .5 || (int)Trio.getVariable("Y", "MOVES_BUFFERED") > .5) { Thread.Sleep(10); }
            Trio.waitForEndOfMove(true);
            if(controller.IsConnected){
                SlimDX.XInput.Vibration v = new SlimDX.XInput.Vibration();
                v.LeftMotorSpeed = 30000;
                v.RightMotorSpeed = 30000;
                controller.SetVibration(v);
                Thread.Sleep(1000);
                v.LeftMotorSpeed = 0;
                v.RightMotorSpeed = 0;
                controller.SetVibration(v);
            }
            backgroundWorker1.ReportProgress(0);
            speedOverrideLabel.ForeColor = Color.Black;
            speed2OverrideLabel.ForeColor = Color.Black;
            Trio.turnOffMerge();
            //Trio.turnOffWatchDogAndServos(true);
            manualModeController = true;
            if (SureJetSettings.writeToConsole)
            Console.WriteLine("Exiting run basic code");
        }

        private void updateSpeedLabelColor()
        {
            if (Trio.speedMode == 1){
                speedOverrideLabel.ForeColor = Color.Red;
                speed2OverrideLabel.ForeColor = Color.Black;
            }
            else
            {
                speed2OverrideLabel.ForeColor = Color.Red;
                speedOverrideLabel.ForeColor = Color.Black;
            }

        }
        private bool lineStartsWith(string line, string s)
        {
            if (s.Length > line.Length)
                return false;
            return line.Substring(0, s.Length) == s;
        }

        private void offsetButton_Click(object sender, EventArgs e)
        {
            backgroundWorker2.CancelAsync();
            new OffsetForm(Offset, Trio).ShowDialog();
            RefreshValues();
            backgroundWorker2.RunWorkerAsync();
            //moveToOffset, returns back if they didnt accept the changed offset
            //Trio.turnOnWatchDogAndServos();
            //Trio.moveAbs(0, 0);
            //Trio.waitForEndOfMovement();
            //Trio.turnOffWatchDogAndServos();

            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "CURRENT OFFSET" + ".txt", "MOVEABS(0,0)");
            //enableAllButtons(false);
            //if (backgroundWorker1.IsBusy != true)
            //{
            //    // Start the asynchronous operation.
            //    backgroundWorker1.RunWorkerAsync(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "CURRENT OFFSET" + ".txt"));
            //}
            //this returns to the offset.. decided to take it outta here
        }

        private void resetHome_Click(object sender, EventArgs e)
        {
            Trio.setAbsoluteZeroHere("X", "Y");
        }

        private void makeLineButton_Click(object sender, EventArgs e)
        {
            writePolylineFile();
            enableAllButtons(false);
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                updateLogFile("POLYLINE");
                currentPath = new MotionPath(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "POLYLINE CODE" + ".txt"), 0);
                backgroundWorker1.RunWorkerAsync(currentPath);
                
            }
            Clipboard.SetText(addOffsetToText(Clipboard.GetText()));
        }


        private void cutOutsideCircleCB_CheckedChanged(object sender, EventArgs e)
        {
            cutInsideCircleCB.Checked = !cutOutsideCircleCB.Checked;
        }

        private void cutInsideCircleCB_CheckedChanged(object sender, EventArgs e)
        {
            cutOutsideCircleCB.Checked = !cutInsideCircleCB.Checked;
        }

        private void saveCircleButton_Click(object sender, EventArgs e)
        {
            if (!checkCircleInputs())
            {
                MessageBox.Show("Circle input is nt valid. Remember to check a lead-in.");
                return;
            }
            writeCircleFile();
            currentPath = new MotionPath(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "CIRCLE CODE" + ".txt"), 0);
            String path = addOffsetToText(currentPath.motionCommands);
            if (!withinBoundaries(path))
            {
                MessageBox.Show("Not Within Boundaries.");
            }
            Clipboard.SetText(path);
        }

        private void writeCircleFile()
        {
            if (!checkCircleInputs())
                return;
            string circleFile = "";
            double xLocation = Convert.ToDouble(circleXLocationTB.Text);
            double yLocation = Convert.ToDouble(circleYLocationTB.Text);
            double radius = Convert.ToDouble(circleDiameterTB.Text) / 2;
            

            double beginLeadInLocationX = xLocation;
            double beginLeadInLocationY = yLocation + radius - Trio.leadInLength;
            if (cutOutsideCircleCB.Checked)
            {
                beginLeadInLocationY = yLocation + radius + Trio.leadInLength;
                circleFile += "OFF\n";
                circleFile += "SPEED = RAPID\n";
                circleFile += "MOVEABS(" + beginLeadInLocationX.ToString() + "," + beginLeadInLocationY.ToString() + ")\n";
                circleFile += "ON\n";
                circleFile += "SPEED = FEED\n";
                circleFile += "MOVEABS(" + xLocation.ToString() + "," + (yLocation + radius + Trio.cutterDiameter / 2).ToString() + ")\n";
                circleFile += "MOVECIRC(0,0,0," + (-radius - Trio.cutterDiameter/2).ToString() + ",1)\n";
                circleFile += "SPEED = RAPID\n";
                circleFile += "OFF\n";
                //circleFile += "MOVEABS(0,0)\n";
            }
            else
            {
                circleFile += "OFF\n";
                circleFile += "SPEED = RAPID\n";
                circleFile += "MOVEABS(" + beginLeadInLocationX.ToString() + "," + beginLeadInLocationY.ToString() + ")\n";
                circleFile += "ON\n";
                circleFile += "SPEED = FEED\n";
                circleFile += "MOVEABS(" + xLocation.ToString() + "," + (yLocation + radius - Trio.cutterDiameter / 2).ToString() + ")\n";
                circleFile += "MOVECIRC(0,0,0," + (-radius + Trio.cutterDiameter / 2).ToString() + ",1)\n";
                circleFile += "OFF\n";
                circleFile += "SPEED = RAPID\n";
                //circleFile += "MOVEABS(0,0)\n";
            }


            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "CIRCLE CODE" + ".txt", circleFile);

            Clipboard.SetText(circleFile);


            
        }

        private void saveRectangleButton_Click(object sender, EventArgs e)
        {
            if (!checkRectangleInputs())
            {
                MessageBox.Show("Rectangle input is not valid. Remember to check a lead-in.");
                return;
            }
            writeRectangleFile();
            currentPath = new MotionPath(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "RECTANGLE CODE" + ".txt"), 0);
            String path = addOffsetToText(currentPath.motionCommands);
            if (!withinBoundaries(path))
            {
                MessageBox.Show("Not Within Boundaries.");
            }
            Clipboard.SetText(path);
        }

        private void writeRectangleFile()
        {
            string rectangleFile = "";

            double xLocation = Convert.ToDouble(recXLocationTB.Text);
            double yLocation = Convert.ToDouble(recYLocationTB.Text);
            double height = Convert.ToDouble(recHeightTB.Text);
            double width = Convert.ToDouble(recWidthTB.Text);
            double startX = xLocation - Trio.leadInLength;
            double startY = yLocation + height / 2;
            double radius = Convert.ToDouble(recEdgeRadiusTB.Text);
            if (cutInsideRectangleCB.Checked)
            {
                if (radius > Trio.cutterDiameter / 2)
                {
                    double x1 = xLocation + Trio.leadInLength;
                    double y1 = yLocation + height / 2;

                    double x2 = xLocation + Trio.cutterDiameter / 2;
                    double y2 = yLocation + height / 2;

                    double x3 = xLocation + Trio.cutterDiameter / 2;
                    double y3 = yLocation + height - radius;

                    double x4 = xLocation + width - radius;
                    double y4 = yLocation + height - Trio.cutterDiameter / 2;

                    double x5 = xLocation + width - Trio.cutterDiameter / 2;
                    double y5 = yLocation + radius;

                    double x6 = xLocation + radius;
                    double y6 = yLocation + Trio.cutterDiameter / 2;

                    double x7 = xLocation + Trio.cutterDiameter / 2;
                    double y7 = yLocation + height / 2;

                    rectangleFile += "SPEED = RAPID\n";
                    rectangleFile += "MOVEABS(" + x1.ToString() + "," + y1.ToString() + ")\n";
                    rectangleFile += "SPEED = FEED\n";
                    rectangleFile += "ON\n";
                    rectangleFile += "MOVEABS(" + x2.ToString() + "," + y2.ToString() + ")\n";
                    rectangleFile += "MOVEABS(" + x3.ToString() + "," + y3.ToString() + ")\n";
                    rectangleFile += "MOVECIRC(" + (radius - Trio.cutterDiameter / 2).ToString() + "," + (radius - Trio.cutterDiameter / 2).ToString() + "," + (radius - Trio.cutterDiameter / 2).ToString() + ",0,1)\n";
                    rectangleFile += "MOVEABS(" + x4.ToString() + "," + y4.ToString() + ")\n";
                    rectangleFile += "MOVECIRC(" + (radius - Trio.cutterDiameter / 2).ToString() + "," + (-radius + Trio.cutterDiameter / 2).ToString() + ",0," + (-radius + Trio.cutterDiameter / 2).ToString() + ",1)\n";
                    rectangleFile += "MOVEABS(" + x5.ToString() + "," + y5.ToString() + ")\n";
                    rectangleFile += "MOVECIRC(" + (-radius + Trio.cutterDiameter / 2).ToString() + "," + (-radius + Trio.cutterDiameter / 2).ToString() + "," + (-radius + Trio.cutterDiameter / 2).ToString() + ",0,1)\n";
                    rectangleFile += "MOVEABS(" + x6.ToString() + "," + y6.ToString() + ")\n";
                    rectangleFile += "MOVECIRC(" + (-radius + Trio.cutterDiameter / 2).ToString() + "," + (radius - Trio.cutterDiameter / 2).ToString() + ",0," + (radius - Trio.cutterDiameter / 2).ToString() + ",1)\n";
                    rectangleFile += "MOVEABS(" + x7.ToString() + "," + y7.ToString() + ")\n";
                    rectangleFile += "OFF\n";
                    rectangleFile += "SPEED = RAPID\n";
                    //rectangleFile += "MOVEABS(0,0)\n";
                }
                else
                {
                    double x1 = xLocation + Trio.leadInLength;
                    double y1 = yLocation + height / 2;

                    double x2 = xLocation + Trio.cutterDiameter / 2;
                    double y2 = yLocation + height / 2;

                    double x3 = xLocation + Trio.cutterDiameter / 2;
                    double y3 = yLocation + height - Trio.cutterDiameter/2;

                    double x4 = xLocation + width - Trio.cutterDiameter/2;
                    double y4 = yLocation + height - Trio.cutterDiameter / 2;

                    double x5 = xLocation + width - Trio.cutterDiameter / 2;
                    double y5 = yLocation + Trio.cutterDiameter / 2;

                    double x6 = xLocation + Trio.cutterDiameter/2;
                    double y6 = yLocation + Trio.cutterDiameter / 2;

                    double x7 = xLocation + Trio.cutterDiameter / 2;
                    double y7 = yLocation + height / 2;

                    rectangleFile += "SPEED = RAPID\n";
                    rectangleFile += "MOVEABS(" + x1.ToString() + "," + y1.ToString() + ")\n";
                    rectangleFile += "SPEED = FEED\n";
                    rectangleFile += "ON\n";
                    rectangleFile += "MOVEABS(" + x2.ToString() + "," + y2.ToString() + ")\n";
                    rectangleFile += "MOVEABS(" + x3.ToString() + "," + y3.ToString() + ")\n";
                    rectangleFile += "MOVEABS(" + x4.ToString() + "," + y4.ToString() + ")\n";
                    rectangleFile += "MOVEABS(" + x5.ToString() + "," + y5.ToString() + ")\n";
                    rectangleFile += "MOVEABS(" + x6.ToString() + "," + y6.ToString() + ")\n";
                    rectangleFile += "MOVEABS(" + x7.ToString() + "," + y7.ToString() + ")\n";
                    rectangleFile += "OFF\n";
                    rectangleFile += "SPEED = RAPID\n";
                   // rectangleFile += "MOVEABS(0,0)\n";
                }
            }
            else
            {
                if(radius > Trio.cutterDiameter/2){
                double x1 = xLocation - Trio.leadInLength;
                double y1 = yLocation + height / 2;

                double x2 = xLocation - Trio.cutterDiameter / 2;
                double y2 = yLocation + height / 2;

                double x3 = xLocation - Trio.cutterDiameter / 2;
                double y3 = yLocation + height - radius;

                double x4 = xLocation + width - radius;
                double y4 = yLocation + height + Trio.cutterDiameter / 2;

                double x5 = xLocation + width + Trio.cutterDiameter / 2;
                double y5 = yLocation + radius;

                double x6 = xLocation + radius;
                double y6 = yLocation - Trio.cutterDiameter / 2;

                double x7 = xLocation - Trio.cutterDiameter / 2;
                double y7 = yLocation + height/2;

                rectangleFile += "SPEED = RAPID\n";
                rectangleFile += "MOVEABS(" + x1.ToString() + "," + y1.ToString() + ")\n";
                rectangleFile += "SPEED = FEED\n";
                rectangleFile += "ON\n";
                rectangleFile += "MOVEABS(" + x2.ToString() + "," + y2.ToString() + ")\n";
                rectangleFile += "MOVEABS(" + x3.ToString() + "," + y3.ToString() + ")\n";
                rectangleFile += "MOVECIRC(" + (radius + Trio.cutterDiameter/2).ToString() + "," + (radius + Trio.cutterDiameter/2).ToString() + "," + (radius + Trio.cutterDiameter/2).ToString() + ",0,1)\n";
                rectangleFile += "MOVEABS(" + x4.ToString() + "," + y4.ToString() + ")\n";
                rectangleFile += "MOVECIRC(" + (radius + Trio.cutterDiameter/2).ToString() + "," + (-radius - Trio.cutterDiameter/2).ToString() + ",0," + (-radius - Trio.cutterDiameter/2).ToString() + ",1)\n"; 
                rectangleFile += "MOVEABS(" + x5.ToString() + "," + y5.ToString() + ")\n";
                rectangleFile += "MOVECIRC(" + (-radius - Trio.cutterDiameter/2).ToString() + "," + (-radius - Trio.cutterDiameter/2).ToString() + "," + (-radius - Trio.cutterDiameter/2).ToString() + ",0,1)\n";
                rectangleFile += "MOVEABS(" + x6.ToString() + "," + y6.ToString() + ")\n";
                rectangleFile += "MOVECIRC(" + (-radius - Trio.cutterDiameter/2).ToString() + "," + (radius + Trio.cutterDiameter/2).ToString() + ",0," + (radius + Trio.cutterDiameter/2).ToString() + ",1)\n";
                rectangleFile += "MOVEABS(" + x7.ToString() + "," + y7.ToString() + ")\n";
                rectangleFile += "OFF\n";
                rectangleFile += "SPEED = RAPID\n";
                //rectangleFile += "MOVEABS(0,0)\n";
                }
                else{

                    double x1 = xLocation - Trio.leadInLength;
                    double y1 = yLocation + height / 2;

                    double x2 = xLocation - Trio.cutterDiameter / 2;
                    double y2 = yLocation + height / 2;

                    double x3 = xLocation - Trio.cutterDiameter / 2;
                    double y3 = yLocation + height + Trio.cutterDiameter / 2;

                    double x4 = xLocation + width + Trio.cutterDiameter / 2;
                    double y4 = yLocation + height + Trio.cutterDiameter / 2;

                    double x5 = xLocation + width + Trio.cutterDiameter / 2;
                    double y5 = yLocation - Trio.cutterDiameter / 2;

                    double x6 = xLocation + - Trio.cutterDiameter/2;
                    double y6 = yLocation - Trio.cutterDiameter / 2;

                    double x7 = xLocation - Trio.cutterDiameter / 2;
                    double y7 = yLocation + height / 2;

                    rectangleFile += "SPEED = RAPID\n";
                    rectangleFile += "MOVEABS(" + x1.ToString() + "," + y1.ToString() + ")\n";
                    rectangleFile += "SPEED = FEED\n";
                    rectangleFile += "ON\n";
                    rectangleFile += "MOVEABS(" + x2.ToString() + "," + y2.ToString() + ")\n";
                    rectangleFile += "MOVEABS(" + x3.ToString() + "," + y3.ToString() + ")\n";
                    rectangleFile += "MOVEABS(" + x4.ToString() + "," + y4.ToString() + ")\n";
                    rectangleFile += "MOVEABS(" + x5.ToString() + "," + y5.ToString() + ")\n";
                    rectangleFile += "MOVEABS(" + x6.ToString() + "," + y6.ToString() + ")\n";
                    rectangleFile += "MOVEABS(" + x7.ToString() + "," + y7.ToString() + ")\n";
                    rectangleFile += "OFF\n";
                    rectangleFile += "SPEED = RAPID\n";
                    //rectangleFile += "MOVEABS(0,0)\n";
                }
            }

            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "RECTANGLE CODE" + ".txt", rectangleFile);

            Clipboard.SetText(rectangleFile);
        }

        private void cutOutsideRectangleCB_CheckedChanged(object sender, EventArgs e)
        {
            cutInsideRectangleCB.Checked = !cutOutsideRectangleCB.Checked;
        }

        private void cutInsideRectangleCB_CheckedChanged(object sender, EventArgs e)
        {
            cutOutsideRectangleCB.Checked = !cutInsideRectangleCB.Checked;
        }

        private void saveLineButton_Click(object sender, EventArgs e)
        {
            //TODO: Verify line file...
            writePolylineFile();
            Clipboard.SetText(addOffsetToText(Clipboard.GetText()));
        }

        private void writePolylineFile()
        {
            String s = getBasicCodeFromPolygonString(polygonTextBox.Text);

            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "POLYLINE CODE" + ".txt", s);

            Clipboard.SetText(s);
        }

        private void leftOfLineCB_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkChanging)
            {
                checkChanging = true;
                centerOfLineCB.Checked = false;
                rightOfLineCB.Checked = false;
                leftOfLineCB.Checked = true;
                checkChanging = false;
            }

        }

        private void rightOfLineCB_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkChanging)
            {
                checkChanging = true;
                centerOfLineCB.Checked = false;
                rightOfLineCB.Checked = true;
                leftOfLineCB.Checked = false;
                checkChanging = false;
            }
        }

        private void centerOfLineCB_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkChanging)
            {
                checkChanging = true;
                centerOfLineCB.Checked = true;
                rightOfLineCB.Checked = false;
                leftOfLineCB.Checked = false;
                checkChanging = false;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            new ProjectorDisplayForm().ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //STOP button
            Trio.turnOffJog(true);
            Trio.turnOffWatchDogAndServos(true);
            Trio.Cancelled = true;
            backgroundWorker1.CancelAsync();
            Trio.WaterOn = false; //turns off abrasive also.
            Thread.Sleep(100);
           // Trio.axTrioPC.
            Trio.CancelAllMoves("X");
            Trio.CancelAllMoves("Y");
            //Trio.Cancelled = false;
            RefreshValues();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new CutterDiameterForm(Trio).ShowDialog();
            cutterdiamlabel.Text = "Cutter Diameter (in): " + Trio.cutterDiameter.ToString();
            leadInLengthLabel.Text = "Lead-In Length (in): " + Trio.leadInLength.ToString();
            pierceDelayLabel.Text = "Pierce Delay (s): " + Trio.PierceDelay.ToString();
        }

        private void backgroundWorker1_DoWork_1(object sender, DoWorkEventArgs e)
        {
            if (backgroundWorker1.CancellationPending == true)
            {
                e.Cancel = true;
            }
            Trio.Cancelled = false;
            manualModeController = false;
            workerActive = true;
            currentPath = (MotionPath)e.Argument;
            runBasicCode(currentPath);
        }


        private void pictureBox12_MouseClick(object sender, MouseEventArgs e)
        {
            double speedPercentage = ((double)e.X) / speedPB.Width;
            Trio.PercentageSpeedOverride = speedPercentage;

            repaintSpeedBoxes();
        }

        

        private void repaintSpeedBoxes()
        {
            speedGraphics.Clear(Color.Black);
            speedGraphics.FillRectangle(myBrush, new Rectangle(0, 0, (int)(speedPB.Width * Trio.PercentageSpeedOverride), speedPB.Height));

            speed2Graphics.Clear(Color.Black);
            speed2Graphics.FillRectangle(myBrush, new Rectangle(0, 0, (int)(speed2PB.Width * Trio.PercentageSpeedOverride2), speed2PB.Height));

            updateSpeedLabels();


        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            workerActive = false;
            enableAllButtons(true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            double middle = speedPB.Width / 2;
            double speedPercentage = (middle) / speedPB.Width * 2;//goes to 200%
            Trio.PercentageSpeedOverride = speedPercentage;
            repaintSpeedBoxes();
            updateSpeedLabels();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            double zero = 0;
            double speedPercentage = (zero) / speedPB.Width * 2;//goes to 200%
            Trio.PercentageSpeedOverride = speedPercentage;
            repaintSpeedBoxes();
            updateSpeedLabels();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            double end = speedPB.Width;
            double speedPercentage = (end) / speedPB.Width * 2;//goes to 200%
            Trio.PercentageSpeedOverride = speedPercentage;
            repaintSpeedBoxes();
            updateSpeedLabels();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            RefreshValues();
            Trio.axTrioPC.Datum(0);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(addOffsetToText(Clipboard.GetText()));
        }

        private String addOffsetToText(String text)
        {
            Console.WriteLine("Adding offset: " + Trio.Offset.X + " , " + Trio.Offset.Y);
            String clip = text;
            if (clip == lastAbsolutePath)
                MessageBox.Show("WARNING: You are adding the offset to this sequence of motion commands twice.");
            String results = "";
            for (int i = 0; i < clip.Split('\n').Length; i++)
            {
                String line = clip.Split('\n')[i];
                if (lineStartsWith(line, "MOVEABS"))
                {
                    double xVal = Convert.ToDouble(line.Split('(')[1].Split(',')[0]) + Trio.Offset.X;
                    double yVal = Convert.ToDouble(line.Split('(')[1].Split(',')[1].Split(')')[0]) + Trio.Offset.Y;
                    results += "MOVEABS(" + xVal + "," + yVal + ")\n";
                }
                else if (lineStartsWith(line, "MOVECIRC"))
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

        private void button9_Click(object sender, EventArgs e)
        {
            string line1 = "Clicking a yellow button will save a sequence of commands to a .txt file and to the clipboard. You can  then paste ";
            string line2 = "from the clipboard to the cad2Motion software to see the motion path. If you want to see the big picture with ";
            string line3 = "the offsets added to the motion path, click the button to the left. This adds the offset to the current clipboard ";
            string line4 = "and sets the clipboard again.";
            //MessageBox.Show(line1 + line2 + line3 + line4);
            MessageBox.Show(Convert.ToBoolean(0).ToString());
        }
       
        private String compensateString(String theFile)
        {
            //ASSUMPTION: Start at (0,0)
            //Vector startPos = new Vector(0, 0);
            //String theFile = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE" + ".txt");
            String result = "";
            double cutterCompensation = Trio.cutterDiameter / 2;
            //if (!cutterCompCB.Checked)
            //    cutterCompensation = 0;

            Line originalLine = new Line();
            Line previousLine = new Line(); //Projected line of previous move
            String PreviousMoveType = "";
            double XPrevious = 0;
            double YPrevious = 0;
            Vector previousVector = new Vector(XPrevious, YPrevious);
            Circle previousCircle = new Circle(new Vector(0, 0), 0);
            double nextXMid = 0;
            double nextYMid = 0;
            double nextDirection = 0;
            double prevCircleBeginX = 0;
            double prevCircleBeginY = 0;
            double prevCircleEndX = 0;
            double prevCircleEndY = 0;
            double prevCircleMidX = 0;
            double prevCircleMidY = 0;
            bool compon = true;
            double xend2 = 0;
            double yend2 = 0;
            double xmid2 = 0;
            double ymid2 = 0;

            for (int i = 0; i < theFile.Split('\n').Length; i++)
            {
                String line = theFile.Split('\n')[i];
                if (lineStartsWith(line, "MOVEABS"))
                {
                    double xVal = Convert.ToDouble(line.Split('(')[1].Split(',')[0]);
                    double yVal = Convert.ToDouble(line.Split('(')[1].Split(',')[1].Split(')')[0]);
                    if (compon)
                    {
                        if (PreviousMoveType == "")
                        {
                            //first move
                            //previousVector = new Vector(xVal, yVal);
                            previousLine = new Line(previousVector.x, previousVector.y, xVal, yVal);
                            Segment seg = new Segment(previousVector.x, previousVector.y, xVal, yVal);
                            seg.addPerpendicular(cutterCompensation);

                            //MessageBox.Show("Temp line: " + previousLine.ToString() + " From previous vector: " + previousVector.ToString()+  " and point: " + xVal.ToString() + "," + yVal.ToString());
                            Vector lineVector = new Vector(previousLine.X2 - previousLine.X1, previousLine.Y2 - previousLine.Y1);
                            lineVector.addPerpendiclar(cutterCompensation);
                            lineVector.x = lineVector.x + previousLine.X1;
                            lineVector.y = lineVector.y + previousLine.Y1;
                            result += "OFF\n";
                            result += "MOVEABS(" + seg.x1.ToString() + "," + seg.y1.ToString() + ")\n";
                            result += "ON\n";
                            //previousVector = lineVector;
                            previousLine = new Line(lineVector.x, lineVector.y, previousLine.slope);
                            originalLine = previousLine;
                            //MessageBox.Show("Temp line: " + previousLine.ToString() + " From previous vector: " + previousVector.ToString());
                            //MessageBox.Show("Move command: " + xVal.ToString() + "," + yVal.ToString());
                            //MessageBox.Show("MOVE TO: " + lineVector.x.ToString() + "," + lineVector.y.ToString());
                            //result += "MOVEABS(" + lineVector.x.ToString() + "," + lineVector.y.ToString() + ")here\n";
                            previousVector = new Vector(xVal, yVal);
                            PreviousMoveType = "MOVEABS";
                        }
                        else if (PreviousMoveType == "MOVEABS")
                        {
                            //line to line
                            Line next = new Line(previousVector.x, previousVector.y, xVal, yVal);
                            Vector lineVector = new Vector(next.X2 - next.X1, next.Y2 - next.Y1);
                            double tempX = lineVector.x;
                            double tempy = lineVector.y;
                            lineVector.addPerpendiclar(cutterCompensation);
                            double xdiff = lineVector.x - tempX;
                            double ydiff = lineVector.y - tempy;
                            lineVector.x = lineVector.x + next.X1;
                            lineVector.y = lineVector.y + next.Y1;
                            next = new Line(lineVector.x, lineVector.y, next.slope);
                            Vector intersect = previousLine.getIntersection(next);
                            if (intersect == null)
                            {
                                //identical lines
                                //MessageBox.Show("HERE");
                                // MessageBox.Show("at val: " + previousVector.x.ToString() + "," + previousVector.y.ToString());
                                
                                result += "MOVEABS(" + (previousVector.x + xdiff).ToString() + "," + (previousVector.y + ydiff).ToString() + ")here\n";
                            }
                            else
                            {
                                
                                result += "MOVEABS(" + intersect.x.ToString() + "," + intersect.y.ToString() + ")\n";
                            }
                            previousLine = next;
                            XPrevious = previousVector.x;
                            YPrevious = previousVector.y;
                            previousVector = new Vector(xVal, yVal);
                        }
                        else if (PreviousMoveType == "MOVECIRC")
                        {
                            Line next = new Line(previousVector.x, previousVector.y, xVal, yVal);
                            Vector lineVector = new Vector(next.X2 - next.X1, next.Y2 - next.Y1);
                            lineVector.addPerpendiclar(cutterCompensation);
                            lineVector.x = lineVector.x + next.X1;
                            lineVector.y = lineVector.y + next.Y1;
                            next = new Line(lineVector.x, lineVector.y, next.slope);
                            Vector[] intersect = previousCircle.getIntersections(next);
                            Vector v = intersect[0];
                            if (intersect[0] == null)
                            {
                                double xend = prevCircleEndX;
                                double yend = prevCircleEndY;
                                double xmid = prevCircleMidX;
                                double ymid = prevCircleMidY;
                                short direction = (short)nextDirection;
                                //MessageBox.Show("xend: " + xend.ToString() + " yend: " + yend.ToString() + " xmid: " + xmid.ToString() + " ymid: " + ymid.ToString());

                                Vector toStartPos = new Vector(xmid, ymid);
                                Vector diff = new Vector(xmid, ymid);
                                if (cutterCompensation > 0 && direction == 0)
                                {
                                    //smaller
                                    diff = toStartPos.addMagnitudeInReverseDirection(cutterCompensation);
                                }
                                else if (cutterCompensation < 0 && direction == 0)
                                {
                                    //larger
                                    diff = toStartPos.addMagnitudeInSameDirection(-cutterCompensation);
                                }
                                else if (cutterCompensation > 0 && direction == 1)
                                {
                                    //larger
                                    diff = toStartPos.addMagnitudeInSameDirection(cutterCompensation);
                                }
                                else if (cutterCompensation < 0 && direction == 1)
                                {
                                    //smaller
                                    diff = toStartPos.addMagnitudeInReverseDirection(-cutterCompensation);
                                }
                                else
                                {
                                    MessageBox.Show("Unimplemented");
                                }
                                Vector theMid = diff.Copy();


                                Vector midToEnd = new Vector(xend - xmid, yend - ymid);

                                //diff = midToEnd.addMagnitude(cutterCompensation);
                                if (cutterCompensation > 0 && direction == 0)
                                {
                                    //smaller
                                    diff = midToEnd.addMagnitude(-cutterCompensation);
                                }
                                else if (cutterCompensation < 0 && direction == 0)
                                {
                                    //larger
                                    diff = midToEnd.addMagnitude(-cutterCompensation);
                                }
                                else if (cutterCompensation > 0 && direction == 1)
                                {
                                    //larger
                                    diff = midToEnd.addMagnitude(cutterCompensation);
                                }
                                else if (cutterCompensation < 0 && direction == 1)
                                {
                                    //smaller
                                    diff = midToEnd.addMagnitude(cutterCompensation);
                                }
                                else
                                {
                                    MessageBox.Show("Unimplemented");
                                }
                                double xDiff = diff.x - midToEnd.x;
                                double yDiff = diff.y - midToEnd.y;

                                Vector endPoint = new Vector(diff.x + theMid.x, diff.y + theMid.y);

                                result += "MOVECIRC(" + endPoint.x.ToString() + "," + endPoint.y.ToString() + "," + theMid.x.ToString() + "," + theMid.y.ToString() + "," + direction.ToString() + ")\n";

                                result += "OFF\n";


                                Vector temp = new Vector(xVal - previousVector.x, yVal - previousVector.y);
                                Vector temp2 = new Vector(temp.x, temp.y);
                                temp2.addPerpendiclar(cutterCompensation);
                                double xdiff = temp2.x - temp.x;
                                double ydiff = temp2.y - temp.y;
                                double x = previousVector.x + xdiff;
                                double y = previousVector.y + ydiff;
                                result += "MOVEABS(" + x.ToString() + "," + y.ToString() + ")\n";
                                result += "ON\n";

                                XPrevious = previousVector.x;
                                YPrevious = previousVector.y;
                                //previousVector = new Vector(xVal, yVal);
                                previousLine = next;



                            }
                            else
                            {
                                if (previousVector.getDistance(intersect[1]) < previousVector.getDistance(v))
                                    v = intersect[1];

                             
                                result += "MOVECIRC(" + (v.x - prevCircleBeginX).ToString() + "," + (v.y - prevCircleBeginY).ToString() + "," + nextXMid.ToString() + "," + nextYMid.ToString() + "," + nextDirection.ToString() + ")\n";
                                XPrevious = previousVector.x;
                                YPrevious = previousVector.y;
                                //previousVector = new Vector(xVal, yVal);
                                previousLine = next;
                            }
                        }
                        if (PreviousMoveType == "MOVECIRCFULL")
                        {
                            Line next = new Line(previousVector.x, previousVector.y, xVal, yVal);
                            Vector lineVector = new Vector(next.X2 - next.X1, next.Y2 - next.Y1);
                            lineVector.addPerpendiclar(cutterCompensation);
                            lineVector.x = lineVector.x + next.X1;
                            lineVector.y = lineVector.y + next.Y1;
                            next = new Line(lineVector.x, lineVector.y, next.slope);
                            Vector[] intersect = previousCircle.getIntersections(next);
                            Vector v = intersect[0];
                            if (previousVector.getDistance(intersect[1]) < previousVector.getDistance(v))
                                v = intersect[1];

                         
                            result += "MOVECIRC(" + (v.x - prevCircleBeginX).ToString() + "," + (v.y - prevCircleBeginY).ToString() + "," + nextXMid.ToString() + "," + nextYMid.ToString() + "," + nextDirection.ToString() + ")\n";
                            result += "MOVECIRC(0,0," + (nextXMid + -(v.x - prevCircleBeginX)).ToString() + "," + (nextYMid + -(v.y - prevCircleBeginY)).ToString() + "," + nextDirection.ToString() + ")\n";
                            previousVector = new Vector(xVal, yVal);
                            previousLine = next;

                        }
                        if (noMovesLeft(theFile, i))
                        {
                            //line to line
                            // MessageBox.Show("No moves left after: " + i.ToString());

                            Segment seg = new Segment(previousVector.x, previousVector.y, xVal, yVal);
                            seg.addPerpendicular(cutterCompensation);
                            Line next = new Line(previousVector.x, previousVector.y, xVal, yVal);
                            Vector lineVector = new Vector(next.X2 - next.X1, next.Y2 - next.Y1);
                            lineVector.addPerpendiclar(cutterCompensation);
                            lineVector.x = lineVector.x + next.X1;
                            lineVector.y = lineVector.y + next.Y1;
                            next = originalLine;// new Line(lineVector.x, lineVector.y, next.slope);
                            //MessageBox.Show("The first line: " + previousLine.slope.ToString() + " :Slope & yint: " + previousLine.yIntercept.ToString());
                            //MessageBox.Show("The second line: " + next.slope.ToString() + " :Slope & yint: " + next.yIntercept.ToString());

                            Vector intersect = previousLine.getIntersection(next);
                            if (intersect != null)
                            {
                                if (Math.Abs(intersect.x - lineVector.x) < 1)//arbitrary value here, don't really like this
                                    result += "MOVEABS(" + intersect.x.ToString() + "," + intersect.y.ToString() + ")\n";
                                else
                                {
                                    result += "MOVEABS(" + seg.x2.ToString() + "," + seg.y2.ToString() + ")\n";
                                }
                            }
                            previousLine = next;
                            previousVector = new Vector(xVal, yVal);

                        }
                        PreviousMoveType = "MOVEABS";
                        //XPrevious = xVal;
                        //YPrevious = yVal;
                        previousVector = new Vector(xVal, yVal);
                        //new
                    }
                    else
                    {
                        if (PreviousMoveType == "MOVECIRC")
                            MessageBox.Show("Unimplemented. Last move is arc.");
                        result += "MOVEABS(" + xVal.ToString() + "," + yVal.ToString() + ")\n";
                        previousVector = new Vector(xVal, yVal);
                        PreviousMoveType = "";
                    }
                }
                else if (lineStartsWith(line, "MOVECIRC"))
                {
                    double xend = Convert.ToDouble(line.Split('(')[1].Split(',')[0]);
                    double yend = Convert.ToDouble(line.Split('(')[1].Split(',')[1]);
                    double xmid = Convert.ToDouble(line.Split('(')[1].Split(',')[2]);
                    double ymid = Convert.ToDouble(line.Split('(')[1].Split(',')[3]);
                    //prevCircleEndX = xend;
                    //prevCircleEndY = yend;
                    short direction = Convert.ToInt16(line.Split('(')[1].Split(',')[4].Split(')')[0]);

                    if (compon)
                    {
                        if (PreviousMoveType == "MOVEABS")
                        {
                            //line to arc
                            //at this point, you know the previous line and the circle, but not where the compensated circle ends.
                            //make a circle object, find intersection with line, and find closest to naive point.
                            Circle arcCircle = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)));


                            if (direction == 0 && cutterCompensation > 0)
                            {
                                //smaller circle
                                arcCircle = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) - cutterCompensation);
                            }
                            else if (direction == 0 && cutterCompensation < 0)
                            {
                                //larger circle
                                arcCircle = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) - cutterCompensation);
                            }
                            else if (direction == 1 && cutterCompensation > 0)
                            {
                                //larger circle
                                arcCircle = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) + cutterCompensation);
                            }
                            else if (direction == 1 && cutterCompensation < 0)
                            {
                                //smaller circle
                                arcCircle = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) + cutterCompensation);
                            }
                            else
                            {
                                MessageBox.Show("Circle combination not implemented");
                            }

                            //first circle point
                            Vector[] circleStarts = arcCircle.getIntersections(previousLine);
                            Vector theIntersection = circleStarts[0];
                            if (theIntersection == null)
                            {
                                //no intersection, try turning off and skipping the area.
                                Vector temp = new Vector(previousVector.x, previousVector.y);
                                temp.x = temp.x - XPrevious;
                                temp.y = temp.y - YPrevious;
                                temp.addPerpendiclar(cutterCompensation);
                                temp.x = temp.x + XPrevious;
                                temp.y = temp.y + YPrevious;
                                result += "MOVEABS(" + temp.x.ToString() + "," + temp.y.ToString() + ")\n";
                                result += "OFF\n";


                                Vector toStartPos = new Vector(xmid, ymid);
                                Vector diff = new Vector(xmid, ymid);
                                if (cutterCompensation > 0 && direction == 0)
                                {
                                    diff = toStartPos.addMagnitude(-cutterCompensation);
                                }
                                else if (cutterCompensation < 0 && direction == 0)
                                {
                                    diff = toStartPos.addMagnitude(cutterCompensation);
                                }
                                else if (cutterCompensation > 0 && direction == 1)
                                {
                                    diff = toStartPos.addMagnitude(cutterCompensation);
                                }
                                else if (cutterCompensation < 0 && direction == 1)
                                {
                                    diff = toStartPos.addMagnitude(cutterCompensation);
                                }
                                else
                                {
                                    MessageBox.Show("Unimplemented");
                                }
                                double xDiff = toStartPos.x - diff.x;
                                double yDiff = toStartPos.y - diff.y;
                                result += "MOVEABS(" + (xDiff + previousVector.x).ToString() + "," + (yDiff + previousVector.y).ToString() + ")\n";
                                result += "ON\n";
                                nextXMid = xmid - xDiff;
                                nextYMid = ymid - yDiff;
                                nextDirection = direction;
                                prevCircleBeginX = xDiff + previousVector.x;
                                prevCircleBeginY = yDiff + previousVector.y;

                                previousVector = new Vector(previousVector.x + xend, previousVector.y + yend);
                                previousCircle = arcCircle;
                            }
                            else
                            {
                                if (previousVector.getDistance(circleStarts[1]) < previousVector.getDistance(circleStarts[0]))
                                {
                                    theIntersection = circleStarts[1];
                                }
                              
                                result += "MOVEABS(" + theIntersection.x + "," + theIntersection.y + ")\n";
                                nextXMid = xmid + previousVector.x - theIntersection.x;
                                nextYMid = ymid + previousVector.y - theIntersection.y;
                                nextDirection = direction;
                                prevCircleBeginX = theIntersection.x;
                                prevCircleBeginY = theIntersection.y;

                                previousVector = new Vector(previousVector.x + xend, previousVector.y + yend);
                                previousCircle = arcCircle;
                            }
                        }
                        if (PreviousMoveType == "MOVECIRC")
                        {
                            Circle circ = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)));

                            if (direction == 0 && cutterCompensation > 0)
                            {
                                //smaller circle
                                circ = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) - cutterCompensation);
                            }
                            else if (direction == 0 && cutterCompensation < 0)
                            {
                                //larger circle
                                circ = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) - cutterCompensation);
                            }
                            else if (direction == 1 && cutterCompensation > 0)
                            {
                                //larger circle
                                circ = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) + cutterCompensation);
                            }
                            else if (direction == 1 && cutterCompensation < 0)
                            {
                                //smaller circle
                                circ = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) + cutterCompensation);
                            }
                            else
                            {
                                MessageBox.Show("Circle combination not implemented");
                            }

                            Vector oldIntersection = new Vector(previousVector.x, previousVector.y);

                            Vector[] intersections = previousCircle.getIntersections(circ);

                            Vector v = intersections[0];
                            if (v == null)
                            {
                                MessageBox.Show("No intersection");
                                //recovery algorithm
                                //double xend = prevCircleEndX;
                                //double yend = prevCircleEndY;
                                //double xmid = nextXMid;
                                //double ymid = nextYMid;
                                //short direction = nextDirection;

                                Vector toStartPos = new Vector(xmid2, ymid2);
                                Vector diff = new Vector(xmid2, ymid2);
                                if (cutterCompensation > 0 && direction == 0)
                                {
                                    diff = toStartPos.addMagnitude(-cutterCompensation);
                                }
                                else if (cutterCompensation < 0 && direction == 0)
                                {
                                    diff = toStartPos.addMagnitude(cutterCompensation);
                                }
                                else if (cutterCompensation > 0 && direction == 1)
                                {
                                    diff = toStartPos.addMagnitude(cutterCompensation);
                                }
                                else if (cutterCompensation < 0 && direction == 1)
                                {
                                    diff = toStartPos.addMagnitude(-cutterCompensation);
                                }
                                else
                                {
                                    MessageBox.Show("Unimplemented");
                                }
                                double xDiff = toStartPos.x - diff.x;
                                double yDiff = toStartPos.y - diff.y;


                                Vector newCenter = new Vector(xmid2 + -xDiff, ymid2 + -yDiff);

                                Vector midToEnd = new Vector(xend2 - xmid2, yend2 - ymid2);

                                //diff = midToEnd.addMagnitude(cutterCompensation);
                                if (cutterCompensation > 0 && direction == 0)
                                {
                                    diff = midToEnd.addMagnitude(-cutterCompensation);
                                }
                                else if (cutterCompensation < 0 && direction == 0)
                                {
                                    diff = midToEnd.addMagnitude(cutterCompensation);
                                }
                                else if (cutterCompensation > 0 && direction == 1)
                                {
                                    diff = midToEnd.addMagnitude(cutterCompensation);

                                }
                                else if (cutterCompensation < 0 && direction == 1)
                                {
                                    diff = midToEnd.addMagnitude(-cutterCompensation);
                                }
                                else
                                {
                                    MessageBox.Show("Unimplemented");
                                }

                                Vector endPoint = new Vector(newCenter.x + diff.x, newCenter.y + diff.y);
                                //prevCircleEndX
                                result += "MOVECIRC(" + endPoint.x.ToString() + "," + endPoint.y.ToString() + "," + nextXMid.ToString() + "," + nextYMid.ToString() + "," + direction.ToString() + ")\n";

                                Vector toStartPos2 = new Vector(xmid, ymid);
                                Vector diff2 = new Vector(xmid, ymid);
                                if (cutterCompensation > 0 && direction == 0)
                                {
                                    diff2 = toStartPos2.addMagnitude(-cutterCompensation);
                                }
                                else if (cutterCompensation < 0 && direction == 0)
                                {
                                    diff2 = toStartPos2.addMagnitude(cutterCompensation);
                                }
                                else if (cutterCompensation > 0 && direction == 1)
                                {
                                    diff2 = toStartPos2.addMagnitude(cutterCompensation);
                                }
                                else if (cutterCompensation < 0 && direction == 1)
                                {
                                    diff2 = toStartPos2.addMagnitude(-cutterCompensation);
                                }
                                else
                                {
                                    MessageBox.Show("Unimplemented");
                                }
                                double xDiff2 = toStartPos2.x - diff2.x;
                                double yDiff2 = toStartPos2.y - diff2.y;
                                result += "OFF\n";
                                result += "MOVEABS(" + (xDiff2 + previousVector.x).ToString() + "," + (yDiff2 + previousVector.y).ToString() + ")\n";
                                result += "ON\n";

                                XPrevious = XPrevious + xend;
                                YPrevious = YPrevious + yend;
                                nextXMid = xmid - xDiff2;
                                nextYMid = ymid - yDiff2;
                                nextDirection = direction;
                                prevCircleBeginX = previousVector.x + xDiff2;
                                prevCircleBeginY = previousVector.y + yDiff2;

                                previousVector = new Vector(previousVector.x + xend, previousVector.y + yend);
                                previousCircle = circ;
                            }
                            else
                            {
                                if (intersections[1].getDistance(oldIntersection) < v.getDistance(oldIntersection))
                                    v = intersections[1];
                                //MessageBox.Show(v.ToString() + " Intersection between circles");

                                //MessageBox.Show("Intersection: " + v.ToString());

                                result += "MOVECIRC(" + (v.x - prevCircleBeginX).ToString() + "," + (v.y - prevCircleBeginY) + "," + nextXMid.ToString() + "," + nextYMid.ToString() + "," + nextDirection.ToString() + ")\n";
                                nextXMid = xmid + previousVector.x - v.x;
                                nextYMid = ymid + previousVector.y - v.y;
                                nextDirection = direction;
                                prevCircleBeginX = v.x;
                                prevCircleBeginY = v.y;

                                previousVector = new Vector(previousVector.x + xend, previousVector.y + yend);
                                previousCircle = circ;
                            }
                        }
                        if (PreviousMoveType == "MOVECIRCFULL")
                        {
                            MessageBox.Show("Consecutive full circles not implemented");
                        }
                        PreviousMoveType = "MOVECIRC";
                        if (xend == 0 && yend == 0)
                            PreviousMoveType = "MOVECIRCFULL";
                    }
                    else
                    {
                        PreviousMoveType = "";
                        //previousVector = new Vector(previousVector.x +)
                        //should be something here for previous vector
                    }
                    prevCircleEndX = xend;
                    prevCircleEndY = yend;
                    prevCircleMidX = xmid;
                    prevCircleMidY = ymid;
                    xend2 = xend;
                    yend2 = yend;
                    xmid2 = xmid;
                    ymid2 = ymid;
                    //previousVector.x = previousVector.x + xend + cutterCompensation;
                    //previousVector.y = previousVector.y + yend + cutterCompensation;
                }
                else if (lineStartsWith(line, "CUTTER COMPENSATION LEFT"))
                {
                    compon = true;
                    PreviousMoveType = "";
                    if (cutterCompensation < 0)
                        cutterCompensation = -cutterCompensation;
                }
                else if (lineStartsWith(line, "CUTTER COMPENSATION RIGHT"))
                {
                    compon = true;
                    PreviousMoveType = "";
                    if (cutterCompensation > 0)
                        cutterCompensation = -cutterCompensation;
                }

                else if (lineStartsWith(line, "CUTTER COMPENSATION OFF"))
                {
                    //take previous move and use simple algorithm. No intersect is known
                    if (PreviousMoveType == "MOVEABS")
                    {
                        Vector temp = new Vector(previousVector.x - XPrevious, previousVector.y - YPrevious);
                        temp.addPerpendiclar(cutterCompensation);
                        double x = temp.x + XPrevious;
                        double y = temp.y + YPrevious;
                        result += "MOVEABS(" + x.ToString() + "," + y.ToString() + ")\n";
                    }
                    if (PreviousMoveType == "MOVECIRC" || PreviousMoveType == "MOVECIRCFULL")
                    {
                        //MessageBox.Show("HERE");
                        result += "MOVECIRC(" + prevCircleEndX.ToString() + "," + prevCircleEndY.ToString() + "," + nextXMid.ToString() + "," + nextYMid.ToString() + "," + nextDirection.ToString() + ")\n";
                    }
                    //result += "MOVEABS(" + previousVector.x.ToString() + "," + previousVector.y.ToString() + ")\n";
                    PreviousMoveType = "";
                    compon = false;
                }
                else
                {
                        result += line + '\n';
                }
            }
            return result;
        }
        private void button10_Click(object sender, EventArgs e)
        {
            //ASSUMPTION: Start at (0,0)
            //Vector startPos = new Vector(0, 0);
            String theFile = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE" + ".txt");
            String result = compensateString(theFile);
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE COMPENSATION" + ".txt", result);
            Clipboard.SetText(result);
            return;
        //    double cutterCompensation = Trio.cutterDiameter / 2;
        //    //if (!cutterCompCB.Checked)
        //    //    cutterCompensation = 0;

        //    Line originalLine = new Line(); 
        //    Line previousLine = new Line(); //Projected line of previous move
        //    String PreviousMoveType = ""; 
        //    double XPrevious = 0;
        //    double YPrevious = 0;
        //    Vector previousVector = new Vector(XPrevious, YPrevious);
        //    Circle previousCircle = new Circle(new Vector(0,0),0);
        //    double nextXMid = 0;
        //    double nextYMid = 0;
        //    double nextDirection = 0;
        //    double prevCircleBeginX = 0;
        //    double prevCircleBeginY = 0;
        //    double prevCircleEndX = 0;
        //    double prevCircleEndY = 0;
        //    bool compon = true;
        //    double xend2 = 0;
        //    double yend2 = 0;
        //    double xmid2 = 0;
        //    double ymid2 = 0;

        //    for (int i = 0; i < theFile.Split('\n').Length; i++)
        //    {
        //        String line = theFile.Split('\n')[i];
        //        if (lineStartsWith(line, "MOVEABS"))
        //        {
        //            double xVal = Convert.ToDouble(line.Split('(')[1].Split(',')[0]);
        //            double yVal = Convert.ToDouble(line.Split('(')[1].Split(',')[1].Split(')')[0]);
        //            if (compon)
        //            {
        //                if (PreviousMoveType == "")
        //                {
        //                    //first move
        //                    //previousVector = new Vector(xVal, yVal);
        //                    previousLine = new Line(previousVector.x, previousVector.y, xVal, yVal);

        //                    //MessageBox.Show("Temp line: " + previousLine.ToString() + " From previous vector: " + previousVector.ToString()+  " and point: " + xVal.ToString() + "," + yVal.ToString());
        //                    Vector lineVector = new Vector(previousLine.X2 - previousLine.X1, previousLine.Y2 - previousLine.Y1);
        //                    lineVector.addPerpendiclar(cutterCompensation);
        //                    lineVector.x = lineVector.x + previousLine.X1;
        //                    lineVector.y = lineVector.y + previousLine.Y1;
        //                    //previousVector = lineVector;
        //                    previousLine = new Line(lineVector.x, lineVector.y, previousLine.slope);
        //                    originalLine = previousLine;
        //                    //MessageBox.Show("Temp line: " + previousLine.ToString() + " From previous vector: " + previousVector.ToString());
        //                    //MessageBox.Show("Move command: " + xVal.ToString() + "," + yVal.ToString());
        //                    //MessageBox.Show("MOVE TO: " + lineVector.x.ToString() + "," + lineVector.y.ToString());
        //                    //result += "MOVEABS(" + lineVector.x.ToString() + "," + lineVector.y.ToString() + ")here\n";
        //                    previousVector = new Vector(xVal, yVal);
        //                    PreviousMoveType = "MOVEABS";
        //                }
        //                else if (PreviousMoveType == "MOVEABS")
        //                {
        //                    //line to line
        //                    Line next = new Line(previousVector.x, previousVector.y, xVal, yVal);
        //                    Vector lineVector = new Vector(next.X2 - next.X1, next.Y2 - next.Y1);
        //                    double tempX = lineVector.x;
        //                    double tempy = lineVector.y;
        //                    lineVector.addPerpendiclar(cutterCompensation);
        //                    double xdiff = lineVector.x - tempX;
        //                    double ydiff = lineVector.y - tempy;
        //                    lineVector.x = lineVector.x + next.X1;
        //                    lineVector.y = lineVector.y + next.Y1;
        //                    next = new Line(lineVector.x, lineVector.y, next.slope);
        //                    Vector intersect = previousLine.getIntersection(next);
        //                    if (intersect == null)
        //                    {
        //                        //identical lines
        //                        //MessageBox.Show("HERE");
        //                       // MessageBox.Show("at val: " + previousVector.x.ToString() + "," + previousVector.y.ToString());
        //                        result += "MOVEABS(" + (previousVector.x + xdiff).ToString() + "," + (previousVector.y + ydiff).ToString() + ")here\n";
        //                    }
        //                    else
        //                    {
        //                        result += "MOVEABS(" + intersect.x.ToString() + "," + intersect.y.ToString() + ")\n";
        //                    }
        //                    previousLine = next;
        //                    XPrevious = previousVector.x;
        //                    YPrevious = previousVector.y;
        //                    previousVector = new Vector(xVal, yVal);
        //                }
        //                else if (PreviousMoveType == "MOVECIRC")
        //                {

        //                    Line next = new Line(previousVector.x, previousVector.y, xVal, yVal);
        //                    Vector lineVector = new Vector(next.X2 - next.X1, next.Y2 - next.Y1);
        //                    lineVector.addPerpendiclar(cutterCompensation);
        //                    lineVector.x = lineVector.x + next.X1;
        //                    lineVector.y = lineVector.y + next.Y1;
        //                    next = new Line(lineVector.x, lineVector.y, next.slope);
        //                    Vector[] intersect = previousCircle.getIntersections(next);
        //                    Vector v = intersect[0];
        //                    if (intersect[0] == null)
        //                    {
        //                        double xend = prevCircleEndX;
        //                        double yend = prevCircleEndY;
        //                        double xmid = nextXMid;
        //                        double ymid = nextYMid;
        //                        short direction = (short)nextDirection;

        //                        Vector toStartPos = new Vector(xmid, ymid);
        //                        Vector diff = new Vector(xmid, ymid);
        //                        if (cutterCompensation > 0 && direction == 0)
        //                        {
        //                            diff = toStartPos.addMagnitude(-cutterCompensation);
        //                        }
        //                        else if (cutterCompensation < 0 && direction == 0)
        //                        {
        //                            diff = toStartPos.addMagnitude(cutterCompensation);
        //                        }
        //                        else if (cutterCompensation > 0 && direction == 1)
        //                        {
        //                            diff = toStartPos.addMagnitude(cutterCompensation);
        //                        }
        //                        else if (cutterCompensation < 0 && direction == 1)
        //                        {
        //                            diff = toStartPos.addMagnitude(-cutterCompensation);
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show("Unimplemented");
        //                        }
        //                        double xDiff = toStartPos.x - diff.x;
        //                        double yDiff = toStartPos.y - diff.y;

        //                        //result += "MOVEABS(" + (xDiff + XPrevious).ToString() + "," + (yDiff + YPrevious).ToString() + ")\n";

        //                        Vector newCenter = new Vector(xmid, ymid);

        //                        Vector midToEnd = new Vector(xend - xmid, yend - ymid);

        //                        //diff = midToEnd.addMagnitude(cutterCompensation);
        //                        if (cutterCompensation > 0 && direction == 0)
        //                        {
        //                            diff = midToEnd.addMagnitude(-cutterCompensation);
        //                        }
        //                        else if (cutterCompensation < 0 && direction == 0)
        //                        {
        //                            diff = midToEnd.addMagnitude(cutterCompensation);
        //                        }
        //                        else if (cutterCompensation > 0 && direction == 1)
        //                        {
        //                            diff = midToEnd.addMagnitude(cutterCompensation);

        //                        }
        //                        else if (cutterCompensation < 0 && direction == 1)
        //                        {
        //                            diff = midToEnd.addMagnitude(-cutterCompensation);
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show("Unimplemented");
        //                        }

        //                        Vector endPoint = new Vector(newCenter.x + diff.x - xDiff, newCenter.y + diff.y - yDiff);

        //                        result += "MOVECIRC(" + endPoint.x.ToString() + "," + endPoint.y.ToString() + "," + newCenter.x.ToString() + "," + newCenter.y.ToString() + "," + direction.ToString() + ")\n";

        //                        result += "OFF\n";


        //                        Vector temp = new Vector(xVal - previousVector.x, yVal - previousVector.y);
        //                        Vector temp2 = new Vector(temp.x,temp.y);
        //                        temp2.addPerpendiclar(cutterCompensation);
        //                        double xdiff = temp2.x - temp.x;
        //                        double ydiff = temp2.y - temp.y;
        //                        double x = previousVector.x + xdiff;
        //                        double y = previousVector.y + ydiff;
        //                        result += "MOVEABS(" +  x.ToString() + "," + y.ToString() + ")\n";
        //                        result += "ON\n";

        //                        XPrevious = previousVector.x;
        //                        YPrevious = previousVector.y;
        //                        previousVector = new Vector(xVal, yVal);
        //                        previousLine = next;
                                

                                
        //                    }
        //                    else
        //                    {
        //                        if (previousVector.getDistance(intersect[1]) < previousVector.getDistance(v))
        //                            v = intersect[1];

        //                        result += "MOVECIRC(" + (v.x - prevCircleBeginX).ToString() + "," + (v.y - prevCircleBeginY).ToString() + "," + nextXMid.ToString() + "," + nextYMid.ToString() + "," + nextDirection.ToString() + ")\n";
        //                        XPrevious = previousVector.x;
        //                        YPrevious = previousVector.y;
        //                        previousVector = new Vector(xVal, yVal);
        //                        previousLine = next;
        //                    }
        //                }
        //                if (PreviousMoveType == "MOVECIRCFULL")
        //                {
        //                    Line next = new Line(previousVector.x, previousVector.y, xVal, yVal);
        //                    Vector lineVector = new Vector(next.X2 - next.X1, next.Y2 - next.Y1);
        //                    lineVector.addPerpendiclar(cutterCompensation);
        //                    lineVector.x = lineVector.x + next.X1;
        //                    lineVector.y = lineVector.y + next.Y1;
        //                    next = new Line(lineVector.x, lineVector.y, next.slope);
        //                    Vector[] intersect = previousCircle.getIntersections(next);
        //                    Vector v = intersect[0];
        //                    if (previousVector.getDistance(intersect[1]) < previousVector.getDistance(v))
        //                        v = intersect[1];

        //                    result += "MOVECIRC(" + (v.x - prevCircleBeginX).ToString() + "," + (v.y - prevCircleBeginY).ToString() + "," + nextXMid.ToString() + "," + nextYMid.ToString() + "," + nextDirection.ToString() + ")\n";
        //                    result += "MOVECIRC(0,0," + (nextXMid + -(v.x - prevCircleBeginX)).ToString() + "," + (nextYMid + -(v.y - prevCircleBeginY)).ToString() + "," + nextDirection.ToString() + ")\n";
        //                    previousVector = new Vector(xVal, yVal);
        //                    previousLine = next;
                            
        //                }
        //                if (noMovesLeft(theFile, i))
        //                {
        //                    //line to line
        //                   // MessageBox.Show("No moves left after: " + i.ToString());
        //                    Line next = new Line(previousVector.x, previousVector.y, xVal, yVal);
        //                    Vector lineVector = new Vector(next.X2 - next.X1, next.Y2 - next.Y1);
        //                    lineVector.addPerpendiclar(cutterCompensation);
        //                    lineVector.x = lineVector.x + next.X1;
        //                    lineVector.y = lineVector.y + next.Y1;
        //                    next = originalLine;// new Line(lineVector.x, lineVector.y, next.slope);
        //                    //MessageBox.Show("The first line: " + previousLine.slope.ToString() + " :Slope & yint: " + previousLine.yIntercept.ToString());
        //                    //MessageBox.Show("The second line: " + next.slope.ToString() + " :Slope & yint: " + next.yIntercept.ToString());

        //                    Vector intersect = previousLine.getIntersection(next);
        //                    result += "MOVEABS(" + intersect.x.ToString() + "," + intersect.y.ToString() + ")\n";
        //                    previousLine = next;
        //                    previousVector = new Vector(xVal, yVal);

        //                }
        //                PreviousMoveType = "MOVEABS";
        //                //XPrevious = xVal;
        //                //YPrevious = yVal;
        //                previousVector = new Vector(xVal, yVal);
        //                //new
        //            }
        //            else
        //            {
        //                if (PreviousMoveType == "MOVECIRC")
        //                    MessageBox.Show("Unimplemented. Last move is arc.");
        //                result += "MOVEABS(" + xVal.ToString() + "," + yVal.ToString() + ")\n";
        //                previousVector = new Vector(xVal, yVal);
        //                PreviousMoveType = "";
        //            }
        //        }
        //        else if (lineStartsWith(line, "MOVECIRC"))
        //        {
        //            double xend = Convert.ToDouble(line.Split('(')[1].Split(',')[0]);
        //            double yend = Convert.ToDouble(line.Split('(')[1].Split(',')[1]);
        //            double xmid = Convert.ToDouble(line.Split('(')[1].Split(',')[2]);
        //            double ymid = Convert.ToDouble(line.Split('(')[1].Split(',')[3]);
        //            //prevCircleEndX = xend;
        //            //prevCircleEndY = yend;
        //            short direction = Convert.ToInt16(line.Split('(')[1].Split(',')[4].Split(')')[0]);
                    
        //            if (compon)
        //            {
        //                if (PreviousMoveType == "MOVEABS")
        //                {
        //                    //line to arc
        //                    //at this point, you know the previous line and the circle, but not where the compensated circle ends.
        //                    //make a circle object, find intersection with line, and find closest to naive point.
        //                    Circle arcCircle = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)));


        //                    if (direction == 0 && cutterCompensation > 0)
        //                    {
        //                        //smaller circle
        //                        arcCircle = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) - cutterCompensation);
        //                    }
        //                    else if (direction == 0 && cutterCompensation < 0)
        //                    {
        //                        //larger circle
        //                        arcCircle = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) - cutterCompensation);
        //                    }
        //                    else if (direction == 1 && cutterCompensation > 0)
        //                    {
        //                        //larger circle
        //                        arcCircle = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) + cutterCompensation);
        //                    }
        //                    else if (direction == 1 && cutterCompensation < 0)
        //                    {
        //                        //smaller circle
        //                        arcCircle = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) + cutterCompensation);
        //                    }
        //                    else
        //                    {
        //                        MessageBox.Show("Circle combination not implemented");
        //                    }

        //                    //first circle point
        //                    Vector[] circleStarts = arcCircle.getIntersections(previousLine);
        //                    Vector theIntersection = circleStarts[0];
        //                    if (theIntersection == null)
        //                    {
        //                        //no intersection, try turning off and skipping the area.
        //                        Vector temp = new Vector(previousVector.x, previousVector.y);
        //                        temp.x = temp.x - XPrevious;
        //                        temp.y = temp.y - YPrevious;
        //                        temp.addPerpendiclar(cutterCompensation);
        //                        temp.x = temp.x + XPrevious;
        //                        temp.y = temp.y + YPrevious;
        //                        result += "MOVEABS(" + temp.x.ToString() + "," + temp.y.ToString() + ")\n";
        //                        result += "OFF\n";


        //                        Vector toStartPos = new Vector(xmid, ymid);
        //                        Vector diff = new Vector(xmid, ymid);
        //                        if (cutterCompensation > 0 && direction == 0)
        //                        {
        //                            diff = toStartPos.addMagnitude(-cutterCompensation);
        //                        }
        //                        else if (cutterCompensation < 0 && direction == 0)
        //                        {
        //                            diff = toStartPos.addMagnitude(cutterCompensation);
        //                        }
        //                        else if (cutterCompensation > 0 && direction == 1)
        //                        {
        //                            diff = toStartPos.addMagnitude(cutterCompensation);
        //                        }
        //                        else if (cutterCompensation < 0 && direction == 1)
        //                        {
        //                            diff = toStartPos.addMagnitude(-cutterCompensation);
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show("Unimplemented");
        //                        }
        //                        double xDiff = toStartPos.x - diff.x;
        //                        double yDiff = toStartPos.y - diff.y;
        //                        result += "MOVEABS(" + (xDiff + previousVector.x).ToString() + "," + (yDiff + previousVector.y).ToString() + ")\n";
        //                        result += "ON\n";
        //                        nextXMid = xmid - xDiff;
        //                        nextYMid = ymid - yDiff;
        //                        nextDirection = direction;
        //                        prevCircleBeginX = xDiff + previousVector.x;
        //                        prevCircleBeginY = yDiff + previousVector.y;

        //                        previousVector = new Vector(previousVector.x + xend, previousVector.y + yend);
        //                        previousCircle = arcCircle;
        //                    }
        //                    else
        //                    {
        //                        if (previousVector.getDistance(circleStarts[1]) < previousVector.getDistance(circleStarts[0]))
        //                        {
        //                            theIntersection = circleStarts[1];
        //                        }
        //                        result += "MOVEABS(" + theIntersection.x + "," + theIntersection.y + ")\n";
        //                        nextXMid = xmid + previousVector.x - theIntersection.x;
        //                        nextYMid = ymid + previousVector.y - theIntersection.y;
        //                        nextDirection = direction;
        //                        prevCircleBeginX = theIntersection.x;
        //                        prevCircleBeginY = theIntersection.y;

        //                        previousVector = new Vector(previousVector.x + xend, previousVector.y + yend);
        //                        previousCircle = arcCircle;
        //                    }
        //                }
        //                if (PreviousMoveType == "MOVECIRC")
        //                {
        //                    Circle circ = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)));

        //                    if (direction == 0 && cutterCompensation > 0)
        //                    {
        //                        //smaller circle
        //                        circ = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) - cutterCompensation);
        //                    }
        //                    else if (direction == 0 && cutterCompensation < 0)
        //                    {
        //                        //larger circle
        //                        circ = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) - cutterCompensation);
        //                    }
        //                    else if (direction == 1 && cutterCompensation > 0)
        //                    {
        //                        //larger circle
        //                        circ = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) + cutterCompensation);
        //                    }
        //                    else if (direction == 1 && cutterCompensation < 0)
        //                    {
        //                        //smaller circle
        //                        circ = new Circle(new Vector(previousVector.x + xmid, previousVector.y + ymid), Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2)) + cutterCompensation);
        //                    }
        //                    else
        //                    {
        //                        MessageBox.Show("Circle combination not implemented");
        //                    }

        //                    Vector oldIntersection = new Vector(previousVector.x, previousVector.y);

        //                    Vector[] intersections = previousCircle.getIntersections(circ);

        //                    Vector v = intersections[0];
        //                    if (v == null)
        //                    {
        //                        MessageBox.Show("No intersection");
        //                        //recovery algorithm
        //                        //double xend = prevCircleEndX;
        //                        //double yend = prevCircleEndY;
        //                        //double xmid = nextXMid;
        //                        //double ymid = nextYMid;
        //                        //short direction = nextDirection;

        //                        Vector toStartPos = new Vector(xmid2, ymid2);
        //                        Vector diff = new Vector(xmid2, ymid2);
        //                        if (cutterCompensation > 0 && direction == 0)
        //                        {
        //                            diff = toStartPos.addMagnitude(-cutterCompensation);
        //                        }
        //                        else if (cutterCompensation < 0 && direction == 0)
        //                        {
        //                            diff = toStartPos.addMagnitude(cutterCompensation);
        //                        }
        //                        else if (cutterCompensation > 0 && direction == 1)
        //                        {
        //                            diff = toStartPos.addMagnitude(cutterCompensation);
        //                        }
        //                        else if (cutterCompensation < 0 && direction == 1)
        //                        {
        //                            diff = toStartPos.addMagnitude(-cutterCompensation);
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show("Unimplemented");
        //                        }
        //                        double xDiff = toStartPos.x - diff.x;
        //                        double yDiff = toStartPos.y - diff.y;


        //                        Vector newCenter = new Vector(xmid2 + -xDiff, ymid2 + -yDiff);

        //                        Vector midToEnd = new Vector(xend2 - xmid2, yend2 - ymid2);

        //                        //diff = midToEnd.addMagnitude(cutterCompensation);
        //                        if (cutterCompensation > 0 && direction == 0)
        //                        {
        //                            diff = midToEnd.addMagnitude(-cutterCompensation);
        //                        }
        //                        else if (cutterCompensation < 0 && direction == 0)
        //                        {
        //                            diff = midToEnd.addMagnitude(cutterCompensation);
        //                        }
        //                        else if (cutterCompensation > 0 && direction == 1)
        //                        {
        //                            diff = midToEnd.addMagnitude(cutterCompensation);

        //                        }
        //                        else if (cutterCompensation < 0 && direction == 1)
        //                        {
        //                            diff = midToEnd.addMagnitude(-cutterCompensation);
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show("Unimplemented");
        //                        }

        //                        Vector endPoint = new Vector(newCenter.x + diff.x, newCenter.y + diff.y);
        //                        //prevCircleEndX
        //                        result += "MOVECIRC(" + endPoint.x.ToString() + "," + endPoint.y.ToString() + "," + nextXMid.ToString() + "," + nextYMid.ToString() + "," + direction.ToString() + ")\n";
                                
        //                        Vector toStartPos2 = new Vector(xmid, ymid);
        //                        Vector diff2 = new Vector(xmid, ymid);
        //                        if (cutterCompensation > 0 && direction == 0)
        //                        {
        //                            diff2 = toStartPos2.addMagnitude(-cutterCompensation);
        //                        }
        //                        else if (cutterCompensation < 0 && direction == 0)
        //                        {
        //                            diff2 = toStartPos2.addMagnitude(cutterCompensation);
        //                        }
        //                        else if (cutterCompensation > 0 && direction == 1)
        //                        {
        //                            diff2 = toStartPos2.addMagnitude(cutterCompensation);
        //                        }
        //                        else if (cutterCompensation < 0 && direction == 1)
        //                        {
        //                            diff2 = toStartPos2.addMagnitude(-cutterCompensation);
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show("Unimplemented");
        //                        }
        //                        double xDiff2 = toStartPos2.x - diff2.x;
        //                        double yDiff2 = toStartPos2.y - diff2.y;
        //                        result += "OFF\n";
        //                        result += "MOVEABS(" + (xDiff2 + previousVector.x).ToString() + "," + (yDiff2 + previousVector.y).ToString() + ")\n";
        //                        result += "ON\n";

        //                        XPrevious = XPrevious + xend;
        //                        YPrevious = YPrevious + yend;
        //                        nextXMid = xmid - xDiff2;
        //                        nextYMid = ymid - yDiff2;
        //                        nextDirection = direction;
        //                        prevCircleBeginX = previousVector.x + xDiff2;
        //                        prevCircleBeginY = previousVector.y + yDiff2;

        //                        previousVector = new Vector(previousVector.x + xend, previousVector.y + yend);
        //                        previousCircle = circ;
        //                    }
        //                    else
        //                    {
        //                        if (intersections[1].getDistance(oldIntersection) < v.getDistance(oldIntersection))
        //                            v = intersections[1];
        //                        //MessageBox.Show(v.ToString() + " Intersection between circles");

        //                        //MessageBox.Show("Intersection: " + v.ToString());

        //                        result += "MOVECIRC(" + (v.x - prevCircleBeginX).ToString() + "," + (v.y - prevCircleBeginY) + "," + nextXMid.ToString() + "," + nextYMid.ToString() + "," + nextDirection.ToString() + ")\n";
        //                        nextXMid = xmid + previousVector.x - v.x;
        //                        nextYMid = ymid + previousVector.y - v.y;
        //                        nextDirection = direction;
        //                        prevCircleBeginX = v.x;
        //                        prevCircleBeginY = v.y;

        //                        previousVector = new Vector(previousVector.x + xend, previousVector.y + yend);
        //                        previousCircle = circ;
        //                    }
        //                }
        //                if (PreviousMoveType == "MOVECIRCFULL")
        //                {
        //                    MessageBox.Show("Consecutive full circles not implemented");
        //                }
        //                PreviousMoveType = "MOVECIRC";
        //                if (xend == 0 && yend == 0)
        //                    PreviousMoveType = "MOVECIRCFULL";
        //            }
        //            else
        //            {
        //                PreviousMoveType = "";
        //                //previousVector = new Vector(previousVector.x +)
        //                //should be something here for previous vector
        //            }
        //            prevCircleEndX = xend;
        //            prevCircleEndY = yend;
        //            xend2 = xend;
        //            yend2 = yend;
        //            xmid2 = xmid;
        //            ymid2 = ymid;
        //            //previousVector.x = previousVector.x + xend + cutterCompensation;
        //            //previousVector.y = previousVector.y + yend + cutterCompensation;
        //        }
        //        else if (lineStartsWith(line, "CUTTER COMPENSATION LEFT"))
        //        {
        //            compon = true;
        //            PreviousMoveType = "";
        //            if (cutterCompensation < 0)
        //                cutterCompensation = -cutterCompensation;
        //        }
        //        else if (lineStartsWith(line, "CUTTER COMPENSATION RIGHT")){
        //            compon = true;
        //            PreviousMoveType = "";
        //            if (cutterCompensation > 0)
        //                cutterCompensation = -cutterCompensation;
        //        }
                    
        //        else if (lineStartsWith(line, "CUTTER COMPENSATION OFF"))
        //        {
        //            //take previous move and use simple algorithm. No intersect is known
        //            if (PreviousMoveType == "MOVEABS")
        //            {
        //                Vector temp = new Vector(previousVector.x - XPrevious, previousVector.y - YPrevious);
        //                temp.addPerpendiclar(cutterCompensation);
        //                double x = temp.x + XPrevious;
        //                double y = temp.y + YPrevious;
        //                result += "MOVEABS(" + x.ToString() + "," + y.ToString() + ")\n";
        //            }
        //            if (PreviousMoveType == "MOVECIRC" || PreviousMoveType == "MOVECIRCFULL")
        //            {
        //                //MessageBox.Show("HERE");
        //                result += "MOVECIRC(" + prevCircleEndX.ToString() + "," + prevCircleEndY.ToString() + "," + nextXMid.ToString() + "," + nextYMid.ToString() + "," + nextDirection.ToString() + ")\n";
        //            }
        //            //result += "MOVEABS(" + previousVector.x.ToString() + "," + previousVector.y.ToString() + ")\n";
        //            PreviousMoveType = "";
        //            compon = false;
        //        }
        //        else
        //        {
        //            result += line + '\n';
        //        }
        //    }
        //    File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE COMPENSATION" + ".txt", result);
        }

        private bool noMovesLeft(String file, int i)
        {
            for (int j = i + 1; j < file.Split('\n').Length; j++)
            {
                String line = file.Split('\n')[j];
                if (lineStartsWith(line, "MOVEABS"))
                    return false;
                if (lineStartsWith(line, "MOVECIRC"))
                    return false;
            }
            return true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //ASSUMPTION: Start at (0,0)
            String theFile = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE" + ".txt");
            String result = "";
            double cutterCompensation = .05;
            double XPrevious = 0;
            double YPrevious = 0;
            Vector previousVector = new Vector(XPrevious, YPrevious);
            bool compMode = true;
            for (int i = 0; i < theFile.Split('\n').Length; i++)
            {
                String line = theFile.Split('\n')[i];
                if (lineStartsWith(line, "CUTTER COMPENSATION LEFT"))
                    compMode = true;
                if (lineStartsWith(line, "CUTTER COMPENSATION RIGHT"))
                {
                    compMode = true;
                    MessageBox.Show("Right not implemented");
                    cutterCompensation = -cutterCompensation;
                }
                if (lineStartsWith(line, "CUTTER COMPENSATION OFF"))
                    compMode = false;
                if (compMode)
                {
                    if (lineStartsWith(line, "MOVEABS"))
                    {
                        double xVal = Convert.ToDouble(line.Split('(')[1].Split(',')[0]);
                        double yVal = Convert.ToDouble(line.Split('(')[1].Split(',')[1].Split(')')[0]);

                        Segment seg = new Segment(XPrevious, YPrevious, xVal, yVal);
                        seg.addPerpendicular(cutterCompensation);
                        result += "MOVEABS(" + (seg.x1).ToString() + "," + (seg.y1).ToString() + ")\n";
                        result += "MOVEABS(" + (seg.x2).ToString() + "," + (seg.y2).ToString() + ")\n";


                        if (noMovesLeft(theFile, i))
                        {

                        }
                        XPrevious = xVal;
                        YPrevious = yVal;
                    }
                    else if (lineStartsWith(line, "MOVECIRC"))
                    {
                        double xend = Convert.ToDouble(line.Split('(')[1].Split(',')[0]);
                        double yend = Convert.ToDouble(line.Split('(')[1].Split(',')[1]);
                        double xmid = Convert.ToDouble(line.Split('(')[1].Split(',')[2]);
                        double ymid = Convert.ToDouble(line.Split('(')[1].Split(',')[3]);
                        short direction = Convert.ToInt16(line.Split('(')[1].Split(',')[4].Split(')')[0]);

                        Vector toStartPos = new Vector(xmid, ymid);
                        Vector diff = new Vector(xmid, ymid);
                        if (cutterCompensation > 0 && direction == 0)
                        {
                            diff = toStartPos.addMagnitude(-cutterCompensation);
                        }
                        else if (cutterCompensation < 0 && direction == 0)
                        {
                            diff = toStartPos.addMagnitude(cutterCompensation);
                        }
                        else if (cutterCompensation > 0 && direction == 1)
                        {
                            diff = toStartPos.addMagnitude(cutterCompensation);
                        }
                        else if (cutterCompensation < 0 && direction == 1)
                        {
                            diff = toStartPos.addMagnitude(-cutterCompensation);
                        }
                        else
                        {
                            MessageBox.Show("Unimplemented");
                        }
                        double xDiff = toStartPos.x - diff.x;
                        double yDiff = toStartPos.y - diff.y;

                        result += "MOVEABS(" + (xDiff + XPrevious).ToString() + "," + (yDiff + YPrevious).ToString() + ")\n";

                        Vector newCenter = new Vector(xmid + -xDiff, ymid + -yDiff);

                        Vector midToEnd = new Vector(xend - xmid, yend - ymid);

                        //diff = midToEnd.addMagnitude(cutterCompensation);
                        if (cutterCompensation > 0 && direction == 0)
                        {
                            diff = midToEnd.addMagnitude(-cutterCompensation);
                        }
                        else if (cutterCompensation < 0 && direction == 0)
                        {
                            diff = midToEnd.addMagnitude(cutterCompensation);
                        }
                        else if (cutterCompensation > 0 && direction == 1)
                        {
                            diff = midToEnd.addMagnitude(cutterCompensation);

                        }
                        else if (cutterCompensation < 0 && direction == 1)
                        {
                            diff = midToEnd.addMagnitude(-cutterCompensation);
                        }
                        else
                        {
                            MessageBox.Show("Unimplemented");
                        }

                        Vector endPoint = new Vector(newCenter.x + diff.x, newCenter.y + diff.y);

                        result += "MOVECIRC(" + endPoint.x.ToString() + "," + endPoint.y.ToString() + "," + newCenter.x.ToString() + "," + newCenter.y.ToString() + "," + direction.ToString() + ")\n";

                        XPrevious = XPrevious + xend;
                        YPrevious = YPrevious + yend;

                    }
                    else
                    {
                        result += line + '\n';
                    }
                }
                else
                {
                    if (lineStartsWith(line, "MOVEABS"))
                    {
                        double xVal = Convert.ToDouble(line.Split('(')[1].Split(',')[0]);
                        double yVal = Convert.ToDouble(line.Split('(')[1].Split(',')[1].Split(')')[0]);
                        XPrevious = xVal;
                        YPrevious = yVal;
                    }
                    result += line + '\n';
                }
            }
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE COMPENSATION-SIMPLE" + ".txt", result);
        }
        
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            xboxControllerButton.BackColor = Color.Green;
            bool XJogForwardOn = false;
            bool YJogForwardOn = false;
            bool XJogBackwardOn = false;
            bool YJogBackwardOn = false;
            bool RightTriggerWasOff = true;
            bool rightButtonWasOff = true;
            int triggerThreshold = 100;
            int controllerTouchiness = 8000;
            int threadSleep = 50;
            while (controller.IsConnected && Trio.axTrioPC.IsOpen(3240))
            {
                if (this.backgroundWorker2.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                #region ControllerState
                var state = controller.GetState();
                int readX = state.Gamepad.LeftThumbX;
                int readY = state.Gamepad.LeftThumbY;
                //touchiness
                if (Math.Abs(readX) < controllerTouchiness)
                    readX = 0;
                if (Math.Abs(readY) < controllerTouchiness)
                    readY = 0;
                if (readX > 0) //readX > controllerTouchiness
                    readX = readX - controllerTouchiness;
                if (readY > 0)
                    readY = readY - controllerTouchiness;
                if (readX < 0)
                    readX = readX + controllerTouchiness;
                if (readY < 0)
                    readY = readY + controllerTouchiness;

                double x = readX / analogJoyStickRange;
                double y = readY / analogJoyStickRange;

                int speedSig = state.Gamepad.RightThumbY;
                short waterSig = state.Gamepad.RightTrigger;
                bool rightButtonPressed = state.Gamepad.Buttons == SlimDX.XInput.GamepadButtonFlags.RightShoulder;
                bool bButtonPressed = state.Gamepad.Buttons == SlimDX.XInput.GamepadButtonFlags.B;
                bool backButtonPressed = state.Gamepad.Buttons == SlimDX.XInput.GamepadButtonFlags.Back;
                bool startButtonPressed = state.Gamepad.Buttons == SlimDX.XInput.GamepadButtonFlags.Start;
                bool leftTriggerPressed = state.Gamepad.LeftTrigger > 50;
                bool xButtonPressed = state.Gamepad.Buttons == SlimDX.XInput.GamepadButtonFlags.X;
                bool yButtonPressed = state.Gamepad.Buttons == SlimDX.XInput.GamepadButtonFlags.Y;
                bool aButtonPressed = state.Gamepad.Buttons == SlimDX.XInput.GamepadButtonFlags.A;
                if (xButtonPressed)
                    y = 0;
                if (yButtonPressed)
                    x = 0;
                #endregion

                #region startAndStop
                //if (backButtonPressed)
                //{
                //    //stop and allow jogging until the start button is pressed
                //    if (!manualModeController)//code is running
                //    {
                //        if (SureJetSettings.writeToConsole)
                //            Console.WriteLine("Back button pressed");
                //        Trio.Cancelled = true;
                //        manualModeController = true;
                //        pauseMode = true;

                //        //Trio.PercentageSpeedOverride = 0;
                //        //Trio.PercentageSpeedOverride2 = 0;
                //        //while (Trio.inMotion("X", "Y")) {  }
                //        ////wait for it to stop

                //        //pauseMode = true;
                //        //manualModeController = true; //allows for jogging

                //        //Trio.VirtualXLocation = Trio.getVariable("VirtualX", "MPOS");
                //        //Trio.VirtualYLocation = Trio.getVariable("VirtualY", "MPOS");
                         
                //    }
                //}
                //if (startButtonPressed && pauseMode)
                //{
                //    object[] a = {-1.0, -1};
                //    backgroundWorker2.ReportProgress(1,a); //This calls the other background worker from the main thread.
                //    //backgroundWorker1.RunWorkerAsync(currentPath); //This is running a background worker from a background worker.. not good.
                //    pauseMode = false;
                //    manualModeController = false;
                //    //Trio.moveAbs("VirtualX", "VirtualY", Trio.VirtualXLocation, Trio.VirtualYLocation, 0, 0);

                //    //while (Trio.inMotion("VirtualX", "VirtualY")) { Thread.Sleep(10); }

                //    //pauseMode = false;
                //    //manualModeController = false;
                //}
                #endregion

                #region jogging
                if (manualModeController)
                {
                    String xAxis = "X";
                    String yAxis = "Y";

                    //if(pauseMode || true)//use virtual axis
                    //{
                    //    xAxis = "VirtualX";
                    //    yAxis = "VirtualY";
                    //}
                    //if ((x > 0 || x < 0 || y > 0 || y < 0) && Trio.getVariable("X", "WDOG") == 0)
                    //    Trio.turnOnWatchDogAndServos(true);
                    //if (!(x > 0 || x < 0 || y > 0 || y < 0) && Trio.getVariable("X", "WDOG") == 1 && !pauseMode)
                    //    Trio.turnOffWatchDogAndServos(true);
                    if (Trio.WaterOn)
                    {
                        Trio.setJogSpeed(xAxis, Math.Abs(Trio.feedSpeed*Trio.PercentageSpeedOverride * x));
                        Trio.setJogSpeed(yAxis, Math.Abs(Trio.feedSpeed*Trio.PercentageSpeedOverride * y));
                    }
                    else
                    {
                        //Console.WriteLine("X: " + x);
                        //Console.WriteLine("Y: " + y);
                        Trio.setJogSpeed(xAxis, Math.Abs(x) * Trio.rapidSpeed * Trio.PercentageSpeedOverride2);
                        Trio.setJogSpeed(yAxis, Math.Abs(y) * Trio.rapidSpeed * Trio.PercentageSpeedOverride2);
                    }
                    if (x > 0)
                    {
                        if (!XJogForwardOn)
                        {
                            Trio.JogForward(xAxis);
                            XJogForwardOn = true;
                            XJogBackwardOn = false;
                        }
                    }
                    if (x < 0)
                    {
                        if (!XJogBackwardOn)
                        {
                            Trio.JogBackward(xAxis);
                            XJogBackwardOn = true;
                            XJogForwardOn = false;
                        }
                    }
                    if (y > 0)
                    {
                        if (!YJogForwardOn)
                        {
                            Trio.JogForward(yAxis);
                            YJogForwardOn = true;
                            YJogBackwardOn = false;
                        }
                    }
                    if (y < 0)
                    {
                        
                        if (!YJogBackwardOn)
                        {
                            Trio.JogBackward(yAxis);
                            YJogForwardOn = false;
                            YJogBackwardOn = true;
                        }
                    }
                    if (x == 0 && y == 0)
                    {
                        YJogForwardOn = false;
                        YJogBackwardOn = false;
                        XJogForwardOn = false;
                        XJogBackwardOn = false;
                        Trio.turnOffJog(true);
                    }

                }
                #endregion
                //end of jog

                #region WaterToggle
                if (waterSig > triggerThreshold)
                {
                    if (RightTriggerWasOff)
                    {
                        //toggle
                        if (manualModeController)
                        {
                            if (Trio.WaterOn)
                            {
                                waterLabel.ForeColor = Color.Black;
                                Trio.WaterOn = false;
                            }
                            else
                            {
                                waterLabel.ForeColor = Color.Red;
                                Trio.WaterOn = true;
                            }
                        }
                        else
                        {
                            //water trigger pressed while in programmed movement, dry run mode. Turn water off. 
                        }
                    }
                    RightTriggerWasOff = false;
                }
                else
                {
                    RightTriggerWasOff = true;
                }
                #endregion

                #region abrasiveToggle
                if (rightButtonPressed == true)
                {
                    if (rightButtonWasOff)
                    {
                        if (Trio.AbrasiveOn)
                        {
                            abrasiveCB.ForeColor = Color.Black;
                            Trio.AbrasiveOn = false;
                        }
                        else
                        {
                            abrasiveCB.ForeColor = Color.Red;
                            Trio.AbrasiveOn = true;
                        }
                    }
                    rightButtonWasOff = false;
                }
                else
                {
                    rightButtonWasOff = true;
                }
                #endregion
                #region speed
                if (bButtonPressed == true)
                {
                    if (!leftTriggerPressed)
                    {
                        Trio.PercentageSpeedOverride = 0;
                        object[] a = { Trio.PercentageSpeedOverride, 1 };
                        backgroundWorker2.ReportProgress(1, a);
                    }
                    else
                    {
                        Trio.PercentageSpeedOverride2 = 0;
                        object[] a = { Trio.PercentageSpeedOverride2, 2 };
                        backgroundWorker2.ReportProgress(1, a);
                    }
                }
                if (aButtonPressed == true)
                {
                    if (!leftTriggerPressed)
                    {
                        Trio.PercentageSpeedOverride = 1;
                        object[] a = { Trio.PercentageSpeedOverride, 1 };
                        backgroundWorker2.ReportProgress(1, a);
                    }
                    else
                    {
                        Trio.PercentageSpeedOverride2 = 1;
                        object[] a = { Trio.PercentageSpeedOverride2, 2 };
                        backgroundWorker2.ReportProgress(1, a);
                    }
                }
                

                if (Math.Abs(speedSig) < controllerTouchiness)
                    speedSig = 0;

                if (speedSig > 0)
                    speedSig = speedSig - controllerTouchiness;
                if (speedSig < 0)
                    speedSig = speedSig + controllerTouchiness;
                
                double potentialPercentage =  Trio.PercentageSpeedOverride + speedSig / 600000.0;
                if(leftTriggerPressed)
                    potentialPercentage = Trio.PercentageSpeedOverride2 + speedSig / 600000.0;

                if (potentialPercentage < 0)
                    potentialPercentage = 0;

                if (potentialPercentage <= 1 && potentialPercentage >= 0 && speedSig != 0)
                {
                    if (!leftTriggerPressed)
                    {
                        Trio.PercentageSpeedOverride = potentialPercentage;
                        object[] a = {Trio.PercentageSpeedOverride, 1};
                        backgroundWorker2.ReportProgress(1, a);
                    }
                    else
                    {
                        Trio.PercentageSpeedOverride2 = potentialPercentage;
                        object[] a = { Trio.PercentageSpeedOverride2, 2};
                        backgroundWorker2.ReportProgress(1, a);
                    }
                #endregion
                }
                //MessageBox.Show(controller.GetBatteryInformation(SlimDX.XInput.BatteryDeviceType.Gamepad).Level.ToString());
                Thread.Sleep(threadSleep);
            }
            Trio.turnOffJog(true);
            xboxControllerButton.BackColor = Color.Red;
            
        }

        private void button12_Click(object sender, EventArgs e)
        {
            controller = new SlimDX.XInput.Controller(SlimDX.XInput.UserIndex.One);
            if(!backgroundWorker2.IsBusy)
            backgroundWorker2.RunWorkerAsync();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            backgroundWorker1.CancelAsync();
            backgroundWorker2.CancelAsync();
            base.OnClosing(e);
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            int a = (int)((object[])e.UserState)[1];
            double b = (double)((object[])e.UserState)[0];
            if (a == -1)//This is a call to resume the cut
            {
                backgroundWorker1.RunWorkerAsync(currentPath);
            }
            else if (a == 1)
            {
                //speedOverrideLabel.BackColor = Color.Red;
                //speed2OverrideLabel.BackColor = Color.Black;
                repaintSpeedBoxes();
            }
            else
            {
               //speed2OverrideLabel.BackColor = Color.Red;
                //speedOverrideLabel.BackColor = Color.Black;
                repaintSpeedBoxes();
            }
        }

        private void speed2PB_MouseClick(object sender, MouseEventArgs e)
        {
            double speedPercentage = ((double)e.X) / speed2PB.Width;
            Trio.PercentageSpeedOverride2 = speedPercentage;

            repaintSpeedBoxes();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Trio.PercentageSpeedOverride2 = 1;
            
            repaintSpeedBoxes();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Trio.PercentageSpeedOverride2 = 0;
            repaintSpeedBoxes();
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            Trio.PercentageSpeedOverride2 = 2;
            repaintSpeedBoxes();
        }

        private void addPolygonPointButton_Click(object sender, EventArgs e)
        {
            polygonTextBox.Text += "(" + lineX1TB.Text + "," + lineY1TB.Text + ")\n";
            //updatePolygonListBox();
            lineX1TB.Focus();
            lineX1TB.SelectAll();
            
        }

        //private void updatePolygonListBox()
        //{
        //    polygonTextBox.Text = polygonString;
        //}
        private String getBasicCodeFromPolygonString(String polygon)
        {
            String code = "";
            code += "SPEED = RAPID\n";
            code += "CUTTER COMPENSATION OFF\n";
            bool firstMoveDone = false;
            String[] points = polygonTextBox.Text.Split('\n');
            Vector previousVector = new Vector(0, 0);
            Segment previousSegment = null;
            double radius = 0;
            double prevX = 0;
            double prevY = 0;
            try
            {
                 radius = Convert.ToDouble(filletRadiusTB.Text);
            }
            catch
            {
                
            }
            foreach (String s in points)
            {
                if (s == "")
                {
                    break;
                }
                double x = Convert.ToDouble(s.Split('(')[1].Split(',')[0]);
                double y = Convert.ToDouble(s.Split('(')[1].Split(',')[1].Split(')')[0]);
                if (previousSegment == null)
                {
                    previousSegment = new Segment(previousVector.x, previousVector.y, x, y);
                    if(x != previousVector.x || y != previousVector.y)
                    code += "MOVEABS(" + x.ToString() + "," + y.ToString() + ")\n";
                }
                else
                {
                    if (previousSegment.x2 == x && previousSegment.y2 == y)
                        continue;

                    Segment currentSegment = new Segment(previousSegment.x2, previousSegment.y2, x, y);
                    if (previousSegment.Line.CompareSlope(currentSegment.Line.slope) == 0)
                    {
                        code += "MOVEABS(" + previousSegment.x2.ToString() + "," + previousSegment.y2.ToString() + ")\n";
                        previousSegment = currentSegment.Copy();
                        continue;
                    }
                    Segment previousCopy = previousSegment.Copy();
                    Vector p1 = new Vector(previousSegment.x1, previousSegment.y1);
                    Vector p2 = new Vector(previousSegment.x2, previousSegment.y2);
                    Vector p3 = new Vector(x, y);
                    double angle = previousVector.getAngleFrom3Points(p2,p1 ,p3 );//The first argument is the vertex. Not an error.
                    double lengthDeleted = radius / Math.Tan(angle / 2);
                    //subtract the length from the previous vector.
                    //MessageBox.Show("BEfore:" + previousSegment.ToString());
                    previousSegment.removeFromEnd(lengthDeleted);
                    //MessageBox.Show("AFTER: " + previousSegment.ToString());
                    currentSegment.removeFromBeginning(lengthDeleted);
                    if (!firstMoveDone)
                    {
                        code += "ON\n";
                        code  += "SPEED = FEED\n";
                        
                        if (leftOfLineCB.Checked)
                            code += "CUTTER COMPENSATION LEFT\n";
                        else if (rightOfLineCB.Checked)
                            code += "CUTTER COMPENSATION RIGHT\n";
                    }
                    if(firstMoveDone)
                    code += "MOVEABS(" + previousSegment.x2.ToString() + "," + previousSegment.y2.ToString() + ")\n";
                    
                    Segment center1 = previousSegment.Copy();
                    center1.addPerpendicular(radius);
                    Vector centerOne = new Vector(center1.x2, center1.y2);
                    center1.addPerpendicular(-radius);
                    center1.addPerpendicular(-radius);
                    Vector centerTwo = new Vector(center1.x2, center1.y2);
                    Segment center2 = currentSegment.Copy();
                    center2.addPerpendicular(radius);
                    Vector centerThree = new Vector(center2.x1, center2.y1);
                    center2.addPerpendicular(-radius);
                    center2.addPerpendicular(-radius);
                    Vector centerFour = new Vector(center2.x1, center2.y1);
                    //MessageBox.Show("Center One: " + centerOne.ToString());
                    //MessageBox.Show("Center two: " + centerTwo.ToString());
                    //MessageBox.Show("Center three: " + centerThree.ToString());
                    //MessageBox.Show("Center four: " + centerFour.ToString());
                    Vector finalCenter = new Vector(0, 0);
                    if (centerOne.CompareTo(centerThree) == 0)
                    {
                        finalCenter = centerOne;
                    }
                    else if (centerOne.CompareTo(centerFour) == 0)
                    {
                        finalCenter = centerOne;
                    }
                    else if (centerTwo.CompareTo(centerThree) == 0)
                    {
                        finalCenter = centerTwo;
                    }
                    else if (centerTwo.CompareTo(centerFour) == 0)
                    {
                        finalCenter = centerTwo;
                    }
                    else
                    {
                        //MessageBox.Show("Centers do not agree: " + previousSegment.ToString());
                    }
                   // MessageBox.Show("Previous segment: " + previousSegment.ToString());
                    //MessageBox.Show("Final center: " + finalCenter.ToString());
                    double xmid = finalCenter.x - previousSegment.x2;
                    double ymid = finalCenter.y - previousSegment.y2;
                    double xend = currentSegment.x1 - previousSegment.x2;
                    double yend = currentSegment.y1- previousSegment.y2;

                    //find direction
                    //MessageBox.Show("Previous segment: " + previousSegment.ToString());
                    Vector directionOfFirstVector = new Vector(previousCopy.x2 - previousCopy.x1, previousCopy.y2 - previousCopy.y1);
                    Vector fromCentertoP1 = new Vector(finalCenter.x - previousCopy.x2, finalCenter.y - previousCopy.y2);
                    Vector copy = fromCentertoP1.Copy();
                    copy.addPerpendiclar(.005);//arbitrary value, ccw direction NOT ARBITRARY.....
                    Vector thePerpendicular = new Vector(copy.x - fromCentertoP1.x, copy.y - fromCentertoP1.y);
                    double direction = 1;
                    Vector addthem = directionOfFirstVector.Copy();
                    addthem.addVector(thePerpendicular);
                    if (addthem.Magnitude < directionOfFirstVector.Magnitude)
                        direction = 0;

                    Line a = new Line(previousSegment.x1, previousSegment.y1, previousSegment.x2, previousSegment.y2);
                    Line b = new Line(currentSegment.x1, currentSegment.y1, currentSegment.x2, currentSegment.y2);
                    if (firstMoveDone && (Math.Abs(xend) > Math.Pow(10,-5) || Math.Abs(yend) > Math.Pow(10,-5)))
                        code += "MOVECIRC(" + xend.ToString() + "," + yend.ToString() + "," + xmid.ToString() + "," + ymid.ToString() + "," + direction.ToString() + ")\n";
                    
                    previousSegment = currentSegment.Copy();
                    previousVector.x = currentSegment.x1;
                    previousVector.y = currentSegment.y1;
                    prevX = x;
                    prevY = y;
                    firstMoveDone = true;
                }
            }
            code += "MOVEABS(" + previousSegment.x2.ToString() + "," + previousSegment.y2.ToString() + ")\n";
            code += "OFF\n";
            if(leftOfLineCB.Checked || rightOfLineCB.Checked)
                code = compensateString(code);
            return code;
        }
        private void closeXboxControllerButton_Click(object sender, EventArgs e)
        {
            backgroundWorker2.CancelAsync();
            xboxControllerButton.BackColor = Color.Red;
        }



        #region dxf
        private void button15_Click(object sender, EventArgs e)
        {
            String inputFileTxt = "";

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";		//sets the initial directory of the openfile dialog

            openFileDialog1.Filter = "dxf files (*.dxf)|*.dxf|All files (*.*)|*.*";	//filters the visible files...

            openFileDialog1.FilterIndex = 1;


            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)		//open file dialog is shown here...if "cancel" button is clicked then nothing will be done...
            {
                inputFileTxt = openFileDialog1.FileName;	//filename is taken (file path is also included to this name example: c:\windows\system\blabla.dxf

                int ino = inputFileTxt.LastIndexOf("\\");	//index no of the last "\" (that is before the filename) is found here

                if (inputFileTxt.Length > 0)
                {
                    ReadFromFile(inputFileTxt);		//the filename is sent to the method for data extraction and interpretation...
                }
            }

            openFileDialog1.Dispose();
        }


        List<Cut> theCuts = new List<Cut>();
        String moveCommands;
        String arcCommands;
        String circleCommands;
        bool doMove = false;
        double prevX = Double.NaN;
        double prevY = Double.NaN;

        public void ReadFromFile(string textFile)			//Reads a text file (in fact a DXF file) for importing an Autocad drawing.
        //In the DXF File structure, data is stored in two-line groupings ( or bi-line, coupling line ...whatever you call it)
        //in this grouping the first line defines the data, the second line contains the data value.
        //..as a result there is always even number of lines in the DXF file..
        {
            moveCommands = "";
            arcCommands = "";
            circleCommands = "";
            string line1, line2;							//these line1 and line2 is used for getting the a/m data groups...

            line1 = "0";									//line1 and line2 are are initialized here...
            line2 = "0";

            long position = 0;

            FileInfo theSourceFile = new FileInfo(textFile);		//the sourceFile is set.

            StreamReader reader = null;						//a reader is prepared...

            try
            {
                reader = theSourceFile.OpenText();			//the reader is set ...
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.FileName.ToString() + " cannot be found");
            }
            catch
            {
                MessageBox.Show("An error occured while opening the DXF file");
                return;
            }



            do
            {
                ////////////////////////////////////////////////////////////////////
                //This part interpretes the drawing objects found in the DXF file...
                ////////////////////////////////////////////////////////////////////
                if (line1.Contains("ENTITIES") || line2.Contains("ENTITIES"))
                {
                    doMove = true;
                }
                else if (line1.Contains("ENDSEC") || line2.Contains("ENDSEC"))
                {
                    doMove = false;
                }

                if (line1 == "0" && line2 == "LINE")
                    LineModule(reader);

                else if (line1 == "0" && line2 == "LWPOLYLINE")
                    MessageBox.Show("Polyline detected.");//PolylineModule(reader);

                else if (line1 == "0" && line2 == "CIRCLE")
                    CircleModule(reader);

                else if (line1 == "0" && line2 == "ARC")
                    ArcModule(reader);

                ////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////


                GetLineCouple(reader, out line1, out line2);		//the related method is called for iterating through the text file and assigning values to line1 and line2...

            }
            while (line2 != "EOF");

            List<Cut> greedyPath = bestPathGreedyAlgo(theCuts);

            String b = connectOrderedCuts(greedyPath.ToArray());
            MessageBox.Show(b);
            //Ordering o = new Ordering();
            //Cut[] a = o.getBestOrder(greedyPath.ToArray());
            //String path = connectOrderedCuts(a);
            //MessageBox.Show(path);
            //MessageBox.Show("raw\n" + moveCommands + "\n\n" + arcCommands + "\n\n" + circleCommands);
            //MessageBox.Show(arcCommands);
            //String patchedArcs = patchMoves(moveCommands, arcCommands, circleCommands);
            //MessageBox.Show(patchedArcs);

            reader.DiscardBufferedData();							//reader is cleared...
            theSourceFile = null;


            reader.Close();											//...and closed.

        }

        private string connectOrderedCuts(Cut[] a)
        {
            String code = "";
            Vector previous = null;
            for (int i = 0; i < a.Length; i++)
            {
                Cut theCut = a[i];
                code += getMoveFromCutCommand(theCut, previous, theCut.switched);
                if (!theCut.switched)
                    previous = theCut.p2;
                else
                    previous = theCut.p1;
            }
            return code;
        }

        private List<Cut> bestPathGreedyAlgo(List<Cut> theCuts)
        {
            List<Cut> theCutsCopy = new List<Cut>();
            foreach(Cut a in theCuts)
                theCutsCopy.Add(a);
            List<Cut> bestList = new List<Cut>();
            double shortestCost = double.MaxValue;
            int index = 0;
            for (int i = 0; i < theCutsCopy.Count; i++)
            {
                theCuts = new List<Cut>();
                foreach(Cut a in theCutsCopy){
                    theCuts.Add(a);
                }
                List<Cut> current = new List<Cut>();
                //theCuts = new List<Cut>(theCutsCopy);
                double cost = 0;
                //String code = "";
                Vector previousVector = null;

                while (theCuts.Count > 0)
                {
                    if (previousVector == null)
                    {
                        //code += getMoveFromCutCommand(theCuts[i], previousVector, false);

                        previousVector = theCuts[i].p2.Copy();
                        cost += theCuts[i].p1.getDistance(new Vector(0, 0));
                        current.Add(theCuts[i].Copy());
                        theCuts.RemoveAt(i);
                    }
                    else
                    {
                        double shortestDistance = Double.MaxValue;
                        int shortestIndex = -1;
                        bool secondPoint = false;
                        for (int j = 0; j < theCuts.Count; j++)
                        {
                            if (previousVector.getDistance(theCuts[j].p1) < shortestDistance)
                            {
                                secondPoint = false;
                                shortestDistance = previousVector.getDistance(theCuts[j].p1.Copy());
                                shortestIndex = j;
                            }
                            if (previousVector.getDistance(theCuts[j].p2) < shortestDistance)
                            {
                                secondPoint = true;
                                shortestDistance = previousVector.getDistance(theCuts[j].p2.Copy());
                                shortestIndex = j;
                            }
                        }
                        cost += shortestDistance;
                        //code += getMoveFromCutCommand(theCuts[shortestIndex], previousVector, secondPoint);

                        if (!secondPoint)
                        {
                            current.Add(theCuts[shortestIndex].Copy());
                            previousVector = theCuts[shortestIndex].p2.Copy();
                        }
                        else
                        {
                            
                            //theCuts[shortestIndex].switched = true;
                            Cut toAdd = theCuts[shortestIndex].Copy();
                            toAdd.switched = true;
                            current.Add(toAdd);
                            //MessageBox.Show(current[current.Count - 1].switched.ToString());
                            previousVector = theCuts[shortestIndex].p1.Copy();
                        }

                        theCuts.RemoveAt(shortestIndex);
                    }

                }
                if (cost < shortestCost)
                {
                    bestList = new List<Cut>();
                    foreach (Cut a in current)
                        bestList.Add(a);
                    //bestList = new List<Cut>(current);
                    shortestCost = cost;
                    index = i;
                    //bestIndex = code;
                }
            }
            return bestList;
        }

        private string getMoveFromCutCommand(Cut cut, Vector previousVector, bool useSecondPoint)
        {
            Vector compareVector = cut.p1;
            if (useSecondPoint)
                compareVector = cut.p2;
            if (previousVector == null || previousVector.CompareTo(compareVector) != 0)
            {
                if (cut.info.StartsWith("LINE"))
                {
                    if(!useSecondPoint)
                    return "OFF\nMOVEABS(" + cut.p1.x.ToString() + "," + cut.p1.y.ToString() + ")\nON\nMOVEABS(" + cut.p2.x.ToString() + "," + cut.p2.y.ToString() + ")\n";
                    
                    return "OFF\nMOVEABS(" + cut.p2.x.ToString() + "," + cut.p2.y.ToString() + ")\nON\nMOVEABS(" + cut.p1.x.ToString() + "," + cut.p1.y.ToString() + ")\n";

                }
                else if(cut.info.StartsWith("CIRCLE")){

                    double xmidAbs = Convert.ToDouble(cut.info.Split(':')[1]);
                    double ymidAbs = Convert.ToDouble(cut.info.Split(':')[2]);
                    double radius = Convert.ToDouble(cut.info.Split(':')[3]);
                    Vector center = new Vector(xmidAbs, ymidAbs);

                    Vector endAbs = center.Copy();
                    Vector startAbs = center.Copy();
                    startAbs.addVector(new Vector(0, radius));
                    endAbs.addVector(new Vector(0, radius));
                    String a = "";
                    a += "OFF\nMOVEABS(" + startAbs.x.ToString() + "," + startAbs.y.ToString() + ")\nON\n";
                    Vector endRel = new Vector(endAbs.x - startAbs.x, endAbs.y - startAbs.y);
                    Vector centerRel = new Vector(center.x - startAbs.x, center.y - startAbs.y);

                    a += "MOVECIRC(" + endRel.x.ToString() + "," + endRel.y.ToString() + "," + centerRel.x.ToString() + "," + centerRel.y.ToString() + ",1)\n";
                    return a;

                }
                else{

                    double xmidAbs = Convert.ToDouble(cut.info.Split(':')[1]);
                    double ymidAbs = Convert.ToDouble(cut.info.Split(':')[2]);
                    double radius = Convert.ToDouble(cut.info.Split(':')[3]);
                    double angle1 = Convert.ToDouble(cut.info.Split(':')[4]);
                    double angle2 = Convert.ToDouble(cut.info.Split(':')[5]);

                    Vector center = new Vector(xmidAbs, ymidAbs);

                    Vector toFirstPoint = center.getVectorFromMagnitudeAndAngle(radius, angle1);
                    Vector toSecondPoint = center.getVectorFromMagnitudeAndAngle(radius, angle2);
                    Vector endAbs = center.Copy();
                    Vector startAbs = center.Copy();
                    endAbs.addVector(toSecondPoint);
                    startAbs.addVector(toFirstPoint);

                    if (!useSecondPoint)
                    {
                        String a = "";
                        a += "OFF\nMOVEABS(" + startAbs.x.ToString() + "," + startAbs.y.ToString() + ")\nON\n";
                        Vector endRel = new Vector(endAbs.x - startAbs.x, endAbs.y - startAbs.y);
                        Vector centerRel = new Vector(center.x - startAbs.x, center.y - startAbs.y);

                        a += "MOVECIRC(" + endRel.x.ToString() + "," + endRel.y.ToString() + "," + centerRel.x.ToString() + "," + centerRel.y.ToString() + ",1)\n";
                        return a;
                    }
                    else
                    {
                        String a = "";
                        a += "OFF\nMOVEABS(" + endAbs.x.ToString() + "," + endAbs.y.ToString() + ")\nON\n";
                        Vector endRel = new Vector(startAbs.x - endAbs.x, startAbs.y - endAbs.y);
                        Vector centerRel = new Vector(center.x - endAbs.x, center.y - endAbs.y);

                        a += "MOVECIRC(" + endRel.x.ToString() + "," + endRel.y.ToString() + "," + centerRel.x.ToString() + "," + centerRel.y.ToString() + ",0)\n";
                        return a;
                    }
                }
            }
            else
            {
                    if (cut.info.StartsWith("LINE"))
                    {
                        if(!useSecondPoint)
                        return "MOVEABS(" + cut.p2.x.ToString() + "," + cut.p2.y.ToString() + ")\n";

                        return "MOVEABS(" + cut.p1.x.ToString() + "," + cut.p1.y.ToString() + ")\n";
                    }
                    else if (cut.info.StartsWith("CIRCLE"))
                    {
                        double xmidAbs = Convert.ToDouble(cut.info.Split(':')[1]);
                        double ymidAbs = Convert.ToDouble(cut.info.Split(':')[2]);
                        double radius = Convert.ToDouble(cut.info.Split(':')[3]);
                        return "MOVECIRC(0,0,0," + (-radius).ToString() + ",1)\n";
                    }
                    else
                    {
                        double xmidAbs = Convert.ToDouble(cut.info.Split(':')[1]);
                        double ymidAbs = Convert.ToDouble(cut.info.Split(':')[2]);
                        double radius = Convert.ToDouble(cut.info.Split(':')[3]);
                        double angle1 = Convert.ToDouble(cut.info.Split(':')[4]);
                        double angle2 = Convert.ToDouble(cut.info.Split(':')[5]);

                        Vector center = new Vector(xmidAbs, ymidAbs);

                        Vector toFirstPoint = center.getVectorFromMagnitudeAndAngle(radius, angle1);
                        Vector toSecondPoint = center.getVectorFromMagnitudeAndAngle(radius, angle2);
                        Vector endAbs = center.Copy();
                        Vector startAbs = center.Copy();
                        endAbs.addVector(toSecondPoint);
                        startAbs.addVector(toFirstPoint);
                        if (!useSecondPoint)
                        {
                            Vector endRel = new Vector(endAbs.x - startAbs.x, endAbs.y - startAbs.y);
                            Vector centerRel = new Vector(center.x - startAbs.x, center.y - startAbs.y);

                            return "MOVECIRC(" + endRel.x.ToString() + "," + endRel.y.ToString() + "," + centerRel.x.ToString() + "," + centerRel.y.ToString() + ",1)\n";
                        }
                        else
                        {
                            Vector endRel = new Vector(startAbs.x - endAbs.x, startAbs.y - endAbs.y);
                            Vector centerRel = new Vector(center.x - endAbs.x, center.y - endAbs.y);

                            return "MOVECIRC(" + endRel.x.ToString() + "," + endRel.y.ToString() + "," + centerRel.x.ToString() + "," + centerRel.y.ToString() + ",0)\n";
                        }
                    }

            }

        }

        private string patchMoves(string moveCommands, string arcCommands, string circleCommands)
        {
            int moveIndex = 0;
            //int arcIndex = 0;
            String[] moveLines = moveCommands.Split('\n');
            String[] arcLines = arcCommands.Split('\n');
            int moveNumLines = moveLines.Length;
            int arcNumLines = arcLines.Length;
            int[] arcsUsed = new int[arcNumLines - 1];
            Vector previousPosition = new Vector();
            String code = "";
            bool doNextMove = true;
            while (moveIndex < moveNumLines)
            {
                if (moveIndex < moveNumLines && lineStartsWith(moveLines[moveIndex],"OFF"))
                {
                    //check all arcs for a fit
                    bool arcFound = false;
                    for (int i = 0; i < arcNumLines - 1; i++)
                    {
                        double xmidAbs = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[0]);
                        double ymidAbs = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[1]);
                        double radius = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[2]);
                        double angle1 = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[3]) * Math.PI / 180.0;
                        double angle2 = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[4]) * Math.PI / 180.0;
                        DXFArc arc = new DXFArc(xmidAbs, ymidAbs, radius, angle1, angle2);
                        //MessageBox.Show("Angle: mid:" + xmidAbs.ToString() + " y: " + ymidAbs.ToString() + " rad: " + radius.ToString() + " angle1: " + angle1.ToString() + " angle2: " + angle2.ToString());

                        Vector center = new Vector(xmidAbs, ymidAbs);
                        //clockwise
                        Vector toFirstPoint1 = center.getVectorFromMagnitudeAndAngle(radius, angle1);
                        Vector toFirstPoint2 = center.getVectorFromMagnitudeAndAngle(radius, angle2);
                        //counterclockwise subtract instead
                        Vector location1 = center.Copy();
                        Vector location2 = center.Copy();
                        location1.addVector(toFirstPoint1);
                        location2.addVector(toFirstPoint2);
                        //MessageBox.Show(arc.ToString() + " transformed to: " + location.ToString());
                        //MessageBox.Show(location.ToString() + "arc start Compared to move end: " + previousPosition.ToString());
                        if (location1.CompareTo(previousPosition) == 0)
                        {
                            arcsUsed[i] = 1;
                            //MessageBox.Show("Counter clockwise found!");
                            //counter clockwise-standard for dxf
                            //make the arc.
                            //MessageBox.Show("HERE: " + location.ToString());
                            //find end location\
                            //clockwise
                            Vector toSecondPoint = center.getVectorFromMagnitudeAndAngle(radius, angle2);
                            Vector endAbs = center.Copy();
                            endAbs.addVector(toSecondPoint);
                            Vector endRel = new Vector(endAbs.x - previousPosition.x, endAbs.y - previousPosition.y );
                            Vector centerRel = new Vector(center.x - previousPosition.x, center.y - previousPosition.y);

                            code += "MOVECIRC(" + endRel.x.ToString() + "," + endRel.y.ToString() + "," + centerRel.x.ToString() + "," + centerRel.y.ToString() + ",0)\n";
                            doNextMove = false;
                            arcFound = true;
                        }
                        else if (location2.CompareTo(previousPosition) == 0)
                        {
                            arcsUsed[i] = 1;
                            //clockwise
                            Vector toSecondPoint = center.getVectorFromMagnitudeAndAngle(radius, angle1);
                            Vector endAbs = center.Copy();
                            endAbs.addVector(toSecondPoint);
                            Vector endRel = new Vector(endAbs.x - previousPosition.x, endAbs.y - previousPosition.y);
                            Vector centerRel = new Vector(center.x - previousPosition.x, center.y - previousPosition.y);

                            code += "MOVECIRC(" + endRel.x.ToString() + "," + endRel.y.ToString() + "," + centerRel.x.ToString() + "," + centerRel.y.ToString() + ",1)\n";
                            doNextMove = false;
                            arcFound = true;
                        }
                    }
                    if (!arcFound)
                        code += moveLines[moveIndex] + "\n";
                    moveIndex++;
                }
                else if(moveIndex < moveNumLines)
                {
                    if (doNextMove)
                    {
                        code += moveLines[moveIndex] + "\n";
                        if (lineStartsWith(moveLines[moveIndex], "MOVEABS"))
                        {
                            previousPosition.x = Convert.ToDouble(moveLines[moveIndex].Split('(')[1].Split(',')[0]);
                            previousPosition.y = Convert.ToDouble(moveLines[moveIndex].Split('(')[1].Split(',')[1].Split(')')[0]);
                        }
                    }
                    else
                    {
                        moveIndex++;
                    }
                    doNextMove = true;
                    moveIndex++;
                }

               
            }

            //add circles

            for (int i = 0; i < circleCommands.Split('\n').Length -1; i++)
            {
                double x = Convert.ToDouble(circleCommands.Split('\n')[i].Split(':')[1].Split(',')[0]);
                double y = Convert.ToDouble(circleCommands.Split('\n')[i].Split(':')[1].Split(',')[1]);
                double r = Convert.ToDouble(circleCommands.Split('\n')[i].Split(':')[1].Split(',')[2]);
                code += "OFF\n" + "MOVEABS(" + x.ToString() + "," + (y + r).ToString() + ")\nON\n" + "MOVECIRC(0,0,0," + (-r).ToString() + ",1)\n";
            }

            //add unused arcs... in order.. could be better mechanism like searching for closest or somethin
            int count = 0;
            for (int i = 0; i < arcsUsed.Length; i++)
            {
                if (arcsUsed[i] == 1)
                    count++;
            }
            MessageBox.Show("Amount of arcs already used: " + count.ToString() + " out of " + arcsUsed.Length.ToString());
            MessageBox.Show(code);
            bool atStartArc = false;
            Vector[] startPoints = new Vector[arcNumLines - 1];
            for (int i = 0; i < arcNumLines - 1; i++)
            {
                double xmidAbs = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[0]);
                double ymidAbs = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[1]);
                double radius = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[2]);
                double angle1 = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[3]) * Math.PI / 180.0;
                double angle2 = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[4]) * Math.PI / 180.0;
                Vector center = new Vector(xmidAbs, ymidAbs);
                //clockwise
                Vector toFirstPoint1 = center.getVectorFromMagnitudeAndAngle(radius, angle1);
                Vector toFirstPoint2 = center.getVectorFromMagnitudeAndAngle(radius, angle2);
                //counterclockwise subtract instead
                Vector location1 = center.Copy();
                Vector location2 = center.Copy();
                location1.addVector(toFirstPoint1);
                location2.addVector(toFirstPoint2);
                startPoints[i] = location2;
            }
                Vector currentPosition = null;
                for (int i = 0; i < arcNumLines - 1; i++)
                {
                    double xmidAbs; // Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[0]);
                    double ymidAbs; // = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[1]);
                    double radius; // = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[2]);
                    double angle1; // = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[3]) * Math.PI / 180.0;
                    double angle2; // = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[4]) * Math.PI / 180.0;
                    if (currentPosition != null)
                    {
                        double shortestDistance = Double.MaxValue;
                        int shortestIndex = 0;
                        for (int j = 0; j < arcNumLines - 1; j++)
                        {
                            if (arcsUsed[j] == 1)
                                continue;
                            if(currentPosition.getDistance(startPoints[j]) < shortestDistance){
                                shortestDistance = currentPosition.getDistance(startPoints[j]);
                                shortestIndex = j;
                            }
                        }
                        arcsUsed[shortestIndex] = 1;
                        xmidAbs = Convert.ToDouble(arcLines[shortestIndex].Split(':')[1].Split(',')[0]);
                        ymidAbs = Convert.ToDouble(arcLines[shortestIndex].Split(':')[1].Split(',')[1]);
                        radius = Convert.ToDouble(arcLines[shortestIndex].Split(':')[1].Split(',')[2]);
                        angle1 = Convert.ToDouble(arcLines[shortestIndex].Split(':')[1].Split(',')[3]) * Math.PI / 180.0;
                        angle2 = Convert.ToDouble(arcLines[shortestIndex].Split(':')[1].Split(',')[4]) * Math.PI / 180.0;
                    }
                    else
                    {
                        if (arcsUsed[i] == 1)
                            continue;
                        xmidAbs = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[0]);
                        ymidAbs = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[1]);
                        radius = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[2]);
                        angle1 = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[3]) * Math.PI / 180.0;
                        angle2 = Convert.ToDouble(arcLines[i].Split(':')[1].Split(',')[4]) * Math.PI / 180.0;
                        arcsUsed[i] = 1;
                    }
                    DXFArc arc = new DXFArc(xmidAbs, ymidAbs, radius, angle1, angle2);
                    //MessageBox.Show("Angle: mid:" + xmidAbs.ToString() + " y: " + ymidAbs.ToString() + " rad: " + radius.ToString() + " angle1: " + angle1.ToString() + " angle2: " + angle2.ToString());

                    Vector center = new Vector(xmidAbs, ymidAbs);
                    ////clockwise
                    //Vector toFirstPoint1 = center.getVectorFromMagnitudeAndAngle(radius, angle1);
                    //Vector toFirstPoint2 = center.getVectorFromMagnitudeAndAngle(radius, angle2);
                    ////counterclockwise subtract instead
                    //Vector location1 = center.Copy();
                    //Vector location2 = center.Copy();
                    //location1.addVector(toFirstPoint1);
                    //location2.addVector(toFirstPoint2);

                    Vector toFirstPoint = center.getVectorFromMagnitudeAndAngle(radius, angle2);
                    Vector toSecondPoint = center.getVectorFromMagnitudeAndAngle(radius, angle1);
                    Vector endAbs = center.Copy();
                    Vector startAbs = center.Copy();
                    endAbs.addVector(toSecondPoint);
                    startAbs.addVector(toFirstPoint);
                    code += "OFF\nMOVEABS(" + startAbs.x.ToString() + "," + startAbs.y.ToString() + ")\nON\n";
                    Vector endRel = new Vector(endAbs.x - startAbs.x, endAbs.y - startAbs.y);
                    Vector centerRel = new Vector(center.x - startAbs.x, center.y - startAbs.y);

                    code += "MOVECIRC(" + endRel.x.ToString() + "," + endRel.y.ToString() + "," + centerRel.x.ToString() + "," + centerRel.y.ToString() + ",1)\n";
                    currentPosition = endAbs;
                }


                return code;
        }


        private void GetLineCouple(StreamReader theReader, out string line1, out string line2)		//this method is used to iterate through the text file and assign values to line1 and line2
        {
            System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            string decimalSeparator = ci.NumberFormat.CurrencyDecimalSeparator;

            line1 = line2 = "";

            if (theReader == null)
                return;

            line1 = theReader.ReadLine();
            if (line1 != null)
            {
                line1 = line1.Trim();
                line1 = line1.Replace('.', decimalSeparator[0]);

            }
            line2 = theReader.ReadLine();
            if (line2 != null)
            {
                line2 = line2.Trim();
                line2 = line2.Replace('.', decimalSeparator[0]);
            }
        }


        private void LineModule(StreamReader reader)		//Interpretes line objects in the DXF file
        {
            string line1, line2;
            line1 = "0";
            line2 = "0";

            double x1 = 0;
            double y1 = 0;
            double x2 = 0;
            double y2 = 0;

            do
            {
                GetLineCouple(reader, out line1, out line2);

                if (line1 == "10")
                {
                    x1 = Convert.ToDouble(line2);
                }

                if (line1 == "20")
                {
                    y1 = Convert.ToDouble(line2);
                }

                if (line1 == "11")
                {
                    x2 = Convert.ToDouble(line2);
                }

                if (line1 == "21")
                {
                    y2 = Convert.ToDouble(line2);
                }


            }
            while (line1 != "21");


            //MessageBox.Show("Line: (" + x1.ToString() + "," + y1.ToString() + ")  to (" + x2.ToString() + "," + y2.ToString() + ")");
            //if(enterBlock <= 0)
            if (doMove)
            {
                theCuts.Add(new Cut(new Vector(x1, y1), new Vector(x2, y2), "LINE"));

                if (x1 == prevX && y1 == prevY)
                {
                    //MessageBox.Show("MOVEABS(" + x2.ToString() + "," + y2.ToString() + ")\n");
                    moveCommands += "MOVEABS(" + x2.ToString() + "," + y2.ToString() + ")\n";
                }
                else
                {
                    //MessageBox.Show("OFF\nMOVEABS(" + x1.ToString() + "," + y1.ToString() + ")\nON\n" + "MOVEABS(" + x2.ToString() + "," + y2.ToString() + ")\n");
                    moveCommands += "OFF\nMOVEABS(" + x1.ToString() + "," + y1.ToString() + ")\nON\n" + "MOVEABS(" + x2.ToString() + "," + y2.ToString() + ")\n";
                }
                prevX = x2;
                prevY = y2;
            }


        }


        //private void PolylineModule(StreamReader reader)	//Interpretes polyline objects in the DXF file
        //{
        //    string line1, line2;
        //    line1 = "0";
        //    line2 = "0";

        //    double x1 = 0;
        //    double y1 = 0;
        //    double x2 = 0;
        //    double y2 = 0;


        //    thePolyLine = new polyline(Color.White, 1);

        //    int ix = drawingList.Add(thePolyLine);
        //    objectIdentifier.Add(new DrawingObject(5, ix));

        //    int counter = 0;
        //    int numberOfVertices = 1;
        //    int openOrClosed = 0;
        //    ArrayList pointList = new ArrayList();


        //    do
        //    {
        //        GetLineCouple(reader, out line1, out line2);

        //        if (line1 == "90")
        //            numberOfVertices = Convert.ToInt32(line2);

        //        if (line1 == "70")
        //            openOrClosed = Convert.ToInt32(line2);


        //        if (line1 == "10")
        //        {
        //            x1 = Convert.ToDouble(line2);
        //        }

        //        if (line1 == "20")
        //        {
        //            y1 = Convert.ToDouble(line2);
        //            pointList.Add(new Point((int)x1, (int)-y1));
        //            counter++;
        //        }

        //    }
        //    while (counter < numberOfVertices);
        //}


        private void CircleModule(StreamReader reader)		//Interpretes circle objects in the DXF file
        {
            string line1, line2;
            line1 = "0";
            line2 = "0";

            double x1 = 0;
            double y1 = 0;

            double radius = 0;

            do
            {
                GetLineCouple(reader, out line1, out line2);

                if (line1 == "10")
                {
                    x1 = Convert.ToDouble(line2);

                }


                if (line1 == "20")
                {
                    y1 = Convert.ToDouble(line2);

                }


                if (line1 == "40")
                {
                    radius = Convert.ToDouble(line2);
                }



            }
            while (line1 != "40");

            theCuts.Add(new Cut(new Vector(x1, y1 + radius), new Vector(x1, y1 + radius), "CIRCLE:" + x1.ToString() + ":" + y1.ToString() + ":" + radius.ToString()));

            circleCommands += "Circle:" + x1.ToString() + "," + y1.ToString() + "," + radius.ToString() + "\n";
        }


        private void ArcModule(StreamReader reader)		//Interpretes arc objects in the DXF file
        {
            string line1, line2;
            line1 = "0";
            line2 = "0";

            double x1 = 0;
            double y1 = 0;

            double radius = 0;
            double angle1 = 0;
            double angle2 = 0;

            do
            {
                GetLineCouple(reader, out line1, out line2);

                if (line1 == "10")
                {
                    x1 = Convert.ToDouble(line2);
                }


                if (line1 == "20")
                {
                    y1 = Convert.ToDouble(line2);
                }


                if (line1 == "40")
                {
                    radius = Convert.ToDouble(line2);
                }

                if (line1 == "50")
                    angle1 = Convert.ToDouble(line2);

                if (line1 == "51")
                    angle2 = Convert.ToDouble(line2);


            }
            while (line1 != "51");

            angle1 = angle1 * Math.PI / 180.0;
            angle2 = angle2 * Math.PI / 180.0;

            Vector center = new Vector(x1, y1);
            Vector toFirstPoint1 = center.getVectorFromMagnitudeAndAngle(radius, angle1);
            Vector toFirstPoint2 = center.getVectorFromMagnitudeAndAngle(radius, angle2);
            Vector location1 = center.Copy();
            Vector location2 = center.Copy();
            location1.addVector(toFirstPoint1);
            location2.addVector(toFirstPoint2);
            theCuts.Add(new Cut(location2, location1, "ARC:" + x1.ToString() + ":" + y1.ToString() + ":" + radius.ToString() + ":" + angle2.ToString() + ":" + angle1.ToString()));
            arcCommands += "ARC:" + x1.ToString() + "," + y1.ToString() + "," + radius.ToString() + "," + angle1.ToString() + "," + angle2.ToString() + "\n";
        }

        private double transformAngleToNormalPlane(double angle1)
        {
            double a = 540 - angle1;
            while (a >= 360)
            {
                a = a - 360;
            }
            return a;
        }
        #endregion

        private void button16_Click(object sender, EventArgs e)
        {
            try { 
            //cases:
            //line to line any angle: Go to intersection of the lines offset by the radius.
            //line to arc: Use vectors, add the radius
            //arc to line: 
            String TrioBasicCode = "";
            String currentSpeed = "";
            String theFile = "";
            OpenFileDialog file = new OpenFileDialog();
            file.FileName = "";
            file.Title = "Open A Text document.";
            file.Filter = "(*.tap)|*.tap|All Files (*.*)|*.*";
            DialogResult result = file.ShowDialog();
            if (result == DialogResult.OK)
            {
                System.IO.StreamReader OpenFile = new System.IO.StreamReader(file.FileName);
                theFile = OpenFile.ReadToEnd();
                OpenFile.Close();

                //Read X Y Z coordinates from the opened file:

                Regex Gcode = new Regex("[ngxyzfijm][+-]?[0-9]*\\.?[0-9]*", RegexOptions.IgnoreCase);
                MatchCollection m = Gcode.Matches(theFile);
                int g_code = 0;
                int m_code = 0;
                double x_code = 0, y_code = 0, i_code, j_code, f_code;
                double xpos = 0;
                double ypos = 0;

                String[] lines = theFile.Split('\n');
                string lastCommand = "";
                for (int i = 1; i < lines.Length; i++)
                {
                    String line = lines[i];
                    g_code = -1;
                    x_code = 0;
                    y_code = 0;
                    i_code = 0;
                    j_code = 0;
                    m_code = 0;
                    f_code = 0;
                    if ((line.Length > 0 && line.Substring(0, 1) == "("))
                        continue;

                    m = Gcode.Matches(lines[i]);
                    bool xValChanged = false;
                    bool yValChanged = false;
                    foreach (Match n in m)
                    {
                        if (n.Value.StartsWith("G"))
                        {
                            g_code = Convert.ToInt32(ExtractNumbers(n.Value));
                            if (g_code == 40)
                                TrioBasicCode += "CUTTER COMPENSATION OFF\n";
                            if (g_code == 41)
                                TrioBasicCode += "CUTTER COMPENSATION LEFT\n";
                            if (g_code == 42)
                                TrioBasicCode += "CUTTER COMPENSATION RIGHT\n";
                            if (g_code == 0)
                            {
                                //rapid speed
                                if (currentSpeed != "RAPID")
                                {
                                    currentSpeed = "RAPID";
                                    TrioBasicCode += "SPEED = RAPID\n";
                                }
                            }
                            if (g_code == 1)
                            {
                                //feed speed
                                if (currentSpeed != "FEED")
                                {
                                    currentSpeed = "FEED";
                                    TrioBasicCode += "SPEED = FEED\n";
                                }
                            }
                            if (g_code == 28)
                            {
                                TrioBasicCode += "MOVEABS(0,0)\n";
                            }

                        }

                        if (n.Value.StartsWith("X"))
                        {
                            x_code = Convert.ToDouble(ExtractNumbers(n.Value));
                            xValChanged = true;
                        }

                        if (n.Value.StartsWith("Y"))
                        {
                            y_code = Convert.ToDouble(ExtractNumbers(n.Value));
                            yValChanged = true;
                        }

                        if (n.Value.StartsWith("I"))
                        {
                            i_code = Convert.ToDouble(ExtractNumbers(n.Value));
                        }

                        if (n.Value.StartsWith("J"))
                        {
                            j_code = Convert.ToDouble(ExtractNumbers(n.Value));
                        }

                        if (n.Value.StartsWith("Z"))
                        {
                            //MessageBox.Show("Z Coordinate Detected at line: " + i.ToString());
                        }
                        if (n.Value.StartsWith("M"))
                        {
                            m_code = Convert.ToInt32(ExtractNumbers(n.Value));
                        }
                        if (n.Value.StartsWith("F"))
                        {
                            f_code = Convert.ToDouble(ExtractNumbers(n.Value));
                        }
                    }
                    string nextCommand = "";
                    string onCommand = "";
                    switch (m_code)
                    {
                        case 4:
                            onCommand = "ON";
                            break;
                        case 5:
                            onCommand = "OFF";
                            break;
                        default:
                            break;
                    }
                    switch (g_code)
                    {
                        case 0:
                            //Rapid Position, moveABS, adjust speed
                            if (xValChanged)
                                xpos = x_code;
                            if (yValChanged)
                                ypos = y_code;
                            if (xValChanged || yValChanged)
                            {
                                nextCommand = "MOVEABS(" + xpos + "," + ypos + ")\n";
                            }
                            break;
                        case 1:
                            if (xValChanged)
                                xpos = x_code;
                            if (yValChanged)
                                ypos = y_code;
                            if (xValChanged || yValChanged)
                            {
                                nextCommand = "MOVEABS(" + xpos + "," + ypos + ")\n";
                            }
                            break;
                        case 2:
                            //Clockwise circle
                            double xEndMoveCirc = 0;
                            if (xValChanged)
                            {
                                xEndMoveCirc = x_code - xpos;
                            }
                            double yEndMoveCirc = 0;
                            if (yValChanged)
                            {
                                yEndMoveCirc = y_code - ypos;
                            }

                            nextCommand = "MOVECIRC(" + xEndMoveCirc + "," + yEndMoveCirc + "," + i_code + "," + j_code + "," + "1)\n";
                            if (xValChanged)
                                xpos = x_code;
                            if (yValChanged)
                                ypos = y_code;
                            break;
                        case 3:
                            //Counter Clockwise circle
                            double xEndMoveCirc1 = 0;
                            if (xValChanged)
                            {
                                xEndMoveCirc1 = x_code - xpos;
                            }
                            double yEndMoveCirc1 = 0;
                            if (yValChanged)
                            {
                                yEndMoveCirc1 = y_code - ypos;
                            }
                            nextCommand = "MOVECIRC(" + xEndMoveCirc1 + "," + yEndMoveCirc1 + "," + i_code + "," + j_code + "," + "0)\n";
                            if (xValChanged)
                                xpos = x_code;
                            if (yValChanged)
                                ypos = y_code;
                            break;
                        default:
                            if (xValChanged)
                                xpos = x_code;
                            if (yValChanged)
                                ypos = y_code;
                            if (xValChanged || yValChanged)
                                nextCommand = "MOVEABS(" + xpos + "," + ypos + ")\n";
                            break;
                    }
                    if (f_code != 0)
                        TrioBasicCode += "FEED SPEED = " + f_code.ToString() + "\n";
                    if (onCommand != "")
                    {
                        TrioBasicCode += onCommand + '\n';
                    }
                    if (nextCommand != "")
                    {
                        if (lastCommand != nextCommand)
                            TrioBasicCode += nextCommand;
                        //MessageBox.Show("Last Command :" + lastCommand + " Next Command: " + nextCommand);
                        lastCommand = nextCommand;
                    }
                }
                Clipboard.SetText(addOffsetToText(TrioBasicCode));

                if (!withinBoundaries(Clipboard.GetText()))
                {
                    MessageBox.Show("Not Within Boundaries!");
                }

                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//REL//SURE-Jet//" + "BASIC CODE" + ".txt", TrioBasicCode);
                MessageBox.Show("Latest G-Code program is on file");
            }
            }
                catch{
                    MessageBox.Show("Failed to read g-code file. Use Camsoft plasma post processor");
                }
            
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (feedSpeedTB.Text == "" || feedSpeedTB.Text.Contains("-"))
            {
                feedSpeedTB.BackColor = Color.Red;
                feedSpeedTB.Refresh();
                Thread.Sleep(300);
                feedSpeedTB.BackColor = Color.White;
                return;
            }
            double wantedSpeed = Convert.ToDouble(feedSpeedTB.Text);
            if (wantedSpeed > Trio.maxSpeed || wantedSpeed <= 0)
            {
                MessageBox.Show("Invalid speed. Max speed is " + Trio.maxSpeed.ToString() + " in/s.");
            }
            else
            {
                Trio.feedSpeed = Convert.ToDouble(feedSpeedTB.Text);
                Trio.PercentageSpeedOverride = Trio.PercentageSpeedOverride;//This actually sends the command to controller
            }
            updateSpeedLabels();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (rapidSpeedTB.Text == "" || rapidSpeedTB.Text.Contains("-"))
            {
                rapidSpeedTB.BackColor = Color.Red;
                rapidSpeedTB.Refresh();
                Thread.Sleep(300);
                rapidSpeedTB.BackColor = Color.White;
                return;
            }
            double wantedSpeed = Convert.ToDouble(rapidSpeedTB.Text);
            if (wantedSpeed > Trio.maxSpeed || wantedSpeed <= 0)
            {
                MessageBox.Show("Invalid speed. Max speed is " + Trio.maxSpeed.ToString() + " in/s.");
            }
            else
            {
                Trio.rapidSpeed = Convert.ToDouble(rapidSpeedTB.Text);
                Trio.PercentageSpeedOverride2 = Trio.PercentageSpeedOverride2;
            }
            updateSpeedLabels();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            polygonTextBox.Text = "";
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                int iPortID = 3240;
                if (Trio.axTrioPC.IsOpen(iPortID))
                {
                    Thread.Sleep(10);
                    backgroundWorker3.ReportProgress(1);
                }
            }
        }
        private void backgroundWorker3_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //read the inputs. 1 should be 0, 2 should be 1, and 3 should be 0.
            Console.WriteLine("Background worker3 progress being reported");
            int input1 = Trio.digitalRead(1);
            int input2 = Trio.digitalRead(2);
            int input3 = Trio.digitalRead(3);

            if (input1 != 0 || input2 != 1 || input3 != 0)
            {
                //stop, hit the boundaries
                Trio.axTrioPC.RapidStop();
                Trio.turnOffWatchDogAndServos(true);
                Console.WriteLine("STOP! inputs: " + input1.ToString() + input2.ToString() + input3.ToString());
            }

            xCurrentLocation.Text = "X: " + Math.Round((Trio.getVariable("X", "MPOS") - Trio.Offset.X),2).ToString();
            yCurrentLocation.Text = "Y: " + Math.Round((Trio.getVariable("Y", "MPOS") - Trio.Offset.Y),2).ToString();
            //double newReadVal;
            //if (axTrioPC.GetVariable("NIO", out newReadVal))
            //{
                
            //    String newState = input13.ToString() + input14.ToString() + input15.ToString();

            //    if (!stopSensors.Equals(newState))
            //    {
            //        stopSensors = newState;
            //        stopSensorsFiredRTB.Text += "\n" + newState;
            //    }
                
            //}
        }

        private void button20_Click(object sender, EventArgs e)
        {
            XboxControllerDiagramForm a = new XboxControllerDiagramForm();
            a.Show();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            //same function as the xbox back button
            //stop and allow jogging until the start button is pressed
            if (!manualModeController)//code is running
            {
                pauseMode = true;
                backgroundWorker1.CancelAsync();
                manualModeController = true;
                Trio.Cancelled = true;
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            //same function as the xbox start button
            if (pauseMode)
            {
                Trio.Cancelled = false;
                manualModeController = false;
                pauseMode = false;
                backgroundWorker1.RunWorkerAsync(currentPath);
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            cutInsideCircleCB.Checked = true;
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            cutOutsideCircleCB.Checked = true;
        }

        private void pictureBox4_MouseClick(object sender, MouseEventArgs e)
        {
            cutInsideRectangleCB.Checked = true;
        }

        private void pictureBox3_MouseClick(object sender, MouseEventArgs e)
        {
            cutOutsideRectangleCB.Checked = true;
        }

        private void pictureBox10_MouseClick(object sender, MouseEventArgs e)
        {
            centerOfLineCB.Checked = true;
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            leftOfLineCB.Checked = true;
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            rightOfLineCB.Checked = true;
        }
        private bool withinBoundaries(String code)
        {
            double currentX = 0; //Trio.Offset.X; //This might be wrong. Maybe MPOS?
            double currentY = 0; // Trio.Offset.Y; //""
            bool withinBoundaries = true;
            String[] lines = code.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                String line = lines[i];
                if (line.StartsWith("MOVEABS"))
                {
                    double xVal = Convert.ToDouble(line.Split('(')[1].Split(',')[0]);
                    double yVal = Convert.ToDouble(line.Split('(')[1].Split(',')[1].Split(')')[0]);
                    currentX = xVal;// +Trio.Offset.X;
                    currentY = yVal;// +Trio.Offset.Y;
                    if (currentX > Trio.xLimitUpper || currentX < Trio.xLimitLower || currentY > Trio.yLimitUpper || currentY < Trio.yLimitLower)
                    {
                        Console.WriteLine(i.ToString() + ": " + line);
                        withinBoundaries = false;
                    }
                }
                else if (line.StartsWith("MOVECIRC"))
                {
                    double xend = Convert.ToDouble(line.Split('(')[1].Split(',')[0]);
                    double yend = Convert.ToDouble(line.Split('(')[1].Split(',')[1]);
                    double xmid = Convert.ToDouble(line.Split('(')[1].Split(',')[2]);
                    double ymid = Convert.ToDouble(line.Split('(')[1].Split(',')[3]);
                    short direction = Convert.ToInt16(line.Split('(')[1].Split(',')[4].Split(')')[0]);
                    double radius = Math.Sqrt(Math.Pow(xmid, 2) + Math.Pow(ymid, 2));

                    if (arcIntersectsBoundaries(new Vector(currentX, currentY), xend, yend, xmid, ymid, direction))
                    {
                        Console.WriteLine(i.ToString() + ": " + line);
                        withinBoundaries = false;
                    }
                    //if ((currentX + radius) > Trio.xLimitUpper || (currentX - radius) < Trio.xLimitLower || (currentY + radius) > Trio.yLimitUpper || (currentY - radius) < Trio.yLimitLower)
                    //    possiblyOutOfBounds = true;
                    currentX = currentX + xend;
                    currentY = currentY + yend;
                }
            }
            return withinBoundaries;
        }
        
        private void button23_Click(object sender, EventArgs e)
        {
            Trio.setVariable("X", "MPOS", 0);
            Trio.setVariable("Y", "MPOS", 0);
            Console.WriteLine("MPOS on X axis " + Trio.getVariable("X", "MPOS"));
            //Trio.xLimitLower = 0;
            //Trio.yLimitUpper = 100;
            //Trio.xLimitUpper = 100;
            //Trio.yLimitLower = 0;
            //MessageBox.Show("Path is within boundaries: " + withinBoundaries(Clipboard.GetText()).ToString());
        }

        private bool arcIntersectsBoundaries(Vector loc, double xend, double yend, double xmid, double ymid, short dir)
        {
            Line lowerX = new Line(Trio.xLimitLower, 0,Trio.xLimitLower, 10);
            Line upperX = new Line(Trio.xLimitUpper, 0, Trio.xLimitUpper, 10);
            Line lowerY = new Line(0, Trio.yLimitLower,10, Trio.yLimitLower);
            Line upperY = new Line(0, Trio.yLimitUpper, 10, Trio.yLimitUpper);

            Vector start = new Vector(loc.x, loc.y);
            Vector center = new Vector(loc.x + xmid, loc.y + ymid);
            Vector end = new Vector(loc.x + xend, loc.y + yend);
            bool clockwise = dir == 1;
            double angle1 = new Vector(start.x - center.x, start.y - center.y).angle;
            double angle2 = new Vector(end.x - center.x, end.y - center.y).angle;
            angle1 = putAngleInNormalRange(angle1);
            angle2 = putAngleInNormalRange(angle2);
            Circle circ = new Circle(center, center.getDistance(start));
            List<Vector[]> theList = new List<Vector[]>();
            theList.Add(circ.getIntersections(lowerX));
            theList.Add(circ.getIntersections(upperX));
            theList.Add(circ.getIntersections(lowerY));
            theList.Add(circ.getIntersections(upperY));
            int idx = 0;
            foreach (Vector[] a in theList)
            {
                if (a[0] != null)
                {
                    //Console.WriteLine("Intersections with circle at: " + a[0].ToString() + " and at " + a[1].ToString());
                    //MessageBox.Show(a[0].ToString() + " :Intersection 1, Intersection 2: " + a[1].ToString());
                    a[0].addVector(new Vector(-center.x, -center.y));
                    double ang1 = a[0].angle;
                    ang1 = putAngleInNormalRange(ang1);

                    a[1].addVector(new Vector(-center.x, -center.y));
                    double ang2 = a[1].angle;
                    ang2 = putAngleInNormalRange(ang2);

                    if (angleIsWithinOtherAngles(ang1, angle1, angle2, dir))
                    {
                        Console.WriteLine("Ang1 : " + ang1.ToString() + " is within " + angle1.ToString() + " and " + angle2.ToString() + " Dir: " + dir);
                        return true;
                    }
                    if (angleIsWithinOtherAngles(ang2, angle1, angle2, dir))
                    {
                        Console.WriteLine(" Ang 2: " + ang2.ToString() + " is within " + angle1.ToString() + " and " + angle2.ToString() + " Dir: " + dir);
                        return true;
                    }
                }
            }
            return false;
        }

        private double putAngleInNormalRange(double a)
        {
            while (a < 0)
                a = a + Math.PI * 2;
            while (a > Math.PI * 2)
                a = a - Math.PI * 2;
            return a;
        }
        
        private void button24_Click(object sender, EventArgs e)
        {

        }
        private bool angleIsWithinOtherAngles(double theAngle, double angle1, double angle2, double direction)
        {
            if (angle1 == angle2)
                return true; //must be true
            //if (angle1 < angle2)
            //{
            //    //normal case, no overlap around the unit circle
            //    if (theAngle >= angle1 && theAngle <= angle2 && direction == 0)
            //        return true;

            //    if ((theAngle <= angle1 && theAngle >= 0) || (theAngle >= angle2 && theAngle <= Math.PI*2) && direction == 1)
            //        return true;
            //}
            //else
            //{
            //    //angle1 > angle2
            //    //there is overlap, so you have to check that somehow
            //    if ((theAngle >= angle1 && theAngle <= Math.PI*2) || (theAngle >= 0 && theAngle <= angle2 )  && direction == 0)
            //        return true;
            //    if (theAngle <= angle1 && theAngle >= angle2 && direction == 1)
            //        return true;
            //}
            if (direction == 1)
            {
                //swap angle1 and angle2 so youre always going ccw or crossing the 2pi line.
                //if (angle1 > angle2)
                //{
                    double temp = angle2;
                    angle2 = angle1;
                    angle1 = temp;
                //}
                //else
                //{

                //}
            }
            if (angle1 > angle2)
            {
                //it wraps around.
                if (theAngle >= angle1 && theAngle <= 2 * Math.PI || theAngle <= angle2 && theAngle >= 0)
                    return true;
            }
            else
            {
                //it's normal
                if (theAngle >= angle1 && theAngle <= angle2)
                    return true;
            }


            return false;
        }

        private void button24_Click_1(object sender, EventArgs e)
        {
            Trio.turnOnWatchDogAndServos(true);

            //Trio.moveAbs("X", "Y", Convert.ToDouble(recXLocationTB.Text), Convert.ToDouble(recYLocationTB.Text), 0, 0);
            Thread.Sleep(1000);
            Trio.setSpeed(true,0);
            Console.WriteLine("Speed set to zero, see if other axis can make it go.");

            //Trio.moveAbs("VirtualX", "VirtualY", Convert.ToDouble(circleXLocationTB.Text), Convert.ToDouble(circleYLocationTB.Text), 0, 0);
            //Trio.waitForEndOfMove();
            while(Trio.inMotion("VirtualX","VirtualY")) { }

            Console.WriteLine("Virtual move is done, finsihing orig");
            Thread.Sleep(1000);
            Trio.setSpeed(true,200);
            Trio.waitForEndOfMove(true);

            Trio.turnOffWatchDogAndServos(true);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            progressBar1.Update();
        }

        private void waterLabel_DoubleClick(object sender, EventArgs e)
        {
            Trio.WaterOn = !Trio.WaterOn;
        }

        private void setZeroButton_Click(object sender, EventArgs e)
        {
            setAbsoluteZeroAndLimits();
        }

        private void setAbsoluteZeroAndLimits()
        {
            //runs code to find the limits and set the 0.
            Trio.unsetLimits();
            manualModeController = false;
            _timer.Stop();
            Trio.axTrioPC.Stop("ENDSTOP");

            Trio.turnOnWatchDogAndServos(true);
            //find x = 0.
            //read the inputs. 2 should be 0, 3 should be 1, and 4 should be 0.
            while (Trio.digitalRead(2) == 0)
            {
                //keep jogging toward -x
                if (Trio.digitalRead(3) != 1 || Trio.digitalRead(4) != 0)
                {
                    Trio.turnOffJog(true);
                    Trio.turnOffWatchDogAndServos(true);
                    MessageBox.Show("Was trying to find the lower X limit and another limit was tripped.");
                    return;
                }
                Trio.setJogSpeed("X", .5);
                Trio.JogBackward("X");
            }
            //you are on the -x end stop. Move forward .5 inch.
            Trio.turnOffJog(true);
            Console.WriteLine("Found X-");
            Trio.PercentageSpeedOverride = 1;
            Trio.PercentageSpeedOverride2 = 1;
            Trio.move("X", .5);
            Thread.Sleep(10);
            Trio.waitForEndOfMove(true);
            Console.WriteLine("Move Done, Found x-.");

            //now find Y-

            //Trio.turnOnWatchDogAndServos(true);
            while (Trio.digitalRead(4) == 0)
            {
                if (Trio.digitalRead(2) != 0 || Trio.digitalRead(3) != 1)
                {
                    Trio.turnOffJog(true);
                    Trio.turnOffWatchDogAndServos(true);
                    MessageBox.Show("Was trying to find the lower Y limit and another limit was tripped.");
                    return;
                }
                Trio.setJogSpeed("Y", .4);
                Trio.JogBackward("Y");
            }
            //you are on the Y-. Move up .5 inch.
            Trio.axTrioPC.RapidStop();
            Trio.turnOffJog(true);
            Thread.Sleep(500);
            Console.WriteLine("Found Y-");
            Trio.PercentageSpeedOverride = 1;
            Trio.PercentageSpeedOverride2 = 1;
            Trio.move("Y", .5);
            Thread.Sleep(50);
            Trio.waitForEndOfMove(true);
            Console.WriteLine("Move Done, Found y-.");

            //Trio.turnOffWatchDogAndServos(true);
            Trio.setAbsoluteZeroHere("X", "Y");
            Trio.setLimits();
            _timer.Start();
            manualModeController = true;
            Trio.axTrioPC.Run("ENDSTOP");
        }
        //private void abrasiveLabel_DoubleClick(object sender, EventArgs e)
        //{
        //    Trio.AbrasiveOn = !Trio.AbrasiveOn;
        //}

        private void button25_Click(object sender, EventArgs e)
        {
            double x = Convert.ToDouble(xOffsetNumBox.Text);
            double y = Convert.ToDouble(yOffSetNumBox.Text);
            if (x < Trio.xLimitLower || x > Trio.xLimitUpper || y < Trio.yLimitLower || y > Trio.yLimitUpper)
            {
                MessageBox.Show("Invalid offset.");
                return;
            }
            Trio.Offset.X = Convert.ToDouble(xOffsetNumBox.Text);
            Trio.Offset.Y = Convert.ToDouble(yOffSetNumBox.Text);
            RefreshValues();
        }

        private void clearErrorsAndEnableButton_Click(object sender, EventArgs e)
        {
            Trio.axTrioPC.Datum(0);
            Thread.Sleep(50);
            Trio.turnOnWatchDogAndServos(true);
        }
        private void updateSpeedLabels()
        {
            speedOverrideLabel.Text = "Speed Override: " + (Math.Round(Trio.PercentageSpeedOverride, 2) * 100).ToString() + "%, " + Math.Round(Trio.PercentageSpeedOverride * Trio.feedSpeed, 3).ToString();
            speed2OverrideLabel.Text = "Speed Override: " + (Math.Round(Trio.PercentageSpeedOverride2, 2) * 100).ToString() + "%, " + Math.Round(Trio.PercentageSpeedOverride2 * Trio.rapidSpeed, 3).ToString();
        }

        private void abrasiveCB_CheckedChanged(object sender, EventArgs e)
        {
            Trio.abrasiveEnabled = abrasiveCB.Checked;
        }

        private void feedSpeed25_Click(object sender, EventArgs e)
        {
            Trio.PercentageSpeedOverride = .25;

            repaintSpeedBoxes();
            updateSpeedLabels();
        }

        private void feedSpeed50_Click(object sender, EventArgs e)
        {
            Trio.PercentageSpeedOverride = .5;

            repaintSpeedBoxes();
            updateSpeedLabels();
        }

        private void feedSpeed75_Click(object sender, EventArgs e)
        {
            Trio.PercentageSpeedOverride = .75;

            repaintSpeedBoxes();
            updateSpeedLabels();
        }

        private void feedSpeed100_Click(object sender, EventArgs e)
        {
            Trio.PercentageSpeedOverride = 1;

            repaintSpeedBoxes();
            updateSpeedLabels();
        }

        private void rapidSpeed25_Click(object sender, EventArgs e)
        {
            Trio.PercentageSpeedOverride2 = .25;

            repaintSpeedBoxes();
            updateSpeedLabels();
        }

        private void rapidSpeed50_Click(object sender, EventArgs e)
        {
            Trio.PercentageSpeedOverride2 = .5;

            repaintSpeedBoxes();
            updateSpeedLabels();
        }

        private void rapidSpeed75_Click(object sender, EventArgs e)
        {
            Trio.PercentageSpeedOverride2 = .75;

            repaintSpeedBoxes();
            updateSpeedLabels();
        }

        private void rapidSpeed100_Click(object sender, EventArgs e)
        {
            Trio.PercentageSpeedOverride2 = 1;

            repaintSpeedBoxes();
            updateSpeedLabels();
        }

        private void button26_Click(object sender, EventArgs e)
        {
            new EditStartPositionForm(Trio).ShowDialog();
        }

        private void disableDROButton_Click(object sender, EventArgs e)
        {
            if (_timer.Enabled)
                _timer.Stop();
            else
                _timer.Start();
        }

        private void resetMCButton_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("Are you sure you want to reset the controller? Please wait 1 minute after resetting.","!", MessageBoxButtons.YesNo);
            if (d == DialogResult.Yes)
            {
                Console.WriteLine("Resetting controller.");
                _timer.Stop();
                Trio.resetController();
                Close();
            }
            else
            {
                Console.WriteLine("Reset cancelled.");
            }
        }
    }
}
