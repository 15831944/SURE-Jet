using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxTrioPCLib;
using System.Windows.Forms;
using System.Threading;

namespace SURE_Jet
{
    public class TrioWrapper
    {
        public static Mutex mutex = new Mutex();
        private int mutWait = 1000;

        public AxTrioPC axTrioPC;
        public Offset Offset;
        public double maxSpeed = 10;
        public double XEncoderZero = 42097715;
        public double YEncoderZero = 25792370;
        public double YtoXEncoderRatio = 332917.8 / 131070.0;
        public double xLimitUpper = 30.936;
        public double yLimitUpper = 23.84;
        public double xLimitLower = -.1;
        public double yLimitLower = -.1;
        public double cutterDiameter = .04;
        public double rapidSpeed = 1;
        public double feedSpeed = .3;
        private double percentageSpeedOverride = 1;
        private double precision = .01;
        //private double Units = 327549;
        public double Units = 332917.8;
        private double Accel = 500;
        private double Decel = 500;
        public double VirtualXLocation = 0;
        public double VirtualYLocation = 0;
        //public double jogSpeed = .5;
        public double PierceDelay = .5;
        private bool waterOn = false;
        public bool abrasiveEnabled = true;
        public bool WaterOn
        {
            get
            {
                return waterOn;
            }
            set
            {
                if (value == true)
                {
                    digitalWrite(8, 1);
                    waterOn = true;
                    Thread.Sleep(100);
                    AbrasiveOn = true;
                    //pause for the pierce.
                    Console.WriteLine("Before Pierce Delay.");
                    Thread.Sleep((int)(PierceDelay * Math.Pow(10, 3)));
                    Console.WriteLine("After Pierce Delay.");
                }
                else
                {
                    AbrasiveOn = false;
                    Thread.Sleep(100);
                    digitalWrite(8,0);
                    waterOn = false;
                }
                
                //setVariable("X", "WATER", Convert.ToInt16(value));
                //waterOn = Convert.ToBoolean(getVariable("X", "WATER"));
            }
        }
        private bool abrasiveOn = false;
        public bool AbrasiveOn
        {
            get
            {
                return abrasiveOn;
            }
            set
            {
                if (value == true)
                {
                    if (abrasiveEnabled)
                    {
                        digitalWrite(9, 1);
                        abrasiveOn = true;
                    }
                }
                else
                {
                    digitalWrite(9, 0);
                    abrasiveOn = false;
                }

                //setVariable("X", "WATER", Convert.ToInt16(value));
                //waterOn = Convert.ToBoolean(getVariable("X", "WATER"));
            }
        }
        public double PercentageSpeedOverride
        {
            get
            {
                return percentageSpeedOverride;
            }
            set
            {
                if(value >= 0 && value <= 2)
                percentageSpeedOverride = value;
                if (SpeedMode == 1)
                {
                    setSpeed(true, feedSpeed * percentageSpeedOverride);
                }
            }
        }
        private double percentageSpeedOverride2 = 1;
        public double PercentageSpeedOverride2
        {
            get
            {
                return percentageSpeedOverride2;
            }
            set
            {
                if(value >= 0 && value <= 2)
                    percentageSpeedOverride2 = value;
                if (SpeedMode == 2)
                {
                    setSpeed(true, rapidSpeed * percentageSpeedOverride2);
                }
            }
        }
        public double leadInLength = .1;
        public double previousSpeedBeforeZero;
        public int speedMode = 1;
        public int SpeedMode
        {
            get
            {
                return speedMode;
            }
            set
            {
                if (speedMode == value)
                    return;
                speedMode = value;
                if (speedMode == 1)
                    setSpeed(true,feedSpeed * percentageSpeedOverride);
                else
                    setSpeed(true,rapidSpeed * percentageSpeedOverride2);
            }
        }

