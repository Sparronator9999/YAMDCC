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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace YAMDCC.ConfigEditor.Dialogs
{
    internal sealed partial class ProgressDialog : Form
    {
        #region Disable close button
        private const int CP_NOCLOSE_BUTTON = 0x200;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle |= CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        #endregion

        public bool Cancelled;
        public object Result;

        private readonly object Argument;
        private readonly string Caption;
        private readonly Action<DoWorkEventArgs> DoWork;

        private readonly BackgroundWorker Worker = new();
        private readonly Timer DisplayTimer = new();

        /// <inheritdoc cref="ProgressDialog(string, Action{DoWorkEventArgs}, object, bool, bool)"/>
        public ProgressDialog(
            string caption,
            Action<DoWorkEventArgs> doWork,
            bool reportsProgress = false,
            bool canCancel = false)
            : this(caption, doWork, null, reportsProgress, canCancel)
        { }

        /// <summary>
        /// Initialises a new instance of the <see cref="ProgressDialog"/> class.
        /// </summary>
        /// <param name="caption">
        /// The window caption to use.
        /// </param>
        /// <param name="doWork">
        /// The <see cref="Action"/> to run when showing this window.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to <paramref name="doWork"/>.
        /// </param>
        /// <param name="reportsProgress">
        /// Set to <see langword="true"/> if <paramref name="doWork"/>
        /// reports progress, otherwise <see langword="false"/>.
        /// </param>
        /// <param name="canCancel">
        /// Set to <see langword="true"/> if <paramref name="doWork"/>
        /// supports cancellation, otherwise <see langword="false"/>.
        /// </param>
        /// <exception cref="ArgumentNullException"/>
        public ProgressDialog(
            string caption,
            Action<DoWorkEventArgs> doWork,
            object argument,
            bool reportsProgress = false,
            bool canCancel = false)
        {
            Opacity = 0;
            Argument = argument;
            InitializeComponent();

            // sanity check
            if (doWork is null)
            {
                throw new ArgumentNullException(nameof(doWork), "The doWork parameter was null.");
            }
            DoWork = doWork;

            // set title text
            Caption = caption ?? "Please wait...";
            if (reportsProgress && caption is null)
            {
                Caption += " ({0}% complete)";
            }
            SetTitle(Caption);

            // event setup
            Worker.DoWork += Worker_DoWork;
            Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            if (reportsProgress)
            {
                Worker.ProgressChanged += Worker_ProgressChanged;
            }
            else
            {
                pbProgress.Style = ProgressBarStyle.Marquee;
            }

            // cancel support stuff
            if (canCancel)
            {
                Worker.WorkerSupportsCancellation = true;
            }
            else
            {
                btnCancel.Enabled = false;
                btnCancel.Visible = false;
            }

            DisplayTimer.Interval = 1000;
            DisplayTimer.Tick += DisplayTimer_Tick;
        }

        private void ProgressDialog_Load(object sender, EventArgs e)
        {
            DisplayTimer.Start();
            Worker.RunWorkerAsync(Argument);
        }

        private void DisplayTimer_Tick(object sender, EventArgs e)
        {
            Opacity = 1;
            DisplayTimer.Stop();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            DoWork?.Invoke(e);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 0)
            {
                pbProgress.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                pbProgress.Style = ProgressBarStyle.Blocks;
                pbProgress.Value = e.ProgressPercentage;
            }
            SetTitle(Caption);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error is not null)
            {
                throw e.Error;
            }
            Cancelled = e.Cancelled;
            Result = e.Result;
            Worker.Dispose();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (Worker.WorkerSupportsCancellation && Worker.IsBusy && !Worker.CancellationPending)
            {
                btnCancel.Enabled = false;
                Worker.CancelAsync();
                Text = "Cancelling...";
                pbProgress.Style = ProgressBarStyle.Marquee;
            }
        }

        private void SetTitle(string title)
        {
            lblCaption.Text = string.Format(CultureInfo.InvariantCulture, title, pbProgress.Value);
        }
    }
}
