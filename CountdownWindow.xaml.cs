using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MozaCounter
{
    public partial class CountdownWindow : Window
    {
        private DispatcherTimer? countdownTimer;
        private int currentCount;
        private int startTime;

        public CountdownWindow(KeySettings keySettings)
        {
            InitializeComponent();

            startTime = keySettings.StartTime;
            currentCount = startTime;

            // 위치 설정
            this.Left = keySettings.CounterPosX;
            this.Top = keySettings.CounterPosY;

            // 폰트 설정
            var fontFamily = new FontFamily(keySettings.FontFamily);
            TxtCountdown.FontFamily = fontFamily;
            TxtCountdown.FontSize = keySettings.FontSize;
            TxtCountdown.Foreground = new SolidColorBrush(keySettings.GetFontColor());

            // 배경 효과 설정
            if (keySettings.BorderThickness > 0)
            {
                TxtCountdown.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = keySettings.GetBorderColor(),
                    Direction = 0,
                    ShadowDepth = 0,
                    BlurRadius = keySettings.BorderThickness * 2,
                    Opacity = 1
                };
            }

            // 초기 텍스트 (2자리 포맷)
            TxtCountdown.Text = currentCount.ToString("D2");

            // 타이머 설정
            countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            countdownTimer.Tick += CountdownTimer_Tick;
        }

        public void StartCountdown()
        {
            countdownTimer?.Start();
        }

        public void ResetCountdown()
        {
            currentCount = startTime;
            TxtCountdown.Text = currentCount.ToString("D2");
            this.Show();
            countdownTimer?.Stop();
            countdownTimer?.Start();
        }

        public void StopCountdown()
        {
            countdownTimer?.Stop();
            this.Close();
        }

        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            currentCount--;
            TxtCountdown.Text = currentCount.ToString("D2");

            if (currentCount <= 0)
            {
                countdownTimer?.Stop();
                // 카운트다운 완료 - 윈도우 숨기기
                this.Hide();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            countdownTimer?.Stop();
            base.OnClosed(e);
        }
    }
}
