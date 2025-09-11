// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 and Contributors 2025.
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

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using YAMDCC.Common;
using YAMDCC.Common.Configs;
using YAMDCC.Common.Dialogs;
using YAMDCC.HotkeyHandler.Config;
using YAMDCC.HotkeyHandler.Win32;
using YAMDCC.IPC;

namespace YAMDCC.HotkeyHandler;

internal sealed partial class MainForm : Form
{
    private readonly NamedPipeClient<ServiceResponse, ServiceCommand> IPCClient =
        new("YAMDCC-Server");

    private YamdccCfg Config;
    private HotkeyConf HotkeyConf;

    private readonly List<TextBox> txtHotkeys = [];
    private readonly List<ComboBox> cboActionDatas = [];

    private Hotkey OldHotkey;
    private bool BindInProgress, Startup;

    /// <summary>
    /// The set of hotkey IDs currently registered with Windows
    /// (using the <see cref="User32.RegisterHotKey"/> function).
    /// </summary>
    private readonly Dictionary<int, Hotkey> RegisteredKeys = [];

    /// <summary>
    /// Temporary variable to control whether the keyboard backlight brightness
    /// should be increased or decreased in brightness when receiving a
    /// <see cref="Response.KeyLightBright"/> message.
    /// </summary>
    private bool KeyLightUp;

    private readonly ToolTip ttMain = new();

    public MainForm(bool startup)
    {
        Startup = startup;
        InitializeComponent();

        TrayIcon.Text = Text = $"YAMDCC hotkey handler - v{Utils.GetVerString()}";
        Icon = Utils.GetEntryAssemblyIcon();
        TrayIcon.Icon = Icon;

        #region Set tooltips
        ttMain.SetToolTip(btnApply, Strings.GetString("ttApply"));
        ttMain.SetToolTip(btnRevert, Strings.GetString("ttRevert"));
        tsiExit.ToolTipText = tsiTrayExit.ToolTipText = Strings.GetString("ttExit");
        tsiEnabled.ToolTipText = Strings.GetString("ttEnabled");
        tsiTrayMin.ToolTipText = Strings.GetString("ttTrayMin");
        tsiTrayClose.ToolTipText = Strings.GetString("ttTrayClose");
        #endregion // Set tooltips

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
            throw new TimeoutException(Strings.GetString("exSvcTimeout"));
        }
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

