using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Threading;

namespace MozaCounter
{
    // 트리거 키 매핑 클래스 (표시 이름과 실제 키 값을 분리)
    public class KeyMapping
    {
        public string DisplayName { get; set; } = "";
        public string ActualKey { get; set; } = "";
        
        public override string ToString() => DisplayName;
    }

    public partial class FontSettingsWindow : Window
    {
        private bool isInitializing = true;
        private DispatcherTimer? mouseTrackTimer;
        private Dictionary<string, FontFamily> fontFamilyMap = new Dictionary<string, FontFamily>();
        private AppSettings settings;

        // Key1 설정
        public FontFamily Key1_SelectedFontFamily { get; private set; } = new FontFamily("Arial");
        public double Key1_SelectedFontSize { get; private set; }
        public Color Key1_SelectedFontColor { get; private set; }
        public double Key1_SelectedBorderThickness { get; private set; }
        public Color Key1_SelectedBorderColor { get; private set; }
        public int Key1_CounterPosX { get; private set; }
        public int Key1_CounterPosY { get; private set; }
        public int Key1_StartTime { get; private set; }
        public string Key1_TriggerKey { get; private set; } = "Space";

        // Key2 설정
        public FontFamily Key2_SelectedFontFamily { get; private set; } = new FontFamily("Arial");
        public double Key2_SelectedFontSize { get; private set; }
        public Color Key2_SelectedFontColor { get; private set; }
        public double Key2_SelectedBorderThickness { get; private set; }
        public Color Key2_SelectedBorderColor { get; private set; }
        public int Key2_CounterPosX { get; private set; }
        public int Key2_CounterPosY { get; private set; }
        public int Key2_StartTime { get; private set; }
        public string Key2_TriggerKey { get; private set; } = "D";

