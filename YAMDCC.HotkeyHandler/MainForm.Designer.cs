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
            System.Windows.Forms.ToolStripMenuItem tsiEnabled;
            System.Windows.Forms.ToolStripMenuItem tsiAppBtn;
            System.Windows.Forms.ToolStripMenuItem tsiAppBtnAction;
            System.Windows.Forms.ToolStripSeparator sep3;
            System.Windows.Forms.ToolStripSeparator sep1;
            System.Windows.Forms.ToolStripMenuItem tsiSysStart;
            System.Windows.Forms.ToolStripMenuItem tsiHelp;
            System.Windows.Forms.ToolStripMenuItem tsiAbout;
            System.Windows.Forms.ToolStripMenuItem tsiSource;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.ContextMenuStrip TrayMenu;
            System.Windows.Forms.ToolStripMenuItem tsiTrayAbout;
            System.Windows.Forms.ToolStripSeparator sep2;
            System.Windows.Forms.ToolStripMenuItem tsiTrayExit;
            System.Windows.Forms.Label lblKeyLightUp;
            System.Windows.Forms.Label lblConfEditor;
            System.Windows.Forms.Label lblWinFn;
            System.Windows.Forms.Label lblFullBlast;
            System.Windows.Forms.Label lblKeyLightDown;
            System.Windows.Forms.Button btnProf1Clear;
            System.Windows.Forms.Button btnProf2Clear;
            System.Windows.Forms.Button btnProf3Clear;
            System.Windows.Forms.Button btnConfEditorClear;
            System.Windows.Forms.Button btnFullBlastClear;
            System.Windows.Forms.Button btnWinFnClear;
            System.Windows.Forms.Button btnKeyLightUpClear;
            System.Windows.Forms.Button btnKeyLightDownClear;
            this.tsiAppBtnDisabled = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiAppBtnConfEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiAppBtnDefaultDisable = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiTrayMin = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiTrayClose = new System.Windows.Forms.ToolStripMenuItem();
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.tcHotkeys = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.tblGeneral = new System.Windows.Forms.TableLayoutPanel();
            this.txtKeyLightDown = new System.Windows.Forms.TextBox();
            this.txtKeyLightUp = new System.Windows.Forms.TextBox();
            this.txtConfEditor = new System.Windows.Forms.TextBox();
            this.txtFullBlast = new System.Windows.Forms.TextBox();
            this.txtWinFn = new System.Windows.Forms.TextBox();
            this.tabFanProfs = new System.Windows.Forms.TabPage();
            this.tblFanProfs = new System.Windows.Forms.TableLayoutPanel();
            this.txtProf1 = new System.Windows.Forms.TextBox();
            this.txtProf2 = new System.Windows.Forms.TextBox();
            this.txtProf3 = new System.Windows.Forms.TextBox();
            this.lblProf1 = new System.Windows.Forms.Label();
            this.lblProf3 = new System.Windows.Forms.Label();
            this.lblProf2 = new System.Windows.Forms.Label();
            menuStrip = new System.Windows.Forms.MenuStrip();
            tsiFile = new System.Windows.Forms.ToolStripMenuItem();
            tsiExit = new System.Windows.Forms.ToolStripMenuItem();
            tsiOptions = new System.Windows.Forms.ToolStripMenuItem();
            tsiEnabled = new System.Windows.Forms.ToolStripMenuItem();
            tsiAppBtn = new System.Windows.Forms.ToolStripMenuItem();
            tsiAppBtnAction = new System.Windows.Forms.ToolStripMenuItem();
            sep3 = new System.Windows.Forms.ToolStripSeparator();
            sep1 = new System.Windows.Forms.ToolStripSeparator();
            tsiSysStart = new System.Windows.Forms.ToolStripMenuItem();
            tsiHelp = new System.Windows.Forms.ToolStripMenuItem();
            tsiAbout = new System.Windows.Forms.ToolStripMenuItem();
            tsiSource = new System.Windows.Forms.ToolStripMenuItem();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            tsiTrayAbout = new System.Windows.Forms.ToolStripMenuItem();
            sep2 = new System.Windows.Forms.ToolStripSeparator();
            tsiTrayExit = new System.Windows.Forms.ToolStripMenuItem();
            lblKeyLightUp = new System.Windows.Forms.Label();
            lblConfEditor = new System.Windows.Forms.Label();
            lblWinFn = new System.Windows.Forms.Label();
            lblFullBlast = new System.Windows.Forms.Label();
            lblKeyLightDown = new System.Windows.Forms.Label();
            btnProf1Clear = new System.Windows.Forms.Button();
            btnProf2Clear = new System.Windows.Forms.Button();
            btnProf3Clear = new System.Windows.Forms.Button();
            btnConfEditorClear = new System.Windows.Forms.Button();
            btnFullBlastClear = new System.Windows.Forms.Button();
            btnWinFnClear = new System.Windows.Forms.Button();
            btnKeyLightUpClear = new System.Windows.Forms.Button();
            btnKeyLightDownClear = new System.Windows.Forms.Button();
            menuStrip.SuspendLayout();
            TrayMenu.SuspendLayout();
            this.tcHotkeys.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tblGeneral.SuspendLayout();
            this.tabFanProfs.SuspendLayout();
            this.tblFanProfs.SuspendLayout();
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
            tsiExit.Size = new System.Drawing.Size(180, 22);
            tsiExit.Text = "E&xit";
            tsiExit.Click += new System.EventHandler(this.Exit);
            // 
            // tsiOptions
            // 
            tsiOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsiEnabled,
            tsiAppBtn,
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
            tsiEnabled.Checked = true;
            tsiEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            tsiEnabled.Enabled = false;
            tsiEnabled.Name = "tsiEnabled";
            tsiEnabled.Size = new System.Drawing.Size(244, 22);
            tsiEnabled.Text = "Enable hotkeys";
            // 
            // tsiAppBtn
            // 
            tsiAppBtn.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsiAppBtnAction,
            this.tsiAppBtnDefaultDisable});
            tsiAppBtn.Enabled = false;
            tsiAppBtn.Name = "tsiAppBtn";
            tsiAppBtn.Size = new System.Drawing.Size(244, 22);
            tsiAppBtn.Text = "MSI Center shortcut key settings";
            // 
            // tsiAppBtnAction
            // 
            tsiAppBtnAction.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiAppBtnDisabled,
            this.tsiAppBtnConfEditor});
            tsiAppBtnAction.Name = "tsiAppBtnAction";
            tsiAppBtnAction.Size = new System.Drawing.Size(222, 22);
            tsiAppBtnAction.Text = "Shortcut key action";
            // 
            // tsiAppBtnDisabled
            // 
            this.tsiAppBtnDisabled.Name = "tsiAppBtnDisabled";
            this.tsiAppBtnDisabled.Size = new System.Drawing.Size(226, 22);
            this.tsiAppBtnDisabled.Text = "Do nothing";
            // 
            // tsiAppBtnConfEditor
            // 
            this.tsiAppBtnConfEditor.Name = "tsiAppBtnConfEditor";
            this.tsiAppBtnConfEditor.Size = new System.Drawing.Size(226, 22);
            this.tsiAppBtnConfEditor.Text = "Open YAMDCC config editor";
            // 
            // tsiAppBtnDefaultDisable
            // 
            this.tsiAppBtnDefaultDisable.Name = "tsiAppBtnDefaultDisable";
            this.tsiAppBtnDefaultDisable.Size = new System.Drawing.Size(222, 22);
            this.tsiAppBtnDefaultDisable.Text = "Try to prevent default action";
            // 
            // sep3
            // 
            sep3.Name = "sep3";
            sep3.Size = new System.Drawing.Size(241, 6);
            // 
            // tsiTrayMin
            // 
            this.tsiTrayMin.Checked = true;
            this.tsiTrayMin.CheckOnClick = true;
            this.tsiTrayMin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsiTrayMin.Name = "tsiTrayMin";
            this.tsiTrayMin.Size = new System.Drawing.Size(244, 22);
            this.tsiTrayMin.Text = "&Minimise to tray";
            // 
            // tsiTrayClose
            // 
            this.tsiTrayClose.CheckOnClick = true;
            this.tsiTrayClose.Name = "tsiTrayClose";
            this.tsiTrayClose.Size = new System.Drawing.Size(244, 22);
            this.tsiTrayClose.Text = "&Close to tray";
            // 
            // sep1
            // 
            sep1.Name = "sep1";
            sep1.Size = new System.Drawing.Size(241, 6);
            // 
            // tsiSysStart
            // 
            tsiSysStart.Enabled = false;
            tsiSysStart.Name = "tsiSysStart";
            tsiSysStart.Size = new System.Drawing.Size(244, 22);
            tsiSysStart.Text = "&Start on boot";
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
            tsiAbout.Size = new System.Drawing.Size(180, 22);
            tsiAbout.Text = "&About";
            tsiAbout.Click += new System.EventHandler(this.tsiAbout_Click);
            // 
            // tsiSource
            // 
            tsiSource.Name = "tsiSource";
            tsiSource.Size = new System.Drawing.Size(180, 22);
            tsiSource.Text = "Source &code";
            tsiSource.Click += new System.EventHandler(this.tsiSource_Click);
            // 
            // label1
            // 
            label1.AutoSize = true;
            this.tblGeneral.SetColumnSpan(label1, 2);
            label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label1.ForeColor = System.Drawing.Color.Red;
            label1.Location = new System.Drawing.Point(3, 145);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(546, 30);
            label1.TabIndex = 13;
            label1.Text = "Most of these options do not work yet.\r\nPlease do not open an issue regarding non" +
    "-functioning Hotkey Handler functionality; it is still WIP.";
            // 
            // label2
            // 
            label2.AutoSize = true;
            this.tblFanProfs.SetColumnSpan(label2, 2);
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label2.ForeColor = System.Drawing.Color.Red;
            label2.Location = new System.Drawing.Point(3, 87);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(546, 30);
            label2.TabIndex = 14;
            label2.Text = "Most of these options do not work yet.\r\nPlease do not open an issue regarding non" +
    "-functioning Hotkey Handler functionality; it is still WIP.";
            // 
            // TrayIcon
            // 
            this.TrayIcon.ContextMenuStrip = TrayMenu;
            this.TrayIcon.Text = "YAMDCC hotkey handler";
            this.TrayIcon.Visible = true;
            this.TrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TrayIcon_MouseDoubleClick);
            // 
            // TrayMenu
            // 
            TrayMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsiTrayAbout,
            sep2,
            tsiTrayExit});
            TrayMenu.Name = "TrayMenu";
            TrayMenu.Size = new System.Drawing.Size(169, 54);
            // 
            // tsiTrayAbout
            // 
            tsiTrayAbout.Name = "tsiTrayAbout";
            tsiTrayAbout.Size = new System.Drawing.Size(168, 22);
            tsiTrayAbout.Text = "About YAMDCC...";
            tsiTrayAbout.Click += new System.EventHandler(this.tsiAbout_Click);
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
            // tcHotkeys
            // 
            this.tcHotkeys.Controls.Add(this.tabGeneral);
            this.tcHotkeys.Controls.Add(this.tabFanProfs);
            this.tcHotkeys.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcHotkeys.Location = new System.Drawing.Point(0, 24);
            this.tcHotkeys.Name = "tcHotkeys";
            this.tcHotkeys.SelectedIndex = 0;
            this.tcHotkeys.Size = new System.Drawing.Size(624, 357);
            this.tcHotkeys.TabIndex = 2;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.tblGeneral);
            this.tabGeneral.Location = new System.Drawing.Point(4, 24);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Size = new System.Drawing.Size(616, 329);
            this.tabGeneral.TabIndex = 1;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // tblGeneral
            // 
            this.tblGeneral.ColumnCount = 3;
            this.tblGeneral.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblGeneral.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblGeneral.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblGeneral.Controls.Add(btnKeyLightDownClear, 2, 4);
            this.tblGeneral.Controls.Add(btnKeyLightUpClear, 2, 3);
            this.tblGeneral.Controls.Add(btnWinFnClear, 2, 2);
            this.tblGeneral.Controls.Add(btnFullBlastClear, 2, 1);
            this.tblGeneral.Controls.Add(btnConfEditorClear, 2, 0);
            this.tblGeneral.Controls.Add(this.txtKeyLightDown, 1, 4);
            this.tblGeneral.Controls.Add(this.txtKeyLightUp, 1, 3);
            this.tblGeneral.Controls.Add(lblKeyLightUp, 0, 3);
            this.tblGeneral.Controls.Add(lblConfEditor, 0, 0);
            this.tblGeneral.Controls.Add(lblWinFn, 0, 2);
            this.tblGeneral.Controls.Add(lblFullBlast, 0, 1);
            this.tblGeneral.Controls.Add(this.txtConfEditor, 1, 0);
            this.tblGeneral.Controls.Add(this.txtFullBlast, 1, 1);
            this.tblGeneral.Controls.Add(this.txtWinFn, 1, 2);
            this.tblGeneral.Controls.Add(lblKeyLightDown, 0, 4);
            this.tblGeneral.Controls.Add(label1, 0, 5);
            this.tblGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblGeneral.Location = new System.Drawing.Point(0, 0);
            this.tblGeneral.Name = "tblGeneral";
            this.tblGeneral.RowCount = 6;
            this.tblGeneral.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblGeneral.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblGeneral.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblGeneral.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblGeneral.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblGeneral.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblGeneral.Size = new System.Drawing.Size(616, 329);
            this.tblGeneral.TabIndex = 7;
            // 
            // txtKeyLightDown
            // 
            this.txtKeyLightDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtKeyLightDown.Enabled = false;
            this.txtKeyLightDown.Location = new System.Drawing.Point(170, 119);
            this.txtKeyLightDown.Name = "txtKeyLightDown";
            this.txtKeyLightDown.Size = new System.Drawing.Size(414, 23);
            this.txtKeyLightDown.TabIndex = 12;
            // 
            // txtKeyLightUp
            // 
            this.txtKeyLightUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtKeyLightUp.Enabled = false;
            this.txtKeyLightUp.Location = new System.Drawing.Point(170, 90);
            this.txtKeyLightUp.Name = "txtKeyLightUp";
            this.txtKeyLightUp.Size = new System.Drawing.Size(414, 23);
            this.txtKeyLightUp.TabIndex = 11;
            // 
            // lblKeyLightUp
            // 
            lblKeyLightUp.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblKeyLightUp.AutoSize = true;
            lblKeyLightUp.Location = new System.Drawing.Point(7, 94);
            lblKeyLightUp.Name = "lblKeyLightUp";
            lblKeyLightUp.Size = new System.Drawing.Size(157, 15);
            lblKeyLightUp.TabIndex = 9;
            lblKeyLightUp.Text = "Increase keyboard backlight:";
            // 
            // lblConfEditor
            // 
            lblConfEditor.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblConfEditor.AutoSize = true;
            lblConfEditor.Location = new System.Drawing.Point(54, 7);
            lblConfEditor.Name = "lblConfEditor";
            lblConfEditor.Size = new System.Drawing.Size(110, 15);
            lblConfEditor.TabIndex = 3;
            lblConfEditor.Text = "Open config editor:";
            // 
            // lblWinFn
            // 
            lblWinFn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblWinFn.AutoSize = true;
            lblWinFn.Location = new System.Drawing.Point(47, 65);
            lblWinFn.Name = "lblWinFn";
            lblWinFn.Size = new System.Drawing.Size(117, 15);
            lblWinFn.TabIndex = 5;
            lblWinFn.Text = "Toggle Win/Fn swap:";
            // 
            // lblFullBlast
            // 
            lblFullBlast.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblFullBlast.AutoSize = true;
            lblFullBlast.Location = new System.Drawing.Point(69, 36);
            lblFullBlast.Name = "lblFullBlast";
            lblFullBlast.Size = new System.Drawing.Size(95, 15);
            lblFullBlast.TabIndex = 2;
            lblFullBlast.Text = "Toggle Full Blast:";
            // 
            // txtConfEditor
            // 
            this.txtConfEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConfEditor.Enabled = false;
            this.txtConfEditor.Location = new System.Drawing.Point(170, 3);
            this.txtConfEditor.Name = "txtConfEditor";
            this.txtConfEditor.Size = new System.Drawing.Size(414, 23);
            this.txtConfEditor.TabIndex = 6;
            // 
            // txtFullBlast
            // 
            this.txtFullBlast.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFullBlast.Enabled = false;
            this.txtFullBlast.Location = new System.Drawing.Point(170, 32);
            this.txtFullBlast.Name = "txtFullBlast";
            this.txtFullBlast.Size = new System.Drawing.Size(414, 23);
            this.txtFullBlast.TabIndex = 7;
            // 
            // txtWinFn
            // 
            this.txtWinFn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtWinFn.Enabled = false;
            this.txtWinFn.Location = new System.Drawing.Point(170, 61);
            this.txtWinFn.Name = "txtWinFn";
            this.txtWinFn.Size = new System.Drawing.Size(414, 23);
            this.txtWinFn.TabIndex = 8;
            // 
            // lblKeyLightDown
            // 
            lblKeyLightDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblKeyLightDown.AutoSize = true;
            lblKeyLightDown.Location = new System.Drawing.Point(3, 123);
            lblKeyLightDown.Name = "lblKeyLightDown";
            lblKeyLightDown.Size = new System.Drawing.Size(161, 15);
            lblKeyLightDown.TabIndex = 10;
            lblKeyLightDown.Text = "Decrease keyboard backlight:";
            // 
            // tabFanProfs
            // 
            this.tabFanProfs.Controls.Add(this.tblFanProfs);
            this.tabFanProfs.Location = new System.Drawing.Point(4, 24);
            this.tabFanProfs.Name = "tabFanProfs";
            this.tabFanProfs.Size = new System.Drawing.Size(616, 329);
            this.tabFanProfs.TabIndex = 2;
            this.tabFanProfs.Text = "Fan profiles";
            this.tabFanProfs.UseVisualStyleBackColor = true;
            // 
            // tblFanProfs
            // 
            this.tblFanProfs.ColumnCount = 3;
            this.tblFanProfs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFanProfs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFanProfs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFanProfs.Controls.Add(btnProf3Clear, 2, 2);
            this.tblFanProfs.Controls.Add(btnProf2Clear, 2, 1);
            this.tblFanProfs.Controls.Add(label2, 0, 3);
            this.tblFanProfs.Controls.Add(this.txtProf1, 1, 0);
            this.tblFanProfs.Controls.Add(this.txtProf2, 1, 1);
            this.tblFanProfs.Controls.Add(this.txtProf3, 1, 2);
            this.tblFanProfs.Controls.Add(this.lblProf1, 0, 0);
            this.tblFanProfs.Controls.Add(this.lblProf3, 0, 2);
            this.tblFanProfs.Controls.Add(this.lblProf2, 0, 1);
            this.tblFanProfs.Controls.Add(btnProf1Clear, 2, 0);
            this.tblFanProfs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblFanProfs.Location = new System.Drawing.Point(0, 0);
            this.tblFanProfs.Name = "tblFanProfs";
            this.tblFanProfs.RowCount = 4;
            this.tblFanProfs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFanProfs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFanProfs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFanProfs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFanProfs.Size = new System.Drawing.Size(616, 329);
            this.tblFanProfs.TabIndex = 8;
            // 
            // txtProf1
            // 
            this.txtProf1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProf1.Enabled = false;
            this.txtProf1.Location = new System.Drawing.Point(136, 3);
            this.txtProf1.Name = "txtProf1";
            this.txtProf1.Size = new System.Drawing.Size(448, 23);
            this.txtProf1.TabIndex = 6;
            // 
            // txtProf2
            // 
            this.txtProf2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProf2.Enabled = false;
            this.txtProf2.Location = new System.Drawing.Point(136, 32);
            this.txtProf2.Name = "txtProf2";
            this.txtProf2.Size = new System.Drawing.Size(448, 23);
            this.txtProf2.TabIndex = 7;
            // 
            // txtProf3
            // 
            this.txtProf3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProf3.Enabled = false;
            this.txtProf3.Location = new System.Drawing.Point(136, 61);
            this.txtProf3.Name = "txtProf3";
            this.txtProf3.Size = new System.Drawing.Size(448, 23);
            this.txtProf3.TabIndex = 8;
            // 
            // lblProf1
            // 
            this.lblProf1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblProf1.AutoSize = true;
            this.lblProf1.Location = new System.Drawing.Point(33, 7);
            this.lblProf1.Name = "lblProf1";
            this.lblProf1.Size = new System.Drawing.Size(97, 15);
            this.lblProf1.TabIndex = 5;
            this.lblProf1.Text = "Switch to Default";
            // 
            // lblProf3
            // 
            this.lblProf3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblProf3.AutoSize = true;
            this.lblProf3.Location = new System.Drawing.Point(3, 65);
            this.lblProf3.Name = "lblProf3";
            this.lblProf3.Size = new System.Drawing.Size(127, 15);
            this.lblProf3.TabIndex = 2;
            this.lblProf3.Text = "Switch to Performance";
            // 
            // lblProf2
            // 
            this.lblProf2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblProf2.AutoSize = true;
            this.lblProf2.Location = new System.Drawing.Point(42, 36);
            this.lblProf2.Name = "lblProf2";
            this.lblProf2.Size = new System.Drawing.Size(88, 15);
            this.lblProf2.TabIndex = 3;
            this.lblProf2.Text = "Switch to Silent";
            // 
            // btnProf1Clear
            // 
            btnProf1Clear.Enabled = false;
            btnProf1Clear.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnProf1Clear.Location = new System.Drawing.Point(590, 3);
            btnProf1Clear.Name = "btnProf1Clear";
            btnProf1Clear.Size = new System.Drawing.Size(23, 23);
            btnProf1Clear.TabIndex = 15;
            btnProf1Clear.Text = "X";
            btnProf1Clear.UseVisualStyleBackColor = true;
            // 
            // btnProf2Clear
            // 
            btnProf2Clear.Enabled = false;
            btnProf2Clear.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnProf2Clear.Location = new System.Drawing.Point(590, 32);
            btnProf2Clear.Name = "btnProf2Clear";
            btnProf2Clear.Size = new System.Drawing.Size(23, 23);
            btnProf2Clear.TabIndex = 16;
            btnProf2Clear.Text = "X";
            btnProf2Clear.UseVisualStyleBackColor = true;
            // 
            // btnProf3Clear
            // 
            btnProf3Clear.Enabled = false;
            btnProf3Clear.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnProf3Clear.Location = new System.Drawing.Point(590, 61);
            btnProf3Clear.Name = "btnProf3Clear";
            btnProf3Clear.Size = new System.Drawing.Size(23, 23);
            btnProf3Clear.TabIndex = 17;
            btnProf3Clear.Text = "X";
            btnProf3Clear.UseVisualStyleBackColor = true;
            // 
            // btnConfEditorClear
            // 
            btnConfEditorClear.Enabled = false;
            btnConfEditorClear.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnConfEditorClear.Location = new System.Drawing.Point(590, 3);
            btnConfEditorClear.Name = "btnConfEditorClear";
            btnConfEditorClear.Size = new System.Drawing.Size(23, 23);
            btnConfEditorClear.TabIndex = 16;
            btnConfEditorClear.Text = "X";
            btnConfEditorClear.UseVisualStyleBackColor = true;
            // 
            // btnFullBlastClear
            // 
            btnFullBlastClear.Enabled = false;
            btnFullBlastClear.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnFullBlastClear.Location = new System.Drawing.Point(590, 32);
            btnFullBlastClear.Name = "btnFullBlastClear";
            btnFullBlastClear.Size = new System.Drawing.Size(23, 23);
            btnFullBlastClear.TabIndex = 17;
            btnFullBlastClear.Text = "X";
            btnFullBlastClear.UseVisualStyleBackColor = true;
            // 
            // btnWinFnClear
            // 
            btnWinFnClear.Enabled = false;
            btnWinFnClear.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnWinFnClear.Location = new System.Drawing.Point(590, 61);
            btnWinFnClear.Name = "btnWinFnClear";
            btnWinFnClear.Size = new System.Drawing.Size(23, 23);
            btnWinFnClear.TabIndex = 18;
            btnWinFnClear.Text = "X";
            btnWinFnClear.UseVisualStyleBackColor = true;
            // 
            // btnKeyLightUpClear
            // 
            btnKeyLightUpClear.Enabled = false;
            btnKeyLightUpClear.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnKeyLightUpClear.Location = new System.Drawing.Point(590, 90);
            btnKeyLightUpClear.Name = "btnKeyLightUpClear";
            btnKeyLightUpClear.Size = new System.Drawing.Size(23, 23);
            btnKeyLightUpClear.TabIndex = 19;
            btnKeyLightUpClear.Text = "X";
            btnKeyLightUpClear.UseVisualStyleBackColor = true;
            // 
            // btnKeyLightDownClear
            // 
            btnKeyLightDownClear.Enabled = false;
            btnKeyLightDownClear.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnKeyLightDownClear.Location = new System.Drawing.Point(590, 119);
            btnKeyLightDownClear.Name = "btnKeyLightDownClear";
            btnKeyLightDownClear.Size = new System.Drawing.Size(23, 23);
            btnKeyLightDownClear.TabIndex = 20;
            btnKeyLightDownClear.Text = "X";
            btnKeyLightDownClear.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(624, 381);
            this.Controls.Add(this.tcHotkeys);
            this.Controls.Add(menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = menuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            TrayMenu.ResumeLayout(false);
            this.tcHotkeys.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tblGeneral.ResumeLayout(false);
            this.tblGeneral.PerformLayout();
            this.tabFanProfs.ResumeLayout(false);
            this.tblFanProfs.ResumeLayout(false);
            this.tblFanProfs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.TabControl tcHotkeys;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TableLayoutPanel tblGeneral;
        private System.Windows.Forms.TextBox txtConfEditor;
        private System.Windows.Forms.TextBox txtFullBlast;
        private System.Windows.Forms.TextBox txtWinFn;
        private System.Windows.Forms.ToolStripMenuItem tsiAppBtnDisabled;
        private System.Windows.Forms.ToolStripMenuItem tsiAppBtnConfEditor;
        private System.Windows.Forms.ToolStripMenuItem tsiAppBtnDefaultDisable;
        private System.Windows.Forms.TextBox txtKeyLightDown;
        private System.Windows.Forms.TextBox txtKeyLightUp;
        private System.Windows.Forms.TabPage tabFanProfs;
        private System.Windows.Forms.TableLayoutPanel tblFanProfs;
        private System.Windows.Forms.Label lblProf2;
        private System.Windows.Forms.Label lblProf1;
        private System.Windows.Forms.Label lblProf3;
        private System.Windows.Forms.TextBox txtProf1;
        private System.Windows.Forms.TextBox txtProf2;
        private System.Windows.Forms.TextBox txtProf3;
        private System.Windows.Forms.ToolStripMenuItem tsiTrayMin;
        private System.Windows.Forms.ToolStripMenuItem tsiTrayClose;
    }
}
