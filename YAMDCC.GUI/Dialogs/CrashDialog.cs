using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace YAMDCC.GUI.Dialogs
{
    public partial class CrashDialog : Form
    {
        public CrashDialog(Exception ex, bool threadException)
        {
            InitializeComponent();
            txtStackTrace.Text = threadException
                ? "Called from Application.ThreadException"
                : "Called from AppDomain.CurrentDomain.UnhandledException";

            txtStackTrace.Text += $"{ex.Message}\n{ex.StackTrace}";
        }

        private void btnReportIssue_Click(object sender, EventArgs e)
        {
            // TODO: work out GitHub Issues stuff
            // Process.Start("https://github.com/Sparronator9999/MSIFanControl/issues");
            Process.Start("https://youtu.be/dQw4w9WgXcQ");
        }

        private void btnSaveReport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                AddExtension = true,
                FileName = $"crash-{DateTime.Now:s}.txt",
                Filter = "Text files (*.txt)|*.txt",
                Title = "Save crash report",
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(sfd.FileName);
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