        public FontSettingsWindow(AppSettings appSettings)
        {
            InitializeComponent();
            settings = appSettings;
            
            InitializeFontFamilies();
            InitializeTriggerKeys();
            InitializeMouseTracking();
            LoadSettings();
            
            isInitializing = false;
            UpdateKey1Preview();
            UpdateKey2Preview();
            
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
            // Key1 설정 불러오기
            Key1_SelectedFontSize = settings.Key1.FontSize;
            Key1_SelectedFontColor = settings.Key1.GetFontColor();
            Key1_SelectedBorderThickness = settings.Key1.BorderThickness;
            Key1_SelectedBorderColor = settings.Key1.GetBorderColor();
            Key1_CounterPosX = settings.Key1.CounterPosX;
            Key1_CounterPosY = settings.Key1.CounterPosY;
            Key1_StartTime = settings.Key1.StartTime;
            Key1_TriggerKey = settings.Key1.TriggerKey;

            // Key1 UI 컨트롤에 값 설정
            Key1_TxtFontSize.Text = settings.Key1.FontSize.ToString();
            Key1_FontColorPicker.SelectedColor = settings.Key1.GetFontColor();
            Key1_SliderBorderThickness.Value = settings.Key1.BorderThickness;
            Key1_LblBorderThickness.Content = settings.Key1.BorderThickness.ToString();
            Key1_BorderColorPicker.SelectedColor = settings.Key1.GetBorderColor();
            Key1_TxtPosX.Text = settings.Key1.CounterPosX.ToString();
            Key1_TxtPosY.Text = settings.Key1.CounterPosY.ToString();
            Key1_TxtStartTime.Text = settings.Key1.StartTime.ToString();
            Key1_TxtMousePos.Text = "마우스 위치(F1 입력시 설정): -";

            // Key1 폰트 패밀리 선택
            for (int i = 0; i < Key1_CmbFontFamily.Items.Count; i++)
            {
                if (Key1_CmbFontFamily.Items[i].ToString() == settings.Key1.FontFamily)
                {
                    Key1_CmbFontFamily.SelectedIndex = i;
                    break;
                }
            }

            // Key1 트리거 키 선택
            for (int i = 0; i < Key1_CmbTriggerKey.Items.Count; i++)
            {
                if (Key1_CmbTriggerKey.Items[i] is KeyMapping mapping && mapping.ActualKey == settings.Key1.TriggerKey)
                {
                    Key1_CmbTriggerKey.SelectedIndex = i;
                    break;
                }
            }

            // Key2 설정 불러오기
            Key2_SelectedFontSize = settings.Key2.FontSize;
            Key2_SelectedFontColor = settings.Key2.GetFontColor();
            Key2_SelectedBorderThickness = settings.Key2.BorderThickness;
            Key2_SelectedBorderColor = settings.Key2.GetBorderColor();
            Key2_CounterPosX = settings.Key2.CounterPosX;
            Key2_CounterPosY = settings.Key2.CounterPosY;
            Key2_StartTime = settings.Key2.StartTime;
            Key2_TriggerKey = settings.Key2.TriggerKey;

            // Key2 UI 컨트롤에 값 설정
            Key2_TxtFontSize.Text = settings.Key2.FontSize.ToString();
            Key2_FontColorPicker.SelectedColor = settings.Key2.GetFontColor();
            Key2_SliderBorderThickness.Value = settings.Key2.BorderThickness;
            Key2_LblBorderThickness.Content = settings.Key2.BorderThickness.ToString();
            Key2_BorderColorPicker.SelectedColor = settings.Key2.GetBorderColor();
            Key2_TxtPosX.Text = settings.Key2.CounterPosX.ToString();
            Key2_TxtPosY.Text = settings.Key2.CounterPosY.ToString();
            Key2_TxtStartTime.Text = settings.Key2.StartTime.ToString();
            Key2_TxtMousePos.Text = "마우스 위치(F1 입력시 설정): -";

            // Key2 폰트 패밀리 선택
            for (int i = 0; i < Key2_CmbFontFamily.Items.Count; i++)
            {
                if (Key2_CmbFontFamily.Items[i].ToString() == settings.Key2.FontFamily)
                {
                    Key2_CmbFontFamily.SelectedIndex = i;
                    break;
                }
            }

            // Key2 트리거 키 선택
            for (int i = 0; i < Key2_CmbTriggerKey.Items.Count; i++)
            {
                if (Key2_CmbTriggerKey.Items[i] is KeyMapping mapping && mapping.ActualKey == settings.Key2.TriggerKey)
                {
                    Key2_CmbTriggerKey.SelectedIndex = i;
                    break;
                }
            }
        }

