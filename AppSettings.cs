using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace MozaCounter
{
    public class KeySettings
    {
        public string FontFamily { get; set; } = "맑은 고딕";
        public double FontSize { get; set; } = 24;
        public byte FontColorR { get; set; } = 255;
        public byte FontColorG { get; set; } = 255;
        public byte FontColorB { get; set; } = 255;
        public double BorderThickness { get; set; } = 2;
        public byte BorderColorR { get; set; } = 0;
        public byte BorderColorG { get; set; } = 0;
        public byte BorderColorB { get; set; } = 0;
        public int CounterPosX { get; set; } = 100;
        public int CounterPosY { get; set; } = 100;
        public int StartTime { get; set; } = 15;
        public string TriggerKey { get; set; } = "Space";

        public Color GetFontColor()
        {
            return Color.FromRgb(FontColorR, FontColorG, FontColorB);
        }

        public void SetFontColor(Color color)
        {
            FontColorR = color.R;
            FontColorG = color.G;
            FontColorB = color.B;
        }

        public Color GetBorderColor()
        {
            return Color.FromRgb(BorderColorR, BorderColorG, BorderColorB);
        }

        public void SetBorderColor(Color color)
        {
            BorderColorR = color.R;
            BorderColorG = color.G;
            BorderColorB = color.B;
        }
    }

    public class AppSettings
    {
        public KeySettings Key1 { get; set; } = new KeySettings();
        public KeySettings Key2 { get; set; } = new KeySettings 
        { 
            CounterPosX = 200, 
            CounterPosY = 200,
            TriggerKey = "D"
        };
        
        // 키 활성화 상태
        public bool Key1Enabled { get; set; } = true;
        public bool Key2Enabled { get; set; } = true;
        
        // 윈도우 위치
        public double MainWindowLeft { get; set; } = 100;
        public double MainWindowTop { get; set; } = 100;
        public double SettingsWindowLeft { get; set; } = 150;
        public double SettingsWindowTop { get; set; } = 150;

        private static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MozaCounter",
            "settings.json"
        );

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설정 불러오기 실패: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(SettingsFilePath)!;
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설정 저장 실패: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
