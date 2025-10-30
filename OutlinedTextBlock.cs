using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace MozaCounter
{
    public class OutlinedTextBlock : FrameworkElement
    {
        private FormattedText? formattedText;
        private Geometry? textGeometry;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender, OnFormattedTextChanged));

        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
            "FontFamily", typeof(FontFamily), typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(new FontFamily("Arial"), FrameworkPropertyMetadataOptions.AffectsRender, OnFormattedTextChanged));

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize", typeof(double), typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(24.0, FrameworkPropertyMetadataOptions.AffectsRender, OnFormattedTextChanged));

        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill", typeof(Brush), typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke", typeof(Brush), typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof(double), typeof(OutlinedTextBlock),
            new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        private static void OnFormattedTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var outlinedTextBlock = (OutlinedTextBlock)d;
            outlinedTextBlock.CreateFormattedText();
            outlinedTextBlock.InvalidateMeasure();
            outlinedTextBlock.InvalidateVisual();
        }

        private void CreateFormattedText()
        {
            formattedText = new FormattedText(
                Text ?? string.Empty,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                FontSize,
                Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            textGeometry = formattedText.BuildGeometry(new Point(0, 0));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (formattedText == null)
                CreateFormattedText();

            var extraSpace = StrokeThickness * 4;
            return new Size(formattedText!.Width + extraSpace, formattedText.Height + extraSpace);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (textGeometry == null)
                CreateFormattedText();

            if (textGeometry != null)
            {
                var offset = StrokeThickness * 2;
                drawingContext.PushTransform(new TranslateTransform(offset, offset));

                // 테두리 그리기 (테두리가 있을 때만)
                if (StrokeThickness > 0 && Stroke != null)
                {
                    var pen = new Pen(Stroke, StrokeThickness * 2);
                    pen.LineJoin = PenLineJoin.Round;
                    drawingContext.DrawGeometry(null, pen, textGeometry);
                }

                // 채우기 그리기
                drawingContext.DrawGeometry(Fill, null, textGeometry);

                drawingContext.Pop();
            }
        }
    }
}
