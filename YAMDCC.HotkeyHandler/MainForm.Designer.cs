namespace YAMDCC.HotkeyHandler
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.MenuStrip menuStrip;
            System.Windows.Forms.ToolStripMenuItem tsiFile;
            System.Windows.Forms.ToolStripMenuItem tsiExit;
            System.Windows.Forms.ToolStripMenuItem tsiOptions;
            System.Windows.Forms.ToolStripSeparator sep3;
            System.Windows.Forms.ToolStripSeparator sep1;
            System.Windows.Forms.ToolStripMenuItem tsiSysStart;
            System.Windows.Forms.ToolStripMenuItem tsiHelp;
            System.Windows.Forms.ToolStripMenuItem tsiAbout;
            System.Windows.Forms.ToolStripMenuItem tsiSource;
            System.Windows.Forms.ContextMenuStrip TrayMenu;
            System.Windows.Forms.ToolStripSeparator sep2;
            System.Windows.Forms.ToolStripMenuItem tsiTrayExit;
            System.Windows.Forms.GroupBox grpHKeys;
            this.tsiEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiTrayMin = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiTrayClose = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiTrayAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tblHotKeys = new System.Windows.Forms.TableLayoutPanel();
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblBindInProgress = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnRevert = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            menuStrip = new System.Windows.Forms.MenuStrip();
            tsiFile = new System.Windows.Forms.ToolStripMenuItem();
            tsiExit = new System.Windows.Forms.ToolStripMenuItem();
            tsiOptions = new System.Windows.Forms.ToolStripMenuItem();
            sep3 = new System.Windows.Forms.ToolStripSeparator();
            sep1 = new System.Windows.Forms.ToolStripSeparator();
            tsiSysStart = new System.Windows.Forms.ToolStripMenuItem();
            tsiHelp = new System.Windows.Forms.ToolStripMenuItem();
            tsiAbout = new System.Windows.Forms.ToolStripMenuItem();
            tsiSource = new System.Windows.Forms.ToolStripMenuItem();
            TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            sep2 = new System.Windows.Forms.ToolStripSeparator();
            tsiTrayExit = new System.Windows.Forms.ToolStripMenuItem();
            grpHKeys = new System.Windows.Forms.GroupBox();
            menuStrip.SuspendLayout();
            TrayMenu.SuspendLayout();
            grpHKeys.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsiFile,
            tsiOptions,
            tsiHelp});
            menuStrip.Location = new System.Drawing.Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            menuStrip.Size = new System.Drawing.Size(624, 24);
            menuStrip.TabIndex = 1;
            menuStrip.Text = "menuStrip1";
            // 
            // tsiFile
            // 
            tsiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsiExit});
            tsiFile.Name = "tsiFile";
            tsiFile.Size = new System.Drawing.Size(37, 20);
            tsiFile.Text = "&File";
            // 
            // tsiExit
            // 
            tsiExit.Name = "tsiExit";
            tsiExit.Size = new System.Drawing.Size(93, 22);
            tsiExit.Text = "E&xit";
            tsiExit.Click += new System.EventHandler(this.Exit);
            // 
            // tsiOptions
            // 
            tsiOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiEnabled,
            sep3,
            this.tsiTrayMin,
            this.tsiTrayClose,
            sep1,
            tsiSysStart});
            tsiOptions.Name = "tsiOptions";
            tsiOptions.Size = new System.Drawing.Size(61, 20);
            tsiOptions.Text = "&Options";
            // 
            // tsiEnabled
            // 
            this.tsiEnabled.Checked = true;
            this.tsiEnabled.CheckOnClick = true;
            this.tsiEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsiEnabled.Name = "tsiEnabled";
            this.tsiEnabled.Size = new System.Drawing.Size(196, 22);
            this.tsiEnabled.Text = "Enable hotkeys";
            // 
            // sep3
            // 
            sep3.Name = "sep3";
            sep3.Size = new System.Drawing.Size(193, 6);
            // 
            // tsiTrayMin
            // 
            this.tsiTrayMin.Checked = true;
            this.tsiTrayMin.CheckOnClick = true;
            this.tsiTrayMin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsiTrayMin.Name = "tsiTrayMin";
            this.tsiTrayMin.Size = new System.Drawing.Size(196, 22);
            this.tsiTrayMin.Text = "&Minimise to tray";
            // 
            // tsiTrayClose
            // 
            this.tsiTrayClose.CheckOnClick = true;
            this.tsiTrayClose.Name = "tsiTrayClose";
            this.tsiTrayClose.Size = new System.Drawing.Size(196, 22);
            this.tsiTrayClose.Text = "&Close to tray";
            // 
            // sep1
            // 
            sep1.Name = "sep1";
            sep1.Size = new System.Drawing.Size(193, 6);
            // 
            // tsiSysStart
            // 
            tsiSysStart.CheckOnClick = true;
            tsiSysStart.Name = "tsiSysStart";
            tsiSysStart.Size = new System.Drawing.Size(196, 22);
            tsiSysStart.Text = "&Start on boot (all users)";
            tsiSysStart.Click += new System.EventHandler(this.tsiSysStart_Click);
            // 
            // tsiHelp
            // 
            tsiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsiAbout,
            tsiSource});
            tsiHelp.Name = "tsiHelp";
            tsiHelp.Size = new System.Drawing.Size(44, 20);
            tsiHelp.Text = "&Help";
            // 
            // tsiAbout
            // 
            tsiAbout.Name = "tsiAbout";
            tsiAbout.Size = new System.Drawing.Size(139, 22);
            tsiAbout.Text = "&About";
            tsiAbout.Click += new System.EventHandler(this.tsiAbout_Click);
            // 
            // tsiSource
            // 
            tsiSource.Name = "tsiSource";
            tsiSource.Size = new System.Drawing.Size(139, 22);
            tsiSource.Text = "Source &code";
            tsiSource.Click += new System.EventHandler(this.tsiSource_Click);
            // 
            // TrayMenu
            // 
            TrayMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiTrayAbout,
            sep2,
            tsiTrayExit});
            TrayMenu.Name = "TrayMenu";
            TrayMenu.Size = new System.Drawing.Size(169, 54);
            // 
            // tsiTrayAbout
            // 
            this.tsiTrayAbout.Name = "tsiTrayAbout";
            this.tsiTrayAbout.Size = new System.Drawing.Size(168, 22);
            this.tsiTrayAbout.Text = "About YAMDCC...";
            this.tsiTrayAbout.Click += new System.EventHandler(this.tsiAbout_Click);
            // 
            // sep2
            // 
            sep2.Name = "sep2";
            sep2.Size = new System.Drawing.Size(165, 6);
            // 
            // tsiTrayExit
            // 
            tsiTrayExit.Name = "tsiTrayExit";
            tsiTrayExit.Size = new System.Drawing.Size(168, 22);
            tsiTrayExit.Text = "Exit";
            tsiTrayExit.Click += new System.EventHandler(this.Exit);
            // 
            // grpHKeys
            // 
            this.tableLayoutPanel1.SetColumnSpan(grpHKeys, 3);
            grpHKeys.Controls.Add(this.tblHotKeys);
            grpHKeys.Dock = System.Windows.Forms.DockStyle.Fill;
            grpHKeys.Location = new System.Drawing.Point(3, 3);
            grpHKeys.Name = "grpHKeys";
            grpHKeys.Size = new System.Drawing.Size(618, 322);
            grpHKeys.TabIndex = 8;
            grpHKeys.TabStop = false;
            grpHKeys.Text = "Hotkeys";
            // 
            // tblHotKeys
            // 
            this.tblHotKeys.ColumnCount = 5;
            this.tblHotKeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 182F));
            this.tblHotKeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 134F));
            this.tblHotKeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblHotKeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblHotKeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblHotKeys.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblHotKeys.Location = new System.Drawing.Point(3, 19);
            this.tblHotKeys.Name = "tblHotKeys";
            this.tblHotKeys.RowCount = 2;
            this.tblHotKeys.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblHotKeys.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblHotKeys.Size = new System.Drawing.Size(612, 300);
            this.tblHotKeys.TabIndex = 7;
            // 
            // TrayIcon
            // 
            this.TrayIcon.ContextMenuStrip = TrayMenu;
            this.TrayIcon.Visible = true;
            this.TrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TrayIcon_MouseDoubleClick);
            // 
            // lblBindInProgress
            // 
            this.lblBindInProgress.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblBindInProgress.AutoSize = true;
            this.lblBindInProgress.Location = new System.Drawing.Point(3, 335);
            this.lblBindInProgress.Name = "lblBindInProgress";
            this.lblBindInProgress.Size = new System.Drawing.Size(33, 15);
            this.lblBindInProgress.TabIndex = 9;
            this.lblBindInProgress.Text = "False";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(grpHKeys, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblBindInProgress, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnRevert, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnApply, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(624, 357);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // btnRevert
            // 
            this.btnRevert.Location = new System.Drawing.Point(465, 331);
            this.btnRevert.Name = "btnRevert";
            this.btnRevert.Size = new System.Drawing.Size(75, 23);
            this.btnRevert.TabIndex = 10;
            this.btnRevert.Text = "Revert";
            this.btnRevert.UseVisualStyleBackColor = true;
            this.btnRevert.Click += new System.EventHandler(this.btnRevert_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(546, 331);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 11;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(624, 381);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(menuStrip);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = menuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            TrayMenu.ResumeLayout(false);
            grpHKeys.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.TableLayoutPanel tblHotKeys;
        private System.Windows.Forms.ToolStripMenuItem tsiTrayMin;
        private System.Windows.Forms.ToolStripMenuItem tsiTrayClose;
        private System.Windows.Forms.ToolStripMenuItem tsiEnabled;
        private System.Windows.Forms.ToolStripMenuItem tsiTrayAbout;
        private System.Windows.Forms.Label lblBindInProgress;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnRevert;
        private System.Windows.Forms.Button btnApply;
    }
}
