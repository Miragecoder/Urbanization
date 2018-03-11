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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newCityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCityAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cityBudgetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.evaluationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.overlayMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomFullMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomHalfMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.rendererToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGrowthPathFinding = new System.Windows.Forms.ToolStripMenuItem();
            this.enableMoneyCheatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toggleOverlayNumbers = new System.Windows.Forms.ToolStripMenuItem();
            this.debugWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.forceZedGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.monthAndYearLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.cityBudgetLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.projectedIncomeLabel = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(586, 24);
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
            this.toolStripMenuItem1,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newCityToolStripMenuItem
            // 
            this.newCityToolStripMenuItem.Name = "newCityToolStripMenuItem";
            this.newCityToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newCityToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.newCityToolStripMenuItem.Text = "New city...";
            this.newCityToolStripMenuItem.Click += new System.EventHandler(this.newCityToolStripMenuItem_Click);
            // 
            // openCityToolStripMenuItem
            // 
            this.openCityToolStripMenuItem.Name = "openCityToolStripMenuItem";
            this.openCityToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openCityToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.openCityToolStripMenuItem.Text = "Open city";
            this.openCityToolStripMenuItem.Click += new System.EventHandler(this.openCityToolStripMenuItem_Click);
            // 
            // saveCityToolStripMenuItem
            // 
            this.saveCityToolStripMenuItem.Enabled = false;
            this.saveCityToolStripMenuItem.Name = "saveCityToolStripMenuItem";
            this.saveCityToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveCityToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.saveCityToolStripMenuItem.Text = "Save city";
            this.saveCityToolStripMenuItem.Click += new System.EventHandler(this.saveCityToolStripMenuItem_Click);
            // 
            // saveCityAsToolStripMenuItem
            // 
            this.saveCityAsToolStripMenuItem.Enabled = false;
            this.saveCityAsToolStripMenuItem.Name = "saveCityAsToolStripMenuItem";
            this.saveCityAsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.saveCityAsToolStripMenuItem.Text = "Save city as...";
            this.saveCityAsToolStripMenuItem.Click += new System.EventHandler(this.saveCityAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(169, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(172, 22);
            this.toolStripMenuItem1.Text = "Start webserver";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(169, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cityBudgetToolStripMenuItem,
            this.evaluationToolStripMenuItem,
            this.statisticsToolStripMenuItem,
            this.toolStripSeparator2,
            this.overlayMenuItem,
            this.zoomToolStripMenuItem,
            this.toolStripSeparator4,
            this.rendererToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // cityBudgetToolStripMenuItem
            // 
            this.cityBudgetToolStripMenuItem.Name = "cityBudgetToolStripMenuItem";
            this.cityBudgetToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.cityBudgetToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.cityBudgetToolStripMenuItem.Text = "City budget";
            this.cityBudgetToolStripMenuItem.Click += new System.EventHandler(this.cityBudgetToolStripMenuItem_Click);
            // 
            // evaluationToolStripMenuItem
            // 
            this.evaluationToolStripMenuItem.Name = "evaluationToolStripMenuItem";
            this.evaluationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.evaluationToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.evaluationToolStripMenuItem.Text = "Evaluation...";
            this.evaluationToolStripMenuItem.Click += new System.EventHandler(this.evaluationToolStripMenuItem_Click);
            // 
            // statisticsToolStripMenuItem
            // 
            this.statisticsToolStripMenuItem.Name = "statisticsToolStripMenuItem";
            this.statisticsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.statisticsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.statisticsToolStripMenuItem.Text = "Statistics...";
            this.statisticsToolStripMenuItem.Click += new System.EventHandler(this.statisticsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(175, 6);
            // 
            // overlayMenuItem
            // 
            this.overlayMenuItem.Name = "overlayMenuItem";
            this.overlayMenuItem.Size = new System.Drawing.Size(178, 22);
            this.overlayMenuItem.Text = "Overlay";
            // 
            // zoomToolStripMenuItem
            // 
            this.zoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomFullMenuItem,
            this.zoomHalfMenuItem});
            this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            this.zoomToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.zoomToolStripMenuItem.Text = "Zoom";
            // 
            // zoomFullMenuItem
            // 
            this.zoomFullMenuItem.Name = "zoomFullMenuItem";
            this.zoomFullMenuItem.Size = new System.Drawing.Size(102, 22);
            this.zoomFullMenuItem.Text = "100%";
            this.zoomFullMenuItem.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // zoomHalfMenuItem
            // 
            this.zoomHalfMenuItem.Name = "zoomHalfMenuItem";
            this.zoomHalfMenuItem.Size = new System.Drawing.Size(102, 22);
            this.zoomHalfMenuItem.Text = "50%";
            this.zoomHalfMenuItem.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(175, 6);
            // 
            // rendererToolStripMenuItem
            // 
            this.rendererToolStripMenuItem.Name = "rendererToolStripMenuItem";
            this.rendererToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.rendererToolStripMenuItem.Text = "Renderer";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showGrowthPathFinding,
            this.enableMoneyCheatToolStripMenuItem,
            this.toolStripSeparator5,
            this.toggleOverlayNumbers,
            this.debugWindowToolStripMenuItem,
            this.toolStripSeparator6,
            this.forceZedGraphToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // showGrowthPathFinding
            // 
            this.showGrowthPathFinding.CheckOnClick = true;
            this.showGrowthPathFinding.Name = "showGrowthPathFinding";
            this.showGrowthPathFinding.Size = new System.Drawing.Size(210, 22);
            this.showGrowthPathFinding.Text = "Show Growth Pathfinding";
            // 
            // enableMoneyCheatToolStripMenuItem
            // 
            this.enableMoneyCheatToolStripMenuItem.CheckOnClick = true;
            this.enableMoneyCheatToolStripMenuItem.Name = "enableMoneyCheatToolStripMenuItem";
            this.enableMoneyCheatToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.enableMoneyCheatToolStripMenuItem.Text = "Enable money cheat";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(207, 6);
            // 
            // toggleOverlayNumbers
            // 
            this.toggleOverlayNumbers.CheckOnClick = true;
            this.toggleOverlayNumbers.Name = "toggleOverlayNumbers";
            this.toggleOverlayNumbers.Size = new System.Drawing.Size(210, 22);
            this.toggleOverlayNumbers.Text = "Toggle overlay numbers";
            // 
            // debugWindowToolStripMenuItem
            // 
            this.debugWindowToolStripMenuItem.Name = "debugWindowToolStripMenuItem";
            this.debugWindowToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.debugWindowToolStripMenuItem.Text = "Log output";
            this.debugWindowToolStripMenuItem.Click += new System.EventHandler(this.debugWindowToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(207, 6);
            // 
            // forceZedGraphToolStripMenuItem
            // 
            this.forceZedGraphToolStripMenuItem.CheckOnClick = true;
            this.forceZedGraphToolStripMenuItem.Name = "forceZedGraphToolStripMenuItem";
            this.forceZedGraphToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.forceZedGraphToolStripMenuItem.Text = "Force ZedGraph";
            this.forceZedGraphToolStripMenuItem.Click += new System.EventHandler(this.forceZedGraphToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.monthAndYearLabel,
            this.cityBudgetLabel,
            this.projectedIncomeLabel,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 426);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(586, 24);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // monthAndYearLabel
            // 
            this.monthAndYearLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.monthAndYearLabel.Name = "monthAndYearLabel";
            this.monthAndYearLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.monthAndYearLabel.Size = new System.Drawing.Size(119, 19);
            this.monthAndYearLabel.Text = "monthAndYearLabel";
            // 
            // cityBudgetLabel
            // 
            this.cityBudgetLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.cityBudgetLabel.Name = "cityBudgetLabel";
            this.cityBudgetLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cityBudgetLabel.Size = new System.Drawing.Size(96, 19);
            this.cityBudgetLabel.Text = "cityBudgetLabel";
            // 
            // projectedIncomeLabel
            // 
            this.projectedIncomeLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.projectedIncomeLabel.Name = "projectedIncomeLabel";
            this.projectedIncomeLabel.Size = new System.Drawing.Size(129, 19);
            this.projectedIncomeLabel.Text = "projectedIncomeLabel";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(81, 19);
            this.toolStripStatusLabel1.Text = "messageLabel";
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "Main Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomFullMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomHalfMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newCityToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem rendererToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableMoneyCheatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCityAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCityToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveCityDialog;
        private System.Windows.Forms.OpenFileDialog openCityDialog;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem evaluationToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel cityBudgetLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem overlayMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleOverlayNumbers;
        private System.Windows.Forms.ToolStripStatusLabel projectedIncomeLabel;
        private System.Windows.Forms.ToolStripMenuItem cityBudgetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGrowthPathFinding;
        private System.Windows.Forms.ToolStripMenuItem debugWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem forceZedGraphToolStripMenuItem;
    }
}

