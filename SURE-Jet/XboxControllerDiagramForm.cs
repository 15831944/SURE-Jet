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
    public partial class XboxControllerDiagramForm : Form
    {
        public XboxControllerDiagramForm()
        {
            InitializeComponent();
            //this.Height = this.Height * pictureBox1.Width / pictureBox1.Height;
            Size = new Size(799, 753);
            this.CenterToScreen();
            //pictureBox1.Size = this.Size;
        }

        private void XboxControllerDiagramForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Height" + this.Height.ToString() + " Width: " + this.Width.ToString());
        }
    }
}
