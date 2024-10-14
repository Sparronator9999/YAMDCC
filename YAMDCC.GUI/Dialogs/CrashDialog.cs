using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace YAMDCC.GUI.Dialogs
{
    internal sealed partial class CrashDialog : Form
    {
        public CrashDialog(Exception ex, bool threadException)
        {
            InitializeComponent();
            txtStackTrace.Text = threadException
                ? "(called from Application.ThreadException)\r\n"
                : "(called from AppDomain.CurrentDomain.UnhandledException)\r\n";

            txtStackTrace.Text += $"{ex.GetType()}: {ex.Message}\r\n{ex.StackTrace}";
        }

        private void btnReportIssue_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Sparronator9999/YAMDCC/issues");
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtStackTrace.Text);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
