using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Un4seen.Bass;

namespace MediaRecognizer.Core.FFT
{
    public class Spectrum
    {
        private float _time;
        private List<float[]> _data = new List<float[]>();

        public Bitmap GetBitmapSpectrum(string filename, FftSize size, float fps, int maxwidth, int startscan = 0)
        {
            Bitmap image;
            GetFftData(filename, size, fps, maxwidth);

            var heigth = (int)size;
            var width = _data.Count;
            if (width == 0)
            {
                image = new Bitmap(1, 1);
                return image;
            }
            image = new Bitmap(width, heigth);
            float min = 9999;
            float max = -9999;
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < heigth; y++)
                {
                    if (_data[x][y] > max)
                    {
                        max = _data[x][y];
                    }
                    else
                    {
                        if (_data[x][y] < min)
                        {
                            min = _data[x][y];
                        }
                    }
                }
            }
            var amin = Math.Abs(min);
            var kor = 1 / (max + amin);
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < heigth; y++)
                {
                    _data[x][y] = _data[x][y] + amin;
                    _data[x][y] = _data[x][y] * kor * 16 * 50;
                    _data[x][y] = _data[x][y] * _data[x][y];
                }
            }
            for (var x = 0; x < width; x++)
            {
                double dd = 0;
                double dd2 = 0;
                for (var y = 0; y < heigth; y++)
                {
                    var d = Math.Ceiling(_data[x][y]);
                    if (d > 255)
                        d = 255;
                    if (d < 0)
                        d = 0;
                    dd = dd * 100 + d; dd = dd / 101;
                    dd2 = dd2 * 10 + d; dd2 = dd2 / 11;
                    var d1 = (byte)d;
                    var d2 = Math.Ceiling(dd2);
                    var d3 = (byte)d2;
                    var d4 = (byte)Math.Ceiling(dd);
                    double dd3 = d4 + d3 + d1;
                    dd3 = dd3 / 3;
                    var d5 = (byte)Math.Ceiling(dd3);

                    var c = Color.FromArgb(255, d3, d4, d5);
                    image.SetPixel(x, y, c);
                }
            }
            return image;
        }

        public float[] GetRawSpectrData(string filename, FftSize size, float fps, int maxwidth, int startscan = 0)
        {
            GetFftData(filename, size, fps, maxwidth, startscan);
            var spectr = DoScanSpectr(ref _data);
            return spectr;
        }

        public float[] DoScanSpectr(ref List<float[]> data)
        {
            var rawSpectr = new float[1024];
            foreach (var spectrumArrayRight in data)
            {
                if (spectrumArrayRight == null) continue;
                var pik = 1F;
                var max = -spectrumArrayRight.Max();
                var maf = 0.05F / max;
                for (var i = 0; i < spectrumArrayRight.Length; i++)
                {
                    var ddd = i / 1023F;
                    pik = pik + ddd / (pik / 2F);
                    var k = (spectrumArrayRight[i]) * 4 * pik;
                    k = k * maf;
                    if (k > 0.9) { k = 0.9F; }
                    if (k > 0.0005)
                        rawSpectr[i] = (rawSpectr[i] * 5500 + k) / 5501;
                    if (k > 0.05)
                        rawSpectr[i] = (rawSpectr[i] * 100 + k) / 101;
                    if (k > 0.1)
                        rawSpectr[i] = (rawSpectr[i] * 70 + k) / 71;
                    if (k > 0.3)
                        rawSpectr[i] = (rawSpectr[i] * 30 + k) / 31;
                    if (k > 0.5)
                        rawSpectr[i] = (rawSpectr[i] * 15 + k) / 16;
                    if (k > 0.9)
                        rawSpectr[i] = (rawSpectr[i] * 10 + k) / 11;
                }
                var ffts = new float[5];
                for (var i = 2; i < rawSpectr.Length - 2; i += 1)
                {
                    ffts[0] = rawSpectr[i - 2] / 4F;
                    ffts[1] = rawSpectr[i + 2] / 4F;
                    ffts[2] = rawSpectr[i - 1] / 2F;
                    ffts[3] = rawSpectr[i + 1] / 2F;
                    ffts[4] = rawSpectr[i];
                    rawSpectr[i] = ffts[0] + ffts[1] + ffts[2] + ffts[3] + ffts[4];
                    rawSpectr[i] = rawSpectr[i] / 3F;
                }
            }
            var maxx = rawSpectr.Max();
            var maff = 5F / maxx;
            for (var i = 0; i < rawSpectr.Length; i++)
                rawSpectr[i] = rawSpectr[i] * maff;
            return rawSpectr;
        }

        public List<float[]> GetFftData(string filename, FftSize size, float fps, int maxwidth, int startscan = 0)
        {
            var fftType = BASSData.BASS_DATA_FFT256;
            if (size == FftSize.Fft128) fftType = BASSData.BASS_DATA_FFT256;
            if (size == FftSize.Fft256) fftType = BASSData.BASS_DATA_FFT512;
            if (size == FftSize.Fft512) fftType = BASSData.BASS_DATA_FFT1024;
            if (size == FftSize.Fft1024) fftType = BASSData.BASS_DATA_FFT2048;
            if (size == FftSize.Fft2048) fftType = BASSData.BASS_DATA_FFT4096;
            if (size == FftSize.Fft4096) fftType = BASSData.BASS_DATA_FFT8192;
            if (size == FftSize.Fft8192) fftType = BASSData.BASS_DATA_FFT16384;
            var arlen = (int)size;
            var len = (int)fftType;

            GC.Collect();
            _time = 0;
            _data.Clear();
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_NOSPEAKER, IntPtr.Zero);
            // create the stream
            var chan = Bass.BASS_StreamCreateFile(filename, 0, 0,
                BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE);
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0);
            var pos = Bass.BASS_ChannelGetLength(chan);
            Bass.BASS_ChannelBytes2Seconds(chan, pos);
            long bytePos = 0;
            var n = 0;
            while (bytePos < pos)
            {
                n++;

                if (_data.Count > maxwidth)
                    break;
                if (n > startscan)
                {
                    bytePos = Bass.BASS_ChannelSeconds2Bytes(chan, _time);
                    Bass.BASS_ChannelSetPosition(chan, bytePos, BASSMode.BASS_POS_BYTES);

                    var fft = new float[arlen];
                    Bass.BASS_ChannelGetData(chan, fft, len);
                    _data.Add(fft);
                }
                _time += 1F / fps;
            }
            Bass.BASS_ChannelPause(chan);
            Bass.BASS_Stop();
            Bass.BASS_StreamFree(chan);
            GC.Collect();
            return _data;
        }
    }
}