        public TrioWrapper(AxTrioPC pc, Offset of)
        {
            axTrioPC = pc;
            Offset = of;
        }
        public void Base(String axis)
        {
            //can't put mutex here because it would be embedded in other mutexed commands
            if (!axTrioPC.Base(1, axisNumberFromString(axis)))
            {
                Thread.Sleep(5);
                if (!axTrioPC.Base(1, axisNumberFromString(axis)))//try it again
                {
                    if (SureJetSettings.showFailedCommands)
                        MessageBox.Show("Failed to change base");
                }
            }
        }
        public double getVariable(String axis, String variable)
        {
            if (mutex.WaitOne(mutWait))
            {
                Base(axis);
                double a = -1;
                if (!axTrioPC.GetVariable(variable, out a))
                {
                    if (SureJetSettings.showFailedCommands)
                        MessageBox.Show("Failed to get variable: " + variable + " From axis " + axis);
                }
                mutex.ReleaseMutex();
                return a;
            }
            else
            {
                return -1;
            }
        }
        public void setVariable(String axis, String variable, double value)
        {
            if (mutex.WaitOne(mutWait))
            {
                Base(axis);
                if (!axTrioPC.SetVariable(variable, value))
                {
                    if (SureJetSettings.showFailedCommands)
                        MessageBox.Show("Failed to set variable: " + variable + " On axis: " + axis + " to value " + value);
                }
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("Failed to set variable. Communicate line full.");
            }
        }
        private bool isMoving(bool physicalAxis)
        {
            String xAxis, yAxis;
            setAxisNames(physicalAxis, out xAxis, out yAxis);
            double remainX = getVariable(xAxis, "REMAIN");
            double endmoveX = getVariable(xAxis, "ENDMOVE");
            double atPosX = getVariable(xAxis, "MPOS");

            double remainY = getVariable(yAxis, "REMAIN");
            double endmoveY = getVariable(yAxis, "ENDMOVE");
            double atPosY = getVariable(yAxis, "MPOS");

            double diffX = Math.Abs(endmoveX - atPosX);
            double diffy = Math.Abs(endmoveY - atPosY);

            return Math.Abs(remainX) > 0 || Math.Abs(remainY) > 0 || diffX > precision || diffy > precision;
        }
        public bool inMotion(String xAxis, String yAxis)
        {
            double xspeed = getVariable(xAxis, "MPSPEED");
            double yspeed = getVariable(yAxis, "MPSPEED");

            return Math.Abs(xspeed) > .5 || Math.Abs(yspeed) > .5;
        }
        public void setSpeed(bool physicalAxis, double speed)
        {
            String xAxis, yAxis;
            setAxisNames(physicalAxis, out xAxis, out yAxis);
            setSpeed(xAxis, speed);
            setSpeed(yAxis, speed);
        }
        public bool speedIsAbove(bool physicalAxis, double a)
        {
            String xAxis, yAxis;
            setAxisNames(physicalAxis, out xAxis, out yAxis);
            double speed = getVariable(xAxis, "MSPEED");
            double speed2 = getVariable(yAxis, "MSPEED");

            return Math.Abs(speed) > a || Math.Abs(speed2) > a;
        }

