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
    partial class VersionDialog
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
            System.Windows.Forms.PictureBox picLogo;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VersionDialog));
            System.Windows.Forms.Label lblTitle;
            System.Windows.Forms.Label lblSubtitle;
            System.Windows.Forms.TableLayoutPanel tblMain;
            System.Windows.Forms.FlowLayoutPanel flwLinks;
            System.Windows.Forms.Button btnLicense;
            System.Windows.Forms.Button btnSource;
            System.Windows.Forms.Button btnFAQ;
            System.Windows.Forms.Button btnIssues;
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblDesc = new System.Windows.Forms.Label();
            this.lblRevision = new System.Windows.Forms.Label();
            picLogo = new System.Windows.Forms.PictureBox();
            lblTitle = new System.Windows.Forms.Label();
            lblSubtitle = new System.Windows.Forms.Label();
            tblMain = new System.Windows.Forms.TableLayoutPanel();
            flwLinks = new System.Windows.Forms.FlowLayoutPanel();
            btnLicense = new System.Windows.Forms.Button();
            btnSource = new System.Windows.Forms.Button();
            btnFAQ = new System.Windows.Forms.Button();
            btnIssues = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(picLogo)).BeginInit();
            tblMain.SuspendLayout();
            flwLinks.SuspendLayout();
            this.SuspendLayout();
            // 
            // picLogo
            // 
            picLogo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            picLogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picLogo.BackgroundImage")));
            picLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            picLogo.Location = new System.Drawing.Point(6, 6);
            picLogo.Name = "picLogo";
            tblMain.SetRowSpan(picLogo, 6);
            picLogo.Size = new System.Drawing.Size(128, 128);
            picLogo.TabIndex = 0;
            picLogo.TabStop = false;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblTitle.Location = new System.Drawing.Point(140, 3);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(114, 32);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "YAMDCC";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new System.Drawing.Point(140, 35);
            lblSubtitle.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new System.Drawing.Size(214, 15);
            lblSubtitle.TabIndex = 2;
            lblSubtitle.Text = "(Yet Another MSI Dragon Center Clone)";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(140, 56);
            this.lblVersion.Margin = new System.Windows.Forms.Padding(3);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(48, 15);
            this.lblVersion.TabIndex = 3;
            this.lblVersion.Text = "Version ";
            // 
            // tblMain
            // 
            tblMain.AutoSize = true;
            tblMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tblMain.ColumnCount = 2;
            tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblMain.Controls.Add(picLogo, 0, 0);
            tblMain.Controls.Add(lblTitle, 1, 0);
            tblMain.Controls.Add(lblSubtitle, 1, 1);
            tblMain.Controls.Add(this.lblVersion, 1, 2);
            tblMain.Controls.Add(this.lblCopyright, 0, 6);
            tblMain.Controls.Add(this.lblDesc, 1, 4);
            tblMain.Controls.Add(flwLinks, 1, 5);
            tblMain.Controls.Add(this.lblRevision, 1, 3);
            tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tblMain.Location = new System.Drawing.Point(0, 0);
            tblMain.Name = "tblMain";
            tblMain.Padding = new System.Windows.Forms.Padding(3);
            tblMain.RowCount = 7;
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.Size = new System.Drawing.Size(524, 181);
            tblMain.TabIndex = 5;
            // 
            // lblCopyright
            // 
            this.lblCopyright.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblCopyright.AutoSize = true;
            tblMain.SetColumnSpan(this.lblCopyright, 2);
            this.lblCopyright.Location = new System.Drawing.Point(232, 151);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(60, 15);
            this.lblCopyright.TabIndex = 4;
            this.lblCopyright.Text = "Copyright";
            this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblDesc
            // 
            this.lblDesc.AutoSize = true;
            this.lblDesc.Location = new System.Drawing.Point(140, 98);
            this.lblDesc.Margin = new System.Windows.Forms.Padding(3);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(67, 15);
            this.lblDesc.TabIndex = 5;
            this.lblDesc.Text = "Description";
            // 
            // flwLinks
            // 
            flwLinks.AutoSize = true;
            flwLinks.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            flwLinks.Controls.Add(btnLicense);
            flwLinks.Controls.Add(btnSource);
            flwLinks.Controls.Add(btnFAQ);
            flwLinks.Controls.Add(btnIssues);
            flwLinks.Location = new System.Drawing.Point(140, 119);
            flwLinks.Name = "flwLinks";
            flwLinks.Size = new System.Drawing.Size(324, 29);
            flwLinks.TabIndex = 6;
            // 
            // btnLicense
            // 
            btnLicense.Location = new System.Drawing.Point(3, 3);
            btnLicense.Name = "btnLicense";
            btnLicense.Size = new System.Drawing.Size(75, 23);
            btnLicense.TabIndex = 0;
            btnLicense.Text = "License";
            btnLicense.UseVisualStyleBackColor = true;
            btnLicense.Click += new System.EventHandler(this.btnLicense_Click);
            // 
            // btnSource
            // 
            btnSource.Location = new System.Drawing.Point(84, 3);
            btnSource.Name = "btnSource";
            btnSource.Size = new System.Drawing.Size(75, 23);
            btnSource.TabIndex = 1;
            btnSource.Text = "Source";
            btnSource.UseVisualStyleBackColor = true;
            btnSource.Click += new System.EventHandler(this.btnSource_Click);
            // 
            // btnFAQ
            // 
            btnFAQ.Location = new System.Drawing.Point(165, 3);
            btnFAQ.Name = "btnFAQ";
            btnFAQ.Size = new System.Drawing.Size(75, 23);
            btnFAQ.TabIndex = 2;
            btnFAQ.Text = "FAQ";
            btnFAQ.UseVisualStyleBackColor = true;
            btnFAQ.Click += new System.EventHandler(this.btnFAQ_Click);
            // 
            // btnIssues
            // 
            btnIssues.Location = new System.Drawing.Point(246, 3);
            btnIssues.Name = "btnIssues";
            btnIssues.Size = new System.Drawing.Size(75, 23);
            btnIssues.TabIndex = 3;
            btnIssues.Text = "Issues";
            btnIssues.UseVisualStyleBackColor = true;
            btnIssues.Click += new System.EventHandler(this.btnIssues_Click);
            // 
            // lblRevision
            // 
            this.lblRevision.AutoSize = true;
            this.lblRevision.Location = new System.Drawing.Point(140, 77);
            this.lblRevision.Margin = new System.Windows.Forms.Padding(3);
            this.lblRevision.Name = "lblRevision";
            this.lblRevision.Size = new System.Drawing.Size(83, 15);
            this.lblRevision.TabIndex = 7;
            this.lblRevision.Text = "Revision (Git): ";
            // 
            // VersionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(524, 181);
            this.Controls.Add(tblMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VersionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About YAMDCC";
            ((System.ComponentModel.ISupportInitialize)(picLogo)).EndInit();
            tblMain.ResumeLayout(false);
            tblMain.PerformLayout();
            flwLinks.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Label lblRevision;
    }
}
