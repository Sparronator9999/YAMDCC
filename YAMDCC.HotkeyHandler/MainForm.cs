using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YAMDCC.Common;
using YAMDCC.Common.Dialogs;

namespace YAMDCC.HotkeyHandler;

public partial class MainForm : Form
{
    private readonly KeyHook KeyHook = new();

    public MainForm()
    {
        InitializeComponent();

        Text = $"YAMDCC hotkey handler - v{Utils.GetVerString()}";
        Icon = Utils.GetEntryAssemblyIcon();
        TrayIcon.Icon = Icon;

        KeyHook.KeyDown += KeyHook_KeyDown;
        //KeyHook.KeyUp += KeyHook_KeyUp;
        if (!KeyHook.Install())
        {
            int err = Marshal.GetLastWin32Error();
            Utils.ShowError("Failed to install keyboard hook:\n" +
                $"{new Win32Exception(err).Message} ({err})");
        }
    }

    protected override void SetVisibleCore(bool value)
    {
        // hide form on first launch
        if (!IsHandleCreated)
        {
            value = false;
            CreateHandle();
        }
        base.SetVisibleCore(value);
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing && tsiTrayClose.Checked)
        {
            Hide();
            e.Cancel = true;
        }
    }

    protected override void WndProc(ref Message m)
    {
        // catch Minimise events before the form actually minimises
        // 0x112 = WM_SYSCOMMAND, 0xF020 = SC_MINIMIZE
        if (tsiTrayMin.Checked && m.Msg == 0x0112 &&
            (m.WParam.ToInt32() & 0xfff0) == 0xF020)
        {
            Hide();
            return;
        }
        base.WndProc(ref m);
    }

    private void tsiAbout_Click(object sender, EventArgs e)
    {
        new VersionDialog().ShowDialog();
    }

    private void tsiSource_Click(object sender, EventArgs e)
    {
        Process.Start(Paths.GitHubPage);
    }

    private void Exit(object sender, EventArgs e)
    {
        TrayIcon.Visible = false;
        KeyHook.Uninstall();
        Application.Exit();
    }

    private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        Show();
        Activate();
    }

    private void KeyHook_KeyDown(object sender, KeyHookEventArgs e)
    {
        // corresponds to the MSI Center key (Fn+F7) on my laptop (GF63 Thin 11SC)
        if (e.KeyCode == (Keys)255 && e.ScanCode == 10 && e.ExtendedKey)
        {
            try
            {
                Process p = Process.Start("ConfigEditor.exe");
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    Utils.ShowError("Could not find ConfigEditor.exe!");
                }
                // If UAC is denied, a Win32Exception is thrown.
                // Ignore and throw all other exceptions.
                else if (ex is not Win32Exception)
                {
                    throw;
                }
            }
        }
    }
}
