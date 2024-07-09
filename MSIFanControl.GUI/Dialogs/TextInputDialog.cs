// This file is part of MSI Fan Control.
// Copyright © Sparronator9999 2023-2024.
//
// MSI Fan Control is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// MSI Fan Control is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// MSI Fan Control. If not, see <https://www.gnu.org/licenses/>.

using System.Windows.Forms;

namespace MSIFanControl.GUI
{
    public partial class TextInputDialog : Form
    {
        /// <summary>
        /// The text that the user entered in this dialog.
        /// </summary>
        public string Result;

        public TextInputDialog(string caption, string title = "", string text = "")
        {
            InitializeComponent();
            lblCaption.Text = caption;
            txtInput.Text = text;
            Text = title;
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            Result = txtInput.Text;
        }
    }
}
