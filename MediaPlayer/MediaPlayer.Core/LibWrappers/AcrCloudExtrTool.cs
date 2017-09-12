using System;
using System.Runtime.InteropServices;

namespace MediaPlayer.Core.LibWrappers
{
    public class AcrCloudExtrTool
    {
        public AcrCloudExtrTool()
        {
            acr_init();
        }
        public byte[] CreateFingerprint(byte[] pcmBuffer, int pcmBufferLen, bool isDb)
        {
            if (pcmBuffer == null || pcmBufferLen <= 0)
                return null;
            if (pcmBufferLen > pcmBuffer.Length)
                pcmBufferLen = pcmBuffer.Length;
            var tIsDb = (isDb) ? (byte)1 : (byte)0;
            var pFpBuffer = IntPtr.Zero;
            var fpBufferLen = create_fingerprint(pcmBuffer, pcmBufferLen, tIsDb, ref pFpBuffer);
            if (fpBufferLen <= 0)
                return null;

            var fpBuffer = new byte[fpBufferLen];
            Marshal.Copy(pFpBuffer, fpBuffer, 0, fpBufferLen);
            acr_free(pFpBuffer);

            return fpBuffer;
        }

        public byte[] CreateHummingFingerprint(byte[] pcmBuffer, int pcmBufferLen)
        {
            if (pcmBuffer == null || pcmBufferLen <= 0)
                return null;
            if (pcmBufferLen > pcmBuffer.Length)
                pcmBufferLen = pcmBuffer.Length;

            var pFpBuffer = IntPtr.Zero;
            var fpBufferLen = create_humming_fingerprint(pcmBuffer, pcmBufferLen, ref pFpBuffer);
            if (fpBufferLen <= 0)
                return null;

            var fpBuffer = new byte[fpBufferLen];
            Marshal.Copy(pFpBuffer, fpBuffer, 0, fpBufferLen);
            acr_free(pFpBuffer);

            return fpBuffer;
        }
        
        public byte[] CreateFingerprintByFile(string filePath, int startTimeSeconds, int audioLenSeconds, bool isDb)
        {
            var tIsDb = (isDb) ? (byte)1 : (byte)0;
            var pFpBuffer = IntPtr.Zero;
            var fpBufferLen = create_fingerprint_by_file(filePath, startTimeSeconds, audioLenSeconds, tIsDb, ref pFpBuffer);
            switch (fpBufferLen)
            {
                case -1:
                    throw new Exception(filePath + " is not readable!");
                case -2:
                    throw new Exception(filePath + " can not be decoded audio data!");
                case 0:
                    return null;
            }

            var fpBuffer = new byte[fpBufferLen];
            Marshal.Copy(pFpBuffer, fpBuffer, 0, fpBufferLen);
            acr_free(pFpBuffer);

            return fpBuffer;
        }

        public byte[] CreateHummingFingerprintByFile(string filePath, int startTimeSeconds, int audioLenSeconds)
        {
            var pFpBuffer = IntPtr.Zero;
            var fpBufferLen = create_humming_fingerprint_by_file(filePath, startTimeSeconds, audioLenSeconds, ref pFpBuffer);
            switch (fpBufferLen)
            {
                case -1:
                    throw new Exception(filePath + " is not readable!");
                case -2:
                    throw new Exception(filePath + " can not be decoded audio data!");
                case 0:
                    return null;
            }

            var fpBuffer = new byte[fpBufferLen];
            Marshal.Copy(pFpBuffer, fpBuffer, 0, fpBufferLen);
            acr_free(pFpBuffer);

            return fpBuffer;
        }
        
        public byte[] CreateFingerprintByFileBuffer(byte[] fileBuffer, int fileBufferLen, int startTimeSeconds, int audioLenSeconds, bool isDb)
        {
            if (fileBufferLen > fileBuffer.Length)
                fileBufferLen = fileBuffer.Length;

            var tIsDb = (isDb) ? (byte)1 : (byte)0;
            var pFpBuffer = IntPtr.Zero;
            var fpBufferLen = create_fingerprint_by_filebuffer(fileBuffer, fileBufferLen, startTimeSeconds, audioLenSeconds, tIsDb, ref pFpBuffer);
            switch (fpBufferLen)
            {
                case -1:
                    throw new Exception("fileBuffer is not audio/video data!");
                case -2:
                    throw new Exception("fileBuffer can not be decoded audio data!");
                case 0:
                    return null;
            }

            var fpBuffer = new byte[fpBufferLen];
            Marshal.Copy(pFpBuffer, fpBuffer, 0, fpBufferLen);
            acr_free(pFpBuffer);
            return fpBuffer;
        }
        
