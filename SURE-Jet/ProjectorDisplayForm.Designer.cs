namespace SURE_Jet
{
    partial class ProjectorDisplayForm
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
            this.projectorDisplayPB = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.xCB = new System.Windows.Forms.CheckBox();
            this.yCB = new System.Windows.Forms.CheckBox();
            this.rightButton = new System.Windows.Forms.Button();
            this.downButton = new System.Windows.Forms.Button();
            this.leftButton = new System.Windows.Forms.Button();
            this.upButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.projectorDisplayPB)).BeginInit();
            this.SuspendLayout();
            // 
            // projectorDisplayPB
            // 
            this.projectorDisplayPB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.projectorDisplayPB.Location = new System.Drawing.Point(245, 173);
            this.projectorDisplayPB.Name = "projectorDisplayPB";
            this.projectorDisplayPB.Size = new System.Drawing.Size(907, 647);
            this.projectorDisplayPB.TabIndex = 0;
            this.projectorDisplayPB.TabStop = false;
            this.projectorDisplayPB.MouseClick += new System.Windows.Forms.MouseEventHandler(this.projectorDisplayPB_MouseClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(629, 53);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 65);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // xCB
            // 
            this.xCB.AutoSize = true;
            this.xCB.Location = new System.Drawing.Point(670, 908);
            this.xCB.Name = "xCB";
            this.xCB.Size = new System.Drawing.Size(53, 30);
            this.xCB.TabIndex = 2;
            this.xCB.Text = "X";
            this.xCB.UseVisualStyleBackColor = true;
            this.xCB.CheckedChanged += new System.EventHandler(this.xCB_CheckedChanged);
            // 
            // yCB
            // 
            this.yCB.AutoSize = true;
            this.yCB.Location = new System.Drawing.Point(150, 416);
            this.yCB.Name = "yCB";
            this.yCB.Size = new System.Drawing.Size(54, 30);
            this.yCB.TabIndex = 3;
            this.yCB.Text = "Y";
            this.yCB.UseVisualStyleBackColor = true;
            this.yCB.CheckedChanged += new System.EventHandler(this.yCB_CheckedChanged);
            // 
            // rightButton
            // 
            this.rightButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 36.18848F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rightButton.Location = new System.Drawing.Point(845, 835);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(199, 191);
            this.rightButton.TabIndex = 7;
            this.rightButton.Text = "→";
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.Click += new System.EventHandler(this.rightButton_Click);
            // 
            // downButton
            // 
            this.downButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 36.18848F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downButton.Location = new System.Drawing.Point(12, 495);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(199, 191);
            this.downButton.TabIndex = 6;
            this.downButton.Text = "↓";
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // leftButton
            // 
            this.leftButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 36.18848F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.leftButton.Location = new System.Drawing.Point(338, 835);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(199, 191);
            this.leftButton.TabIndex = 5;
            this.leftButton.Text = "←";
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.Click += new System.EventHandler(this.leftButton_Click);
            // 
            // upButton
            // 
            this.upButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 36.18848F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.upButton.Location = new System.Drawing.Point(5, 193);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(199, 191);
            this.upButton.TabIndex = 4;
            this.upButton.Text = "↑";
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // ProjectorDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1439, 1336);
            this.Controls.Add(this.rightButton);
            this.Controls.Add(this.downButton);
            this.Controls.Add(this.leftButton);
            this.Controls.Add(this.upButton);
            this.Controls.Add(this.yCB);
            this.Controls.Add(this.xCB);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.projectorDisplayPB);
            this.Name = "ProjectorDisplayForm";
            this.Text = "ProjectorDisplayForm";
            ((System.ComponentModel.ISupportInitialize)(this.projectorDisplayPB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox projectorDisplayPB;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox xCB;
        private System.Windows.Forms.CheckBox yCB;
        private System.Windows.Forms.Button rightButton;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.Button leftButton;
        private System.Windows.Forms.Button upButton;
    }
}