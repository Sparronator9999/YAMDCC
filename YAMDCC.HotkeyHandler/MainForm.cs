using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
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
        // only handle shortcuts if hotkeys are
        // enabled and the config window isn't open
        if (!tsiEnabled.Checked || Visible)
        {
            return;
        }

        switch (e.KeyCode)
        {
            case (Keys)255:
                // corresponds to the MSI Center key (Fn+F7) on my laptop (GF63 Thin 11SC)
                if (e.ExtendedKey && e.ScanCode == 10)
                {
                    if (tsiAppBtnDefaultDisable.Checked)
                    {
                        e.SuppressKeyPress = true;
                    }

                    if (!tsiAppBtnConfEditor.Checked)
                    {
                        break;
                    }

                    try
                    {
                        Process p = Process.Start(@".\ConfigEditor.exe");
                    }
                    catch (Win32Exception ex)
                    {
                        if (ex.NativeErrorCode == 2)    // ERROR_FILE_NOT_FOUND
                        {
                            Utils.ShowError("Could not find ConfigEditor.exe!");
                        }
                        // Ignore exception if user cancelled UAC
                        // (should not be the case since we get run with admin permissons)
                        else if (ex.NativeErrorCode != 1223)    // ERROR_CANCELLED
                        {
                            throw;
                        }
                    }
                }
                break;
        }
    }

    private void tsiSysStart_Click(object sender, EventArgs e)
    {
        ToolStripMenuItem tsi = (ToolStripMenuItem)sender;

        RegistryKey key = Registry.LocalMachine.OpenSubKey(
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

        if (tsi.Checked)
        {
            key.SetValue("YAMDCC hotkey handler", $"\"{Assembly.GetEntryAssembly().Location}\" --startup");
        }
        else
        {
            key.DeleteValue("YAMDCC hotkey handler", false);
        }
    }
}
