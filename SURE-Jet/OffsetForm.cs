using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxTrioPCLib;
//using System.Threading;

namespace SURE_Jet
{
    public partial class OffsetForm : Form
    {
        Offset Offset;
        TrioWrapper Trio;
        private readonly Timer _timer = new Timer();
        bool offSetSaved = false;
        public OffsetForm(Offset off, TrioWrapper ax)
        {
            InitializeComponent();
            Offset = off;
            Trio = ax;
            refreshValues();
            _timer.Interval = 500;
            _timer.Tick += TimerTick;
            _timer.Enabled = true;

            Trio.setJogSpeed("X", Trio.rapidSpeed);
            Trio.setJogSpeed("Y", Trio.rapidSpeed);
            
        }
        void TimerTick(object sender, EventArgs e)
        {
            refreshValues();
        }

        private void refreshValues()
        {
            offsetXLabel.Text = Math.Round(Trio.getVariable("X","MPOS"),3).ToString();
            offsetYLabel.Text = Math.Round(Trio.getVariable("Y","MPOS"),3).ToString();
        }


        private void enableButtons(bool p)
        {
            rightButton.Enabled = p;
            leftButton.Enabled = p;
            upButton.Enabled = p;
            downButton.Enabled = p;
            acceptOffsetButton.Enabled = p;

            rightButton.Refresh();
            leftButton.Refresh();
            upButton.Refresh();
            downButton.Refresh();
            acceptOffsetButton.Refresh();

        }

        private void OffsetXLabel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            refreshValues();
        }


        private void rightButton_MouseDown(object sender, MouseEventArgs e)
        {
            offSetSaved = false;
            //Trio.turnOnWatchDogAndServos(true);
            Trio.JogForward("X");
        }

        private void rightButton_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp();   
        }

        private void mouseUp()
        {
            Trio.turnOffJog(true);

            enableButtons(false);
            while (Trio.speedIsAbove(true,0))
            {
                
            }
            enableButtons(true);

            //Trio.turnOffWatchDogAndServos(true);
        }

        private void downButton_MouseDown(object sender, MouseEventArgs e)
        {
            offSetSaved = false;
            //Trio.turnOnWatchDogAndServos(true);
            Trio.JogBackward("Y");
        }

        private void downButton_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp();
        }

        private void leftButton_MouseDown(object sender, MouseEventArgs e)
        {
            offSetSaved = false;
            //Trio.turnOnWatchDogAndServos(true);
            Trio.JogBackward("X");
        }

        private void leftButton_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp();
        }

        private void upButton_MouseDown(object sender, MouseEventArgs e)
        {
            offSetSaved = false;
            //Trio.turnOnWatchDogAndServos(true);
            Trio.JogForward("Y");
        }

        private void upButton_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUp();
        }


        private void acceptOffsetButton_Click(object sender, EventArgs e)
        {
            offSetSaved = true;
            Offset.X = Trio.getVariable("X", "MPOS");
            Offset.Y = Trio.getVariable("Y", "MPOS");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!offSetSaved)
            {
                if (MessageBox.Show("Are you sure you want to close? The offset was not saved and the jet will return to the previous offset. \n", "Warning",
             MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                }

            }
            _timer.Dispose();
        }

        private void upButton_Click(object sender, EventArgs e)
        {

        }

        private void goToOffsetButton_Click(object sender, EventArgs e)
        {
            if (!checkOffsetInput())
                return;
            double x = Convert.ToDouble(offsetXTB.Text);
            double y = Convert.ToDouble(offSetYTB.Text);
            enableButtons(false);
            //Trio.turnOnWatchDogAndServos(true);
            Trio.PercentageSpeedOverride = 2;
            Trio.moveAbs("X", "Y", x, y);//, 0, 0);
            
            Trio.waitForEndOfMove(true);
            //Trio.turnOffWatchDogAndServos(true);
            enableButtons(true);
           
        }

        private bool checkOffsetInput()
        {
            try
            {
                double x = Convert.ToDouble(offsetXTB.Text);
                double y = Convert.ToDouble(offSetYTB.Text);
                if (x < 0 || x > Trio.xLimitUpper)
                    throw new Exception();
                if (y < 0 || y > Trio.yLimitUpper)
                    throw new Exception();
            }
            catch
            {
                MessageBox.Show("Invalid Offset.");
                return false;
            }
            return true;
        }
        
    }
}
