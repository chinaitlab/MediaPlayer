using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace MediaPlayer.Core.LibWrappers
{
    public class AcrCloudRecognizer
    {
        public enum RecognizerType
        {
            AcrRecTypeAudio, AcrRecTypeHumming, AcrRecTypeBoth
        };
        private readonly string _host = "";
        private readonly string _accessKey = "";
        private readonly string _accessSecret = "";
        private readonly int _timeout = 5 * 1000; // ms
        private readonly RecognizerType _recType = RecognizerType.AcrRecTypeAudio;
        private readonly AcrCloudExtrTool _acrTool = new AcrCloudExtrTool();

        public AcrCloudRecognizer(IDictionary<string, object> config)
        {
            if (config.ContainsKey("host"))
                _host = (string)config["host"];
            if (config.ContainsKey("access_key"))
                _accessKey = (string)config["access_key"];
            if (config.ContainsKey("access_secret"))
                _accessSecret = (string)config["access_secret"];
            if (config.ContainsKey("timeout"))
                _timeout = 1000 * (int)config["timeout"];
            if (config.ContainsKey("rec_type"))
                _recType = (RecognizerType)config["rec_type"];
        }

        public string Recognize(byte[] wavAudioBuffer, int wavAudioBufferLen)
        {
            byte[] extFp;
            byte[] humFp;
            var queryData = new Dictionary<string, object>();
            switch (_recType)
            {
                case RecognizerType.AcrRecTypeAudio:
                    extFp = _acrTool.CreateFingerprint(wavAudioBuffer, wavAudioBufferLen, false);
                    queryData.Add("ext_fp", extFp);
                    break;
                case RecognizerType.AcrRecTypeHumming:
                    humFp = _acrTool.CreateHummingFingerprint(wavAudioBuffer, wavAudioBufferLen);
                    queryData.Add("hum_fp", humFp);
                    break;
                case RecognizerType.AcrRecTypeBoth:
                    extFp = _acrTool.CreateFingerprint(wavAudioBuffer, wavAudioBufferLen, false);
                    queryData.Add("ext_fp", extFp);
                    humFp = _acrTool.CreateHummingFingerprint(wavAudioBuffer, wavAudioBufferLen);
                    queryData.Add("hum_fp", humFp);
                    break;
                default:
                    return AcrCloudStatusCode.NoResult;
            }

            return DoRecognize(queryData);
        }

        public string RecognizeByFile(string filePath, int startSeconds)
        {
            byte[] extFp = null;
            byte[] humFp = null;
            var queryData = new Dictionary<string, object>();
            try
            {
                switch (_recType)
                {
                    case RecognizerType.AcrRecTypeAudio:
                        extFp = _acrTool.CreateFingerprintByFile(filePath, startSeconds, 12, false);
                        queryData.Add("ext_fp", extFp);
                        break;
                    case RecognizerType.AcrRecTypeHumming:
                        humFp = _acrTool.CreateHummingFingerprintByFile(filePath, startSeconds, 12);
                        queryData.Add("hum_fp", humFp);
                        break;
                    case RecognizerType.AcrRecTypeBoth:
                        extFp = _acrTool.CreateFingerprintByFile(filePath, startSeconds, 12, false);
                        queryData.Add("ext_fp", extFp);
                        humFp = _acrTool.CreateHummingFingerprintByFile(filePath, startSeconds, 12);
                        queryData.Add("hum_fp", humFp);
                        break;
                    default:
                        return AcrCloudStatusCode.NoResult;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return AcrCloudStatusCode.DecodeAudioError;
            }

            if (extFp == null && humFp == null)
                return AcrCloudStatusCode.NoResult;
            return DoRecognize(queryData);
        }

        public string RecognizeByFileBuffer(byte[] fileBuffer, int fileBufferLen, int startSeconds)
        {
            byte[] extFp = null;
            byte[] humFp = null;
            var queryData = new Dictionary<string, object>();
            try
            {
                switch (_recType)
                {
                    case RecognizerType.AcrRecTypeAudio:
                        extFp = _acrTool.CreateFingerprintByFileBuffer(fileBuffer, fileBufferLen, startSeconds, 12, false);
                        queryData.Add("ext_fp", extFp);
                        break;
                    case RecognizerType.AcrRecTypeHumming:
                        humFp = _acrTool.CreateHummingFingerprintByFileBuffer(fileBuffer, fileBufferLen, startSeconds, 12);
                        queryData.Add("hum_fp", humFp);
                        break;
                    case RecognizerType.AcrRecTypeBoth:
                        extFp = _acrTool.CreateFingerprintByFileBuffer(fileBuffer, fileBufferLen, startSeconds, 12, false);
                        queryData.Add("ext_fp", extFp);
                        humFp = _acrTool.CreateHummingFingerprintByFileBuffer(fileBuffer, fileBufferLen, startSeconds, 12);
                        queryData.Add("hum_fp", humFp);
                        break;
                    default:
                        return AcrCloudStatusCode.NoResult;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return AcrCloudStatusCode.DecodeAudioError;
            }

            if (extFp == null && humFp == null)
                return AcrCloudStatusCode.NoResult;

            return DoRecognize(queryData);
        }

        private string PostHttp(string url, IDictionary<string, Object> postParams)
        {
            string result = null;

            var boundarystr = "acrcloud***copyright***2015***" + DateTime.Now.Ticks.ToString("x");
            var boundary = "--" + boundarystr + "\r\n";
            var endboundary = Encoding.ASCII.GetBytes("--" + boundarystr + "--\r\n\r\n");

            var stringKeyHeader = boundary +
                                  "Content-Disposition: form-data; name=\"{0}\"" +
                                  "\r\n\r\n{1}\r\n";
            var filePartHeader = boundary +
                                 "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                                 "Content-Type: application/octet-stream\r\n\r\n";

            var memStream = new MemoryStream();
            foreach (var item in postParams)
            {
                if (item.Value is string)
                {
                    var tmpStr = string.Format(stringKeyHeader, item.Key, item.Value);
                    var tmpBytes = Encoding.UTF8.GetBytes(tmpStr);
                    memStream.Write(tmpBytes, 0, tmpBytes.Length);
                }
                else if (item.Value is byte[])
                {
                    var header = string.Format(filePartHeader, item.Key, item.Key);
                    var headerbytes = Encoding.UTF8.GetBytes(header);
                    memStream.Write(headerbytes, 0, headerbytes.Length);
                    var sample = (byte[])item.Value;
                    memStream.Write(sample, 0, sample.Length);
                    memStream.Write(Encoding.UTF8.GetBytes("\r\n"), 0, 2);
                }
            }
            memStream.Write(endboundary, 0, endboundary.Length);

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream writer = null;
            StreamReader myReader = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = _timeout;
                request.Method = "POST";
                request.ContentType = "multipart/form-data; boundary=" + boundarystr;

                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);

                writer = request.GetRequestStream();
                writer.Write(tempBuffer, 0, tempBuffer.Length);
                writer.Flush();
                writer.Close();
                writer = null;

                response = (HttpWebResponse)request.GetResponse();
                var stream = response.GetResponseStream();
                if (stream != null)
                    myReader = new StreamReader(stream, Encoding.UTF8);
                if (myReader != null) result = myReader.ReadToEnd();
            }
            catch (WebException e)
            {
                Console.WriteLine("timeout:\n" + e);
                result = AcrCloudStatusCode.HttpError;
            }
            catch (Exception e)
            {
                Console.WriteLine("other excption:" + e);
                result = AcrCloudStatusCode.HttpError;
            }
            finally
            {
                memStream.Close();
                writer?.Close();
                myReader?.Close();
                request?.Abort();
                response?.Close();
            }

            return result;
        }

        private static string EncryptByHmacsha1(string input, string key)
        {
            var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(key));
            var stringBytes = Encoding.UTF8.GetBytes(input);
            var hashedValue = hmac.ComputeHash(stringBytes);
            return EncodeToBase64(hashedValue);
        }

        private static string EncodeToBase64(byte[] input)
        {
            var res = Convert.ToBase64String(input, 0, input.Length);
            return res;
        }

        private string DoRecognize(IDictionary<string, Object> queryData)
        {
            byte[] extFp = null;
            byte[] humFp = null;
            const string method = "POST";
            const string httpUrl = "/v1/identify";
            const string dataType = "fingerprint";
            const string sigVersion = "1";
            var timestamp = ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString();

            var reqUrl = "http://" + _host + httpUrl;

            var sigStr = method + "\n" + httpUrl + "\n" + _accessKey + "\n" + dataType + "\n" + sigVersion + "\n" + timestamp;
            var signature = EncryptByHmacsha1(sigStr, _accessSecret);

            var dict = new Dictionary<string, object> {{"access_key", _accessKey}};
            if (queryData.ContainsKey("ext_fp"))
            {
                extFp = (byte[])queryData["ext_fp"];
                if (extFp != null)
                {
                    dict.Add("sample_bytes", extFp.Length.ToString());
                    dict.Add("sample", extFp);
                }
            }
            if (queryData.ContainsKey("hum_fp"))
            {
                humFp = (byte[])queryData["hum_fp"];
                if (humFp != null)
                {
                    dict.Add("sample_hum_bytes", humFp.Length.ToString());
                    dict.Add("sample_hum", humFp);
                }
            }
            if (extFp == null && humFp == null)
                return AcrCloudStatusCode.NoResult;
            dict.Add("timestamp", timestamp);
            dict.Add("signature", signature);
            dict.Add("data_type", dataType);
            dict.Add("signature_version", sigVersion);

            var res = PostHttp(reqUrl, dict);

            return res;
        }
    }

}