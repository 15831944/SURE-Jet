namespace SURE_Jet
{
    partial class CutterDiameterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cutterDiameterTB = new Simple_Grapher.NumericTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.leadInLengthTB = new Simple_Grapher.NumericTextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.pierceDelayNumBox = new Simple_Grapher.NumericTextBox();
            this.SuspendLayout();
            // 
            // cutterDiameterTB
            // 
            this.cutterDiameterTB.AllowSpace = false;
            this.cutterDiameterTB.Location = new System.Drawing.Point(314, 212);
            this.cutterDiameterTB.Name = "cutterDiameterTB";
            this.cutterDiameterTB.Size = new System.Drawing.Size(100, 31);
            this.cutterDiameterTB.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(90, 212);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "Cutter Diameter (in)";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(451, 193);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(236, 69);
            this.button1.TabIndex = 2;
            this.button1.TabStop = false;
            this.button1.Text = "Set Cutter Diameter";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(451, 303);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(236, 69);
            this.button2.TabIndex = 5;
            this.button2.TabStop = false;
            this.button2.Text = "Set Lead-In Length";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(90, 322);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(194, 26);
            this.label2.TabIndex = 4;
            this.label2.Text = "Lead-In Length (in)";
            // 
            // leadInLengthTB
            // 
            this.leadInLengthTB.AllowSpace = false;
            this.leadInLengthTB.Location = new System.Drawing.Point(314, 322);
            this.leadInLengthTB.Name = "leadInLengthTB";
            this.leadInLengthTB.Size = new System.Drawing.Size(100, 31);
            this.leadInLengthTB.TabIndex = 3;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(451, 401);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(236, 69);
            this.button3.TabIndex = 8;
            this.button3.TabStop = false;
            this.button3.Text = "Set Pierce Delay";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(90, 420);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(173, 26);
            this.label3.TabIndex = 7;
            this.label3.Text = "Pierce Delay (s):";
            // 
            // pierceDelayNumBox
            // 
            this.pierceDelayNumBox.AllowSpace = false;
            this.pierceDelayNumBox.Location = new System.Drawing.Point(314, 420);
            this.pierceDelayNumBox.Name = "pierceDelayNumBox";
            this.pierceDelayNumBox.Size = new System.Drawing.Size(100, 31);
            this.pierceDelayNumBox.TabIndex = 6;
            // 
            // CutterDiameterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1105, 826);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pierceDelayNumBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.leadInLengthTB);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cutterDiameterTB);
            this.Name = "CutterDiameterForm";
            this.Text = "CutterDiameterForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Simple_Grapher.NumericTextBox cutterDiameterTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private Simple_Grapher.NumericTextBox leadInLengthTB;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label3;
        private Simple_Grapher.NumericTextBox pierceDelayNumBox;
    }
}