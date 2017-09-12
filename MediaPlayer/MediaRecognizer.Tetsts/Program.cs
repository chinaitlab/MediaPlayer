using System.Drawing.Imaging;
using MediaRecognizer.Core.FFT;

namespace MediaRecognizer.Tetsts
{
    internal class Program
    {
        private static void Main()
        {
            var spectrum = new Spectrum();
            var bitmap = spectrum.GetBitmapSpectrum("test.mp3", FftSize.Fft2048, 100, 2000);
            bitmap.Save("test.png", ImageFormat.Png);
        }
    }
}