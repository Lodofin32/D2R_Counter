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

        public CountdownWindow(AppSettings settings)
        {
            InitializeComponent();

            startTime = settings.StartTime;
            currentCount = startTime;

            // 위치 설정
            this.Left = settings.CounterPosX;
            this.Top = settings.CounterPosY;

            // 폰트 설정
            var fontFamily = new FontFamily(settings.FontFamily);
            TxtCountdown.FontFamily = fontFamily;
            TxtCountdown.FontSize = settings.FontSize;
            TxtCountdown.Foreground = new SolidColorBrush(settings.GetFontColor());

            // 배경 효과 설정
            if (settings.BorderThickness > 0)
            {
                TxtCountdown.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = settings.GetBorderColor(),
                    Direction = 0,
                    ShadowDepth = 0,
                    BlurRadius = settings.BorderThickness * 2,
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
