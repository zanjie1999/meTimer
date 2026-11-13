using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

[assembly: System.Reflection.AssemblyTitle("咩计时")]
[assembly: System.Reflection.AssemblyProduct("咩计时")]
[assembly: System.Reflection.AssemblyDescription("导播演讲计时工具")]
[assembly: System.Reflection.AssemblyCompany("郑羊羊")]
[assembly: System.Reflection.AssemblyVersion("1.0.0.0")]
[assembly: System.Reflection.AssemblyFileVersion("1.0.0.0")]

namespace MeTimer
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        private static void Main()
        {
            try
            {
                SetProcessDPIAware();
            }
            catch
            {
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Dpi.Initialize();
            Application.Run(new ControlForm());
        }
    }

    internal static class Dpi
    {
        [DllImport("user32.dll")]
        private static extern int GetDpiForSystem();

        private static decimal _scale = 1M;

        public static void Initialize()
        {
            int dpi = 96;

            try
            {
                dpi = GetDpiForSystem();
            }
            catch
            {
                try
                {
                    using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        dpi = (int)Math.Round(graphics.DpiX);
                    }
                }
                catch
                {
                    dpi = 96;
                }
            }

            if (dpi < 96)
            {
                dpi = 96;
            }

            _scale = dpi / 96M;
        }

        public static int S(int designPixels)
        {
            return (int)Math.Round(designPixels * _scale, MidpointRounding.AwayFromZero);
        }

        public static Padding Pad(int left, int top, int right, int bottom)
        {
            return new Padding(S(left), S(top), S(right), S(bottom));
        }
    }

    internal static class NativeMethods
    {
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_NOOWNERZORDER = 0x0200;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            uint uFlags);
    }

    internal enum TimerMode
    {
        Countdown,
        CountUp
    }

    internal enum TimerPhase
    {
        Ready,
        Running,
        Overtime,
        Stopped
    }

    internal static class TimeText
    {
        public static string Format(long totalSeconds)
        {
            if (totalSeconds < 0)
            {
                totalSeconds = 0;
            }

            long hours = totalSeconds / 3600;
            long minutes = (totalSeconds % 3600) / 60;
            long seconds = totalSeconds % 60;

            return hours.ToString("00", CultureInfo.InvariantCulture)
                + ":" + minutes.ToString("00", CultureInfo.InvariantCulture)
                + ":" + seconds.ToString("00", CultureInfo.InvariantCulture);
        }

        public static string FormatCountdownRemaining(double remainingSeconds)
        {
            if (remainingSeconds < 0)
            {
                remainingSeconds = 0;
            }

            return Format((long)Math.Ceiling(remainingSeconds));
        }

        public static string FormatClock(long totalSeconds, bool alwaysShowHours)
        {
            if (totalSeconds < 0)
            {
                totalSeconds = 0;
            }

            if (alwaysShowHours || totalSeconds >= 3600)
            {
                return Format(totalSeconds);
            }

            long minutes = totalSeconds / 60;
            long seconds = totalSeconds % 60;

            return minutes.ToString("00", CultureInfo.InvariantCulture)
                + ":" + seconds.ToString("00", CultureInfo.InvariantCulture);
        }

        public static string FormatCountdownClock(double remainingSeconds, bool alwaysShowHours)
        {
            if (remainingSeconds < 0)
            {
                remainingSeconds = 0;
            }

            return FormatClock((long)Math.Ceiling(remainingSeconds), alwaysShowHours);
        }
    }

    internal sealed class TimerRecord
    {
        public int Serial;
        public string StartTime;
        public int? PlannedSeconds;
        public long ActualSeconds;
        public long OvertimeSeconds;

        public string PlannedText
        {
            get
            {
                return PlannedSeconds.HasValue ? TimeText.Format(PlannedSeconds.Value) : "—";
            }
        }
    }

    internal static class UiTheme
    {
        public static readonly Color PageBack = Color.FromArgb(243, 245, 248);
        public static readonly Color Surface = Color.White;
        public static readonly Color Border = Color.FromArgb(219, 224, 231);
        public static readonly Color Text = Color.FromArgb(32, 39, 48);
        public static readonly Color Muted = Color.FromArgb(105, 115, 128);
        public static readonly Color TimerBack = Color.Black;
        public static readonly Color TimerFore = Color.FromArgb(249, 250, 252);
        public static readonly Color TimerMuted = Color.FromArgb(174, 186, 201);
        public static readonly Color AlertBack = Color.FromArgb(190, 43, 43);
        public static readonly Color AlertFore = Color.White;
        public static readonly Color PausedBack = Color.FromArgb(202, 101, 13);
        public static readonly Color PausedFore = Color.White;
        public static readonly Color Primary = Color.FromArgb(34, 103, 207);
        public static readonly Color PrimaryHover = Color.FromArgb(27, 86, 176);
        public static readonly Color ToggleOnBack = Color.FromArgb(15, 118, 110);
        public static readonly Color ToggleOnHover = Color.FromArgb(13, 98, 91);
        public static readonly Color Danger = Color.FromArgb(190, 50, 50);
        public static readonly Color DangerHover = Color.FromArgb(160, 36, 36);
        public static readonly Color DisabledBack = Color.FromArgb(229, 233, 239);
        public static readonly Color DisabledFore = Color.FromArgb(137, 147, 160);
        public static readonly Color SoftButtonHover = Color.FromArgb(235, 242, 252);

        public static readonly Font BodyFont = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular);
        public static readonly Font SmallFont = new Font("Microsoft YaHei UI", 8.25F, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Microsoft YaHei UI", 9.5F, FontStyle.Bold);
        public static readonly Font TitleFont = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold);
        public static readonly Font SectionFont = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold);
        public static readonly Font TimerFont = new Font("Consolas", 34F, FontStyle.Bold);
        public static readonly Font TimerStatusFont = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular);
        public static readonly Font DisplayFont = new Font("Consolas", 60F, FontStyle.Bold);
        public static readonly Font GridHeaderFont = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
        public static readonly Font DangerBoldFont = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
    }

    internal sealed class ControlForm : Form
    {
        private readonly Timer _uiTimer;
        private readonly Timer _copyFeedbackTimer;
        private readonly Stopwatch _stopwatch;
        private readonly TimerDisplayForm _display;
        private readonly List<TimerRecord> _records;

        private TableLayoutPanel _root;
        private Panel _timerCard;
        private Label _mainTimeLabel;
        private Label _timerStatusLabel;
        private Label _sessionInfoLabel;
        private Label _inputErrorLabel;
        private RadioButton _countdownRadio;
        private RadioButton _countUpRadio;
        private NumericUpDown _hoursInput;
        private NumericUpDown _minutesInput;
        private NumericUpDown _secondsInput;
        private CheckBox _showHoursCheck;
        private Button _startButton;
        private Button _stopButton;
        private Button _toggleDisplayButton;
        private Button _moveDisplayButton;
        private Button _copyRecordsButton;
        private Label _displayStateLabel;
        private DataGridView _recordsGrid;
        private Label _recordCountLabel;

        private TimerMode _mode;
        private TimerMode _sessionMode;
        private TimerPhase _phase;
        private int _sessionPlannedSeconds;
        private DateTime _sessionStartedAt;
        private long _stoppedMainSeconds;
        private long _stoppedDisplaySeconds;
        private string _stoppedStatusText;
        private int _nextSerial;
        private bool _initializing;
        private bool _closeConfirmed;
        private bool _paused;
        private bool _showHours;

        public ControlForm()
        {
            _records = new List<TimerRecord>();
            _display = new TimerDisplayForm();
            _stopwatch = new Stopwatch();
            _uiTimer = new Timer();
            _copyFeedbackTimer = new Timer();
            _copyFeedbackTimer.Interval = 1600;
            _copyFeedbackTimer.Tick += CopyFeedbackTimer_Tick;
            _mode = TimerMode.Countdown;
            _phase = TimerPhase.Ready;
            _nextSerial = 1;
            _initializing = true;

            AutoScaleMode = AutoScaleMode.None;
            Font = UiTheme.BodyFont;
            BackColor = UiTheme.PageBack;
            ClientSize = new Size(Dpi.S(820), Dpi.S(520));
            MinimumSize = new Size(Dpi.S(720), Dpi.S(480));
            StartPosition = FormStartPosition.Manual;
            Text = "咩计时";
            DoubleBuffered = true;

            try
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch
            {
            }

            BuildInterface();
            WireEvents();
            _initializing = false;

            RefreshInteractiveState();
            UpdateSessionInfo();
            RefreshTimerUi(true);
            UpdateCopyButtonState();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CenterOnPrimaryScreen();
            AdjustGridForDpi();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ShowTimerWindow(true);
            Activate();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!_closeConfirmed && e.CloseReason == CloseReason.UserClosing)
            {
                bool active = _phase == TimerPhase.Running || _phase == TimerPhase.Overtime;
                string message = "确定要关闭「咩计时」吗？";

                if (active)
                {
                    message = "当前计时尚未停止，关闭后本轮不会保存记录。\n\n" + message;
                }
                else if (_records.Count > 0)
                {
                    message = "本次计时记录尚未保存，关闭后将全部丢失。\n\n" + message;
                }

                DialogResult result = MessageBox.Show(
                    this,
                    message,
                    "退出咩计时",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }

            _closeConfirmed = true;
            _uiTimer.Stop();
            _copyFeedbackTimer.Stop();
            _stopwatch.Stop();

            if (_display != null && !_display.IsDisposed)
            {
                _display.PrepareForClose();
                _display.Close();
            }

            base.OnFormClosing(e);
        }

        private void BuildInterface()
        {
            SuspendLayout();

            _root = new TableLayoutPanel();
            _root.Dock = DockStyle.Fill;
            _root.BackColor = UiTheme.PageBack;
            _root.Padding = Dpi.Pad(14, 10, 14, 12);
            _root.ColumnCount = 1;
            _root.RowCount = 5;
            _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _root.RowStyles.Add(new RowStyle(SizeType.Absolute, Dpi.S(140)));
            _root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _root.Controls.Add(BuildTimerCard(), 0, 0);
            _root.Controls.Add(BuildSessionInfo(), 0, 1);
            _root.Controls.Add(BuildSettingsCard(), 0, 2);
            _root.Controls.Add(BuildDisplayControls(), 0, 3);
            _root.Controls.Add(BuildRecordsPanel(), 0, 4);

            Controls.Add(_root);
            ResumeLayout(true);
        }

        private Control BuildTimerCard()
        {
            _timerCard = new Panel();
            _timerCard.Dock = DockStyle.Fill;
            _timerCard.BackColor = UiTheme.TimerBack;
            _timerCard.Margin = Dpi.Pad(0, 0, 0, 3);
            _timerCard.Padding = Dpi.Pad(12, 5, 12, 4);

            TableLayoutPanel cardLayout = new TableLayoutPanel();
            cardLayout.Dock = DockStyle.Fill;
            cardLayout.BackColor = Color.Transparent;
            cardLayout.ColumnCount = 1;
            cardLayout.RowCount = 2;
            cardLayout.Margin = new Padding(0);
            cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, Dpi.S(26)));

            _mainTimeLabel = new Label();
            _mainTimeLabel.Dock = DockStyle.Fill;
            _mainTimeLabel.AutoSize = false;
            _mainTimeLabel.Font = UiTheme.TimerFont;
            _mainTimeLabel.ForeColor = UiTheme.TimerFore;
            _mainTimeLabel.BackColor = Color.Transparent;
            _mainTimeLabel.TextAlign = ContentAlignment.MiddleCenter;
            _mainTimeLabel.Margin = new Padding(0);
            _mainTimeLabel.Text = "00:00:00";

            _timerStatusLabel = new Label();
            _timerStatusLabel.Dock = DockStyle.Fill;
            _timerStatusLabel.AutoSize = false;
            _timerStatusLabel.Font = UiTheme.TimerStatusFont;
            _timerStatusLabel.ForeColor = UiTheme.TimerMuted;
            _timerStatusLabel.BackColor = Color.Transparent;
            _timerStatusLabel.TextAlign = ContentAlignment.MiddleCenter;
            _timerStatusLabel.Margin = new Padding(0);
            _timerStatusLabel.Text = "准备就绪";

            cardLayout.Controls.Add(_mainTimeLabel, 0, 0);
            cardLayout.Controls.Add(_timerStatusLabel, 0, 1);
            _timerCard.Controls.Add(cardLayout);
            return _timerCard;
        }

        private Control BuildSessionInfo()
        {
            TableLayoutPanel row = new TableLayoutPanel();
            row.Dock = DockStyle.Top;
            row.AutoSize = true;
            row.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            row.ColumnCount = 2;
            row.RowCount = 1;
            row.BackColor = UiTheme.PageBack;
            row.Margin = Dpi.Pad(2, 0, 0, 2);
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            row.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _sessionInfoLabel = new Label();
            _sessionInfoLabel.AutoSize = false;
            _sessionInfoLabel.Dock = DockStyle.Fill;
            _sessionInfoLabel.Height = Dpi.S(22);
            _sessionInfoLabel.Font = UiTheme.SmallFont;
            _sessionInfoLabel.ForeColor = UiTheme.Muted;
            _sessionInfoLabel.TextAlign = ContentAlignment.MiddleLeft;
            _sessionInfoLabel.AutoEllipsis = true;
            _sessionInfoLabel.Margin = new Padding(0);
            _sessionInfoLabel.Text = "等待开始";

            _showHoursCheck = new CheckBox();
            _showHoursCheck.AutoSize = true;
            _showHoursCheck.Text = "显示小时";
            _showHoursCheck.Checked = false;
            _showHoursCheck.Font = UiTheme.SmallFont;
            _showHoursCheck.ForeColor = UiTheme.Muted;
            _showHoursCheck.Anchor = AnchorStyles.Right;
            _showHoursCheck.Margin = Dpi.Pad(8, 0, 2, 0);
            _showHoursCheck.AccessibleDescription = "勾选后计时时间始终显示小时；默认在不足 1 小时时显示分:秒。";

            row.Controls.Add(_sessionInfoLabel, 0, 0);
            row.Controls.Add(_showHoursCheck, 1, 0);
            return row;
        }

        private Control BuildSettingsCard()
        {
            TableLayoutPanel card = new TableLayoutPanel();
            card.Dock = DockStyle.Top;
            card.AutoSize = true;
            card.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            card.BackColor = UiTheme.Surface;
            card.ColumnCount = 1;
            card.RowCount = 2;
            card.Padding = Dpi.Pad(12, 7, 12, 6);
            card.Margin = Dpi.Pad(0, 0, 0, 4);
            card.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            card.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            card.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            TableLayoutPanel setupRow = new TableLayoutPanel();
            setupRow.Dock = DockStyle.Top;
            setupRow.AutoSize = true;
            setupRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            setupRow.ColumnCount = 2;
            setupRow.RowCount = 1;
            setupRow.Margin = new Padding(0);
            setupRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            setupRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            FlowLayoutPanel optionsFlow = new FlowLayoutPanel();
            optionsFlow.Dock = DockStyle.Fill;
            optionsFlow.AutoSize = true;
            optionsFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            optionsFlow.WrapContents = false;
            optionsFlow.FlowDirection = FlowDirection.LeftToRight;
            optionsFlow.Margin = new Padding(0);
            optionsFlow.Padding = new Padding(0);

            Label modeLabel = MakeFieldLabel("模式");
            modeLabel.Margin = Dpi.Pad(0, 6, 6, 0);

            _countdownRadio = new RadioButton();
            _countdownRadio.AutoSize = true;
            _countdownRadio.Text = "倒计时";
            _countdownRadio.Checked = true;
            _countdownRadio.Font = UiTheme.BodyFont;
            _countdownRadio.ForeColor = UiTheme.Text;
            _countdownRadio.Margin = Dpi.Pad(0, 4, 9, 0);

            _countUpRadio = new RadioButton();
            _countUpRadio.AutoSize = true;
            _countUpRadio.Text = "顺计时";
            _countUpRadio.Font = UiTheme.BodyFont;
            _countUpRadio.ForeColor = UiTheme.Text;
            _countUpRadio.Margin = Dpi.Pad(0, 4, 16, 0);

            Label durationLabel = MakeFieldLabel("时长");
            durationLabel.Margin = Dpi.Pad(0, 6, 6, 0);

            _hoursInput = MakeNumberInput(99, 0);
            _minutesInput = MakeNumberInput(59, 15);
            _secondsInput = MakeNumberInput(59, 0);

            optionsFlow.Controls.Add(modeLabel);
            optionsFlow.Controls.Add(_countdownRadio);
            optionsFlow.Controls.Add(_countUpRadio);
            optionsFlow.Controls.Add(durationLabel);
            optionsFlow.Controls.Add(_hoursInput);
            optionsFlow.Controls.Add(MakeUnitLabel("时"));
            optionsFlow.Controls.Add(_minutesInput);
            optionsFlow.Controls.Add(MakeUnitLabel("分"));
            optionsFlow.Controls.Add(_secondsInput);
            optionsFlow.Controls.Add(MakeUnitLabel("秒"));

            FlowLayoutPanel actionFlow = new FlowLayoutPanel();
            actionFlow.Dock = DockStyle.Fill;
            actionFlow.AutoSize = true;
            actionFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            actionFlow.WrapContents = false;
            actionFlow.FlowDirection = FlowDirection.LeftToRight;
            actionFlow.Margin = Dpi.Pad(6, 0, 0, 0);
            actionFlow.Padding = new Padding(0);

            _startButton = new Button();
            _startButton.Text = "开始计时";
            _startButton.Size = new Size(Dpi.S(96), Dpi.S(34));
            _startButton.Font = UiTheme.ButtonFont;
            _startButton.Margin = Dpi.Pad(0, 0, 6, 0);
            _startButton.TabStop = true;

            _stopButton = new Button();
            _stopButton.Text = "停止";
            _stopButton.Size = new Size(Dpi.S(80), Dpi.S(34));
            _stopButton.Font = UiTheme.ButtonFont;
            _stopButton.Margin = new Padding(0);
            _stopButton.TabStop = true;

            actionFlow.Controls.Add(_startButton);
            actionFlow.Controls.Add(_stopButton);

            setupRow.Controls.Add(optionsFlow, 0, 0);
            setupRow.Controls.Add(actionFlow, 1, 0);

            _inputErrorLabel = new Label();
            _inputErrorLabel.AutoSize = true;
            _inputErrorLabel.Text = string.Empty;
            _inputErrorLabel.Font = UiTheme.SmallFont;
            _inputErrorLabel.ForeColor = UiTheme.Danger;
            _inputErrorLabel.Margin = Dpi.Pad(2, 5, 0, 0);
            _inputErrorLabel.Visible = false;

            card.Controls.Add(setupRow, 0, 0);
            card.Controls.Add(_inputErrorLabel, 0, 1);
            return card;
        }

        private Control BuildDisplayControls()
        {
            TableLayoutPanel row = new TableLayoutPanel();
            row.Dock = DockStyle.Top;
            row.AutoSize = true;
            row.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            row.ColumnCount = 2;
            row.RowCount = 1;
            row.BackColor = UiTheme.PageBack;
            row.Margin = Dpi.Pad(0, 0, 0, 4);
            row.Padding = new Padding(0);
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            row.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            FlowLayoutPanel left = new FlowLayoutPanel();
            left.Dock = DockStyle.Fill;
            left.AutoSize = true;
            left.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            left.WrapContents = false;
            left.FlowDirection = FlowDirection.LeftToRight;
            left.BackColor = UiTheme.PageBack;
            left.Margin = new Padding(0);
            left.Padding = new Padding(0);

            _toggleDisplayButton = new Button();
            _toggleDisplayButton.Text = "隐藏计时窗";
            _toggleDisplayButton.Size = new Size(Dpi.S(112), Dpi.S(30));
            _toggleDisplayButton.Font = UiTheme.BodyFont;
            _toggleDisplayButton.Margin = Dpi.Pad(0, 0, 6, 0);

            _moveDisplayButton = new Button();
            _moveDisplayButton.Text = "移到主屏";
            _moveDisplayButton.Size = new Size(Dpi.S(88), Dpi.S(30));
            _moveDisplayButton.Font = UiTheme.BodyFont;
            _moveDisplayButton.Margin = Dpi.Pad(0, 0, 10, 0);

            _displayStateLabel = new Label();
            _displayStateLabel.AutoSize = true;
            _displayStateLabel.Font = UiTheme.SmallFont;
            _displayStateLabel.ForeColor = UiTheme.Muted;
            _displayStateLabel.Margin = Dpi.Pad(0, 7, 0, 0);
            _displayStateLabel.Text = "计时窗：显示中";

            _copyRecordsButton = new Button();
            _copyRecordsButton.Text = "复制记录到剪贴板";
            _copyRecordsButton.Size = new Size(Dpi.S(142), Dpi.S(30));
            _copyRecordsButton.Font = UiTheme.BodyFont;
            _copyRecordsButton.Margin = Dpi.Pad(8, 0, 0, 0);
            _copyRecordsButton.Anchor = AnchorStyles.Right;

            StyleSecondaryButton(_toggleDisplayButton);
            StyleSecondaryButton(_moveDisplayButton);
            StyleSecondaryButton(_copyRecordsButton);

            left.Controls.Add(_toggleDisplayButton);
            left.Controls.Add(_moveDisplayButton);
            left.Controls.Add(_displayStateLabel);
            row.Controls.Add(left, 0, 0);
            row.Controls.Add(_copyRecordsButton, 1, 0);
            return row;
        }

        private Control BuildRecordsPanel()
        {
            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.ColumnCount = 1;
            panel.RowCount = 2;
            panel.Margin = new Padding(0);
            panel.Padding = new Padding(0);
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, Dpi.S(27)));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            TableLayoutPanel header = new TableLayoutPanel();
            header.Dock = DockStyle.Fill;
            header.ColumnCount = 2;
            header.RowCount = 1;
            header.Margin = new Padding(0);
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            Label sectionLabel = new Label();
            sectionLabel.AutoSize = true;
            sectionLabel.Font = UiTheme.SectionFont;
            sectionLabel.ForeColor = UiTheme.Text;
            sectionLabel.Margin = Dpi.Pad(2, 2, 0, 0);
            sectionLabel.Text = "计时记录";

            _recordCountLabel = new Label();
            _recordCountLabel.AutoSize = true;
            _recordCountLabel.Font = UiTheme.SmallFont;
            _recordCountLabel.ForeColor = UiTheme.Muted;
            _recordCountLabel.TextAlign = ContentAlignment.MiddleRight;
            _recordCountLabel.Margin = Dpi.Pad(0, 7, 4, 0);
            _recordCountLabel.Text = "0 条";

            header.Controls.Add(sectionLabel, 0, 0);
            header.Controls.Add(_recordCountLabel, 1, 0);

            _recordsGrid = new DataGridView();
            _recordsGrid.Dock = DockStyle.Fill;
            _recordsGrid.Margin = new Padding(0);
            _recordsGrid.Name = "recordsGrid";
            _recordsGrid.AccessibleName = "计时记录";
            _recordsGrid.BackgroundColor = UiTheme.Surface;
            _recordsGrid.BorderStyle = BorderStyle.FixedSingle;
            _recordsGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            _recordsGrid.GridColor = UiTheme.Border;
            _recordsGrid.Font = UiTheme.BodyFont;
            _recordsGrid.AllowUserToAddRows = false;
            _recordsGrid.AllowUserToDeleteRows = false;
            _recordsGrid.AllowUserToOrderColumns = false;
            _recordsGrid.AllowUserToResizeRows = false;
            _recordsGrid.ReadOnly = true;
            _recordsGrid.MultiSelect = false;
            _recordsGrid.RowHeadersVisible = false;
            _recordsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _recordsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _recordsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _recordsGrid.ColumnHeadersHeight = Dpi.S(30);
            _recordsGrid.RowTemplate.Height = Dpi.S(30);
            _recordsGrid.EnableHeadersVisualStyles = false;
            _recordsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(234, 238, 244);
            _recordsGrid.ColumnHeadersDefaultCellStyle.ForeColor = UiTheme.Text;
            _recordsGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(234, 238, 244);
            _recordsGrid.ColumnHeadersDefaultCellStyle.Font = UiTheme.GridHeaderFont;
            _recordsGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _recordsGrid.DefaultCellStyle.BackColor = UiTheme.Surface;
            _recordsGrid.DefaultCellStyle.ForeColor = UiTheme.Text;
            _recordsGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 237, 255);
            _recordsGrid.DefaultCellStyle.SelectionForeColor = UiTheme.Text;
            _recordsGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _recordsGrid.DefaultCellStyle.Padding = Dpi.Pad(2, 0, 2, 0);

            DataGridViewTextBoxColumn serialColumn = new DataGridViewTextBoxColumn();
            serialColumn.Name = "serialColumn";
            serialColumn.HeaderText = "序号";
            serialColumn.FillWeight = 12F;
            serialColumn.MinimumWidth = Dpi.S(44);
            serialColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

            DataGridViewTextBoxColumn startColumn = new DataGridViewTextBoxColumn();
            startColumn.Name = "startColumn";
            startColumn.HeaderText = "开始时间";
            startColumn.FillWeight = 20F;
            startColumn.MinimumWidth = Dpi.S(84);
            startColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

            DataGridViewTextBoxColumn plannedColumn = new DataGridViewTextBoxColumn();
            plannedColumn.Name = "plannedColumn";
            plannedColumn.HeaderText = "计时时长";
            plannedColumn.FillWeight = 20F;
            plannedColumn.MinimumWidth = Dpi.S(84);
            plannedColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

            DataGridViewTextBoxColumn actualColumn = new DataGridViewTextBoxColumn();
            actualColumn.Name = "actualColumn";
            actualColumn.HeaderText = "实际用时";
            actualColumn.FillWeight = 20F;
            actualColumn.MinimumWidth = Dpi.S(84);
            actualColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

            DataGridViewTextBoxColumn overtimeColumn = new DataGridViewTextBoxColumn();
            overtimeColumn.Name = "overtimeColumn";
            overtimeColumn.HeaderText = "超时时长";
            overtimeColumn.FillWeight = 20F;
            overtimeColumn.MinimumWidth = Dpi.S(84);
            overtimeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

            DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn();
            deleteColumn.Name = "deleteColumn";
            deleteColumn.HeaderText = "操作";
            deleteColumn.Text = "删除";
            deleteColumn.UseColumnTextForButtonValue = true;
            deleteColumn.FlatStyle = FlatStyle.Flat;
            deleteColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            deleteColumn.Width = Dpi.S(72);
            deleteColumn.MinimumWidth = Dpi.S(64);
            deleteColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            deleteColumn.DefaultCellStyle.BackColor = Color.FromArgb(255, 244, 244);
            deleteColumn.DefaultCellStyle.ForeColor = UiTheme.Danger;
            deleteColumn.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 229, 229);
            deleteColumn.DefaultCellStyle.SelectionForeColor = UiTheme.Danger;
            deleteColumn.DefaultCellStyle.Font = UiTheme.DangerBoldFont;

            _recordsGrid.Columns.Add(serialColumn);
            _recordsGrid.Columns.Add(startColumn);
            _recordsGrid.Columns.Add(plannedColumn);
            _recordsGrid.Columns.Add(actualColumn);
            _recordsGrid.Columns.Add(overtimeColumn);
            _recordsGrid.Columns.Add(deleteColumn);

            panel.Controls.Add(header, 0, 0);
            panel.Controls.Add(_recordsGrid, 0, 1);
            return panel;
        }

        private static Label MakeFieldLabel(string text)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Text = text;
            label.Font = UiTheme.BodyFont;
            label.ForeColor = UiTheme.Muted;
            label.Margin = new Padding(0);
            return label;
        }

        private static Label MakeUnitLabel(string text)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Text = text;
            label.Font = UiTheme.SmallFont;
            label.ForeColor = UiTheme.Muted;
            label.Margin = Dpi.Pad(3, 6, 9, 0);
            return label;
        }

        private static NumericUpDown MakeNumberInput(int maximum, int value)
        {
            NumericUpDown input = new NumericUpDown();
            input.Minimum = 0;
            input.Maximum = maximum;
            input.Value = value;
            input.Width = Dpi.S(50);
            input.Font = UiTheme.BodyFont;
            input.TextAlign = HorizontalAlignment.Center;
            input.Margin = Dpi.Pad(0, 2, 0, 0);
            input.ThousandsSeparator = false;
            return input;
        }

        private void WireEvents()
        {
            _uiTimer.Interval = 100;
            _uiTimer.Tick += UiTimer_Tick;

            _countdownRadio.CheckedChanged += ModeRadio_CheckedChanged;
            _countUpRadio.CheckedChanged += ModeRadio_CheckedChanged;
            _hoursInput.ValueChanged += SettingInput_ValueChanged;
            _minutesInput.ValueChanged += SettingInput_ValueChanged;
            _secondsInput.ValueChanged += SettingInput_ValueChanged;
            _showHoursCheck.CheckedChanged += ShowHoursCheck_CheckedChanged;

            _startButton.Click += StartButton_Click;
            _stopButton.Click += StopButton_Click;
            _toggleDisplayButton.Click += ToggleDisplayButton_Click;
            _moveDisplayButton.Click += MoveDisplayButton_Click;
            _copyRecordsButton.Click += CopyRecordsButton_Click;

            _recordsGrid.CellContentClick += RecordsGrid_CellContentClick;
            _display.UserHidden += Display_UserHidden;
            _display.LocationChanged += Display_LocationChanged;
        }

        private void CenterOnPrimaryScreen()
        {
            Screen screen = Screen.PrimaryScreen;
            if (screen == null)
            {
                return;
            }

            Rectangle area = screen.WorkingArea;
            int x = area.Left + Math.Max(0, (area.Width - Width) / 2);
            int y = area.Top + Math.Max(0, (area.Height - Height) / 2);
            Location = new Point(x, y);
        }

        private void AdjustGridForDpi()
        {
            int rowHeight = Math.Max(Dpi.S(28), _recordsGrid.Font.Height + Dpi.S(12));
            _recordsGrid.RowTemplate.Height = rowHeight;
            _recordsGrid.ColumnHeadersHeight = Math.Max(Dpi.S(30), _recordsGrid.Font.Height + Dpi.S(14));
        }

        private void ShowTimerWindow(bool initial)
        {
            if (_display.IsDisposed)
            {
                return;
            }

            if (!_display.Visible)
            {
                if (initial)
                {
                    _display.Opacity = 0D;
                }

                _display.Show();

                if (initial)
                {
                    PositionDisplayAtDefaultLocation(FindSecondaryScreen() ?? Screen.PrimaryScreen);
                }

                _display.Opacity = 1D;
                _display.EnsureTopMost();
            }

            RefreshDisplayUi();
        }

        private Screen FindSecondaryScreen()
        {
            Screen[] screens = Screen.AllScreens;
            for (int i = 0; i < screens.Length; i++)
            {
                if (!screens[i].Primary)
                {
                    return screens[i];
                }
            }

            return null;
        }

        private Screen GetDisplayScreen()
        {
            try
            {
                if (_display.IsHandleCreated)
                {
                    return Screen.FromControl(_display);
                }
            }
            catch
            {
            }

            return Screen.PrimaryScreen;
        }

        private void PositionDisplayAtDefaultLocation(Screen screen)
        {
            if (screen == null || _display == null || _display.IsDisposed)
            {
                return;
            }

            Rectangle area = screen.WorkingArea;
            int x = Math.Max(area.Left, area.Right - _display.Width);
            int y = Math.Min(Math.Max(area.Top, area.Top + _display.Height), area.Bottom - _display.Height);
            _display.Location = new Point(x, y);
        }

        private void RefreshDisplayUi()
        {
            if (_toggleDisplayButton == null || _display == null)
            {
                return;
            }

            _toggleDisplayButton.Text = _display.Visible ? "隐藏计时窗" : "显示计时窗";
            StyleDisplayToggleButton(_display.Visible);

            Screen secondary = FindSecondaryScreen();
            Screen current = GetDisplayScreen();

            bool hasMultipleScreens = secondary != null;
            bool onSecondary = current != null && !current.Primary;

            _moveDisplayButton.Enabled = hasMultipleScreens;
            _moveDisplayButton.Text = onSecondary ? "移到主屏" : "移到副屏";

            string screenName = onSecondary ? "副屏" : "主屏";
            string state = _display.Visible ? "显示中" : "已隐藏";
            string hint = _display.Visible ? " · 可拖动/缩放" : string.Empty;
            _displayStateLabel.Text = "计时窗：" + state + " · " + screenName + hint;
        }

        private void Display_LocationChanged(object sender, EventArgs e)
        {
            RefreshDisplayUi();
        }

        private void Display_UserHidden(object sender, EventArgs e)
        {
            RefreshDisplayUi();
        }

        private void ToggleDisplayButton_Click(object sender, EventArgs e)
        {
            if (_display.Visible)
            {
                _display.Hide();
            }
            else
            {
                _display.Show();
                _display.EnsureTopMost();
            }

            RefreshDisplayUi();
        }

        private void MoveDisplayButton_Click(object sender, EventArgs e)
        {
            Screen secondary = FindSecondaryScreen();
            if (secondary == null)
            {
                return;
            }

            Screen current = GetDisplayScreen();
            Screen target = (current != null && !current.Primary) ? Screen.PrimaryScreen : secondary;
            PositionDisplayAtDefaultLocation(target);

            if (_display.Visible)
            {
                _display.EnsureTopMost();
            }

            RefreshDisplayUi();
        }

        private void CopyRecordsButton_Click(object sender, EventArgs e)
        {
            if (_records.Count == 0)
            {
                return;
            }

            string tsv = BuildRecordsTsv();

            try
            {
                Clipboard.SetText(tsv, TextDataFormat.UnicodeText);
            }
            catch (Exception)
            {
                try
                {
                    System.Threading.Thread.Sleep(80);
                    Clipboard.SetText(tsv, TextDataFormat.UnicodeText);
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        this,
                        "无法写入剪贴板，请稍后再试。",
                        "复制记录",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }

            _copyFeedbackTimer.Stop();
            _copyRecordsButton.Text = "已复制到剪贴板";
            _copyFeedbackTimer.Start();
        }

        private void CopyFeedbackTimer_Tick(object sender, EventArgs e)
        {
            _copyFeedbackTimer.Stop();
            if (_copyRecordsButton != null && !_copyRecordsButton.IsDisposed)
            {
                _copyRecordsButton.Text = "复制记录到剪贴板";
            }
        }

        private string BuildRecordsTsv()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("序号\t开始时间\t计时时长\t实际用时\t超时时长");

            for (int i = 0; i < _records.Count; i++)
            {
                TimerRecord record = _records[i];
                builder.Append("\r\n");
                builder.Append(record.Serial.ToString(CultureInfo.InvariantCulture));
                builder.Append('\t');
                builder.Append(record.StartTime);
                builder.Append('\t');
                builder.Append(record.PlannedText);
                builder.Append('\t');
                builder.Append(TimeText.Format(record.ActualSeconds));
                builder.Append('\t');
                builder.Append(TimeText.Format(record.OvertimeSeconds));
            }

            return builder.ToString();
        }

        private void UpdateCopyButtonState()
        {
            if (_copyRecordsButton == null || _copyRecordsButton.IsDisposed)
            {
                return;
            }

            bool enabled = _records.Count > 0;
            _copyRecordsButton.Enabled = enabled;
            _copyRecordsButton.Cursor = enabled ? Cursors.Hand : Cursors.Default;

            if (enabled)
            {
                StyleSecondaryButton(_copyRecordsButton);
            }
            else
            {
                _copyRecordsButton.UseVisualStyleBackColor = false;
                _copyRecordsButton.FlatStyle = FlatStyle.Flat;
                _copyRecordsButton.FlatAppearance.BorderSize = 0;
                _copyRecordsButton.BackColor = UiTheme.DisabledBack;
                _copyRecordsButton.ForeColor = UiTheme.DisabledFore;
                _copyRecordsButton.FlatAppearance.MouseOverBackColor = UiTheme.DisabledBack;
                _copyRecordsButton.FlatAppearance.MouseDownBackColor = UiTheme.DisabledBack;

                if (_copyRecordsButton.Text != "复制记录到剪贴板")
                {
                    _copyRecordsButton.Text = "复制记录到剪贴板";
                }
            }
        }

        private void ModeRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (_initializing)
            {
                return;
            }

            if (_phase == TimerPhase.Running || _phase == TimerPhase.Overtime)
            {
                return;
            }

            _mode = _countUpRadio.Checked ? TimerMode.CountUp : TimerMode.Countdown;
            ClearInputError();

            if (_phase == TimerPhase.Stopped)
            {
                _phase = TimerPhase.Ready;
            }

            RefreshInteractiveState();
            UpdateSessionInfo();
            RefreshTimerUi(true);
        }

        private void ShowHoursCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (_initializing)
            {
                return;
            }

            _showHours = _showHoursCheck.Checked;
            RefreshTimerUi(true);
        }

        private void SettingInput_ValueChanged(object sender, EventArgs e)
        {
            if (_initializing)
            {
                return;
            }

            if (_phase == TimerPhase.Running || _phase == TimerPhase.Overtime)
            {
                return;
            }

            ClearInputError();

            if (_phase == TimerPhase.Stopped)
            {
                _phase = TimerPhase.Ready;
            }

            UpdateSessionInfo();
            RefreshTimerUi(true);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (_paused)
            {
                ResumeSession();
                return;
            }

            if (_phase == TimerPhase.Running || _phase == TimerPhase.Overtime)
            {
                PauseSession();
                return;
            }

            TimerMode selectedMode = _countUpRadio.Checked ? TimerMode.CountUp : TimerMode.Countdown;
            int plannedSeconds = ReadConfiguredSeconds();

            if (selectedMode == TimerMode.Countdown && plannedSeconds <= 0)
            {
                ShowInputError("请先把倒计时时长设置为大于 0 的时间。");
                _hoursInput.Focus();
                return;
            }

            ClearInputError();
            _paused = false;
            _sessionMode = selectedMode;
            _sessionPlannedSeconds = plannedSeconds;
            _sessionStartedAt = DateTime.Now;
            _stopwatch.Reset();
            _stopwatch.Start();
            _phase = TimerPhase.Running;

            _uiTimer.Start();
            RefreshInteractiveState();
            UpdateSessionInfo();
            RefreshTimerUi(true);

            if (_display.Visible)
            {
                _display.EnsureTopMost();
            }
        }

        private void PauseSession()
        {
            if (_paused || (_phase != TimerPhase.Running && _phase != TimerPhase.Overtime))
            {
                return;
            }

            if (_phase == TimerPhase.Running
                && _sessionMode == TimerMode.Countdown
                && _stopwatch.Elapsed.TotalSeconds >= _sessionPlannedSeconds)
            {
                _phase = TimerPhase.Overtime;
            }

            _stopwatch.Stop();
            _uiTimer.Stop();
            _paused = true;

            RefreshInteractiveState();
            RefreshTimerUi(true);
        }

        private void ResumeSession()
        {
            if (!_paused)
            {
                return;
            }

            _paused = false;
            _stopwatch.Start();
            _uiTimer.Start();

            RefreshInteractiveState();
            RefreshTimerUi(true);

            if (_display.Visible)
            {
                _display.EnsureTopMost();
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (_phase != TimerPhase.Running && _phase != TimerPhase.Overtime)
            {
                return;
            }

            _stopwatch.Stop();
            _uiTimer.Stop();
            _paused = false;

            long actualSeconds = (long)Math.Floor(_stopwatch.Elapsed.TotalSeconds);
            if (actualSeconds < 0)
            {
                actualSeconds = 0;
            }

            long overtimeSeconds = 0;
            int? plannedSeconds = null;
            long stoppedMainSeconds;
            long stoppedDisplaySeconds;

            if (_sessionMode == TimerMode.Countdown)
            {
                plannedSeconds = _sessionPlannedSeconds;
                if (actualSeconds >= _sessionPlannedSeconds)
                {
                    overtimeSeconds = actualSeconds - _sessionPlannedSeconds;
                    stoppedMainSeconds = overtimeSeconds;
                    stoppedDisplaySeconds = 0;
                }
                else
                {
                    long remainingSeconds = (long)_sessionPlannedSeconds - actualSeconds;
                    stoppedMainSeconds = remainingSeconds;
                    stoppedDisplaySeconds = remainingSeconds;
                }
            }
            else
            {
                if (_sessionPlannedSeconds > 0)
                {
                    plannedSeconds = _sessionPlannedSeconds;
                    if (actualSeconds > _sessionPlannedSeconds)
                    {
                        overtimeSeconds = actualSeconds - _sessionPlannedSeconds;
                    }
                }

                stoppedMainSeconds = actualSeconds;
                stoppedDisplaySeconds = actualSeconds;
            }

            TimerRecord record = new TimerRecord();
            record.Serial = _nextSerial;
            record.StartTime = _sessionStartedAt.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            record.PlannedSeconds = plannedSeconds;
            record.ActualSeconds = actualSeconds;
            record.OvertimeSeconds = overtimeSeconds;
            _nextSerial++;
            _records.Add(record);

            _phase = TimerPhase.Stopped;
            _stoppedMainSeconds = stoppedMainSeconds;
            _stoppedDisplaySeconds = stoppedDisplaySeconds;
            _stoppedStatusText = overtimeSeconds > 0
                ? "已停止 · 本轮用时 " + TimeText.Format(actualSeconds) + " · 超时 " + TimeText.Format(overtimeSeconds)
                : "已停止 · 本轮用时 " + TimeText.Format(actualSeconds);

            RebuildRecordGrid();
            RefreshInteractiveState();
            UpdateSessionInfo();
            RefreshTimerUi(true);
        }

        private void UiTimer_Tick(object sender, EventArgs e)
        {
            if (_paused)
            {
                return;
            }

            if (_phase != TimerPhase.Running && _phase != TimerPhase.Overtime)
            {
                return;
            }

            if (_phase == TimerPhase.Running
                && _sessionMode == TimerMode.Countdown
                && _stopwatch.Elapsed.TotalSeconds >= _sessionPlannedSeconds)
            {
                _phase = TimerPhase.Overtime;
            }

            RefreshTimerUi(false);
        }

        private void RefreshTimerUi(bool force)
        {
            string mainText;
            string displayText;
            string statusText;
            bool alert = false;

            switch (_phase)
            {
                case TimerPhase.Running:
                    if (_sessionMode == TimerMode.Countdown)
                    {
                        double elapsed = _stopwatch.Elapsed.TotalSeconds;
                        double remaining = _sessionPlannedSeconds - elapsed;
                        mainText = TimeText.FormatCountdownClock(remaining, _showHours);
                        displayText = mainText;
                        statusText = _paused ? "已暂停 · 点击“继续计时”恢复" : "倒计时中";
                    }
                    else
                    {
                        long elapsedSeconds = (long)Math.Floor(_stopwatch.Elapsed.TotalSeconds);
                        bool overTarget = _sessionPlannedSeconds > 0
                            && _stopwatch.Elapsed.TotalSeconds >= _sessionPlannedSeconds;
                        long overSeconds = overTarget
                            ? Math.Max(0, elapsedSeconds - _sessionPlannedSeconds)
                            : 0;

                        mainText = TimeText.FormatClock(elapsedSeconds, _showHours);
                        displayText = mainText;

                        if (_paused)
                        {
                            statusText = overTarget
                                ? "已暂停 · 本轮已超时 " + TimeText.Format(overSeconds)
                                : "已暂停 · 点击“继续计时”恢复";
                        }
                        else if (overTarget)
                        {
                            statusText = "已超时 " + TimeText.Format(overSeconds) + " · 请点击“停止”结束本轮";
                            alert = true;
                        }
                        else
                        {
                            statusText = "顺计时中";
                        }
                    }
                    break;

                case TimerPhase.Overtime:
                    long overtime = (long)Math.Floor(_stopwatch.Elapsed.TotalSeconds) - _sessionPlannedSeconds;
                    mainText = TimeText.FormatClock(overtime, _showHours);
                    displayText = TimeText.FormatClock(0, _showHours);
                    statusText = _paused
                        ? "已暂停 · 本轮已超时"
                        : "已超时，请点击“停止”结束本轮 · 计时窗已停住";
                    alert = !_paused;
                    break;

                case TimerPhase.Stopped:
                    mainText = TimeText.FormatClock(_stoppedMainSeconds, _showHours);
                    displayText = TimeText.FormatClock(_stoppedDisplaySeconds, _showHours);
                    statusText = _stoppedStatusText;
                    break;

                default:
                    long readySeconds = _mode == TimerMode.Countdown ? ReadConfiguredSeconds() : 0;
                    mainText = TimeText.FormatClock(readySeconds, _showHours);
                    displayText = mainText;
                    statusText = "准备就绪";
                    break;
            }

            SetLabelText(_mainTimeLabel, mainText);
            SetLabelText(_timerStatusLabel, statusText);
            _display.SetTimeText(displayText);

            bool paused = _paused && (_phase == TimerPhase.Running || _phase == TimerPhase.Overtime);
            Color cardBack = paused ? UiTheme.PausedBack : alert ? UiTheme.AlertBack : UiTheme.TimerBack;
            Color primaryFore = paused ? UiTheme.PausedFore : alert ? UiTheme.AlertFore : UiTheme.TimerFore;
            Color mutedFore = paused ? UiTheme.PausedFore : alert ? UiTheme.AlertFore : UiTheme.TimerMuted;

            if (_timerCard.BackColor != cardBack)
            {
                _timerCard.BackColor = cardBack;
            }

            if (_mainTimeLabel.ForeColor != primaryFore)
            {
                _mainTimeLabel.ForeColor = primaryFore;
            }

            if (_timerStatusLabel.ForeColor != mutedFore)
            {
                _timerStatusLabel.ForeColor = mutedFore;
            }
        }

        private void RefreshInteractiveState()
        {
            bool active = _phase == TimerPhase.Running || _phase == TimerPhase.Overtime;
            bool canEdit = !active;
            bool durationEditable = canEdit;

            _countdownRadio.Enabled = canEdit;
            _countUpRadio.Enabled = canEdit;
            _hoursInput.Enabled = durationEditable;
            _minutesInput.Enabled = durationEditable;
            _secondsInput.Enabled = durationEditable;

            _startButton.Enabled = true;
            _startButton.Text = _paused ? "继续计时" : active ? "暂停计时" : "开始计时";
            _stopButton.Enabled = active;

            StylePrimaryButton(_startButton, true);
            StyleStopButton(active);

            _startButton.Cursor = Cursors.Hand;
            _stopButton.Cursor = _stopButton.Enabled ? Cursors.Hand : Cursors.Default;

            RefreshDisplayUi();
        }

        private void UpdateSessionInfo()
        {
            if (_sessionInfoLabel == null)
            {
                return;
            }

            if (_phase == TimerPhase.Ready)
            {
                int configuredSeconds = ReadConfiguredSeconds();

                if (_mode == TimerMode.Countdown)
                {
                    _sessionInfoLabel.Text = "等待开始  ·  计划时长 " + TimeText.Format(configuredSeconds) + "  ·  倒计时";
                }
                else
                {
                    string planText = configuredSeconds > 0 ? TimeText.Format(configuredSeconds) : "不限";
                    _sessionInfoLabel.Text = "等待开始  ·  计划时长 " + planText + "  ·  顺计时";
                }

                return;
            }

            string modeText = _sessionMode == TimerMode.Countdown ? "倒计时" : "顺计时";
            string sessionPlanText;

            if (_sessionMode == TimerMode.Countdown)
            {
                sessionPlanText = TimeText.Format(_sessionPlannedSeconds);
            }
            else
            {
                sessionPlanText = _sessionPlannedSeconds > 0 ? TimeText.Format(_sessionPlannedSeconds) : "不限";
            }

            _sessionInfoLabel.Text = "开始时间 " + _sessionStartedAt.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                + "  ·  计划时长 " + sessionPlanText
                + "  ·  " + modeText;
        }

        private int ReadConfiguredSeconds()
        {
            if (_hoursInput == null)
            {
                return 0;
            }

            int hours = (int)_hoursInput.Value;
            int minutes = (int)_minutesInput.Value;
            int seconds = (int)_secondsInput.Value;
            return (hours * 3600) + (minutes * 60) + seconds;
        }

        private void ShowInputError(string message)
        {
            _inputErrorLabel.Text = message;
            _inputErrorLabel.Visible = true;
        }

        private void ClearInputError()
        {
            if (_inputErrorLabel == null)
            {
                return;
            }

            _inputErrorLabel.Text = string.Empty;
            _inputErrorLabel.Visible = false;
        }

        private void RebuildRecordGrid()
        {
            _recordsGrid.SuspendLayout();
            try
            {
                _recordsGrid.Rows.Clear();

                for (int i = _records.Count - 1; i >= 0; i--)
                {
                    TimerRecord record = _records[i];
                    int index = _recordsGrid.Rows.Add(
                        record.Serial,
                        record.StartTime,
                        record.PlannedText,
                        TimeText.Format(record.ActualSeconds),
                        TimeText.Format(record.OvertimeSeconds),
                        "删除");

                    DataGridViewRow row = _recordsGrid.Rows[index];
                    row.Tag = record;

                    if (record.OvertimeSeconds > 0)
                    {
                        row.Cells[4].Style.ForeColor = UiTheme.Danger;
                        row.Cells[4].Style.Font = UiTheme.DangerBoldFont;
                    }
                }
            }
            finally
            {
                _recordsGrid.ResumeLayout();
            }

            _recordCountLabel.Text = _records.Count.ToString(CultureInfo.InvariantCulture) + " 条";
            UpdateCopyButtonState();
        }

        private void RecordsGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (_recordsGrid.Columns[e.ColumnIndex].Name != "deleteColumn")
            {
                return;
            }

            DataGridViewRow row = _recordsGrid.Rows[e.RowIndex];
            TimerRecord record = row.Tag as TimerRecord;
            if (record == null)
            {
                return;
            }

            _records.Remove(record);
            RebuildRecordGrid();
        }

        private static void SetLabelText(Label label, string text)
        {
            if (label.Text != text)
            {
                label.Text = text;
            }
        }

        private static void StylePrimaryButton(Button button, bool enabled)
        {
            button.UseVisualStyleBackColor = false;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = enabled ? UiTheme.Primary : UiTheme.DisabledBack;
            button.ForeColor = enabled ? Color.White : UiTheme.DisabledFore;
            button.FlatAppearance.MouseOverBackColor = enabled ? UiTheme.PrimaryHover : UiTheme.DisabledBack;
            button.FlatAppearance.MouseDownBackColor = enabled ? UiTheme.PrimaryHover : UiTheme.DisabledBack;
        }

        private void StyleStopButton(bool enabled)
        {
            _stopButton.UseVisualStyleBackColor = false;
            _stopButton.FlatStyle = FlatStyle.Flat;
            _stopButton.FlatAppearance.BorderSize = 0;
            _stopButton.BackColor = enabled ? UiTheme.Danger : UiTheme.DisabledBack;
            _stopButton.ForeColor = enabled ? Color.White : UiTheme.DisabledFore;
            _stopButton.FlatAppearance.MouseOverBackColor = enabled ? UiTheme.DangerHover : UiTheme.DisabledBack;
            _stopButton.FlatAppearance.MouseDownBackColor = enabled ? UiTheme.DangerHover : UiTheme.DisabledBack;
        }

        private void StyleDisplayToggleButton(bool timerWindowVisible)
        {
            _toggleDisplayButton.UseVisualStyleBackColor = false;
            _toggleDisplayButton.FlatStyle = FlatStyle.Flat;

            if (timerWindowVisible)
            {
                _toggleDisplayButton.FlatAppearance.BorderSize = 0;
                _toggleDisplayButton.BackColor = UiTheme.ToggleOnBack;
                _toggleDisplayButton.ForeColor = Color.White;
                _toggleDisplayButton.FlatAppearance.MouseOverBackColor = UiTheme.ToggleOnHover;
                _toggleDisplayButton.FlatAppearance.MouseDownBackColor = UiTheme.ToggleOnHover;
            }
            else
            {
                _toggleDisplayButton.FlatAppearance.BorderSize = 1;
                _toggleDisplayButton.FlatAppearance.BorderColor = UiTheme.Border;
                _toggleDisplayButton.BackColor = UiTheme.Surface;
                _toggleDisplayButton.ForeColor = UiTheme.Primary;
                _toggleDisplayButton.FlatAppearance.MouseOverBackColor = UiTheme.SoftButtonHover;
                _toggleDisplayButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(224, 235, 249);
            }
        }

        private static void StyleSecondaryButton(Button button)
        {
            button.UseVisualStyleBackColor = false;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = UiTheme.Border;
            button.BackColor = UiTheme.Surface;
            button.ForeColor = UiTheme.Text;
            button.FlatAppearance.MouseOverBackColor = UiTheme.SoftButtonHover;
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(224, 235, 249);
        }
    }

    internal sealed class TimerDisplayForm : Form
    {
        private enum DragZone
        {
            None,
            Left,
            Right,
            Top,
            Bottom,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        private readonly Label _timeLabel;
        private readonly Timer _topMostTimer;
        private Font _fittedFont;
        private bool _allowClose;
        private bool _dragging;
        private DragZone _dragZone;
        private Control _dragCapture;
        private Point _dragStartCursor;
        private Rectangle _dragStartBounds;

        public event EventHandler UserHidden;

        public TimerDisplayForm()
        {
            AutoScaleMode = AutoScaleMode.None;
            Text = "咩计时 · 计时窗";
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            BackColor = UiTheme.TimerBack;
            ClientSize = new Size(Dpi.S(174), Dpi.S(60));
            MinimumSize = new Size(Dpi.S(120), Dpi.S(40));
            Padding = Dpi.Pad(3, 2, 3, 2);
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            Font = UiTheme.BodyFont;
            AccessibleName = "计时窗口";
            Cursor = Cursors.SizeAll;

            _timeLabel = new Label();
            _timeLabel.Dock = DockStyle.Fill;
            _timeLabel.AutoSize = false;
            _timeLabel.Font = UiTheme.DisplayFont;
            _timeLabel.ForeColor = Color.White;
            _timeLabel.BackColor = BackColor;
            _timeLabel.TextAlign = ContentAlignment.MiddleCenter;
            _timeLabel.Text = "00:00:00";
            _timeLabel.Cursor = Cursors.SizeAll;
            _timeLabel.AccessibleName = "当前计时";
            _timeLabel.MouseCaptureChanged += DragTarget_MouseCaptureChanged;
            _timeLabel.MouseLeave += DragTarget_MouseLeave;
            Controls.Add(_timeLabel);

            MouseCaptureChanged += DragTarget_MouseCaptureChanged;
            MouseLeave += DragTarget_MouseLeave;
            AttachDrag(this);
            AttachDrag(_timeLabel);

            _topMostTimer = new Timer();
            _topMostTimer.Interval = 1000;
            _topMostTimer.Tick += TopMostTimer_Tick;
            _topMostTimer.Start();
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW
                return cp;
            }
        }

        public void SetTimeText(string text)
        {
            if (_timeLabel.Text != text)
            {
                bool lengthChanged = _timeLabel.Text.Length != text.Length;
                _timeLabel.Text = text;

                if (lengthChanged)
                {
                    FitTimerFont();
                }
            }
        }

        public void EnsureTopMost()
        {
            if (!IsHandleCreated || !Visible)
            {
                return;
            }

            uint flags = NativeMethods.SWP_NOMOVE
                | NativeMethods.SWP_NOSIZE
                | NativeMethods.SWP_NOACTIVATE
                | NativeMethods.SWP_NOOWNERZORDER;

            NativeMethods.SetWindowPos(
                Handle,
                NativeMethods.HWND_TOPMOST,
                0,
                0,
                0,
                0,
                flags);
        }

        public void PrepareForClose()
        {
            _allowClose = true;
            if (_topMostTimer != null)
            {
                _topMostTimer.Stop();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FitTimerFont();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            FitTimerFont();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            FitTimerFont();
            EnsureTopMost();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!_allowClose)
            {
                e.Cancel = true;
                Hide();
                EventHandler handler = UserHidden;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }

                return;
            }

            _topMostTimer.Stop();
            base.OnFormClosing(e);
        }

        private void TopMostTimer_Tick(object sender, EventArgs e)
        {
            EnsureTopMost();
        }

        private void AttachDrag(Control target)
        {
            target.MouseDown += DragTarget_MouseDown;
            target.MouseMove += DragTarget_MouseMove;
            target.MouseUp += DragTarget_MouseUp;
        }

        private void FitTimerFont()
        {
            if (_timeLabel == null || IsDisposed || !IsHandleCreated)
            {
                return;
            }

            int availableWidth = ClientSize.Width - Padding.Horizontal;
            int availableHeight = ClientSize.Height - Padding.Vertical;
            if (availableWidth < 8 || availableHeight < 8)
            {
                return;
            }

            const float probeSize = 100F;
            TextFormatFlags flags = TextFormatFlags.NoPadding
                | TextFormatFlags.SingleLine
                | TextFormatFlags.NoPrefix;

            float pointSize;

            using (Font probe = new Font("Consolas", probeSize, FontStyle.Bold, GraphicsUnit.Point))
            {
                Size measured = TextRenderer.MeasureText(
                    _timeLabel.Text,
                    probe,
                    new Size(int.MaxValue, int.MaxValue),
                    flags);

                if (measured.Width <= 0 || measured.Height <= 0)
                {
                    return;
                }

                float widthScale = (float)availableWidth / measured.Width;
                float heightScale = (float)availableHeight / measured.Height;
                pointSize = Math.Max(8F, probeSize * Math.Min(widthScale, heightScale) * 0.99F);
            }

            Font fitted = new Font("Consolas", pointSize, FontStyle.Bold, GraphicsUnit.Point);
            for (int attempt = 0; attempt < 8; attempt++)
            {
                Size size = TextRenderer.MeasureText(
                    _timeLabel.Text,
                    fitted,
                    new Size(int.MaxValue, int.MaxValue),
                    flags);

                if (size.Width <= availableWidth && size.Height <= availableHeight)
                {
                    break;
                }

                pointSize = Math.Max(8F, pointSize - 1F);
                fitted.Dispose();
                fitted = new Font("Consolas", pointSize, FontStyle.Bold, GraphicsUnit.Point);
            }

            Font previous = _fittedFont;
            _fittedFont = fitted;
            _timeLabel.Font = fitted;

            if (previous != null)
            {
                previous.Dispose();
            }
        }

        private DragZone HitTestZone(Point clientPoint)
        {
            int edge = Dpi.S(8);
            bool left = clientPoint.X <= edge;
            bool right = clientPoint.X >= ClientSize.Width - edge;
            bool top = clientPoint.Y <= edge;
            bool bottom = clientPoint.Y >= ClientSize.Height - edge;

            if (left && top)
            {
                return DragZone.TopLeft;
            }

            if (right && top)
            {
                return DragZone.TopRight;
            }

            if (left && bottom)
            {
                return DragZone.BottomLeft;
            }

            if (right && bottom)
            {
                return DragZone.BottomRight;
            }

            if (left)
            {
                return DragZone.Left;
            }

            if (right)
            {
                return DragZone.Right;
            }

            if (top)
            {
                return DragZone.Top;
            }

            if (bottom)
            {
                return DragZone.Bottom;
            }

            return DragZone.None;
        }

        private static Cursor CursorForZone(DragZone zone)
        {
            switch (zone)
            {
                case DragZone.Left:
                case DragZone.Right:
                    return Cursors.SizeWE;
                case DragZone.Top:
                case DragZone.Bottom:
                    return Cursors.SizeNS;
                case DragZone.TopLeft:
                case DragZone.BottomRight:
                    return Cursors.SizeNWSE;
                case DragZone.TopRight:
                case DragZone.BottomLeft:
                    return Cursors.SizeNESW;
                default:
                    return Cursors.SizeAll;
            }
        }

        private void UpdateCursor(DragZone zone)
        {
            Cursor cursor = CursorForZone(zone);
            if (Cursor != cursor)
            {
                Cursor = cursor;
            }

            if (_timeLabel.Cursor != cursor)
            {
                _timeLabel.Cursor = cursor;
            }
        }

        private void DragTarget_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _dragging)
            {
                return;
            }

            DragZone zone = HitTestZone(PointToClient(Cursor.Position));
            _dragZone = zone;
            _dragging = true;
            _dragStartCursor = Cursor.Position;
            _dragStartBounds = Bounds;

            Control capture = sender as Control;
            if (capture != null)
            {
                _dragCapture = capture;
                capture.Capture = true;
            }

            UpdateCursor(zone);
        }

        private void DragTarget_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging)
            {
                UpdateCursor(HitTestZone(PointToClient(Cursor.Position)));
                return;
            }

            Point cursor = Cursor.Position;
            int dx = cursor.X - _dragStartCursor.X;
            int dy = cursor.Y - _dragStartCursor.Y;
            Rectangle start = _dragStartBounds;

            if (_dragZone == DragZone.None)
            {
                Location = new Point(start.Left + dx, start.Top + dy);
                return;
            }

            int minWidth = MinimumSize.Width;
            int minHeight = MinimumSize.Height;
            int left = start.Left;
            int top = start.Top;
            int width = start.Width;
            int height = start.Height;

            if (_dragZone == DragZone.Left
                || _dragZone == DragZone.TopLeft
                || _dragZone == DragZone.BottomLeft)
            {
                left = start.Left + dx;
                width = start.Width - dx;
                if (width < minWidth)
                {
                    width = minWidth;
                    left = start.Right - minWidth;
                }
            }
            else if (_dragZone == DragZone.Right
                || _dragZone == DragZone.TopRight
                || _dragZone == DragZone.BottomRight)
            {
                width = Math.Max(minWidth, start.Width + dx);
            }

            if (_dragZone == DragZone.Top
                || _dragZone == DragZone.TopLeft
                || _dragZone == DragZone.TopRight)
            {
                top = start.Top + dy;
                height = start.Height - dy;
                if (height < minHeight)
                {
                    height = minHeight;
                    top = start.Bottom - minHeight;
                }
            }
            else if (_dragZone == DragZone.Bottom
                || _dragZone == DragZone.BottomLeft
                || _dragZone == DragZone.BottomRight)
            {
                height = Math.Max(minHeight, start.Height + dy);
            }

            Bounds = new Rectangle(left, top, width, height);
        }

        private void DragTarget_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            _dragging = false;
            if (_dragCapture != null)
            {
                _dragCapture.Capture = false;
                _dragCapture = null;
            }
        }

        private void DragTarget_MouseCaptureChanged(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (_dragging && control != null && !control.Capture)
            {
                _dragging = false;
                _dragCapture = null;
            }
        }

        private void DragTarget_MouseLeave(object sender, EventArgs e)
        {
            if (!_dragging)
            {
                UpdateCursor(DragZone.None);
            }
        }
    }
}
