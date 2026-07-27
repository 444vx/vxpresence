using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VxPresence.Inspectors;
using VxPresence.Native;
using VxPresence.Services;
using VxPresence.Telemetry;

namespace VxPresence
{
    public partial class MainWindow : Window
    {
        private const string DISCORD_CLIENT_ID = "";
        private const string GITHUB_URL = "https://github.com/444vx/vxpresence"; 

        private CpuMonitor? _cpuMonitor;
        private RamMonitor? _ramMonitor;
        private AppInspector? _appInspector;
        private BackgroundAppMonitor? _backgroundMonitor;
        private DiscordPresenceService? _discordService;

        private CancellationTokenSource? _cts;

        public MainWindow()
        {
            InitializeComponent();
            
            ChkAutostart.IsChecked = AutostartManager.IsAutostartEnabled();
            ChkAutostart.Click += ChkAutostart_Click;

            StartEngine();
        }

        private void StartEngine()
        {
            try
            {
                _cpuMonitor = new CpuMonitor();
                _ramMonitor = new RamMonitor();
                _appInspector = new AppInspector();
                _backgroundMonitor = new BackgroundAppMonitor();
                _discordService = new DiscordPresenceService(DISCORD_CLIENT_ID);

                _cts = new CancellationTokenSource();
                Task.Run(() => EngineLoop(_cts.Token));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Init Error] {ex.Message}");
            }
        }

        private async Task EngineLoop(CancellationToken token)
        {
            DateTime startTime = DateTime.UtcNow;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    double idleSeconds = IdleDetector.GetIdleSeconds();
                    bool isAfk = idleSeconds >= 300; // 300 sekund = 5 minut

                    string details = "🖥️ Windows Desktop";
                    string state = "Active";
                    bool isMedia = false;

                    bool showTelemetry = false;
                    bool showBgApps = false;
                    bool showTabs = false;

                    Dispatcher.Invoke(() =>
                    {
                        showTelemetry = ChkTelemetry.IsChecked == true;
                        showBgApps = ChkBgApps.IsChecked == true;
                        showTabs = ChkShowTabs.IsChecked == true;
                    });

                    if (isAfk)
                    {
                        int idleMinutes = (int)(idleSeconds / 60);
                        details = "💤 Away From Keyboard (AFK)";
                        state = $"Idle for {idleMinutes}m";
                    }
                    else
                    {
                        IntPtr handle = Win32.GetForegroundWindow();
                        if (handle != IntPtr.Zero)
                        {
                            Win32.GetWindowThreadProcessId(handle, out uint processId);
                            
                            try
                            {
                                Process activeProcess = Process.GetProcessById((int)processId);

                                if (activeProcess.ProcessName.Equals("explorer", StringComparison.OrdinalIgnoreCase))
                                {
                                    details = "🖥️ Windows Desktop";
                                    if (showBgApps)
                                    {
                                        var openApps = _backgroundMonitor?.GetActiveBackgroundApps() ?? new();
                                        string appsStr = openApps.Count > 0 ? string.Join(", ", openApps) : "None";
                                        state = $"Open: {appsStr}";
                                    }
                                    else
                                    {
                                        state = "Idle";
                                    }
                                }
                                else
                                {
                                    var info = _appInspector?.Inspect(activeProcess);
                                    isMedia = info?.IsMedia ?? false;

                                    if (info?.Category.Contains("Media") == true)
                                    {
                                        details = $"🎧 Listening / Watching: {info.DisplayName}";
                                        state = showTabs ? $"🎵 {info.WindowTitle}" : "Playing Media";
                                    }
                                    else if (info?.Category == "🌐 Browser")
                                    {
                                        details = $"{info.Category}: {info.DisplayName}";
                                        state = showTabs ? $"📄 {info.WindowTitle}" : "Browsing the Web";
                                    }
                                    else
                                    {
                                        details = $"{info?.Category}: {info?.DisplayName}";
                                        state = info?.WindowTitle ?? info?.ProcessName ?? "App";
                                    }
                                }
                            }
                            catch
                            {
                                // Gdyby proces chwilowo nie odpowiedział lub został zamknięty
                                details = "🖥️ Windows Desktop";
                                state = "Active";
                            }
                        }

                        if (showTelemetry)
                        {
                            float cpuUsage = _cpuMonitor?.GetUsage() ?? 0;
                            var ramUsage = _ramMonitor?.GetUsage() ?? (0, "N/A");
                            state += $" | CPU: {cpuUsage:F0}% | RAM: {ramUsage.FormattedText}";
                        }
                    }

                    // Wysyłamy aktualizację do Discorda w każdym obiegu pętli
                    _discordService?.Update(details, state, startTime, isMedia, null, GITHUB_URL);

                    Dispatcher.Invoke(() =>
                    {
                        TxtStatusDetails.Text = details;
                        TxtStatusState.Text = state;
                    });
                }
                catch { }

                await Task.Delay(2000, token);
            }
        }

        private void ChkAutostart_Click(object sender, RoutedEventArgs e)
        {
            bool enable = ChkAutostart.IsChecked == true;
            AutostartManager.SetAutostart(enable);
        }

        protected override void OnClosed(EventArgs e)
        {
            _cts?.Cancel();
            _discordService?.Dispose();
            base.OnClosed(e);
        }
    }
}