        public AppSettings GetSettings()
        {
            // Key1 설정 저장
            settings.Key1.FontFamily = Key1_CmbFontFamily.SelectedItem?.ToString() ?? "맑은 고딕";
            settings.Key1.FontSize = Key1_SelectedFontSize;
            settings.Key1.SetFontColor(Key1_SelectedFontColor);
            settings.Key1.BorderThickness = Key1_SelectedBorderThickness;
            settings.Key1.SetBorderColor(Key1_SelectedBorderColor);
            settings.Key1.CounterPosX = Key1_CounterPosX;
            settings.Key1.CounterPosY = Key1_CounterPosY;
            settings.Key1.StartTime = Key1_StartTime;
            settings.Key1.TriggerKey = Key1_TriggerKey;

            // Key2 설정 저장
            settings.Key2.FontFamily = Key2_CmbFontFamily.SelectedItem?.ToString() ?? "맑은 고딕";
            settings.Key2.FontSize = Key2_SelectedFontSize;
            settings.Key2.SetFontColor(Key2_SelectedFontColor);
            settings.Key2.BorderThickness = Key2_SelectedBorderThickness;
            settings.Key2.SetBorderColor(Key2_SelectedBorderColor);
            settings.Key2.CounterPosX = Key2_CounterPosX;
            settings.Key2.CounterPosY = Key2_CounterPosY;
            settings.Key2.StartTime = Key2_StartTime;
            settings.Key2.TriggerKey = Key2_TriggerKey;

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

            // 콤보박스에 추가 (Key1, Key2)
            foreach (var font in fontList)
            {
                Key1_CmbFontFamily.Items.Add(font.DisplayName);
                Key2_CmbFontFamily.Items.Add(font.DisplayName);
            }

            // 기본 폰트 선택 (맑은 고딕이 있으면 선택, 없으면 첫 번째)
            var malgunGothicIndex = -1;
            for (int i = 0; i < Key1_CmbFontFamily.Items.Count; i++)
            {
                var name = Key1_CmbFontFamily.Items[i].ToString();
                if (name == "맑은 고딕" || name == "Malgun Gothic")
                {
                    malgunGothicIndex = i;
                    break;
                }
            }
            
            if (Key1_CmbFontFamily.Items.Count > 0)
            {
                Key1_CmbFontFamily.SelectedIndex = malgunGothicIndex >= 0 ? malgunGothicIndex : 0;
                var selectedName = Key1_CmbFontFamily.SelectedItem?.ToString();
                if (selectedName != null && fontFamilyMap.ContainsKey(selectedName))
                {
                    Key1_SelectedFontFamily = fontFamilyMap[selectedName];
                }
            }

            if (Key2_CmbFontFamily.Items.Count > 0)
            {
                Key2_CmbFontFamily.SelectedIndex = malgunGothicIndex >= 0 ? malgunGothicIndex : 0;
                var selectedName = Key2_CmbFontFamily.SelectedItem?.ToString();
                if (selectedName != null && fontFamilyMap.ContainsKey(selectedName))
                {
                    Key2_SelectedFontFamily = fontFamilyMap[selectedName];
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
                var mapping = new KeyMapping 
                { 
                    ActualKey = key,
                    DisplayName = GetDisplayName(key)
                };
                Key1_CmbTriggerKey.Items.Add(mapping);
                Key2_CmbTriggerKey.Items.Add(new KeyMapping { ActualKey = key, DisplayName = GetDisplayName(key) });
            }
            Key1_CmbTriggerKey.SelectedIndex = 0;
            Key2_CmbTriggerKey.SelectedIndex = 0;
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

        private void InitializeMouseTracking()
        {
            mouseTrackTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            mouseTrackTimer.Tick += MouseTrackTimer_Tick;
        }

        // Key1 이벤트 핸들러
        private void Key1_CmbFontFamily_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (isInitializing || Key1_CmbFontFamily.SelectedItem == null) return;
            
            var selectedName = Key1_CmbFontFamily.SelectedItem.ToString();
            if (selectedName != null && fontFamilyMap.ContainsKey(selectedName))
            {
                Key1_SelectedFontFamily = fontFamilyMap[selectedName];
                UpdateKey1Preview();
            }
        }

        private void Key1_TxtFontSize_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (isInitializing) return;
            
            if (double.TryParse(Key1_TxtFontSize.Text, out double size) && size > 0)
            {
                Key1_SelectedFontSize = size;
                UpdateKey1Preview();
            }
        }

        private void Key1_FontColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (isInitializing || !Key1_FontColorPicker.SelectedColor.HasValue) return;
            
            Key1_SelectedFontColor = Key1_FontColorPicker.SelectedColor.Value;
            UpdateKey1Preview();
        }

        private void Key1_BorderColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (isInitializing || !Key1_BorderColorPicker.SelectedColor.HasValue) return;
            