        public int axisNumberFromString(String a)
        {
            if (a == "Y")
            {
                return 2;
            }
            else if (a == "X")
                return 1;
            //else if (a == "VirtualX")
            //    return 3;
            //else if (a == "VirtualY")
            //    return 4;
            MessageBox.Show("Invalid axis name: " + a);
            return -1;
        }
        public void setSpeed(String axes, double speed)
        {
            setVariable(axes, "SPEED", speed);
        }
        public void setAccel(String axes, double val)
        {
            setVariable(axes, "ACCEL", val);
        }
        public void setDecel(String axes, double val)
        {
            setVariable(axes, "DECEL", val);
        }
        public void setUnits(String axes, double val)
        {
            setVariable(axes, "UNITS", val);
        }
        public void setFELimit(String axes, double val)
        {
            setVariable(axes, "FE_LIMIT", val);
        }
        public void moveAbs(String xaxis, String yaxis, double x, double y)
        {
            //if (!(checkPositionBounds(x + Offset.X, y + Offset.Y)))
              //  return false;
            if (mutex.WaitOne(mutWait))
            {
                int[] nBases = { axisNumberFromString(xaxis), axisNumberFromString(yaxis) };
                double[] dValues = new double[2];

                //oBases = nBases;

                if (!axTrioPC.Base(2, nBases))
                    MessageBox.Show("Failed to set base in moveabs");
                dValues[0] = x;// +offsetX;
                dValues[1] = y;// +offsetY;
                Thread.Sleep(10);
                if (!axTrioPC.MoveAbs(2, dValues, nBases))
                    MessageBox.Show("Move abs command failed");
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("Communication line busy. Timed out.");
            }
            //return waitForEndOfMove();
            //return true;
        }
        public void move(String Axis, double distance)
        {
            //int axis = axisNumberFromString(Axis);
            //oBases = nBases;

            Base(Axis);
            if (!axTrioPC.MoveRel(1, distance))
                MessageBox.Show("Move Relative command failed");

        }
        public bool checkPositionBounds(double x, double y)
        {
            if (x > xLimitUpper || x < xLimitLower)
            {
                MessageBox.Show("demanded X value out of bounds: " + x);
                return false;
            }
            if (y > yLimitUpper || y < yLimitLower)
            {
                MessageBox.Show("demanded Y value out of bounds: " + y);
                return false;
            }
            return true;
        } 
        public void WatchDogOn(bool physicalAxis)
        {
            if (physicalAxis)
                setVariable("X", "WDOG", 1);
            else
                setVariable("VirtualX", "WDOG", 1);
        }
        public void WatchDogOff(bool physicalAxis)
        {
            if (physicalAxis)
                setVariable("X", "WDOG", 0);
            else
                setVariable("VirtualX", "WDOG", 0);
        }
        public void servoOn(String servo)
        {
            setVariable(servo, "SERVO", 1);
        }
        public void servoOff(String servo)
        {
            setVariable(servo, "SERVO", 0);
        }
        public void turnOffWatchDogAndServos(bool physicalAxis)
        {
            
            String xAxis,yAxis;
            setAxisNames(physicalAxis,out xAxis,out yAxis);

            servoOff(xAxis);
            servoOff(yAxis);
            WatchDogOff(physicalAxis);
        }

        private void setAxisNames(bool physical, out string xAxis, out string yAxis)
        {
            if (physical)
            {
                xAxis = "X";
                yAxis = "Y";
            }
            else
            {
                MessageBox.Show("Setting name to virtual");
                xAxis = "VirtualX";
                yAxis = "VirtualY";
            }
        }

        public void turnOnWatchDogAndServos(bool physicalAxis)
        {
            WatchDogOn(physicalAxis);
            String xAxis, yAxis;
            setAxisNames(physicalAxis, out xAxis, out yAxis);
            servoOn(xAxis);
            servoOn(yAxis);
        }

        internal void MoveCircular(double finishX, double finishY, double centerX, double centerY, short dir)
        {
            if (mutex.WaitOne(mutWait))
            {
                Base("X");
                Thread.Sleep(10);
                if (!axTrioPC.MoveCirc(finishX, finishY, centerX, centerY, dir))
                    MessageBox.Show("Move circ commmand failed");
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("Communication line full. Timed outt.");
            }

        }

        public bool waitForEndOfMove(bool physicalAxis)
        {
            Thread.Sleep(20);
            while (isMoving(physicalAxis))
            {
                Thread.Sleep(5);
                if (Cancelled)
                {
                    return false;
                }
                String axis = "X";
                if (!physicalAxis)
                    axis = "VirtualX";
                double a = getVariable(axis, "WDOG");
                if (a == 0)
                    break;
            }
            return true;
        }

        internal void setLimits()
        {
            setVariable("X", "RS_LIMIT", xLimitLower);
            setVariable("X", "FS_LIMIT", xLimitUpper);
            setVariable("Y", "RS_LIMIT", yLimitLower);
            setVariable("Y", "FS_LIMIT", yLimitUpper);
        }

        internal void unsetLimits()
        {
            setVariable("X", "RS_LIMIT", -1000);
            setVariable("X", "FS_LIMIT", 1000);
            setVariable("Y", "RS_LIMIT", -1000);
            setVariable("Y", "FS_LIMIT", 1000);
        }

        public bool Cancelled { get; set; }

        internal void setJogSpeed(String axes, double p)
        {
            setVariable(axes, "JOGSPEED", p);
        }

