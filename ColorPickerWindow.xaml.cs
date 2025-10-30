using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MozaCounter
{
    public partial class ColorPickerWindow : Window
    {
        private bool isUpdating = false;
        public Color SelectedColor { get; private set; }

        public ColorPickerWindow(Color initialColor)
        {
            InitializeComponent();
            SelectedColor = initialColor;
            InitializeColorPalette();
            InitializeSliders();
        }

        private void InitializeSliders()
        {
            isUpdating = true;
            SliderR.Value = SelectedColor.R;
            SliderG.Value = SelectedColor.G;
            SliderB.Value = SelectedColor.B;
            TxtSliderR.Text = SelectedColor.R.ToString();
            TxtSliderG.Text = SelectedColor.G.ToString();
            TxtSliderB.Text = SelectedColor.B.ToString();
            isUpdating = false;
            UpdatePreview();
        }

        private void InitializeColorPalette()
        {
            // 기본 색상 팔레트 생성
            var basicColors = new[]
            {
                Colors.White, Colors.Black, Colors.Red, Colors.Green, Colors.Blue,
                Colors.Yellow, Colors.Cyan, Colors.Magenta, Colors.Orange, Colors.Purple,
                Colors.Pink, Colors.Brown, Colors.Gray, Colors.LightGray, Colors.DarkGray,
                Colors.Lime, Colors.Navy, Colors.Teal, Colors.Maroon, Colors.Olive,
                Colors.Silver, Colors.Gold, Colors.Crimson, Colors.Indigo, Colors.Violet,
                Colors.Turquoise, Colors.Coral, Colors.Salmon, Colors.Khaki, Colors.Tan,
                Colors.LightBlue, Colors.LightGreen, Colors.LightYellow, Colors.LightPink, Colors.LightCoral,
                Colors.DarkRed, Colors.DarkGreen, Colors.DarkBlue, Colors.DarkOrange, Colors.DarkViolet
            };

            foreach (var color in basicColors)
            {
                var border = new Border
                {
                    Width = 40,
                    Height = 40,
                    Background = new SolidColorBrush(color),
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(2),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                border.MouseLeftButtonDown += (s, e) =>
                {
                    var selectedColor = ((SolidColorBrush)border.Background).Color;
                    isUpdating = true;
                    SliderR.Value = selectedColor.R;
                    SliderG.Value = selectedColor.G;
                    SliderB.Value = selectedColor.B;
                    TxtSliderR.Text = selectedColor.R.ToString();
                    TxtSliderG.Text = selectedColor.G.ToString();
                    TxtSliderB.Text = selectedColor.B.ToString();
                    isUpdating = false;
                    UpdatePreview();
                };

                ColorPalette.Children.Add(border);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isUpdating) return;

            isUpdating = true;
            TxtSliderR.Text = ((int)SliderR.Value).ToString();
            TxtSliderG.Text = ((int)SliderG.Value).ToString();
            TxtSliderB.Text = ((int)SliderB.Value).ToString();
            isUpdating = false;

            UpdatePreview();
        }

        private void TxtSlider_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdating) return;

            var textBox = sender as TextBox;
            if (textBox == null) return;

            if (byte.TryParse(textBox.Text, out byte value))
            {
                isUpdating = true;
                if (textBox == TxtSliderR)
                    SliderR.Value = value;
                else if (textBox == TxtSliderG)
                    SliderG.Value = value;
                else if (textBox == TxtSliderB)
                    SliderB.Value = value;
                isUpdating = false;

                UpdatePreview();
            }
        }

        private void UpdatePreview()
        {
            byte r = (byte)SliderR.Value;
            byte g = (byte)SliderG.Value;
            byte b = (byte)SliderB.Value;
            
            SelectedColor = Color.FromRgb(r, g, b);
            PreviewRect.Fill = new SolidColorBrush(SelectedColor);
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}