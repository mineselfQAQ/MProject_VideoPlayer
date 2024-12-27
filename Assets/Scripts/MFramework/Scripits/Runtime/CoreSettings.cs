using System;
using System.Globalization;

namespace MFramework
{
    [Serializable]
    public class CoreSettings
    {
        public string language;

        public float SFXSound;
        public float MusicSound;

        public CoreSettings()
        {
            language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;//当前地区语言
            SFXSound = 0.5f;
            MusicSound = 0.5f;
        }
    }

    [Serializable]
    public class Settings
    {
        public string IP;
        public int Port;
        public string PlayCommand;
        public string ContinueCommand;
        public string PauseCommand;
        public string StopCommand;
        public string VolUpCommand;
        public string VolDownCommand;
        //public string VideoPath;
        //public string IdlePicturePath;

        public Settings()
        {
            IP = "0.0.0.0";//任意IP
            Port = 7419;

            PlayCommand = "play1";
            ContinueCommand = "002#";
            PauseCommand = "001#";
            StopCommand = "006#";
            VolUpCommand = "004#";
            VolDownCommand = "005#";

            //VideoPath = "1.mp4";
            //IdlePicturePath = "0.jpg";
        }
    }
}
