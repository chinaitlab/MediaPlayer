namespace MediaPlayer.Core.LibWrappers
{
    public class AcrCloudStatusCode
    {
        public static string HttpError = "{\"status\":{\"msg\":\"Http Error\", \"code\":3000}}";
        public static string NoResult = "{\"status\":{\"msg\":\"No Result\", \"code\":1001}}";
        public static string GenFpError = "{\"status\":{\"msg\":\"Gen Fingerprint Error\", \"code\":2004}}";
        public static string DecodeAudioError = "{\"status\":{\"msg\":\"Can not decode audio data\", \"code\":2005}}";
        public static string RecordError = "{\"status\":{\"msg\":\"Record Error\", \"code\":2000}}";
        public static string JsonError = "{\"status\":{\"msg\":\"json error\", \"code\":2002}}";
    }
}