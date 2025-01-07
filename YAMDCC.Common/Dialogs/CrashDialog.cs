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
    public sealed partial class CrashDialog : Form
    {
        public CrashDialog(Exception ex)
        {
            InitializeComponent();
            lblError.Text = Strings.GetString("Crash");
            txtReport.Text = $"{ex.GetType()}: {ex.Message}\r\n{ex.StackTrace}";
        }

        private void btnReportIssue_Click(object sender, EventArgs e)
        {
            Process.Start($"{Paths.GitHubPage}/issues");
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtReport.Text);

            // should never fail, but better safe than sorry
            // (this is the crash handling dialog after all)
            if (sender is Button b)
            {
                // give confirmation that the crash report has been copied
                b.Text = "Copied!";
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