            Key1_SelectedBorderColor = Key1_BorderColorPicker.SelectedColor.Value;
            UpdateKey1Preview();
        }

        private void Key1_SliderBorderThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isInitializing) return;
            
            Key1_SelectedBorderThickness = Key1_SliderBorderThickness.Value;
            if (Key1_LblBorderThickness != null)
            {
                Key1_LblBorderThickness.Content = Key1_SliderBorderThickness.Value.ToString("F0");
            }
            UpdateKey1Preview();
        }

        private void Key1_BtnTrackMouse_Click(object sender, RoutedEventArgs e)
        {
            if (Key1_BtnTrackMouse.IsChecked == true)
            {
                Key1_BtnTrackMouse.Content = "ON";
                Key2_BtnTrackMouse.IsChecked = false;
                Key2_BtnTrackMouse.Content = "OFF";
                mouseTrackTimer?.Start();
            }
            else
            {
                Key1_BtnTrackMouse.Content = "OFF";
                mouseTrackTimer?.Stop();
                Key1_TxtMousePos.Text = "마우스 위치(F1 입력시 설정): -";
            }
        }

        // Key2 이벤트 핸들러
        private void Key2_CmbFontFamily_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (isInitializing || Key2_CmbFontFamily.SelectedItem == null) return;
            
            var selectedName = Key2_CmbFontFamily.SelectedItem.ToString();
            if (selectedName != null && fontFamilyMap.ContainsKey(selectedName))
            {
                Key2_SelectedFontFamily = fontFamilyMap[selectedName];
                UpdateKey2Preview();
            }
        }

        private void Key2_TxtFontSize_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (isInitializing) return;
            
            if (double.TryParse(Key2_TxtFontSize.Text, out double size) && size > 0)
            {
                Key2_SelectedFontSize = size;
                UpdateKey2Preview();
            }
        }

        private void Key2_FontColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (isInitializing || !Key2_FontColorPicker.SelectedColor.HasValue) return;
            
            Key2_SelectedFontColor = Key2_FontColorPicker.SelectedColor.Value;
            UpdateKey2Preview();
        }

        private void Key2_BorderColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (isInitializing || !Key2_BorderColorPicker.SelectedColor.HasValue) return;
            
            Key2_SelectedBorderColor = Key2_BorderColorPicker.SelectedColor.Value;
            UpdateKey2Preview();
        }

        private void Key2_SliderBorderThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isInitializing) return;
            
            Key2_SelectedBorderThickness = Key2_SliderBorderThickness.Value;
            if (Key2_LblBorderThickness != null)
            {
                Key2_LblBorderThickness.Content = Key2_SliderBorderThickness.Value.ToString("F0");
            }
            UpdateKey2Preview();
        }

        private void Key2_BtnTrackMouse_Click(object sender, RoutedEventArgs e)
        {
            if (Key2_BtnTrackMouse.IsChecked == true)
            {
                Key2_BtnTrackMouse.Content = "ON";
                Key1_BtnTrackMouse.IsChecked = false;
                Key1_BtnTrackMouse.Content = "OFF";
                mouseTrackTimer?.Start();
            }
            else
            {
                Key2_BtnTrackMouse.Content = "OFF";
                mouseTrackTimer?.Stop();
                Key2_TxtMousePos.Text = "마우스 위치(F1 입력시 설정): -";
            }
        }

        private void MouseTrackTimer_Tick(object? sender, EventArgs e)
        {
            var position = System.Windows.Forms.Control.MousePosition;
            if (Key1_BtnTrackMouse.IsChecked == true)
            {
                Key1_TxtMousePos.Text = $"마우스 위치(F1 입력시 설정): {position.X}, {position.Y}";
            }
            else if (Key2_BtnTrackMouse.IsChecked == true)
            {
                Key2_TxtMousePos.Text = $"마우스 위치(F1 입력시 설정): {position.X}, {position.Y}";
            }
        }

        private void FontSettingsWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F1)
            {
                if (Key1_BtnTrackMouse.IsChecked == true)
                {
                    // 현재 마우스 위치를 Key1 카운터 위치에 설정
                    var position = System.Windows.Forms.Control.MousePosition;
                    Key1_TxtPosX.Text = position.X.ToString();
                    Key1_TxtPosY.Text = position.Y.ToString();
                    
                    // 버튼을 OFF로 전환
                    Key1_BtnTrackMouse.IsChecked = false;
                    Key1_BtnTrackMouse.Content = "OFF";
                    mouseTrackTimer?.Stop();
                    Key1_TxtMousePos.Text = "마우스 위치(F1 입력시 설정): -";
                    
                    e.Handled = true;
                }
                else if (Key2_BtnTrackMouse.IsChecked == true)
                {
                    // 현재 마우스 위치를 Key2 카운터 위치에 설정
                    var position = System.Windows.Forms.Control.MousePosition;
                    Key2_TxtPosX.Text = position.X.ToString();
                    Key2_TxtPosY.Text = position.Y.ToString();
                    
                    // 버튼을 OFF로 전환
                    Key2_BtnTrackMouse.IsChecked = false;
                    Key2_BtnTrackMouse.Content = "OFF";
                    mouseTrackTimer?.Stop();
                    Key2_TxtMousePos.Text = "마우스 위치(F1 입력시 설정): -";
                    
                    e.Handled = true;
                }
            }
        }

        private void UpdateKey1Preview()
        {
            if (Key1_PreviewText == null) return;
            
            Key1_PreviewText.FontFamily = Key1_SelectedFontFamily;
            Key1_PreviewText.FontSize = Key1_SelectedFontSize;
            Key1_PreviewText.Foreground = new SolidColorBrush(Key1_SelectedFontColor);
            
            if (Key1_SelectedBorderThickness > 0)
            {
                Key1_PreviewText.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Key1_SelectedBorderColor,
                    Direction = 0,
                    ShadowDepth = 0,
                    BlurRadius = Key1_SelectedBorderThickness * 2,
                    Opacity = 1
                };
            }
            else
            {
                Key1_PreviewText.Effect = null;
            }
        }

        private void UpdateKey2Preview()
        {
            if (Key2_PreviewText == null) return;
            
            Key2_PreviewText.FontFamily = Key2_SelectedFontFamily;
            Key2_PreviewText.FontSize = Key2_SelectedFontSize;
            Key2_PreviewText.Foreground = new SolidColorBrush(Key2_SelectedFontColor);
            
            if (Key2_SelectedBorderThickness > 0)
            {
                Key2_PreviewText.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Key2_SelectedBorderColor,
                    Direction = 0,
                    ShadowDepth = 0,
                    BlurRadius = Key2_SelectedBorderThickness * 2,
                    Opacity = 1
                };
            }
            else
            {
                Key2_PreviewText.Effect = null;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            // Key1 값 저장
            if (int.TryParse(Key1_TxtPosX.Text, out int key1PosX))
                Key1_CounterPosX = key1PosX;
            if (int.TryParse(Key1_TxtPosY.Text, out int key1PosY))
                Key1_CounterPosY = key1PosY;
            if (int.TryParse(Key1_TxtStartTime.Text, out int key1StartTime) && key1StartTime > 0)
                Key1_StartTime = key1StartTime;
            if (Key1_CmbTriggerKey.SelectedItem is KeyMapping key1Mapping)
                Key1_TriggerKey = key1Mapping.ActualKey;

            // Key2 값 저장
            if (int.TryParse(Key2_TxtPosX.Text, out int key2PosX))
                Key2_CounterPosX = key2PosX;
            if (int.TryParse(Key2_TxtPosY.Text, out int key2PosY))
                Key2_CounterPosY = key2PosY;
            if (int.TryParse(Key2_TxtStartTime.Text, out int key2StartTime) && key2StartTime > 0)
                Key2_StartTime = key2StartTime;
            if (Key2_CmbTriggerKey.SelectedItem is KeyMapping key2Mapping)
                Key2_TriggerKey = key2Mapping.ActualKey;

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