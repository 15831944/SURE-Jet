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
    public partial class ProjectorDisplayForm : Form
    {
        private int x = 0;
        private int y = 0;
        System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
        System.Drawing.Graphics formGraphics;
        
            
        public ProjectorDisplayForm()
        {
            InitializeComponent();
            formGraphics = projectorDisplayPB.CreateGraphics();
            x = projectorDisplayPB.Width;
            y = projectorDisplayPB.Height;
            rePaint();
            this.Height = 591;
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        //private void projectorDisplayPB_Click(object sender, MouseEventArgs e)
        //{
        //    int tempx = 5 - projectorDisplayPB.Location.X;

        //    int tempy = 5 - projectorDisplayPB.Location.Y;
        //    MessageBox.Show(tempx.ToString());
        //    if (xCB.Checked)
        //    {

        //        x = tempx;
        //        rePaint();
        //    }
        //    else if (yCB.Checked)
        //    {
        //        y = tempy;
        //        rePaint();
        //    }
        //}
        private void rePaint()
        {
            //System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            //System.Drawing.Graphics formGraphics;
            //formGraphics = projectorDisplayPB.CreateGraphics();
            formGraphics.Clear(Color.Black);
            formGraphics.FillRectangle(myBrush, new Rectangle(x, 0, projectorDisplayPB.Width - x, y));

        }

        private void projectorDisplayPB_MouseClick(object sender, MouseEventArgs e)
        {
            int tempx = e.X;

            int tempy = e.Y;
            if (xCB.Checked)
            {
                x = tempx;
                rePaint();
            }
            else if (yCB.Checked)
            {
                y = tempy;
                rePaint();
            }
        }

        private void yCB_CheckedChanged(object sender, EventArgs e)
        {
            xCB.Checked = !yCB.Checked;
        }

        private void xCB_CheckedChanged(object sender, EventArgs e)
        {
            yCB.Checked = !xCB.Checked;
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            y--;
            rePaint();
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            y++; ;
            rePaint();
        }

        private void leftButton_Click(object sender, EventArgs e)
        {
            x--;
            rePaint();
        }

        private void rightButton_Click(object sender, EventArgs e)
        {
            x++;
            rePaint();
        }

    }
}