        internal void JogForward(String axis)
        {
            setVariable(axis, "REV_JOG", -1);
            setVariable(axis, "FWD_JOG", 1);
        }
        internal void turnOffJog(bool physicalAxis)
        {
            if (physicalAxis)
            {
                setVariable("X", "REV_JOG", -1);
                setVariable("X", "FWD_JOG", -1);
                setVariable("Y", "REV_JOG", -1);
                setVariable("Y", "FWD_JOG", -1);
            }
            else
            {
                setVariable("VirtualX", "REV_JOG", -1);
                setVariable("VirtualX", "FWD_JOG", -1);
                setVariable("VirtualY", "REV_JOG", -1);
                setVariable("VirtualY", "FWD_JOG", -1);
            }
        }

        internal void JogBackward(String axis)
        {
             setVariable(axis, "REV_JOG", 1);
             setVariable(axis, "FWD_JOG", -1);
        }

        internal void setDefaultValues()
        {
            setJogSpeed("X", rapidSpeed);
            setJogSpeed("Y", rapidSpeed);
            setSpeed(true, feedSpeed);
            setUnits("X", Units);
            setAccel("X", Accel);
            setDecel("X", Decel);
            setUnits("Y", Units);
            setAccel("Y", Accel);
            setDecel("Y", Decel);
            setVariable("X", "LIMIT_BUFFERED", 1);
            setVariable("Y", "LIMIT_BUFFERED", 1);
            setVariable("X", "FE_LIMIT", .1);
            setVariable("Y", "FE_LIMIT", .1);

            //Console.WriteLine(axTrioPC.Execute("BASE(1)"));
            
            //axTrioPC.Execute("BASE(2)");
            //axTrioPC.Execute("DAC_SCALE = -1");
            //axTrioPC.Execute("ENCODER_RATIO(-1,1)");
                
            

            //setUnits("VirtualX", Units);
            //setUnits("VirtualY", Units);

            //setAccel("VirtualY", Accel);
            //setDecel("VirtualY", Decel);

            //setAccel("VirtualX", Accel);
            //setDecel("VirtualX", Decel);

            //setVariable("VirtualX", "SPEED", rapidSpeed);
            //setVariable("VirtualY", "SPEED", rapidSpeed);

            //setVariable("VirtualX", "JOGSPEED", jogSpeed);
            //setVariable("VirtualY", "JOGSPEED", jogSpeed);

            //axTrioPC.AddAxis(3, 1);
            //axTrioPC.AddAxis(4, 2);


        }

        internal void turnOnMerge()
        {
            setVariable("X", "MERGE", 1);
            setVariable("Y", "MERGE", 1);
        }
        internal void turnOffMerge()
        {
            setVariable("X", "MERGE", 0);
            setVariable("Y", "MERGE", 0);
        }

        public int digitalRead(short Input)
        {
            if (mutex.WaitOne(mutWait))
            {
                //-1 for failed reading, 1 for true, 0 for false
                int read;
                if (axTrioPC.In(Input, Input, out read))
                {
                    mutex.ReleaseMutex();
                    return read;
                }
                else
                {
                    mutex.ReleaseMutex();
                    return -1;
                }
            }
            else
            {
                
                MessageBox.Show("Failed communication.");
                return -1;
            }
        
        }

        public bool digitalWrite(short output, short val)
        {
            if (mutex.WaitOne(mutWait))
            {
                bool a = axTrioPC.Op(output, val);
                mutex.ReleaseMutex();
                return a;
            }
            else
            {
                
                MessageBox.Show("Communication line not available.");
                return false;
            }
        }



        internal void setAbsoluteZeroHere(String xAxis, String yAxis)
        {
            Base(xAxis);
            axTrioPC.SetVariable("MPOS", 0);
            Base(yAxis);
            axTrioPC.SetVariable("MPOS", 0);
            Base(xAxis);
        }

        internal void setMeasuredPosition(String xAxis, String yAxis, double xPosition, double yPosition)
        {
            Base(xAxis);
            axTrioPC.SetVariable("MPOS", xPosition);
            Base(yAxis);
            axTrioPC.SetVariable("MPOS", yPosition);
            Base(xAxis);
        }

        internal void CancelAllMoves(string p)
        {
            Base(p);
            axTrioPC.Cancel(2);
        }



        internal void resetController()
        {
            if (mutex.WaitOne(mutWait))
            {
                axTrioPC.Execute("EX");
                mutex.ReleaseMutex();
            }
        }
    }
}
