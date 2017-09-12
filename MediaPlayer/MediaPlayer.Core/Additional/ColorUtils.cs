using System;
using System.Collections.Generic;
using System.Drawing;
using Color = System.Windows.Media.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace MediaPlayer.Core.Additional
{
    /// <summary>
    /// Пока что не будем использовать unsafe код, тем более что прирост тут будет не особо большой
    /// Хотя не факт, на будущее:
    /// todo - заменить на unsafe получение пикселя (возможно даст прирост в скорости 10-15%)
    /// </summary>
    public class ColorUtils
    {
        public static Dictionary<int, uint> GetColorHueHistogram(Bitmap bmp, float saturationThreshold, float brightnessThreshold)
        {
            var colorHueHistorgram = new Dictionary<int, uint>();
            for (var i = 0; i <= 360; i++)
                colorHueHistorgram.Add(i, 0);

            for (var i = 0; i < bmp.Height - 1; ++i)
            {
                for (var j = 0; j < bmp.Width - 1; ++j)
                {
                    var data = bmp.GetPixel(j, i);
                    var clr = Color.FromArgb(data.A, data.R, data.G, data.B);

                    if (!(data.GetSaturation() > saturationThreshold) ||
                        !(data.GetBrightness() > brightnessThreshold)) continue;
                    var hue = (int)Math.Round(data.GetHue(), 0);
                    colorHueHistorgram[hue]++;
                }
            }
            return colorHueHistorgram;
        }

        public static Color GetAverageRgbColor(Bitmap bmp)
        {
            var totalRed = 0;
            var totalGreen = 0;
            var totalBlue = 0;

            for (var i = 0; i < bmp.Height; ++i)
            {
                for (var j = 0; j < bmp.Width; ++j)
                {
                    var data = bmp.GetPixel(j, i);
                    var clr = Color.FromArgb(data.A, data.R, data.G, data.B);
                    totalRed += clr.R;
                    totalGreen += clr.G;
                    totalBlue += clr.B;
                }
            }

            var totalPixels = bmp.Width * bmp.Height;
            var avgRed = (byte)(totalRed / totalPixels);
            var avgGreen = (byte)(totalGreen / totalPixels);
            var avgBlue = (byte)(totalBlue / totalPixels);
            return Color.FromArgb(255, avgRed, avgGreen, avgBlue);
        }

        private static int CorrectHueIndex(int hue)
        {
            var result = hue;
            if (result > 360)
                result = result - 360;
            if (result < 0)
                result = result + 360;
            return result;
        }

        public static Color ColorFromHsv(double hue, double saturation, double value)
        {
            var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            var f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            var v = Convert.ToByte(value);
            var p = Convert.ToByte(value * (1 - saturation));
            var q = Convert.ToByte(value * (1 - f * saturation));
            var t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0:
                    return Color.FromArgb(255, v, t, p);
                case 1:
                    return Color.FromArgb(255, q, v, p);
                case 2:
                    return Color.FromArgb(255, p, v, t);
                case 3:
                    return Color.FromArgb(255, p, q, v);
                case 4:
                    return Color.FromArgb(255, t, p, v);
                default:
                    return Color.FromArgb(255, v, p, q);
            }
        }

        public static Dictionary<int, uint> SmoothHistogram(Dictionary<int, uint> colorHueHistogram, int smoothFactor)
        {
            if (smoothFactor < 0 || smoothFactor > 360)
                throw new ArgumentException("smoothFactor may not be negative or bigger then 360", nameof(smoothFactor));
            if (smoothFactor == 0)
                return new Dictionary<int, uint>(colorHueHistogram);

            var newHistogram = new Dictionary<int, uint>();
            var totalNrColumns = (smoothFactor * 2) + 1;
            for (var i = 0; i <= 360; i++)
            {
                uint sum = 0;
                for (var x = i - smoothFactor; x <= i + smoothFactor; x++)
                {
                    var hueIndex = CorrectHueIndex(x);
                    sum += colorHueHistogram[hueIndex];
                }
                var average = (uint)(sum / totalNrColumns);
                newHistogram[i] = average;
            }
            return newHistogram;
        }

        public static byte GetBitsPerPixel(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return 24;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    return 32;
                default:
                    throw new ArgumentException("Only 24 and 32 bit images are supported");

            }
        }
    }
}