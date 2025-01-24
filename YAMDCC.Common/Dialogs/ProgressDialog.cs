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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YAMDCC.Common.Dialogs;

public sealed partial class ProgressDialog<TResult> : Form
{
    #region Disable close button
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cParams = base.CreateParams;
            cParams.ClassStyle |= 0x200;   // CP_NOCLOSE_BUTTON
            return cParams;
        }
    }
    #endregion

    public TResult Result { get; set; }

    private readonly Func<TResult> DoWork;

    private readonly Timer DisplayTimer = new();

    public string Caption
    {
        get => lblCaption.Text;
        set => lblCaption.Text = string.IsNullOrEmpty(value)
            ? "Please wait..."
            : value;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ProgressDialog"/> class.
    /// </summary>
    /// <param name="caption">
    /// The window caption to use.
    /// </param>
    /// <param name="doWork">
    /// The <see cref="Func{T}"/> to run when showing this window.
    /// </param>
    /// <exception cref="ArgumentNullException"/>
    public ProgressDialog(string caption, Func<TResult> doWork)
    {
        Opacity = 0;
        InitializeComponent();

        // sanity check
        if (doWork is null)
        {
            throw new ArgumentNullException(nameof(doWork), "The doWork parameter was null.");
        }
        DoWork = doWork;

        // set title text
        Caption = caption;

        pbProgress.Style = ProgressBarStyle.Marquee;

        DisplayTimer.Interval = 1000;
        DisplayTimer.Tick += new EventHandler(ShowProgress);
    }

    private async void OnLoad(object sender, EventArgs e)
    {
        DisplayTimer.Start();
        Result = await Task.Run(DoWork);
        Close();
    }

    private void ShowProgress(object sender, EventArgs e)
    {
        Opacity = 1;
        DisplayTimer.Stop();
    }
}
