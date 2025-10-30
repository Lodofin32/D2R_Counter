using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Threading;

namespace MozaCounter
{
    public partial class FontSettingsWindow : Window
    {
        private bool isInitializing = true;
        private DispatcherTimer? mouseTrackTimer;
        private Dictionary<string, FontFamily> fontFamilyMap = new Dictionary<string, FontFamily>();
        private AppSettings settings;

        public FontFamily SelectedFontFamily { get; private set; } = new FontFamily("Arial");
        public double SelectedFontSize { get; private set; }
        public Color SelectedFontColor { get; private set; }
        public double SelectedBorderThickness { get; private set; }
        public Color SelectedBorderColor { get; private set; }
        public int CounterPosX { get; private set; }
        public int CounterPosY { get; private set; }
        public int StartTime { get; private set; }
        public string TriggerKey { get; private set; } = "Space";

        public FontSettingsWindow(AppSettings appSettings)
        {
            InitializeComponent();
            settings = appSettings;
            
            InitializeFontFamilies();
            InitializeTriggerKeys();
            InitializeMouseTracking();
            LoadSettings();
            
            isInitializing = false;
            UpdatePreview();
            
            // 윈도우 위치 복원
            this.Left = settings.SettingsWindowLeft;
            this.Top = settings.SettingsWindowTop;
            
            // 키보드 이벤트 핸들러 등록
            this.KeyDown += FontSettingsWindow_KeyDown;
            
            // 윈도우 닫힐 때 위치 저장
            this.Closing += FontSettingsWindow_Closing;
        }

        private void FontSettingsWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            settings.SettingsWindowLeft = this.Left;
            settings.SettingsWindowTop = this.Top;
        }

        private void LoadSettings()
        {
            // 폰트 설정 불러오기
            SelectedFontSize = settings.FontSize;
            SelectedFontColor = settings.GetFontColor();
            SelectedBorderThickness = settings.BorderThickness;
            SelectedBorderColor = settings.GetBorderColor();
            CounterPosX = settings.CounterPosX;
            CounterPosY = settings.CounterPosY;
            StartTime = settings.StartTime;
            TriggerKey = settings.TriggerKey;

            // UI 컨트롤에 값 설정
            TxtFontSize.Text = settings.FontSize.ToString();
            FontColorPicker.SelectedColor = settings.GetFontColor();
            SliderBorderThickness.Value = settings.BorderThickness;
            LblBorderThickness.Content = settings.BorderThickness.ToString();
            BorderColorPicker.SelectedColor = settings.GetBorderColor();
            TxtPosX.Text = settings.CounterPosX.ToString();
            TxtPosY.Text = settings.CounterPosY.ToString();
            TxtStartTime.Text = settings.StartTime.ToString();
            TxtMousePos.Text = "마우스 위치(F1 입력시 설정): -";

            // 폰트 패밀리 선택
            for (int i = 0; i < CmbFontFamily.Items.Count; i++)
            {
                if (CmbFontFamily.Items[i].ToString() == settings.FontFamily)
                {
                    CmbFontFamily.SelectedIndex = i;
                    break;
                }
            }

            // 트리거 키 선택
            for (int i = 0; i < CmbTriggerKey.Items.Count; i++)
            {
                if (CmbTriggerKey.Items[i].ToString() == settings.TriggerKey)
                {
                    CmbTriggerKey.SelectedIndex = i;
                    break;
                }
            }
        }

        public AppSettings GetSettings()
        {
            settings.FontFamily = CmbFontFamily.SelectedItem?.ToString() ?? "맑은 고딕";
            settings.FontSize = SelectedFontSize;
            settings.SetFontColor(SelectedFontColor);
            settings.BorderThickness = SelectedBorderThickness;
            settings.SetBorderColor(SelectedBorderColor);
            settings.CounterPosX = CounterPosX;
            settings.CounterPosY = CounterPosY;
            settings.StartTime = StartTime;
            settings.TriggerKey = TriggerKey;

            return settings;
        }

