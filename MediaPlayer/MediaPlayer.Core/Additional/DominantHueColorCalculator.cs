using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Color = System.Windows.Media.Color;

namespace MediaPlayer.Core.Additional
{
    public static class DominantHueColorCalculator
    {
        private const float SaturationThreshold = 0.3f;
        private const float BrightnessThreshold = 0.0f;
        private const int HueSmoothFactor = 4;
        private static Dictionary<int, uint> _hueHistogram;
        private static Dictionary<int, uint> _smoothedHueHistogram;

        public static Dictionary<int, uint> HueHistogram => new Dictionary<int, uint>(_hueHistogram);

        public static Dictionary<int, uint> SmoothedHueHistorgram => new Dictionary<int, uint>(_smoothedHueHistogram);

        private static int GetDominantHue(Dictionary<int, uint> hueHistogram)
        {
            var dominantHue = hueHistogram.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            return dominantHue;
        }

        public static Color CalculateDominantColor(byte[] img)
        {
            Bitmap bitmap;
            using (var ms = new MemoryStream(img))
            {
                ms.Position = 0;
                bitmap = new Bitmap(ms);
            }
            _hueHistogram = ColorUtils.GetColorHueHistogram(bitmap, SaturationThreshold,
                BrightnessThreshold);
            _smoothedHueHistogram = ColorUtils.SmoothHistogram(_hueHistogram, HueSmoothFactor);
            var dominantHue = GetDominantHue(_smoothedHueHistogram);

            return ColorUtils.ColorFromHsv(dominantHue, 1, 1);
        }

        public static Color GetDarkenColor(Color clr)
        {
            const int offset = 50;

            var r = clr.R >= offset ? Convert.ToByte(clr.R - offset) : (byte)0;
            var g = clr.G >= offset ? Convert.ToByte(clr.G - offset) : (byte)0;
            var b = clr.B >= offset ? Convert.ToByte(clr.B - offset) : (byte)0;

            return Color.FromArgb(clr.A, r, g, b);
        }
    }
}