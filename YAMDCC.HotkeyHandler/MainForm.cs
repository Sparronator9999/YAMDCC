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
using YAMDCC.IPC;

namespace YAMDCC.HotkeyHandler;

public partial class MainForm : Form
{
    private readonly NamedPipeClient<ServiceResponse, ServiceCommand> IPCClient =
        new("YAMDCC-Server");

    private YAMDCC_Config Config;

    private readonly HotkeyConf HotkeyConf;

    private readonly List<TextBox> txtHotkeys = [];

    private readonly List<ComboBox> cboActionDatas = [];

    private Hotkey OldHotkey;

    private bool BindInProgress
    {
        get => bindInProgress;
        set
        {
            bindInProgress = value;
            lblBindInProgress.Text = $"{value}";
        }
    }
    private bool bindInProgress;

    public MainForm()
    {
        InitializeComponent();

        TrayIcon.Text = Text = $"YAMDCC hotkey handler - v{Utils.GetVerString()}";
        Icon = Utils.GetEntryAssemblyIcon();
        TrayIcon.Icon = Icon;

        try
        {
            HotkeyConf = HotkeyConf.Load(Paths.HotkeyConf);
        }
        catch (Exception ex)
        {
            if (ex is FileNotFoundException or InvalidOperationException or InvalidConfigException)
            {
                HotkeyConf = new();
            }
            else
            {
                throw;
            }
        }

        IPCClient.ServerMessage += new EventHandler<PipeMessageEventArgs<ServiceResponse, ServiceCommand>>(IPCMessage);
        IPCClient.Error += new EventHandler<PipeErrorEventArgs<ServiceResponse, ServiceCommand>>(IPCError);
        IPCClient.Start();

        ProgressDialog<bool> dlg = new()
        {
            Caption = "Connecting to YAMDCC service...",
            DoWork = () => !IPCClient.WaitForConnection(5000)
        };
        dlg.ShowDialog();

        if (dlg.Result)
        {
            throw new TimeoutException("Failed to connect to the YAMDCC service (connection timed out).");
        }
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
    }

    private void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        IPCClient.Stop();
        HotkeyConf?.Save(Paths.HotkeyConf);
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

    private void IPCMessage(object sender, PipeMessageEventArgs<ServiceResponse, ServiceCommand> e)
    {
        if (e.Message.Response == Response.ConfLoaded)
        {
            // refresh hotkey handler if the current YAMDCC config is reloaded
            // (i.e. service restarted, user updated config...)
            if (e.Message.Value[0] is int i && i == IPCClient.Connection.ID)
            {
                return;
            }
            ReloadHotkeys();
        }
    }

