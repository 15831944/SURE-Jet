namespace SURE_Jet
{
    partial class OffsetForm
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
            this.upButton = new System.Windows.Forms.Button();
            this.leftButton = new System.Windows.Forms.Button();
            this.downButton = new System.Windows.Forms.Button();
            this.rightButton = new System.Windows.Forms.Button();
            this.offsetYLabel = new System.Windows.Forms.Label();
            this.offsetXLabel = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.acceptOffsetButton = new System.Windows.Forms.Button();
            this.Xlabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.offsetXTB = new Simple_Grapher.NumericTextBox();
            this.offSetYTB = new Simple_Grapher.NumericTextBox();
            this.goToOffsetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // upButton
            // 
            this.upButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 36.18848F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.upButton.Location = new System.Drawing.Point(442, 190);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(199, 191);
            this.upButton.TabIndex = 0;
            this.upButton.Text = "↑";
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            this.upButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.upButton_MouseDown);
            this.upButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.upButton_MouseUp);
            // 
            // leftButton
            // 
            this.leftButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 36.18848F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.leftButton.Location = new System.Drawing.Point(223, 390);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(199, 191);
            this.leftButton.TabIndex = 1;
            this.leftButton.Text = "←";
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.leftButton_MouseDown);
            this.leftButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.leftButton_MouseUp);
            // 
            // downButton
            // 
            this.downButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 36.18848F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downButton.Location = new System.Drawing.Point(442, 602);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(199, 191);
            this.downButton.TabIndex = 2;
            this.downButton.Text = "↓";
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.downButton_MouseDown);
            this.downButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.downButton_MouseUp);
            // 
            // rightButton
            // 
            this.rightButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 36.18848F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rightButton.Location = new System.Drawing.Point(672, 390);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(199, 191);
            this.rightButton.TabIndex = 3;
            this.rightButton.Text = "→";
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rightButton_MouseDown);
            this.rightButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rightButton_MouseUp);
            // 
            // offsetYLabel
            // 
            this.offsetYLabel.AutoSize = true;
            this.offsetYLabel.Location = new System.Drawing.Point(424, 63);
            this.offsetYLabel.Name = "offsetYLabel";
            this.offsetYLabel.Size = new System.Drawing.Size(81, 26);
            this.offsetYLabel.TabIndex = 68;
            this.offsetYLabel.Text = "offsetY";
            // 
            // offsetXLabel
            // 
            this.offsetXLabel.AutoSize = true;
            this.offsetXLabel.Location = new System.Drawing.Point(254, 63);
            this.offsetXLabel.Name = "offsetXLabel";
            this.offsetXLabel.Size = new System.Drawing.Size(80, 26);
            this.offsetXLabel.TabIndex = 67;
            this.offsetXLabel.Text = "offsetX";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.06283F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(43, 54);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(111, 38);
            this.label31.TabIndex = 66;
            this.label31.Text = "Offset";
            // 
            // acceptOffsetButton
            // 
            this.acceptOffsetButton.Location = new System.Drawing.Point(875, 14);
            this.acceptOffsetButton.Name = "acceptOffsetButton";
            this.acceptOffsetButton.Size = new System.Drawing.Size(292, 123);
            this.acceptOffsetButton.TabIndex = 69;
            this.acceptOffsetButton.Text = "Accept Offset";
            this.acceptOffsetButton.UseVisualStyleBackColor = true;
            this.acceptOffsetButton.Click += new System.EventHandler(this.acceptOffsetButton_Click);
            // 
            // Xlabel
            // 
            this.Xlabel.AutoSize = true;
            this.Xlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.06283F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Xlabel.Location = new System.Drawing.Point(197, 54);
            this.Xlabel.Name = "Xlabel";
            this.Xlabel.Size = new System.Drawing.Size(40, 38);
            this.Xlabel.TabIndex = 70;
            this.Xlabel.Text = "X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.06283F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(362, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 38);
            this.label2.TabIndex = 71;
            this.label2.Text = "Y";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.06283F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(362, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 38);
            this.label1.TabIndex = 75;
            this.label1.Text = "Y";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.06283F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(197, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 38);
            this.label3.TabIndex = 74;
            this.label3.Text = "X";
            // 
            // offsetXTB
            // 
            this.offsetXTB.AllowSpace = false;
            this.offsetXTB.Location = new System.Drawing.Point(243, 127);
            this.offsetXTB.Name = "offsetXTB";
            this.offsetXTB.Size = new System.Drawing.Size(100, 31);
            this.offsetXTB.TabIndex = 76;
            // 
            // offSetYTB
            // 
            this.offSetYTB.AllowSpace = false;
            this.offSetYTB.Location = new System.Drawing.Point(408, 127);
            this.offSetYTB.Name = "offSetYTB";
            this.offSetYTB.Size = new System.Drawing.Size(100, 31);
            this.offSetYTB.TabIndex = 77;
            // 
            // goToOffsetButton
            // 
            this.goToOffsetButton.Location = new System.Drawing.Point(535, 111);
            this.goToOffsetButton.Name = "goToOffsetButton";
            this.goToOffsetButton.Size = new System.Drawing.Size(177, 63);
            this.goToOffsetButton.TabIndex = 78;
            this.goToOffsetButton.Text = "Go To Offset";
            this.goToOffsetButton.UseVisualStyleBackColor = true;
            this.goToOffsetButton.Click += new System.EventHandler(this.goToOffsetButton_Click);
            // 
            // OffsetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1198, 997);
            this.Controls.Add(this.goToOffsetButton);
            this.Controls.Add(this.offSetYTB);
            this.Controls.Add(this.offsetXTB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Xlabel);
            this.Controls.Add(this.acceptOffsetButton);
            this.Controls.Add(this.offsetYLabel);
            this.Controls.Add(this.offsetXLabel);
            this.Controls.Add(this.label31);
            this.Controls.Add(this.rightButton);
            this.Controls.Add(this.downButton);
            this.Controls.Add(this.leftButton);
            this.Controls.Add(this.upButton);
            this.Name = "OffsetForm";
            this.Text = "OffsetForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Button leftButton;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.Button rightButton;
        private System.Windows.Forms.Label offsetYLabel;
        private System.Windows.Forms.Label offsetXLabel;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Button acceptOffsetButton;
        private System.Windows.Forms.Label Xlabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private Simple_Grapher.NumericTextBox offsetXTB;
        private Simple_Grapher.NumericTextBox offSetYTB;
        private System.Windows.Forms.Button goToOffsetButton;
    }
}