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
        private CountdownWindow? key1CountdownWindow;
        private CountdownWindow? key2CountdownWindow;
        private bool isRunning = false;

        public MainWindow()
        {
            InitializeComponent();
            settings = AppSettings.Load();
            
            // 윈도우 위치 복원
            this.Left = settings.MainWindowLeft;
            this.Top = settings.MainWindowTop;
            
            // 체크박스 상태 복원 및 텍스트 업데이트
            ChkKey1.IsChecked = settings.Key1Enabled;
            ChkKey2.IsChecked = settings.Key2Enabled;
            UpdateCheckBoxLabels();
            
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
            key1CountdownWindow?.StopCountdown();
            key2CountdownWindow?.StopCountdown();
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
            if (key1CountdownWindow != null)
            {
                key1CountdownWindow.StopCountdown();
                key1CountdownWindow = null;
            }
            
            if (key2CountdownWindow != null)
            {
                key2CountdownWindow.StopCountdown();
                key2CountdownWindow = null;
            }
        }

        private void KeyboardHook_KeyPressed(object? sender, Key e)
        {
            if (!isRunning) return;

            // 설정된 트리거 키와 비교
            string pressedKey = e.ToString();
            
            // Key1 트리거 체크 (활성화된 경우에만)
            if (settings.Key1Enabled && pressedKey == settings.Key1.TriggerKey)
            {
                // BeginInvoke를 사용하여 비동기 실행 (키 블럭 방지)
                Dispatcher.BeginInvoke(() =>
                {
                    if (key1CountdownWindow == null)
                    {
                        // 새 카운트다운 시작
                        key1CountdownWindow = new CountdownWindow(settings.Key1);
                        key1CountdownWindow.Show();
                        key1CountdownWindow.StartCountdown();
                    }
                    else
                    {
                        // 카운트다운 리셋
                        key1CountdownWindow.ResetCountdown();
                    }
                });
            }
            
            // Key2 트리거 체크 (활성화된 경우에만)
            if (settings.Key2Enabled && pressedKey == settings.Key2.TriggerKey)
            {
                // BeginInvoke를 사용하여 비동기 실행 (키 블럭 방지)
                Dispatcher.BeginInvoke(() =>
                {
                    if (key2CountdownWindow == null)
                    {
                        // 새 카운트다운 시작
                        key2CountdownWindow = new CountdownWindow(settings.Key2);
                        key2CountdownWindow.Show();
                        key2CountdownWindow.StartCountdown();
                    }
                    else
                    {
                        // 카운트다운 리셋
                        key2CountdownWindow.ResetCountdown();
                    }
                });
            }
        }

        private void ChkKey_CheckedChanged(object sender, RoutedEventArgs e)
        {
            // 초기화 중이거나 컨트롤이 null이면 무시
            if (ChkKey1 == null || ChkKey2 == null || settings == null) return;
            
            // 체크박스 상태를 settings에 저장
            settings.Key1Enabled = ChkKey1.IsChecked == true;
            settings.Key2Enabled = ChkKey2.IsChecked == true;
            settings.Save();
        }

        private void UpdateCheckBoxLabels()
        {
            ChkKey1.Content = $"Key1 ({GetDisplayName(settings.Key1.TriggerKey)})";
            ChkKey2.Content = $"Key2 ({GetDisplayName(settings.Key2.TriggerKey)})";
        }

        private string GetDisplayName(string key)
        {
            // D0~D9를 0~9로 표시
            if (key.StartsWith("D") && key.Length == 2 && char.IsDigit(key[1]))
            {
                return key.Substring(1); // "D1" -> "1"
            }
            return key;
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
                
                // 체크박스 라벨 업데이트 (트리거 키가 변경되었을 수 있음)
                UpdateCheckBoxLabels();
            }
        }
    }
}