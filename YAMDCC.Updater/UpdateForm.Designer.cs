namespace YAMDCC.Updater
{
    partial class UpdateForm
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
            System.Windows.Forms.TableLayoutPanel tblMain;
            System.Windows.Forms.TableLayoutPanel tblBotBar;
            this.wbChangelog = new System.Windows.Forms.WebBrowser();
            this.grpProgress = new System.Windows.Forms.GroupBox();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnLater = new System.Windows.Forms.Button();
            this.btnDisable = new System.Windows.Forms.Button();
            this.OptMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsiAutoUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiPreRelease = new System.Windows.Forms.ToolStripMenuItem();
            tblMain = new System.Windows.Forms.TableLayoutPanel();
            tblBotBar = new System.Windows.Forms.TableLayoutPanel();
            tblMain.SuspendLayout();
            this.grpProgress.SuspendLayout();
            tblBotBar.SuspendLayout();
            this.OptMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            tblMain.ColumnCount = 1;
            tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblMain.Controls.Add(this.wbChangelog, 0, 0);
            tblMain.Controls.Add(this.grpProgress, 0, 1);
            tblMain.Controls.Add(tblBotBar, 0, 2);
            tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tblMain.Location = new System.Drawing.Point(0, 0);
            tblMain.Name = "tblMain";
            tblMain.RowCount = 3;
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.Size = new System.Drawing.Size(624, 441);
            tblMain.TabIndex = 0;
            // 
            // wbChangelog
            // 
            this.wbChangelog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbChangelog.Location = new System.Drawing.Point(3, 3);
            this.wbChangelog.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbChangelog.Name = "wbChangelog";
            this.wbChangelog.Size = new System.Drawing.Size(618, 342);
            this.wbChangelog.TabIndex = 0;
            this.wbChangelog.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.wbChangelog_Navigating);
            // 
            // grpProgress
            // 
            this.grpProgress.Controls.Add(this.pbProgress);
            this.grpProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpProgress.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpProgress.Location = new System.Drawing.Point(3, 351);
            this.grpProgress.Name = "grpProgress";
            this.grpProgress.Size = new System.Drawing.Size(618, 45);
            this.grpProgress.TabIndex = 1;
            this.grpProgress.TabStop = false;
            this.grpProgress.Text = "Waiting for input...";
            // 
            // pbProgress
            // 
            this.pbProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbProgress.Location = new System.Drawing.Point(3, 19);
            this.pbProgress.MarqueeAnimationSpeed = 20;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(612, 23);
            this.pbProgress.TabIndex = 0;
            // 
            // tblBotBar
            // 
            tblBotBar.AutoSize = true;
            tblBotBar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tblBotBar.ColumnCount = 5;
            tblBotBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblBotBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblBotBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblBotBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblBotBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblBotBar.Controls.Add(this.btnUpdate, 4, 0);
            tblBotBar.Controls.Add(this.btnOptions, 0, 0);
            tblBotBar.Controls.Add(this.btnLater, 3, 0);
            tblBotBar.Controls.Add(this.btnDisable, 2, 0);
            tblBotBar.Dock = System.Windows.Forms.DockStyle.Fill;
            tblBotBar.Location = new System.Drawing.Point(3, 402);
            tblBotBar.Name = "tblBotBar";
            tblBotBar.RowCount = 1;
            tblBotBar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblBotBar.Size = new System.Drawing.Size(618, 36);
            tblBotBar.TabIndex = 2;
            // 
            // btnUpdate
            // 
            this.btnUpdate.AutoSize = true;
            this.btnUpdate.Location = new System.Drawing.Point(502, 3);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(113, 30);
            this.btnUpdate.TabIndex = 3;
            this.btnUpdate.Text = "Check for &updates";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnOptions
            // 
            this.btnOptions.AutoSize = true;
            this.btnOptions.Location = new System.Drawing.Point(3, 3);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(98, 30);
            this.btnOptions.TabIndex = 0;
            this.btnOptions.Text = "Update &options";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // btnLater
            // 
            this.btnLater.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnLater.Enabled = false;
            this.btnLater.Location = new System.Drawing.Point(391, 3);
            this.btnLater.Name = "btnLater";
            this.btnLater.Size = new System.Drawing.Size(105, 30);
            this.btnLater.TabIndex = 2;
            this.btnLater.Text = "Remind me &later";
            this.btnLater.UseVisualStyleBackColor = true;
            this.btnLater.Visible = false;
            this.btnLater.Click += new System.EventHandler(this.btnRemindLater_Click);
            // 
            // btnDisable
            // 
            this.btnDisable.Enabled = false;
            this.btnDisable.Location = new System.Drawing.Point(265, 3);
            this.btnDisable.Name = "btnDisable";
            this.btnDisable.Size = new System.Drawing.Size(120, 30);
            this.btnDisable.TabIndex = 1;
            this.btnDisable.Text = "&Never auto-update";
            this.btnDisable.UseVisualStyleBackColor = true;
            this.btnDisable.Visible = false;
            this.btnDisable.Click += new System.EventHandler(this.btnDisableUpdates_Click);
            // 
            // OptMenu
            // 
            this.OptMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiAutoUpdate,
            this.tsiPreRelease});
            this.OptMenu.Name = "OptionsMenu";
            this.OptMenu.Size = new System.Drawing.Size(246, 48);
            // 
            // tsiAutoUpdate
            // 
            this.tsiAutoUpdate.Name = "tsiAutoUpdate";
            this.tsiAutoUpdate.Size = new System.Drawing.Size(245, 22);
            this.tsiAutoUpdate.Text = "Check for updates automatically";
            this.tsiAutoUpdate.Click += new System.EventHandler(this.tsiAutoUpdate_Click);
            // 
            // tsiPreRelease
            // 
            this.tsiPreRelease.Checked = true;
            this.tsiPreRelease.CheckOnClick = true;
            this.tsiPreRelease.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsiPreRelease.Name = "tsiPreRelease";
            this.tsiPreRelease.Size = new System.Drawing.Size(245, 22);
            this.tsiPreRelease.Text = "Include pre-release versions";
            this.tsiPreRelease.Click += new System.EventHandler(this.tsiPreRelease_Click);
            // 
            // UpdateForm
            // 
            this.AcceptButton = this.btnUpdate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnLater;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(tblMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "UpdateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            tblMain.ResumeLayout(false);
            tblMain.PerformLayout();
            this.grpProgress.ResumeLayout(false);
            tblBotBar.ResumeLayout(false);
            tblBotBar.PerformLayout();
            this.OptMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpProgress;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.WebBrowser wbChangelog;
        private System.Windows.Forms.ToolStripMenuItem tsiAutoUpdate;
        private System.Windows.Forms.ToolStripMenuItem tsiPreRelease;
        private System.Windows.Forms.ContextMenuStrip OptMenu;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnLater;
        private System.Windows.Forms.Button btnDisable;
    }
}
