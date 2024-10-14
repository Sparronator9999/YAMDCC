// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 2023-2024.
//
// YAMDCC is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// YAMDCC is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// YAMDCC. If not, see <https://www.gnu.org/licenses/>.

namespace YAMDCC.GUI.Dialogs
{
    partial class CrashDialog
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
            System.Windows.Forms.TableLayoutPanel tblMain;
            System.Windows.Forms.Button btnCopy;
            System.Windows.Forms.Button btnExit;
            System.Windows.Forms.Label lblError;
            System.Windows.Forms.Button btnIssues;
            System.Windows.Forms.GroupBox grpReport;
            this.txtStackTrace = new System.Windows.Forms.TextBox();
            tblMain = new System.Windows.Forms.TableLayoutPanel();
            btnCopy = new System.Windows.Forms.Button();
            btnExit = new System.Windows.Forms.Button();
            lblError = new System.Windows.Forms.Label();
            btnIssues = new System.Windows.Forms.Button();
            grpReport = new System.Windows.Forms.GroupBox();
            tblMain.SuspendLayout();
            grpReport.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            tblMain.ColumnCount = 4;
            tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblMain.Controls.Add(btnCopy, 2, 2);
            tblMain.Controls.Add(btnExit, 3, 2);
            tblMain.Controls.Add(lblError, 0, 0);
            tblMain.Controls.Add(btnIssues, 0, 2);
            tblMain.Controls.Add(grpReport, 0, 1);
            tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tblMain.Location = new System.Drawing.Point(0, 0);
            tblMain.Name = "tblMain";
            tblMain.RowCount = 3;
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.Size = new System.Drawing.Size(400, 250);
            tblMain.TabIndex = 0;
            // 
            // btnCopy
            // 
            btnCopy.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            btnCopy.Location = new System.Drawing.Point(207, 221);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new System.Drawing.Size(109, 25);
            btnCopy.TabIndex = 1;
            btnCopy.Text = "Copy crash report";
            btnCopy.UseVisualStyleBackColor = true;
            btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnExit
            // 
            btnExit.Location = new System.Drawing.Point(322, 221);
            btnExit.Name = "btnExit";
            btnExit.Size = new System.Drawing.Size(75, 25);
            btnExit.TabIndex = 0;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            tblMain.SetColumnSpan(lblError, 4);
            lblError.Dock = System.Windows.Forms.DockStyle.Fill;
            lblError.Location = new System.Drawing.Point(3, 3);
            lblError.Margin = new System.Windows.Forms.Padding(3);
            lblError.Name = "lblError";
            lblError.Size = new System.Drawing.Size(394, 60);
            lblError.TabIndex = 0;
            lblError.Text = "YAMDCC has crashed!\r\n\r\nBefore reporting a bug, try again on the latest commit of " +
    "YAMDCC.\r\nIf the crash still occurs, please include the following crash report:";
            // 
            // btnIssues
            // 
            btnIssues.Dock = System.Windows.Forms.DockStyle.Fill;
            btnIssues.Location = new System.Drawing.Point(3, 221);
            btnIssues.Name = "btnIssues";
            btnIssues.Size = new System.Drawing.Size(100, 26);
            btnIssues.TabIndex = 1;
            btnIssues.Text = "GitHub Issues";
            btnIssues.UseVisualStyleBackColor = true;
            btnIssues.Click += new System.EventHandler(this.btnReportIssue_Click);
            // 
            // grpReport
            // 
            tblMain.SetColumnSpan(grpReport, 4);
            grpReport.Controls.Add(this.txtStackTrace);
            grpReport.Dock = System.Windows.Forms.DockStyle.Fill;
            grpReport.Location = new System.Drawing.Point(3, 69);
            grpReport.Name = "grpReport";
            grpReport.Size = new System.Drawing.Size(394, 146);
            grpReport.TabIndex = 3;
            grpReport.TabStop = false;
            grpReport.Text = "Crash report";
            // 
            // txtStackTrace
            // 
            this.txtStackTrace.BackColor = System.Drawing.Color.White;
            this.txtStackTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStackTrace.Location = new System.Drawing.Point(3, 19);
            this.txtStackTrace.MaxLength = 2147483647;
            this.txtStackTrace.Multiline = true;
            this.txtStackTrace.Name = "txtStackTrace";
            this.txtStackTrace.ReadOnly = true;
            this.txtStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStackTrace.Size = new System.Drawing.Size(388, 124);
            this.txtStackTrace.TabIndex = 0;
            // 
            // CrashDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(400, 250);
            this.Controls.Add(tblMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CrashDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Crash!";
            tblMain.ResumeLayout(false);
            tblMain.PerformLayout();
            grpReport.ResumeLayout(false);
            grpReport.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtStackTrace;
    }
}
