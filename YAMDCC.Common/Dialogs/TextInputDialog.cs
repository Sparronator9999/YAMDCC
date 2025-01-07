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
using System.Windows.Forms;

namespace YAMDCC.Common.Dialogs;

public sealed partial class TextInputDialog : Form
{
    /// <summary>
    /// The text that the user entered in this dialog.
    /// </summary>
    public string Result { get; set; }

    public TextInputDialog(string caption, string title, string text, bool multiline = false)
    {
        InitializeComponent();
        lblCaption.Text = caption;
        txtInput.Text = text;
        txtInput.Multiline = multiline;
        txtInput.Height = (int)(AutoScaleDimensions.Height / 96 * 69);
        Text = title;
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        Result = txtInput.Text;
    }

    private void txtInput_TextChanged(object sender, EventArgs e)
    {
        // make sure text input isn't empty
        // before allowing user to click "OK":
        btnOK.Enabled = !string.IsNullOrEmpty(txtInput.Text);
    }
}