        private void InitializeFontFamilies()
        {
            var koreanCulture = new CultureInfo("ko-KR");
            var fontList = new List<(string DisplayName, FontFamily FontFamily)>();

            // 시스템에 설치된 모든 폰트 추가
            foreach (var fontFamily in Fonts.SystemFontFamilies)
            {
                string displayName;
                
                // 한글 이름이 있으면 한글 이름 사용, 없으면 영문 이름 사용
                if (fontFamily.FamilyNames.ContainsKey(XmlLanguage.GetLanguage("ko-KR")))
                {
                    displayName = fontFamily.FamilyNames[XmlLanguage.GetLanguage("ko-KR")];
                }
                else if (fontFamily.FamilyNames.ContainsKey(XmlLanguage.GetLanguage("en-US")))
                {
                    displayName = fontFamily.FamilyNames[XmlLanguage.GetLanguage("en-US")];
                }
                else
                {
                    displayName = fontFamily.Source;
                }

                fontList.Add((displayName, fontFamily));
                fontFamilyMap[displayName] = fontFamily;
            }

            // 가나다 순으로 정렬
            fontList.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.CurrentCulture));

            // 콤보박스에 추가
            foreach (var font in fontList)
            {
                CmbFontFamily.Items.Add(font.DisplayName);
            }

            // 기본 폰트 선택 (맑은 고딕이 있으면 선택, 없으면 첫 번째)
            var malgunGothicIndex = -1;
            for (int i = 0; i < CmbFontFamily.Items.Count; i++)
            {
                var name = CmbFontFamily.Items[i].ToString();
                if (name == "맑은 고딕" || name == "Malgun Gothic")
                {
                    malgunGothicIndex = i;
                    break;
                }
            }
            
            if (CmbFontFamily.Items.Count > 0)
            {
                CmbFontFamily.SelectedIndex = malgunGothicIndex >= 0 ? malgunGothicIndex : 0;
                var selectedName = CmbFontFamily.SelectedItem?.ToString();
                if (selectedName != null && fontFamilyMap.ContainsKey(selectedName))
                {
                    SelectedFontFamily = fontFamilyMap[selectedName];
                }
            }
        }

        private void InitializeTriggerKeys()
        {
            // 104키 키보드의 모든 키 목록
            var keys = new[]
            {
                "Escape", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12",
                "PrintScreen", "ScrollLock", "Pause",
                "OemTilde", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "D0", "OemMinus", "OemPlus", "Back",
                "Insert", "Home", "PageUp", "NumLock", "Divide", "Multiply", "Subtract",
                "Tab", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "OemOpenBrackets", "OemCloseBrackets", "OemPipe",
                "Delete", "End", "PageDown", "NumPad7", "NumPad8", "NumPad9", "Add",
                "CapsLock", "A", "S", "D", "F", "G", "H", "J", "K", "L", "OemSemicolon", "OemQuotes", "Return",
                "NumPad4", "NumPad5", "NumPad6",
                "LeftShift", "Z", "X", "C", "V", "B", "N", "M", "OemComma", "OemPeriod", "OemQuestion", "RightShift",
                "Up", "NumPad1", "NumPad2", "NumPad3", "Enter",
                "LeftCtrl", "LWin", "LeftAlt", "Space", "RightAlt", "RWin", "Apps", "RightCtrl",
                "Left", "Down", "Right", "NumPad0", "Decimal"
            };

            foreach (var key in keys)
            {
                CmbTriggerKey.Items.Add(key);
            }
            CmbTriggerKey.SelectedIndex = 0;
        }

        private void InitializeMouseTracking()
        {
            mouseTrackTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            mouseTrackTimer.Tick += MouseTrackTimer_Tick;
        }

        private void CmbFontFamily_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (isInitializing || CmbFontFamily.SelectedItem == null) return;
            
            var selectedName = CmbFontFamily.SelectedItem.ToString();
            if (selectedName != null && fontFamilyMap.ContainsKey(selectedName))
            {
                SelectedFontFamily = fontFamilyMap[selectedName];
                UpdatePreview();
            }
        }

        private void TxtFontSize_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (isInitializing) return;
            
            if (double.TryParse(TxtFontSize.Text, out double size) && size > 0)
            {
                SelectedFontSize = size;
                UpdatePreview();
            }
        }

        private void FontColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (isInitializing || !FontColorPicker.SelectedColor.HasValue) return;
            
            SelectedFontColor = FontColorPicker.SelectedColor.Value;
            UpdatePreview();
        }

        private void BorderColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (isInitializing || !BorderColorPicker.SelectedColor.HasValue) return;
            
            SelectedBorderColor = BorderColorPicker.SelectedColor.Value;
            UpdatePreview();
        }

        private void SliderBorderThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isInitializing) return;
            
            SelectedBorderThickness = SliderBorderThickness.Value;
            if (LblBorderThickness != null)
            {
                LblBorderThickness.Content = SliderBorderThickness.Value.ToString("F0");
            }
            UpdatePreview();
        }

        private void BtnTrackMouse_Click(object sender, RoutedEventArgs e)
        {
            if (BtnTrackMouse.IsChecked == true)
            {
                BtnTrackMouse.Content = "ON";
                mouseTrackTimer?.Start();
            }
            else
            {
                BtnTrackMouse.Content = "OFF";
                mouseTrackTimer?.Stop();
                TxtMousePos.Text = "마우스 위치(F1 입력시 설정): -";
            }
        }

        private void MouseTrackTimer_Tick(object? sender, EventArgs e)
        {
            var position = System.Windows.Forms.Control.MousePosition;
            TxtMousePos.Text = $"마우스 위치(F1 입력시 설정): {position.X}, {position.Y}";
        }

        private void FontSettingsWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F1 && BtnTrackMouse.IsChecked == true)
            {
                // 현재 마우스 위치를 카운터 위치에 설정
                var position = System.Windows.Forms.Control.MousePosition;
                TxtPosX.Text = position.X.ToString();
                TxtPosY.Text = position.Y.ToString();
                
                // 버튼을 OFF로 전환
                BtnTrackMouse.IsChecked = false;
                BtnTrackMouse.Content = "OFF";
                mouseTrackTimer?.Stop();
                TxtMousePos.Text = "마우스 위치(F1 입력시 설정): -";
                
                e.Handled = true;
            }
        }

        private void UpdatePreview()
        {
            if (PreviewText == null) return;
            
            PreviewText.FontFamily = SelectedFontFamily;
            PreviewText.FontSize = SelectedFontSize;
            PreviewText.Foreground = new SolidColorBrush(SelectedFontColor);
            
            if (SelectedBorderThickness > 0)
            {
                PreviewText.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = SelectedBorderColor,
                    Direction = 0,
                    ShadowDepth = 0,
                    BlurRadius = SelectedBorderThickness * 2,
                    Opacity = 1
                };
            }
            else
            {
                PreviewText.Effect = null;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            // 값 저장
            if (int.TryParse(TxtPosX.Text, out int posX))
                CounterPosX = posX;
            if (int.TryParse(TxtPosY.Text, out int posY))
                CounterPosY = posY;
            if (int.TryParse(TxtStartTime.Text, out int startTime) && startTime > 0)
                StartTime = startTime;
            if (CmbTriggerKey.SelectedItem != null)
                TriggerKey = CmbTriggerKey.SelectedItem.ToString()!;

            // 마우스 추적 중지
            mouseTrackTimer?.Stop();

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // 마우스 추적 중지
            mouseTrackTimer?.Stop();

            DialogResult = false;
            Close();
        }
    }
}