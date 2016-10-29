using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SURE_Jet
{
    public partial class CutterDiameterForm : Form
    {
        TrioWrapper trio;
        public CutterDiameterForm(TrioWrapper thetrio)
        {
            trio = thetrio;
            InitializeComponent();
            cutterDiameterTB.Text = trio.cutterDiameter.ToString();
            leadInLengthTB.Text = trio.leadInLength.ToString();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            trio.cutterDiameter = Convert.ToDouble(cutterDiameterTB.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            trio.leadInLength = Convert.ToDouble(leadInLengthTB.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            trio.PierceDelay = Convert.ToDouble(pierceDelayNumBox.Text);
        }

    }
}
