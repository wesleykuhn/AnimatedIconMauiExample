using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AnimatedIconMaui.CustomControls
{
    public class AnimDownloadButton : SKCanvasView
    {
        private const string DownloadIconCode = "\uf019";
        private const string CheckIconCode = "\uf00c";
        private const float AnimSpeed = 5f;

        #region [ Download control props ]

        public readonly static BindableProperty IsDownloadingProperty = BindableProperty.Create(
            nameof(IsDownloading),
            typeof(bool),
            typeof(AnimDownloadButton),
            false,
            BindingMode.TwoWay,
            propertyChanged: OnIsDownloadingPropertyChanged);
        public bool IsDownloading
        {
            get => (bool)GetValue(IsDownloadingProperty);
            set => SetValue(IsDownloadingProperty, value);
        }

        public readonly static BindableProperty DownloadedProperty =
            BindableProperty.Create(nameof(Downloaded), typeof(bool), typeof(AnimDownloadButton), false, propertyChanged: OnDownloadedPropertyChanged);
        public bool Downloaded
        {
            get => (bool)GetValue(DownloadedProperty);
            set => SetValue(DownloadedProperty, value);
        }

        public readonly static BindableProperty ClickedCommandProperty =
            BindableProperty.Create(nameof(ClickedCommand), typeof(ICommand), typeof(AnimDownloadButton), null);
        public ICommand ClickedCommand
        {
            get => (ICommand)GetValue(ClickedCommandProperty);
            set => SetValue(ClickedCommandProperty, value);
        }

        #endregion

        #region [ Style props ]

        public readonly static BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(AnimDownloadButton), 0, propertyChanged: OnStylePropertyChanged);
        public int CornerRadius
        {
            get => (int)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public readonly static BindableProperty BackColorProperty =
            BindableProperty.Create(nameof(BackColor), typeof(Color), typeof(AnimDownloadButton), Colors.Black, propertyChanged: OnStylePropertyChanged);
        public Color BackColor
        {
            get => (Color)GetValue(BackColorProperty);
            set => SetValue(BackColorProperty, value);
        }

        public readonly static BindableProperty ForeColorProperty =
            BindableProperty.Create(nameof(ForeColor), typeof(Color), typeof(AnimDownloadButton), Colors.White, propertyChanged: OnStylePropertyChanged);
        public Color ForeColor
        {
            get => (Color)GetValue(ForeColorProperty);
            set => SetValue(ForeColorProperty, value);
        }

        public readonly static BindableProperty DownloadedColorProperty =
            BindableProperty.Create(nameof(DownloadedColor), typeof(Color), typeof(AnimDownloadButton), Colors.Green, propertyChanged: OnStylePropertyChanged);
        public Color DownloadedColor
        {
            get => (Color)GetValue(DownloadedColorProperty);
            set => SetValue(DownloadedColorProperty, value);
        }

        #endregion

        private readonly System.Timers.Timer _frameTimer;
        private readonly SKTypeface _faTypface;

        private float curMaskHeight;
        private SKRect maskRect;

        public AnimDownloadButton()
        {
            _frameTimer = new System.Timers.Timer(33f);
            _frameTimer.Elapsed += FrameTimer_Elapsed;

            var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AnimatedIconMaui.Resources.FontAwesome.FAS.otf");
            _faTypface = SKTypeface.FromStream(resourceStream);

            PaintSurface += AnimDownloadButton_PaintSurface;

            Touch += AnimDownloadButton_Touch;
            EnableTouchEvents = true;

            maskRect = new();
        }

        private static void OnIsDownloadingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisComp = (AnimDownloadButton)bindable;
            var value = (bool)newValue;

            if (!value)
                thisComp._frameTimer.Stop();
            else
                thisComp._frameTimer.Start();

            thisComp.InvalidateSurface();
        }

        private static void OnDownloadedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisComp = (AnimDownloadButton)bindable;
            var value = (bool)newValue;

            if (value)
            {
                thisComp._frameTimer.Stop();
            }

            thisComp.IsDownloading = false;

            thisComp.InvalidateSurface();
        }

        private static void OnStylePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisComp = (AnimDownloadButton)bindable;
            thisComp.InvalidateSurface();
        }

        private void AnimDownloadButton_Touch(object sender, SkiaSharp.Views.Maui.SKTouchEventArgs e)
        {
            ClickedCommand?.Execute(null);
        }

        private void FrameTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            InvalidateSurface();
        }

        private void AnimDownloadButton_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var width = e.Info.Width;
            var height = e.Info.Height;

            canvas.Clear(SKColors.Transparent);

            if (!Downloaded)
            {
                //Background
                using var bgPaint = new SKPaint();
                bgPaint.Style = SKPaintStyle.Fill;
                bgPaint.Color = BackColor.ToSKColor();
                bgPaint.IsAntialias = true;

                //Fill background
                if (CornerRadius > 0)
                    canvas.DrawRoundRect(0f, 0f, width, height, CornerRadius, CornerRadius, bgPaint);
                else
                    canvas.DrawRect(0f, 0f, width, height, bgPaint);

                using var fgPaint = new SKPaint();
                fgPaint.Style = SKPaintStyle.Fill;
                fgPaint.IsAntialias = true;
                fgPaint.Color = ForeColor.ToSKColor();
                fgPaint.TextSize = 500;
                fgPaint.Typeface = _faTypface;

                //Text draw
                SKRect mRect = new();
                fgPaint.MeasureText(DownloadIconCode, ref mRect);
                canvas.DrawText(DownloadIconCode, (width - mRect.Width) / 2, height - (height - mRect.Height), fgPaint);

                if (!IsDownloading)
                    return;

                if (curMaskHeight <= 0)
                    curMaskHeight = height;
                else
                    curMaskHeight = maskRect.Height - AnimSpeed;

                maskRect = new SKRect(0f, height - curMaskHeight, width, height);

                if (CornerRadius > 0)
                    canvas.DrawRoundRect(maskRect, CornerRadius, CornerRadius, bgPaint);
                else
                    canvas.DrawRect(maskRect, bgPaint);
            }
            else
            {
                using var checkPaint = new SKPaint();
                checkPaint.Color = DownloadedColor.ToSKColor();
                checkPaint.TextSize = 500;
                checkPaint.Typeface = _faTypface;
                checkPaint.IsAntialias = true;

                //Drawn the check icon
                SKRect mRect = new();
                checkPaint.MeasureText(CheckIconCode, ref mRect);
                canvas.DrawText(CheckIconCode, (width - mRect.Width) / 2, height - (height * 10 /100), checkPaint);
            }
        }
    }
}
