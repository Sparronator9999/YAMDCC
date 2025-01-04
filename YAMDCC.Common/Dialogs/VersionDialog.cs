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

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace YAMDCC.Common.Dialogs
{
    public sealed partial class VersionDialog : Form
    {
        public VersionDialog()
        {
            InitializeComponent();
            lblDesc.Text = Strings.GetString("abtDesc");
            lblCopyright.Text = Strings.GetString("abtCopyright");
            lblVersion.Text += Utils.GetVerString();

            string revision = Utils.GetRevision();

            if (string.IsNullOrEmpty(revision))
            {
                lblRevision.Hide();
            }
            else
            {
                lblRevision.Text += revision;
            }
        }

        private void btnLicense_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.gnu.org/licenses/gpl-3.0.html#license-text");
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            Process.Start(Paths.SourcePrefix);
        }

        private void btnFAQ_Click(object sender, EventArgs e)
        {
            Process.Start($"{Paths.SourcePrefix}#faq");
        }

        private void btnIssues_Click(object sender, EventArgs e)
        {
            Process.Start($"{Paths.SourcePrefix}/issues");
        }
    }
}
