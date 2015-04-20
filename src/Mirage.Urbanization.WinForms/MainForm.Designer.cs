namespace Mirage.Urbanization.WinForms
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newCityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCityAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showPollution = new System.Windows.Forms.ToolStripMenuItem();
            this.showCrime = new System.Windows.Forms.ToolStripMenuItem();
            this.showTravelDistancesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLandValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showPopulationDensityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTrafficStats = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomFullMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomHalfMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.rendererToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGrowthPathfindingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.monthAndYearLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.saveCityDialog = new System.Windows.Forms.SaveFileDialog();
            this.openCityDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(782, 28);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newCityToolStripMenuItem,
            this.openCityToolStripMenuItem,
            this.saveCityToolStripMenuItem,
            this.saveCityAsToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newCityToolStripMenuItem
            // 
            this.newCityToolStripMenuItem.Name = "newCityToolStripMenuItem";
            this.newCityToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newCityToolStripMenuItem.Size = new System.Drawing.Size(197, 24);
            this.newCityToolStripMenuItem.Text = "New city...";
            this.newCityToolStripMenuItem.Click += new System.EventHandler(this.newCityToolStripMenuItem_Click);
            // 
            // openCityToolStripMenuItem
            // 
            this.openCityToolStripMenuItem.Name = "openCityToolStripMenuItem";
            this.openCityToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openCityToolStripMenuItem.Size = new System.Drawing.Size(197, 24);
            this.openCityToolStripMenuItem.Text = "Open city";
            this.openCityToolStripMenuItem.Click += new System.EventHandler(this.openCityToolStripMenuItem_Click);
            // 
            // saveCityToolStripMenuItem
            // 
            this.saveCityToolStripMenuItem.Enabled = false;
            this.saveCityToolStripMenuItem.Name = "saveCityToolStripMenuItem";
            this.saveCityToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveCityToolStripMenuItem.Size = new System.Drawing.Size(197, 24);
            this.saveCityToolStripMenuItem.Text = "Save city";
            this.saveCityToolStripMenuItem.Click += new System.EventHandler(this.saveCityToolStripMenuItem_Click);
            // 
            // saveCityAsToolStripMenuItem
            // 
            this.saveCityAsToolStripMenuItem.Enabled = false;
            this.saveCityAsToolStripMenuItem.Name = "saveCityAsToolStripMenuItem";
            this.saveCityAsToolStripMenuItem.Size = new System.Drawing.Size(197, 24);
            this.saveCityAsToolStripMenuItem.Text = "Save city as...";
            this.saveCityAsToolStripMenuItem.Click += new System.EventHandler(this.saveCityAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(194, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(197, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statisticsToolStripMenuItem,
            this.toolStripSeparator1,
            this.showPollution,
            this.showCrime,
            this.showTravelDistancesToolStripMenuItem,
            this.showLandValueToolStripMenuItem,
            this.showPopulationDensityToolStripMenuItem,
            this.showTrafficStats,
            this.toolStripSeparator2,
            this.zoomToolStripMenuItem,
            this.toolStripSeparator4,
            this.rendererToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // statisticsToolStripMenuItem
            // 
            this.statisticsToolStripMenuItem.Name = "statisticsToolStripMenuItem";
            this.statisticsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.statisticsToolStripMenuItem.Size = new System.Drawing.Size(243, 24);
            this.statisticsToolStripMenuItem.Text = "Statistics...";
            this.statisticsToolStripMenuItem.Click += new System.EventHandler(this.statisticsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(240, 6);
            // 
            // showPollution
            // 
            this.showPollution.CheckOnClick = true;
            this.showPollution.Name = "showPollution";
            this.showPollution.Size = new System.Drawing.Size(243, 24);
            this.showPollution.Text = "Show Pollution";
            // 
            // showCrime
            // 
            this.showCrime.CheckOnClick = true;
            this.showCrime.Name = "showCrime";
            this.showCrime.Size = new System.Drawing.Size(243, 24);
            this.showCrime.Text = "Show Crime";
            // 
            // showTravelDistancesToolStripMenuItem
            // 
            this.showTravelDistancesToolStripMenuItem.CheckOnClick = true;
            this.showTravelDistancesToolStripMenuItem.Name = "showTravelDistancesToolStripMenuItem";
            this.showTravelDistancesToolStripMenuItem.Size = new System.Drawing.Size(243, 24);
            this.showTravelDistancesToolStripMenuItem.Text = "Show Travel Distances";
            // 
            // showLandValueToolStripMenuItem
            // 
            this.showLandValueToolStripMenuItem.CheckOnClick = true;
            this.showLandValueToolStripMenuItem.Name = "showLandValueToolStripMenuItem";
            this.showLandValueToolStripMenuItem.Size = new System.Drawing.Size(243, 24);
            this.showLandValueToolStripMenuItem.Text = "Show Land Value";
            // 
            // showPopulationDensityToolStripMenuItem
            // 
            this.showPopulationDensityToolStripMenuItem.CheckOnClick = true;
            this.showPopulationDensityToolStripMenuItem.Name = "showPopulationDensityToolStripMenuItem";
            this.showPopulationDensityToolStripMenuItem.Size = new System.Drawing.Size(243, 24);
            this.showPopulationDensityToolStripMenuItem.Text = "Show Population Density";
            // 
            // showTrafficStats
            // 
            this.showTrafficStats.CheckOnClick = true;
            this.showTrafficStats.Name = "showTrafficStats";
            this.showTrafficStats.Size = new System.Drawing.Size(243, 24);
            this.showTrafficStats.Text = "Show Traffic Stats";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(240, 6);
            // 
            // zoomToolStripMenuItem
            // 
            this.zoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomFullMenuItem,
            this.zoomHalfMenuItem});
            this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            this.zoomToolStripMenuItem.Size = new System.Drawing.Size(243, 24);
            this.zoomToolStripMenuItem.Text = "Zoom";
            // 
            // zoomFullMenuItem
            // 
            this.zoomFullMenuItem.Name = "zoomFullMenuItem";
            this.zoomFullMenuItem.Size = new System.Drawing.Size(114, 24);
            this.zoomFullMenuItem.Text = "100%";
            this.zoomFullMenuItem.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // zoomHalfMenuItem
            // 
            this.zoomHalfMenuItem.Name = "zoomHalfMenuItem";
            this.zoomHalfMenuItem.Size = new System.Drawing.Size(114, 24);
            this.zoomHalfMenuItem.Text = "50%";
            this.zoomHalfMenuItem.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(240, 6);
            // 
            // rendererToolStripMenuItem
            // 
            this.rendererToolStripMenuItem.Name = "rendererToolStripMenuItem";
            this.rendererToolStripMenuItem.Size = new System.Drawing.Size(243, 24);
            this.rendererToolStripMenuItem.Text = "Renderer";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(119, 24);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showGrowthPathfindingToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(66, 24);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // showGrowthPathfindingToolStripMenuItem
            // 
            this.showGrowthPathfindingToolStripMenuItem.CheckOnClick = true;
            this.showGrowthPathfindingToolStripMenuItem.Name = "showGrowthPathfindingToolStripMenuItem";
            this.showGrowthPathfindingToolStripMenuItem.Size = new System.Drawing.Size(246, 24);
            this.showGrowthPathfindingToolStripMenuItem.Text = "Show Growth Pathfinding";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.monthAndYearLabel,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 528);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(782, 25);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // monthAndYearLabel
            // 
            this.monthAndYearLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.monthAndYearLabel.Name = "monthAndYearLabel";
            this.monthAndYearLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.monthAndYearLabel.Size = new System.Drawing.Size(4, 20);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(50, 20);
            this.toolStripStatusLabel1.Text = "Ready";
            // 
            // saveCityDialog
            // 
            this.saveCityDialog.DefaultExt = "xml";
            this.saveCityDialog.Filter = "XML files|*.xml";
            this.saveCityDialog.Title = "Save city";
            // 
            // openCityDialog
            // 
            this.openCityDialog.Filter = "XML files|*.xml";
            this.openCityDialog.Title = "Open city";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 553);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "Main Form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statisticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel monthAndYearLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem showPollution;
        private System.Windows.Forms.ToolStripMenuItem showCrime;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomFullMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomHalfMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showTrafficStats;
        private System.Windows.Forms.ToolStripMenuItem newCityToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem rendererToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGrowthPathfindingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCityAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCityToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveCityDialog;
        private System.Windows.Forms.OpenFileDialog openCityDialog;
        private System.Windows.Forms.ToolStripMenuItem showTravelDistancesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showPopulationDensityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showLandValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

