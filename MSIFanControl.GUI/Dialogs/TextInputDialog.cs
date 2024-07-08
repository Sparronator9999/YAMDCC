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
