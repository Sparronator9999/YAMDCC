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

namespace YAMDCC.ConfigEditor.Dialogs
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
            System.Windows.Forms.Button btnIssues;
            System.Windows.Forms.GroupBox grpReport;
            this.lblError = new System.Windows.Forms.Label();
            this.txtReport = new System.Windows.Forms.TextBox();
            tblMain = new System.Windows.Forms.TableLayoutPanel();
            btnCopy = new System.Windows.Forms.Button();
            btnExit = new System.Windows.Forms.Button();
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
            tblMain.Controls.Add(this.lblError, 0, 0);
            tblMain.Controls.Add(btnIssues, 0, 2);
            tblMain.Controls.Add(grpReport, 0, 1);
            tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tblMain.Location = new System.Drawing.Point(0, 0);
            tblMain.Name = "tblMain";
            tblMain.RowCount = 3;
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.Size = new System.Drawing.Size(480, 320);
            tblMain.TabIndex = 0;
            // 
            // btnCopy
            // 
            btnCopy.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            btnCopy.Location = new System.Drawing.Point(287, 291);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new System.Drawing.Size(109, 25);
            btnCopy.TabIndex = 0;
            btnCopy.Text = "&Copy crash report";
            btnCopy.UseVisualStyleBackColor = true;
            btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnExit
            // 
            btnExit.Location = new System.Drawing.Point(402, 291);
            btnExit.Name = "btnExit";
            btnExit.Size = new System.Drawing.Size(75, 25);
            btnExit.TabIndex = 1;
            btnExit.Text = "E&xit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            tblMain.SetColumnSpan(this.lblError, 4);
            this.lblError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblError.Location = new System.Drawing.Point(3, 3);
            this.lblError.Margin = new System.Windows.Forms.Padding(3);
            this.lblError.Name = "lblError";
            this.lblError.Padding = new System.Windows.Forms.Padding(3);
            this.lblError.Size = new System.Drawing.Size(474, 21);
            this.lblError.TabIndex = 2;
            // 
            // btnIssues
            // 
            btnIssues.Location = new System.Drawing.Point(3, 291);
            btnIssues.Name = "btnIssues";
            btnIssues.Size = new System.Drawing.Size(100, 26);
            btnIssues.TabIndex = 4;
            btnIssues.Text = "GitHub &Issues";
            btnIssues.UseVisualStyleBackColor = true;
            btnIssues.Click += new System.EventHandler(this.btnReportIssue_Click);
            // 
            // grpReport
            // 
            tblMain.SetColumnSpan(grpReport, 4);
            grpReport.Controls.Add(this.txtReport);
            grpReport.Dock = System.Windows.Forms.DockStyle.Fill;
            grpReport.Location = new System.Drawing.Point(3, 30);
            grpReport.Name = "grpReport";
            grpReport.Size = new System.Drawing.Size(474, 255);
            grpReport.TabIndex = 3;
            grpReport.TabStop = false;
            grpReport.Text = "Crash report";
            // 
            // txtReport
            // 
            this.txtReport.BackColor = System.Drawing.Color.White;
            this.txtReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReport.Location = new System.Drawing.Point(3, 19);
            this.txtReport.MaxLength = 2147483647;
            this.txtReport.Multiline = true;
            this.txtReport.Name = "txtReport";
            this.txtReport.ReadOnly = true;
            this.txtReport.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReport.Size = new System.Drawing.Size(468, 233);
            this.txtReport.TabIndex = 0;
            // 
            // CrashDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(480, 320);
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

        private System.Windows.Forms.TextBox txtReport;
        private System.Windows.Forms.Label lblError;
    }
}
