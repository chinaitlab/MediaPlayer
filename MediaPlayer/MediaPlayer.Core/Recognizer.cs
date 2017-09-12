using System.Collections.Generic;
using MediaPlayer.Core.LibWrappers;
using Newtonsoft.Json;

namespace MediaPlayer.Core
{
    public static class Recognizer
    {
        private static AcrCloudRecognizer _recognizer;

        private static Dictionary<string, object> GetConfig()
        {
            var config = new Dictionary<string, object>
            {
                {"host", "identify-eu-west-1.acrcloud.com"},
                {"access_key", "a1f32f3435a8f12ad5fb610dae6d6ca6"},
                {"access_secret", "RIi1mZ13toI871maPhunShCwGbxdIM3UdHKiArY7"},
                {"timeout", 10}
            };
            return config;
        }

        private static AcrCloudRecognizer Reco => _recognizer ?? (_recognizer = new AcrCloudRecognizer(GetConfig()));

        public static AudioInfo GetInfo(string fileName)
        {
            var json = Reco.RecognizeByFile(fileName, 0);
            return JsonConvert.DeserializeObject<AudioInfo>(json);
        }
    }
}