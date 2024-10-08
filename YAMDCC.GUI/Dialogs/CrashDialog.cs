using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace YAMDCC.GUI.Dialogs
{
    internal sealed partial class CrashDialog : Form
    {
        public CrashDialog(Exception ex, bool threadException)
        {
            InitializeComponent();
            txtStackTrace.Text = threadException
                ? "Called from Application.ThreadException\r\n"
                : "Called from AppDomain.CurrentDomain.UnhandledException\r\n";

            txtStackTrace.Text += $"{ex.Message}\r\n{ex.StackTrace}";
        }

        private void btnReportIssue_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Sparronator9999/YAMDCC/issues");
        }

        private void btnSaveReport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new()
            {
                AddExtension = true,
                FileName = $"crash-{DateTime.Now:yyyy'-'MM'-'dd'T'HH'-'mm'-'ss}.txt",
                Filter = "Text files (*.txt)|*.txt",
                Title = "Save crash report",
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new(sfd.FileName);
                sw.WriteLine(txtStackTrace.Text);
                sw.Close();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
