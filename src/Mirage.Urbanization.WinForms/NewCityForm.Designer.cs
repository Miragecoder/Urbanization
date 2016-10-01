namespace Mirage.Urbanization.WinForms
{
    partial class NewCityForm
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
            this.okNewCityButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.sizeTrackBar = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.treesTrackBar = new System.Windows.Forms.TrackBar();
            this.checkBoxHorizontalRiver = new System.Windows.Forms.CheckBox();
            this.checkBoxVerticalRiver = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lakesTrackBar = new System.Windows.Forms.TrackBar();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbEastCoast = new System.Windows.Forms.CheckBox();
            this.cbWestCoast = new System.Windows.Forms.CheckBox();
            this.cbNorthCoast = new System.Windows.Forms.CheckBox();
            this.cbSouthCoast = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.sizeTrackBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treesTrackBar)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lakesTrackBar)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // okNewCityButton
            // 
            this.okNewCityButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okNewCityButton.Location = new System.Drawing.Point(314, 518);
            this.okNewCityButton.Name = "okNewCityButton";
            this.okNewCityButton.Size = new System.Drawing.Size(75, 23);
            this.okNewCityButton.TabIndex = 0;
            this.okNewCityButton.Text = "New City";
            this.okNewCityButton.UseVisualStyleBackColor = true;
            this.okNewCityButton.Click += new System.EventHandler(this.okNewCityButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(395, 518);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // sizeTrackBar
            // 
            this.sizeTrackBar.Location = new System.Drawing.Point(6, 30);
            this.sizeTrackBar.Name = "sizeTrackBar";
            this.sizeTrackBar.Size = new System.Drawing.Size(446, 56);
            this.sizeTrackBar.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.sizeTrackBar);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(458, 90);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Size";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.treesTrackBar);
            this.groupBox2.Location = new System.Drawing.Point(12, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(458, 90);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Trees";
            // 
            // treesTrackBar
            // 
            this.treesTrackBar.Location = new System.Drawing.Point(6, 30);
            this.treesTrackBar.Name = "treesTrackBar";
            this.treesTrackBar.Size = new System.Drawing.Size(446, 56);
            this.treesTrackBar.TabIndex = 2;
            // 
            // checkBoxHorizontalRiver
            // 
            this.checkBoxHorizontalRiver.AutoSize = true;
            this.checkBoxHorizontalRiver.Checked = true;
            this.checkBoxHorizontalRiver.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHorizontalRiver.Location = new System.Drawing.Point(15, 30);
            this.checkBoxHorizontalRiver.Name = "checkBoxHorizontalRiver";
            this.checkBoxHorizontalRiver.Size = new System.Drawing.Size(94, 21);
            this.checkBoxHorizontalRiver.TabIndex = 5;
            this.checkBoxHorizontalRiver.Text = "Horizontal";
            this.checkBoxHorizontalRiver.UseVisualStyleBackColor = true;
            // 
            // checkBoxVerticalRiver
            // 
            this.checkBoxVerticalRiver.AutoSize = true;
            this.checkBoxVerticalRiver.Checked = true;
            this.checkBoxVerticalRiver.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxVerticalRiver.Location = new System.Drawing.Point(15, 57);
            this.checkBoxVerticalRiver.Name = "checkBoxVerticalRiver";
            this.checkBoxVerticalRiver.Size = new System.Drawing.Size(77, 21);
            this.checkBoxVerticalRiver.TabIndex = 6;
            this.checkBoxVerticalRiver.Text = "Vertical";
            this.checkBoxVerticalRiver.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.lakesTrackBar);
            this.groupBox3.Location = new System.Drawing.Point(12, 223);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(458, 90);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Lakes";
            // 
            // lakesTrackBar
            // 
            this.lakesTrackBar.Location = new System.Drawing.Point(6, 30);
            this.lakesTrackBar.Name = "lakesTrackBar";
            this.lakesTrackBar.Size = new System.Drawing.Size(446, 56);
            this.lakesTrackBar.TabIndex = 2;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBoxHorizontalRiver);
            this.groupBox4.Controls.Add(this.checkBoxVerticalRiver);
            this.groupBox4.Location = new System.Drawing.Point(12, 328);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(220, 111);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Rivers";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.cbNorthCoast);
            this.groupBox5.Controls.Add(this.cbSouthCoast);
            this.groupBox5.Controls.Add(this.cbEastCoast);
            this.groupBox5.Controls.Add(this.cbWestCoast);
            this.groupBox5.Location = new System.Drawing.Point(238, 328);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(232, 111);
            this.groupBox5.TabIndex = 9;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Coast";
            // 
            // cbEastCoast
            // 
            this.cbEastCoast.AutoSize = true;
            this.cbEastCoast.Location = new System.Drawing.Point(15, 30);
            this.cbEastCoast.Name = "cbEastCoast";
            this.cbEastCoast.Size = new System.Drawing.Size(58, 21);
            this.cbEastCoast.TabIndex = 5;
            this.cbEastCoast.Text = "East";
            this.cbEastCoast.UseVisualStyleBackColor = true;
            // 
            // cbWestCoast
            // 
            this.cbWestCoast.AutoSize = true;
            this.cbWestCoast.Checked = true;
            this.cbWestCoast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbWestCoast.Location = new System.Drawing.Point(15, 57);
            this.cbWestCoast.Name = "cbWestCoast";
            this.cbWestCoast.Size = new System.Drawing.Size(62, 21);
            this.cbWestCoast.TabIndex = 6;
            this.cbWestCoast.Text = "West";
            this.cbWestCoast.UseVisualStyleBackColor = true;
            // 
            // cbNorthCoast
            // 
            this.cbNorthCoast.AutoSize = true;
            this.cbNorthCoast.Location = new System.Drawing.Point(128, 30);
            this.cbNorthCoast.Name = "cbNorthCoast";
            this.cbNorthCoast.Size = new System.Drawing.Size(65, 21);
            this.cbNorthCoast.TabIndex = 7;
            this.cbNorthCoast.Text = "North";
            this.cbNorthCoast.UseVisualStyleBackColor = true;
            // 
            // cbSouthCoast
            // 
            this.cbSouthCoast.AutoSize = true;
            this.cbSouthCoast.Checked = true;
            this.cbSouthCoast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSouthCoast.Location = new System.Drawing.Point(128, 57);
            this.cbSouthCoast.Name = "cbSouthCoast";
            this.cbSouthCoast.Size = new System.Drawing.Size(67, 21);
            this.cbSouthCoast.TabIndex = 8;
            this.cbSouthCoast.Text = "South";
            this.cbSouthCoast.UseVisualStyleBackColor = true;
            // 
            // NewCityForm
            // 
            this.AcceptButton = this.okNewCityButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(482, 553);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okNewCityButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewCityForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New City";
            ((System.ComponentModel.ISupportInitialize)(this.sizeTrackBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treesTrackBar)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lakesTrackBar)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okNewCityButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TrackBar sizeTrackBar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar treesTrackBar;
        private System.Windows.Forms.CheckBox checkBoxHorizontalRiver;
        private System.Windows.Forms.CheckBox checkBoxVerticalRiver;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TrackBar lakesTrackBar;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox cbEastCoast;
        private System.Windows.Forms.CheckBox cbWestCoast;
        private System.Windows.Forms.CheckBox cbNorthCoast;
        private System.Windows.Forms.CheckBox cbSouthCoast;
    }
}