    private void IPCError(object sender, PipeErrorEventArgs<ServiceResponse, ServiceCommand> e)
    {
        new CrashDialog(e.Exception).ShowDialog();
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
        // load currently applied YAMDCC config
        // (used to populate fan profiles/performance modes)
        Config = YAMDCC_Config.Load(Paths.CurrentConf);

        if (HotkeyConf.Hotkeys.Count == 0)
        {
            HotkeyConf.Hotkeys.Add(new Hotkey());
        }

        float scale = AutoScaleDimensions.Width / 96;

        cboActionDatas.Clear();
        txtHotkeys.Clear();
        tblHotKeys.SuspendLayout();
        tblHotKeys.Controls.Clear();
        tblHotKeys.RowStyles.Clear();
        tblHotKeys.RowCount = HotkeyConf.Hotkeys.Count + 1;

        for (int i = 0; i < HotkeyConf.Hotkeys.Count; i++)
        {
            Hotkey hk = HotkeyConf.Hotkeys[i];
            tblHotKeys.RowStyles.Add(new RowStyle());

            cboActionDatas.Add(new ComboBox()
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false,
                Margin = new Padding((int)(2 * scale)),
                Tag = i,
            });
            cboActionDatas[i].SelectedIndexChanged += new EventHandler(ActionDataChanged);
            tblHotKeys.Controls.Add(cboActionDatas[i], 1, i);

            tblHotKeys.Controls.Add(ActionComboBox(i, scale, hk.Action), 0, i);

            txtHotkeys.Add(new TextBox
            {
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Margin = new Padding((int)(2 * scale)),
                Tag = i,
                Text = HotkeyText(hk.Modifiers, hk.KeyCode),
            });
            txtHotkeys[i].Leave += new EventHandler(KeyBindLeave);
            txtHotkeys[i].KeyDown += new KeyEventHandler(KeyBindDown);
            txtHotkeys[i].KeyUp += new KeyEventHandler(KeyBindUp);
            tblHotKeys.Controls.Add(txtHotkeys[i], 2, i);

            tblHotKeys.Controls.Add(HotkeyButton(i, false, scale), 3, i);
            tblHotKeys.Controls.Add(HotkeyButton(i, true, scale), 4, i);
        }
        tblHotKeys.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tblHotKeys.ResumeLayout();
    }

    private void KeyBindLeave(object sender, EventArgs e)
    {
        if (BindInProgress)
        {
            HotkeyConf.Hotkeys[(int)((Control)sender).Tag] = OldHotkey;
            BindInProgress = false;
        }
    }

    private void KeyBindDown(object sender, KeyEventArgs e)
    {
        TextBox tb = (TextBox)sender;
        int i = (int)tb.Tag;

        Keys vk = e.KeyCode;
        if (!BindInProgress && vk != Keys.None && vk != (Keys)255 &&
            vk != Keys.LWin && vk != Keys.RWin)
        {
            OldHotkey = HotkeyConf.Hotkeys[i];
            HotkeyConf.Hotkeys[i].Modifiers = 0;
            BindInProgress = true;
            tb.Clear();
        }

        switch (e.KeyCode)
        {
            case Keys.None:
            case (Keys)255:
            case Keys.LWin:
            case Keys.RWin:
                // ignore Windows key and unknown/extended keys
                break;
            case Keys.LMenu:
            case Keys.RMenu:
            case Keys.Menu:
                if ((HotkeyConf.Hotkeys[i].Modifiers & HotkeyModifiers.Alt) != HotkeyModifiers.Alt)
                {
                    HotkeyConf.Hotkeys[i].Modifiers |= HotkeyModifiers.Alt;
                    tb.Text = HotkeyText(HotkeyConf.Hotkeys[i].Modifiers);
                }
                break;
            case Keys.LControlKey:
            case Keys.RControlKey:
            case Keys.ControlKey:
                if ((HotkeyConf.Hotkeys[i].Modifiers & HotkeyModifiers.Ctrl) != HotkeyModifiers.Ctrl)
                {
                    HotkeyConf.Hotkeys[i].Modifiers |= HotkeyModifiers.Ctrl;
                    tb.Text = HotkeyText(HotkeyConf.Hotkeys[i].Modifiers);
                }
                break;
            case Keys.LShiftKey:
            case Keys.RShiftKey:
            case Keys.ShiftKey:
                if ((HotkeyConf.Hotkeys[i].Modifiers & HotkeyModifiers.Shift) != HotkeyModifiers.Shift)
                {
                    HotkeyConf.Hotkeys[i].Modifiers |= HotkeyModifiers.Shift;
                    tb.Text = HotkeyText(HotkeyConf.Hotkeys[i].Modifiers);
                }
                break;
            default:
                tb.Text = HotkeyText(HotkeyConf.Hotkeys[i].Modifiers, e.KeyCode);
                HotkeyConf.Hotkeys[i].KeyCode = e.KeyCode;
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
                if ((HotkeyConf.Hotkeys[i].Modifiers & HotkeyModifiers.Alt) == HotkeyModifiers.Alt)
                {
                    HotkeyConf.Hotkeys[i].Modifiers &= ~HotkeyModifiers.Alt;
                }
                break;
            case Keys.LControlKey:
            case Keys.RControlKey:
            case Keys.ControlKey:
                if ((HotkeyConf.Hotkeys[i].Modifiers & HotkeyModifiers.Ctrl) == HotkeyModifiers.Ctrl)
                {
                    HotkeyConf.Hotkeys[i].Modifiers &= ~HotkeyModifiers.Ctrl;
                }
                break;
            case Keys.LShiftKey:
            case Keys.RShiftKey:
            case Keys.ShiftKey:
                if ((HotkeyConf.Hotkeys[i].Modifiers & HotkeyModifiers.Shift) == HotkeyModifiers.Shift)
                {
                    HotkeyConf.Hotkeys[i].Modifiers &= ~HotkeyModifiers.Shift;
                }
                break;
        }
        tb.Text = HotkeyText(HotkeyConf.Hotkeys[i].Modifiers);
    }

    private ComboBox ActionComboBox(int tag, float scale, HotkeyAction action)
    {
        ComboBox cb = new()
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding((int)(2 * scale)),
            Tag = tag,
        };
        cb.Items.AddRange(
        [
            "None",
            "Open config editor",
            "Toggle Full Blast",
            "Toggle Win/Fn swap",
            "Increase keyboard backlight",
            "Decrease keyboard backlight",
            "Switch fan profiles",
            "Switch performance modes",
        ]);
        cb.SelectedIndexChanged += new EventHandler(ActionChanged);
        cb.SelectedIndex = (int)action;
        return cb;
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
        HotkeyConf.Hotkeys.Insert((int)b.Tag + 1, new Hotkey());
        ReloadHotkeys();
    }

    private void ActionDel(object sender, EventArgs e)
    {
        Button b = (Button)sender;
        HotkeyConf.Hotkeys.RemoveAt((int)b.Tag);
        ReloadHotkeys();
    }

    private void ActionChanged(object sender, EventArgs e)
    {
        ComboBox actionCb = (ComboBox)sender;
        int i = (int)actionCb.Tag;

        ComboBox dataCb = cboActionDatas[i];
        Hotkey hk = HotkeyConf.Hotkeys[i];

        hk.Action = (HotkeyAction)actionCb.SelectedIndex;

        dataCb.Enabled = false;
        dataCb.Items.Clear();
        switch (actionCb.SelectedIndex)
        {
            case 6: // switch fan profiles
                dataCb.Items.Add("<next fan profile>");
                foreach (FanCurveConf cfg in Config.FanConfs[0].FanCurveConfs)
                {
                    dataCb.Items.Add(cfg.Name);
                }
                dataCb.Enabled = true;
                break;
            case 7: // switch perf. modes
                dataCb.Items.Add("<next perf. mode>");
                foreach (PerfMode pm in Config.PerfModeConf.PerfModes)
                {
                    dataCb.Items.Add(pm.Name);
                }
                dataCb.Enabled = true;
                break;
        }

        if (dataCb.Items.Count > 0)
        {
            if (hk.ActionData + 1 < dataCb.Items.Count)
            {
                dataCb.SelectedIndex = hk.ActionData + 1;
            }
            else
            {
                dataCb.SelectedIndex = dataCb.Items.Count - 1;
                hk.ActionData = dataCb.SelectedIndex - 1;
            }
        }
    }

    private void ActionDataChanged(object sender, EventArgs e)
    {
        ComboBox cb = (ComboBox)sender;
        HotkeyConf.Hotkeys[(int)cb.Tag].ActionData = cb.SelectedIndex - 1;
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