        LoadHotkeyConf();
    }

    private void LoadHotkeyConf()
    {
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
        RefreshHotkeyUI();
    }

    private void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        IPCClient.Stop();
    }

    protected override void SetVisibleCore(bool value)
    {
        // hide form on first launch
        if (Startup && !IsHandleCreated)
        {
            value = false;
            CreateHandle();
        }
        base.SetVisibleCore(value);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RegisterHotkeys();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)  
    {
        base.OnFormClosing(e);
        if (e.CloseReason == CloseReason.UserClosing && tsiTrayClose.Checked)
        {
            Hide();
            e.Cancel = true;
        }
        UnregisterHotkeys();
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
            case 0x312: // WM_HOTKEY
                // only handle shortcuts if hotkeys are
                // enabled and the config window isn't open
                if (tsiEnabled.Checked && !Visible)
                {
                    foreach (int i in RegisteredKeys.Keys)
                    {
                        if (m.WParam.ToInt32() == i)
                        {
                            RunHotkeyAction(RegisteredKeys[i]);
                        }
                    }
                }
                break;
        }
        base.WndProc(ref m);
    }

    private void IPCMessage(object sender, PipeMessageEventArgs<ServiceResponse, ServiceCommand> e)
    {
        switch (e.Message.Response)
        {
            case Response.ConfLoaded:
            {
                // refresh hotkey handler if the current YAMDCC config is reloaded
                // (i.e. service restarted, user updated config...)
                if (e.Message.Value[0] is int i && i == IPCClient.Connection.ID)
                {
                    return;
                }
                RefreshHotkeyUI();
                break;
            }
            case Response.KeyLightBright:
            {
                // this message should only be received if the keyboard
                // backlight adjustment shortcut is pressed
                // (see RunHotkeyAction() for more)
                object[] value = e.Message.Value;
                if (value.Length > 0 && value[0] is int i)
                {
                    if (KeyLightUp && i < 4)
                    {
                        IPCClient.PushMessage(new ServiceCommand(
                            Command.SetKeyLightBright, (byte)(i + 1)));
                    }
                    else if (!KeyLightUp && i > 0)
                    {
                        IPCClient.PushMessage(new ServiceCommand(
                            Command.SetKeyLightBright, (byte)(i - 1)));
                    }
                }
                break;
            }
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

    private void ShowHotkeyConfig(object sender, EventArgs e)
    {
        if (Visible)
        {
            Hide();
        }
        else
        {
            Show();
            Activate();
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

    private void RefreshHotkeyUI()
    {
        // load currently applied YAMDCC config
        // (used to populate fan profiles/performance modes)
        Config = YamdccCfg.Load(Paths.CurrentConfV2);

        if (HotkeyConf.Hotkeys.Count == 0)
        {
            HotkeyConf.Hotkeys.Add(new Hotkey());
        }

        float scale = AutoScaleDimensions.Width / 96;

        cboActionDatas.Clear();
        txtHotkeys.Clear();
        
        tblHotkeys.AutoScroll = false;
        tblHotkeys.Padding = new Padding(0, 0, SystemInformation.VerticalScrollBarWidth, 0);
        tblHotkeys.SuspendLayout();
        tblHotkeys.Controls.Clear();
        tblHotkeys.RowCount = HotkeyConf.Hotkeys.Count;

        for (int i = 0; i < HotkeyConf.Hotkeys.Count; i++)
        {
            Hotkey hk = HotkeyConf.Hotkeys[i];

            cboActionDatas.Add(new ComboBox()
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false,
                Margin = new Padding((int)(2 * scale)),
                TabIndex = i * 4 + 1,
                Tag = i,
            });
            cboActionDatas[i].SelectedIndexChanged += new EventHandler(ActionDataChanged);
            tblHotkeys.Controls.Add(cboActionDatas[i], 1, i);

            tblHotkeys.Controls.Add(ActionComboBox(i, scale, hk.Action), 0, i);

            txtHotkeys.Add(new TextBox
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.DimGray,
                Margin = new Padding((int)(2 * scale)),
                TabIndex = i * 4 + 2,
                Tag = i,
                Text = "Click to bind a hotkey...",
            });
            string hkText = HotkeyText(hk.Modifiers, hk.KeyCode);
            if (!string.IsNullOrEmpty(hkText))
            {
                txtHotkeys[i].ForeColor = Color.Black;
                txtHotkeys[i].Text = hkText;
            }
            txtHotkeys[i].Enter += new EventHandler(KeyBindEnter);
            txtHotkeys[i].Leave += new EventHandler(KeyBindLeave);
            txtHotkeys[i].KeyDown += new KeyEventHandler(KeyBindDown);
            txtHotkeys[i].KeyUp += new KeyEventHandler(KeyBindUp);
            tblHotkeys.Controls.Add(txtHotkeys[i], 2, i);

            tblHotkeys.Controls.Add(HotkeyButton(i, false, scale), 3, i);
            tblHotkeys.Controls.Add(HotkeyButton(i, true, scale), 4, i);
        }
        tblHotkeys.ResumeLayout(true);
        tblHotkeys.AutoScroll = true;
        tblHotkeys.Padding = new Padding(0);
    }

    private void KeyBindEnter(object sender, EventArgs e)
    {
        TextBox tb = (TextBox)sender;
        tb.Text = string.Empty;
        tb.ForeColor = Color.Black;
    }

    private void KeyBindLeave(object sender, EventArgs e)
    {
        TextBox tb = (TextBox)sender;
        if (BindInProgress)
        {
            HotkeyConf.Hotkeys[(int)tb.Tag] = OldHotkey;
            BindInProgress = false;
        }

        Hotkey hk = HotkeyConf.Hotkeys[(int)tb.Tag];
        if (hk.KeyCode == Keys.None && hk.Modifiers == HotkeyModifiers.None)
        {
            tb.ForeColor = Color.DimGray;
            tb.Text = "Click to bind a hotkey...";
            return;
        }
    }

    private void KeyBindDown(object sender, KeyEventArgs e)
    {
        TextBox tb = (TextBox)sender;
        int i = (int)tb.Tag;

        e.SuppressKeyPress = true;

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
            TabIndex = tag * 4,
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
            "Switch default perf. modes",
        ]);
        cb.SelectedIndexChanged += new EventHandler(ActionChanged);
        cb.SelectedIndex = (int)action;
        ttMain.SetToolTip(cb, Strings.GetString("ttHkAction"));
        return cb;
    }

    private Button HotkeyButton(int tag, bool del, float scale)
    {
        Button b = new()
        {
            Margin = new Padding((int)(2 * scale)),
            Size = new Size((int)(23 * scale), (int)(23 * scale)),
            TabIndex = del ? tag * 4 + 4 : tag * 4 + 3,
            Tag = tag,
            Text = del ? "-" : "+",
        };
        b.Click += del ? new EventHandler(ActionDel) : new EventHandler(ActionAdd);
        ttMain.SetToolTip(b, del
            ? Strings.GetString("ttHkDel")
            : Strings.GetString("ttHkAdd"));
        return b;
    }

    private void ActionAdd(object sender, EventArgs e)
    {
        Button b = (Button)sender;
        HotkeyConf.Hotkeys.Insert((int)b.Tag + 1, new Hotkey());
        RefreshHotkeyUI();
    }

    private void ActionDel(object sender, EventArgs e)
    {
        Button b = (Button)sender;
        HotkeyConf.Hotkeys.RemoveAt((int)b.Tag);
        RefreshHotkeyUI();
    }

    private void ActionChanged(object sender, EventArgs e)
    {
        ComboBox actionCb = (ComboBox)sender;
        int i = (int)actionCb.Tag;

        ComboBox dataCb = cboActionDatas[i];
        Hotkey hk = HotkeyConf.Hotkeys[i];

        hk.Action = (HotkeyAction)actionCb.SelectedIndex;

        dataCb.Items.Clear();
        switch (actionCb.SelectedIndex)
        {
            case 6: // switch fan profiles
                dataCb.Items.Add("<next fan profile>");
                foreach (FanProf cfg in Config.CpuFan.FanProfs)
                {
                    dataCb.Items.Add(cfg.Name);
                }
                ttMain.SetToolTip(dataCb, Strings.GetString("ttHkFanProf"));
                dataCb.Enabled = true;
                break;
            case 7: // switch perf. modes
                dataCb.Items.Add("<next perf. mode>");
                foreach (PerfMode pm in Enum.GetValues(typeof(PerfMode)))
                {
                    dataCb.Items.Add(pm);
                }
                ttMain.SetToolTip(dataCb, Strings.GetString("ttHkPerfMode"));
                dataCb.Enabled = true;
                break;
            default:
                ttMain.SetToolTip(dataCb, string.Empty);
                dataCb.Enabled = false;
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

    private void RegisterHotkeys()
    {
        // unregister old hotkeys (if they exist) first
        UnregisterHotkeys();

        // register new hotkeys
        for (int i = 0; i < HotkeyConf.Hotkeys.Count; i++)
        {
            // ignore key repeats for held hotkey (that's what the 0x4000 is for)
            Hotkey hk = HotkeyConf.Hotkeys[i];
            if (hk.Action != HotkeyAction.None)
            {
                User32.RegisterHotKey(Handle, i, (uint)hk.Modifiers + 0x4000, (uint)hk.KeyCode);
                RegisteredKeys.Add(i, hk);
            }
        }
    }

    private void UnregisterHotkeys()
    {
        foreach (int i in RegisteredKeys.Keys)
        {
            User32.UnregisterHotKey(Handle, i);
        }
        RegisteredKeys.Clear();
    }

    private void RunHotkeyAction(Hotkey hk)
    {
        switch (hk.Action)
        {
            case HotkeyAction.OpenConfEditor:
                string pathCE = Path.GetFullPath(@".\ConfigEditor.exe");
                try
                {
                    Process.Start(pathCE);
                }
                catch (Win32Exception ex)
                {
                    Utils.ShowError(
                        $"Failed to open Config Editor\n" +
                        $"(located at: {pathCE}):\n" +
                        $"{ex.Message} ({ex.NativeErrorCode})");
                }
                break;
            case HotkeyAction.ToggleFullBlast:
                IPCClient.PushMessage(new ServiceCommand(Command.SetFullBlast, -1));
                break;
            case HotkeyAction.ToggleWinFnSwap:
                IPCClient.PushMessage(new ServiceCommand(Command.SetKeySwap, -1));
                break;
            case HotkeyAction.KeyLightUp:
                // Get the current keyboard backlight brightness.
                // The IPCMessage function handles the actual keyboard backlight setting.
                KeyLightUp = true;
                IPCClient.PushMessage(new ServiceCommand(Command.GetKeyLightBright));
                break;
            case HotkeyAction.KeyLightDown:
                KeyLightUp = false;
                IPCClient.PushMessage(new ServiceCommand(Command.GetKeyLightBright));
                break;
            case HotkeyAction.SwitchFanProf:
                IPCClient.PushMessage(new ServiceCommand(Command.SetFanProf, hk.ActionData));
                break;
            case HotkeyAction.SwitchPerfMode:
                IPCClient.PushMessage(new ServiceCommand(Command.SetPerfMode, hk.ActionData));
                break;
        }
    }

    private void btnApply_Click(object sender, EventArgs e)
    {
        HotkeyConf.Save(Paths.HotkeyConf);
        RegisterHotkeys();
    }

    private void btnRevert_Click(object sender, EventArgs e)
    {
        LoadHotkeyConf();
    }
}
