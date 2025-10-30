using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MozaCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppSettings settings;
        private GlobalKeyboardHook? keyboardHook;
        private CountdownWindow? countdownWindow;
        private bool isRunning = false;

        public MainWindow()
        {
            InitializeComponent();
            settings = AppSettings.Load();
            
            // 윈도우 위치 복원
            this.Left = settings.MainWindowLeft;
            this.Top = settings.MainWindowTop;
            
            // 윈도우 닫힐 때 위치 저장
            this.Closing += MainWindow_Closing;

            // 키보드 후킹 초기화
            keyboardHook = new GlobalKeyboardHook();
            keyboardHook.KeyPressed += KeyboardHook_KeyPressed;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            settings.MainWindowLeft = this.Left;
            settings.MainWindowTop = this.Top;
            settings.Save();

            // 키보드 후킹 해제
            keyboardHook?.Unhook();
            keyboardHook?.Dispose();

            // 카운트다운 윈도우 닫기
            countdownWindow?.StopCountdown();
        }

        private void RadioButton_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (TxtStatus == null) return;
            
            if (RadioStart.IsChecked == true)
            {
                TxtStatus.Text = "실행중";
                TxtStatus.Foreground = new SolidColorBrush(System.Windows.Media.Colors.Green);
                StartMonitoring();
            }
            else
            {
                TxtStatus.Text = "중지됨";
                TxtStatus.Foreground = new SolidColorBrush(System.Windows.Media.Colors.Red);
                StopMonitoring();
            }
        }

        private void StartMonitoring()
        {
            isRunning = true;
            keyboardHook?.Hook();
        }

        private void StopMonitoring()
        {
            isRunning = false;
            keyboardHook?.Unhook();
            
            // 카운트다운 윈도우 닫기
            if (countdownWindow != null)
            {
                countdownWindow.StopCountdown();
                countdownWindow = null;
            }
        }

        private void KeyboardHook_KeyPressed(object? sender, Key e)
        {
            if (!isRunning) return;

            // 설정된 트리거 키와 비교
            string pressedKey = e.ToString();
            if (pressedKey == settings.TriggerKey)
            {
                Dispatcher.Invoke(() =>
                {
                    if (countdownWindow == null)
                    {
                        // 새 카운트다운 시작
                        countdownWindow = new CountdownWindow(settings);
                        countdownWindow.Show();
                        countdownWindow.StartCountdown();
                    }
                    else
                    {
                        // 카운트다운 리셋
                        countdownWindow.ResetCountdown();
                    }
                });
            }
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            // 설정 창 열기 전 중지
            RadioStop.IsChecked = true;
            
            var settingsWindow = new FontSettingsWindow(settings);
            settingsWindow.Owner = this;
            if (settingsWindow.ShowDialog() == true)
            {
                // 설정이 변경되면 저장
                settings = settingsWindow.GetSettings();
                settings.Save();
            }
        }
    }
}