        public byte[] CreateHummingFingerprintByFileBuffer(byte[] fileBuffer, int fileBufferLen, int startTimeSeconds, int audioLenSeconds)
        {
            if (fileBufferLen > fileBuffer.Length)
                fileBufferLen = fileBuffer.Length;

            var pFpBuffer = IntPtr.Zero;
            var fpBufferLen = create_humming_fingerprint_by_filebuffer(fileBuffer, fileBufferLen, startTimeSeconds, audioLenSeconds, ref pFpBuffer);
            switch (fpBufferLen)
            {
                case -1:
                    throw new Exception("fileBuffer is not audio/video data!");
                case -2:
                    throw new Exception("fileBuffer can not be decoded audio data!");
                case 0:
                    return null;
            }

            var fpBuffer = new byte[fpBufferLen];
            Marshal.Copy(pFpBuffer, fpBuffer, 0, fpBufferLen);
            acr_free(pFpBuffer);
            return fpBuffer;
        }

        public byte[] DecodeAudioByFile(string filePath, int startTimeSeconds, int audioLenSeconds)
        {
            const byte aa = 1;
            acr_set_debug(aa);

            var pAudioBuffer = IntPtr.Zero;
            var fpBufferLen = decode_audio_by_file(filePath, startTimeSeconds, audioLenSeconds, ref pAudioBuffer);
            switch (fpBufferLen)
            {
                case -1:
                    throw new Exception(filePath + " is not readable!");
                case -2:
                    throw new Exception(filePath + " can not be decoded audio data!");
                case 0:
                    return null;
            }

            var audioBuffer = new byte[fpBufferLen];
            Marshal.Copy(pAudioBuffer, audioBuffer, 0, fpBufferLen);
            acr_free(pAudioBuffer);

            return audioBuffer;
        }

        public byte[] DecodeAudioByFileBuffer(byte[] fileBuffer, int fileBufferLen, int startTimeSeconds, int audioLenSeconds)
        {
            if (fileBufferLen > fileBuffer.Length)
                fileBufferLen = fileBuffer.Length;
            var pAudioBuffer = IntPtr.Zero;
            var fpBufferLen = decode_audio_by_filebuffer(fileBuffer, fileBufferLen, startTimeSeconds, audioLenSeconds, ref pAudioBuffer);
            switch (fpBufferLen)
            {
                case -1:
                    throw new Exception("fileBuffer is not audio/video data!");
                case -2:
                    throw new Exception("fileBuffer can not be decoded audio data!");
                case 0:
                    return null;
            }

            var audioBuffer = new byte[fpBufferLen];
            Marshal.Copy(pAudioBuffer, audioBuffer, 0, fpBufferLen);
            acr_free(pAudioBuffer);
            return audioBuffer;
        }
        
        public int GetDurationMillisecondByFile(string filePath)
        {
            return get_duration_ms_by_file(filePath);
        }

        [DllImport("libacrcloud_extr_tool.dll")]
        private static extern int create_fingerprint(byte[] pcmBuffer, int pcmBufferLen, byte isDbFingerprint, ref IntPtr fpsBuffer);
        [DllImport("libacrcloud_extr_tool.dll")]
        private static extern int create_humming_fingerprint(byte[] pcmBuffer, int pcmBufferLen, ref IntPtr fpsBuffer);
        [DllImport("libacrcloud_extr_tool.dll")]
        private static extern int create_fingerprint_by_file(string filePath, int startTimeSeconds, int audioLenSeconds, byte isDbFingerprint, ref IntPtr fpsBuffer);
        [DllImport("libacrcloud_extr_tool.dll")]
        private static extern int create_humming_fingerprint_by_file(string filePath, int startTimeSeconds, int audioLenSeconds, ref IntPtr fpsBuffer);
        [DllImport("libacrcloud_extr_tool.dll")]
        private static extern int create_fingerprint_by_filebuffer(byte[] fileBuffer, int fileBufferLen, int startTimeSeconds, int audioLenSeconds, byte isDbFingerprint, ref IntPtr fpsBuffer);
        [DllImport("libacrcloud_extr_tool.dll")]
        private static extern int create_humming_fingerprint_by_filebuffer(byte[] fileBuffer, int fileBufferLen, int startTimeSeconds, int audioLenSeconds, ref IntPtr fpsBuffer);
        [DllImport("libacrcloud_extr_tool.dll")]
        private static extern int decode_audio_by_file(string filePath, int startTimeSeconds, int audioLenSeconds, ref IntPtr audioBuffer);
        [DllImport("libacrcloud_extr_tool.dll")]
        private static extern int decode_audio_by_filebuffer(byte[] fileBuffer, int fileBufferLen, int startTimeSeconds, int audioLenSeconds, ref IntPtr audioBuffer);
        [DllImport("libacrcloud_extr_tool.dll")]
        private static extern void acr_free(IntPtr buffer);
        [DllImport("libacrcloud_extr_tool.dll")]
        private static extern int get_duration_ms_by_file(string filePath);
        [DllImport("libacrcloud_extr_tool.dll")]
        public static extern void acr_set_debug(byte isDebug);
        [DllImport("libacrcloud_extr_tool.dll")]
        private static extern void acr_init();
    }
}