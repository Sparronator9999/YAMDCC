// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 and Contributors 2023-2025.
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

namespace YAMDCC.Common.Dialogs
{
    partial class ProgressDialog
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
            System.Windows.Forms.FlowLayoutPanel flwButtons;
            this.btnCancel = new System.Windows.Forms.Button();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.lblCaption = new System.Windows.Forms.Label();
            tblMain = new System.Windows.Forms.TableLayoutPanel();
            flwButtons = new System.Windows.Forms.FlowLayoutPanel();
            tblMain.SuspendLayout();
            flwButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            tblMain.AutoSize = true;
            tblMain.ColumnCount = 1;
            tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblMain.Controls.Add(flwButtons, 0, 2);
            tblMain.Controls.Add(this.pbProgress, 0, 1);
            tblMain.Controls.Add(this.lblCaption, 0, 0);
            tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tblMain.Location = new System.Drawing.Point(0, 0);
            tblMain.Name = "tblMain";
            tblMain.Padding = new System.Windows.Forms.Padding(6);
            tblMain.RowCount = 3;
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.Size = new System.Drawing.Size(300, 92);
            tblMain.TabIndex = 0;
            // 
            // flwButtons
            // 
            flwButtons.AutoSize = true;
            flwButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            flwButtons.Controls.Add(this.btnCancel);
            flwButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            flwButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            flwButtons.Location = new System.Drawing.Point(6, 57);
            flwButtons.Margin = new System.Windows.Forms.Padding(0);
            flwButtons.Name = "flwButtons";
            flwButtons.Size = new System.Drawing.Size(288, 29);
            flwButtons.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(210, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pbProgress
            // 
            this.pbProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbProgress.Location = new System.Drawing.Point(9, 29);
            this.pbProgress.MarqueeAnimationSpeed = 20;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(282, 25);
            this.pbProgress.TabIndex = 2;
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Location = new System.Drawing.Point(9, 6);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(49, 15);
            this.lblCaption.TabIndex = 1;
            this.lblCaption.Text = "Caption";
            // 
            // ProgressDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(300, 92);
            this.Controls.Add(tblMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.ProgressDialog_Load);
            tblMain.ResumeLayout(false);
            tblMain.PerformLayout();
            flwButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblCaption;
    }
}
