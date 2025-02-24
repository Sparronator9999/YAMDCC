using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using YAMDCC.Common;
using YAMDCC.Common.Configs;
using YAMDCC.Common.Dialogs;
using YAMDCC.HotkeyHandler.Config;

namespace YAMDCC.HotkeyHandler;

public partial class MainForm : Form
{
    private readonly HotkeyConf Config;

    private readonly List<TextBox> txtHotkeys = [];

    private Hotkey OldHotkey;
    private bool bindInProgress;

    private bool BindInProgress
    {
        get => bindInProgress;
        set
        {
            bindInProgress = value;
            lblBindInProgress.Text = $"{value}";
        }
    }

    public MainForm()
    {
        InitializeComponent();

        TrayIcon.Text = Text = $"YAMDCC hotkey handler - v{Utils.GetVerString()}";
        Icon = Utils.GetEntryAssemblyIcon();
        TrayIcon.Icon = Icon;

        try
        {
            Config = HotkeyConf.Load(Paths.HotkeyConf);
        }
        catch (Exception ex)
        {
            if (ex is FileNotFoundException or InvalidOperationException or InvalidConfigException)
            {
                Config = new();
            }
            else
            {
                throw;
            }
        }

        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
    }

    private void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        Config?.Save(Paths.HotkeyConf);
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

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ReloadHotkeys();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)  
    {
        base.OnFormClosing(e);
        if (e.CloseReason == CloseReason.UserClosing && tsiTrayClose.Checked)
        {
            Hide();
            e.Cancel = true;
        }
    }

    protected override void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            // catch Minimise events before the form actually minimises
            case 0x112: // WM_SYSCOMMAND
                if (tsiTrayMin.Checked && (m.WParam.ToInt32() & 0xfff0) == 0xF020)  // SC_MINIMIZE
                {
                    Hide();
                    return;
                }
                break;
            /*case 0x312: // WM_HOTKEY
                // only handle shortcuts if hotkeys are
                // enabled and the config window isn't open
                if (tsiEnabled.Checked && !Visible)
                {

                }
            break;*/
        }
        base.WndProc(ref m);
    }

    private void tsiAbout_Click(object sender, EventArgs e)
    {
        VersionDialog dlg = new();
        if (sender == tsiTrayAbout)
        {
            // fix version dialog appearing in corner when using tray icon link
            dlg.StartPosition = FormStartPosition.CenterScreen;
        }
        dlg.ShowDialog();
    }

    private void tsiSource_Click(object sender, EventArgs e)
    {
        Process.Start(Paths.GitHubPage);
    }

    private void Exit(object sender, EventArgs e)
    {
        TrayIcon.Visible = false;
        Application.Exit();
    }

    private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        Show();
        Activate();
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

    private void ReloadHotkeys()
    {
        if (Config.Hotkeys.Count == 0)
        {
            Config.Hotkeys.Add(new Hotkey());
        }

        float scale = AutoScaleDimensions.Width / 96;
        tblHotKeys.SuspendLayout();
        txtHotkeys.Clear();
        tblHotKeys.Controls.Clear();
        tblHotKeys.RowStyles.Clear();
        tblHotKeys.RowCount = Config.Hotkeys.Count + 1;

        for (int i = 0; i < Config.Hotkeys.Count; i++)
        {
            Hotkey hk = Config.Hotkeys[i];
            tblHotKeys.RowStyles.Add(new RowStyle());
            tblHotKeys.Controls.Add(ActionComboBox(i, scale, hk.Action), 0, i);
            txtHotkeys.Add(new TextBox
            {
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Margin = new Padding((int)(2 * scale)),
                Tag = i,
                Text = HotkeyText(hk.Modifiers, hk.KeyCode),
            });
            txtHotkeys[i].Leave += KeyBindLeave;
            txtHotkeys[i].KeyDown += KeyBindDown;
            txtHotkeys[i].KeyUp += KeyBindUp;
            tblHotKeys.Controls.Add(txtHotkeys[i], 1, i);
            tblHotKeys.Controls.Add(HotkeyButton(i, false, scale), 2, i);
            tblHotKeys.Controls.Add(HotkeyButton(i, true, scale), 3, i);
        }
        tblHotKeys.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tblHotKeys.ResumeLayout();
    }

    private void KeyBindLeave(object sender, EventArgs e)
    {
        if (BindInProgress)
        {
            Config.Hotkeys[(int)((Control)sender).Tag] = OldHotkey;
            BindInProgress = false;
        }
    }

    private void KeyBindDown(object sender, KeyEventArgs e)
    {
        TextBox tb = (TextBox)sender;
        int i = (int)tb.Tag;

        if (!BindInProgress)
        {
            OldHotkey = Config.Hotkeys[i];
            Config.Hotkeys[i].Modifiers = 0;
            BindInProgress = true;
            tb.Clear();
        }

        switch (e.KeyCode)
        {
            case Keys.None:
            case (Keys)255:
            case Keys.LWin:
            case Keys.RWin:
                break;
            case Keys.LMenu:
            case Keys.RMenu:
            case Keys.Menu:
                if ((Config.Hotkeys[i].Modifiers & HotkeyModifiers.Alt) != HotkeyModifiers.Alt)
                {
                    Config.Hotkeys[i].Modifiers |= HotkeyModifiers.Alt;
                    tb.Text = HotkeyText(Config.Hotkeys[i].Modifiers);
                }
                break;
            case Keys.LControlKey:
            case Keys.RControlKey:
            case Keys.ControlKey:
                if ((Config.Hotkeys[i].Modifiers & HotkeyModifiers.Ctrl) != HotkeyModifiers.Ctrl)
                {
                    Config.Hotkeys[i].Modifiers |= HotkeyModifiers.Ctrl;
                    tb.Text = HotkeyText(Config.Hotkeys[i].Modifiers);
                }
                break;
            case Keys.LShiftKey:
            case Keys.RShiftKey:
            case Keys.ShiftKey:
                if ((Config.Hotkeys[i].Modifiers & HotkeyModifiers.Shift) != HotkeyModifiers.Shift)
                {
                    Config.Hotkeys[i].Modifiers |= HotkeyModifiers.Shift;
                    tb.Text = HotkeyText(Config.Hotkeys[i].Modifiers);
                }
                break;
            default:
                tb.Text = HotkeyText(Config.Hotkeys[i].Modifiers, e.KeyCode);
                Config.Hotkeys[i].KeyCode = e.KeyCode;
                BindInProgress = false;
                break;
        }
    }

    private void KeyBindUp(object sender, KeyEventArgs e)
    {
        TextBox tb = (TextBox)sender;
        int i = (int)tb.Tag;

        if (!BindInProgress)
        {
            return;
        }

        switch (e.KeyCode)
        {
            case Keys.LMenu:
            case Keys.RMenu:
            case Keys.Menu:
                if ((Config.Hotkeys[i].Modifiers & HotkeyModifiers.Alt) == HotkeyModifiers.Alt)
                {
                    Config.Hotkeys[i].Modifiers &= ~HotkeyModifiers.Alt;
                }
                break;
            case Keys.LControlKey:
            case Keys.RControlKey:
            case Keys.ControlKey:
                if ((Config.Hotkeys[i].Modifiers & HotkeyModifiers.Ctrl) == HotkeyModifiers.Ctrl)
                {
                    Config.Hotkeys[i].Modifiers &= ~HotkeyModifiers.Ctrl;
                }
                break;
            case Keys.LShiftKey:
            case Keys.RShiftKey:
            case Keys.ShiftKey:
                if ((Config.Hotkeys[i].Modifiers & HotkeyModifiers.Shift) == HotkeyModifiers.Shift)
                {
                    Config.Hotkeys[i].Modifiers &= ~HotkeyModifiers.Shift;
                }
                break;
        }
        tb.Text = HotkeyText(Config.Hotkeys[i].Modifiers);
    }

    private ComboBox ActionComboBox(int tag, float scale, HotkeyAction action)
    {
        ComboBox cbo = new()
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding((int)(2 * scale)),
            Tag = tag,
        };
        cbo.Items.AddRange(
        [
            "None",
            "Open config editor",
            "Toggle Full Blast",
            "Toggle Win/Fn swap",
            "Increase keyboard backlight",
            "Decrease keyboard backlight",
            "Switch to next fan profile",
            "Switch to Default",
            "Switch to Silent",
            "Switch to Performance",
        ]);
        cbo.SelectedIndex = (int)action;
        cbo.SelectedIndexChanged += new EventHandler(ActionChanged);
        return cbo;
    }

    private Button HotkeyButton(int tag, bool del, float scale)
    {
        Button b = new()
        {
            Margin = new Padding((int)(2 * scale)),
            Size = new Size((int)(23 * scale), (int)(23 * scale)),
            Tag = tag,
            Text = del ? "-" : "+",
        };
        b.Click += del ? ActionDel : ActionAdd;
        return b;
    }

    private void ActionAdd(object sender, EventArgs e)
    {
        Button b = (Button)sender;
        Config.Hotkeys.Insert((int)b.Tag + 1, new Hotkey());
        ReloadHotkeys();
    }

    private void ActionDel(object sender, EventArgs e)
    {
        Button b = (Button)sender;
        Config.Hotkeys.RemoveAt((int)b.Tag);
        ReloadHotkeys();
    }

    private void ActionChanged(object sender, EventArgs e)
    {
        ComboBox cb = (ComboBox)sender;
        Config.Hotkeys[(int)cb.Tag].Action = (HotkeyAction)cb.SelectedIndex;
    }

    private static string HotkeyText(HotkeyModifiers modifiers, Keys key = Keys.None)
    {
        string s = string.Empty;
        if ((modifiers & HotkeyModifiers.Ctrl) == HotkeyModifiers.Ctrl)
        {
            s += $"{HotkeyModifiers.Ctrl} + ";
        }
        if ((modifiers & HotkeyModifiers.Shift) == HotkeyModifiers.Shift)
        {
            s += $"{HotkeyModifiers.Shift} + ";
        }
        if ((modifiers & HotkeyModifiers.Alt) == HotkeyModifiers.Alt)
        {
            s += $"{HotkeyModifiers.Alt} + ";
        }
        if (key != Keys.None)
        {
            s += $"{key}";
        }
        return s;
    }
}
