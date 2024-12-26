using MFramework;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UDPServer : MonoBehaviour
{
    public GameObject idleImg;
    public VideoPlayer vp;

    public string ip;
    public int port;
    public float defaultVolume = 0.5f;

    private AudioSource audioSource;

    private MEzUDPServer server;

    private bool isPlaying;

    private void Start()
    {
        Application.targetFrameRate = 60;

        audioSource = vp.GetComponent<AudioSource>();
        audioSource.volume = defaultVolume;

        string path = $"{MSettings.RootPath}/Resources/Settings.json";
        if (!File.Exists(path))
        {
            MSerializationUtility.SaveToJson<Settings>(path, new Settings(), true);
            MLog.Print($"已初始化Settings文件，路径:<{path}>");
        }
        var settings = MSerializationUtility.ReadFromJson<Settings>(path);

        //string PNGpath = $"{MSettings.RootPath}/Resources/{settings.IdlePicturePath}";
        //byte[] imageBytes = File.ReadAllBytes(PNGpath);
        //Texture2D texture = new Texture2D(2, 2); 
        //texture.LoadImage(imageBytes);
        //Sprite img = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //idleImg.GetComponent<Image>().sprite = img;

        //string MP4path = $"{MSettings.RootPath}/Resources/{settings.VideoPath}";
        //vp.url = MP4path;
        //vp.Prepare();

        vp.Prepare();
        server = new MEzUDPServer(ip, settings.Port);
        server.OnReceive += (ep, message) =>
        {
            if (message == settings.PlayCommand)//播放
            {
                vp.time = 0;
                vp.Play();

                idleImg.SetActive(false);
                isPlaying = true;

                MCoroutineManager.Instance.DelayNoRecord(() =>
                {
                    idleImg.SetActive(true);
                    MCoroutineManager.Instance.DelayNoRecord(() =>
                    {
                        vp.Stop();
                        vp.Prepare();

                        isPlaying = false;
                    }, 0.2f);
                }, (float)vp.clip.length);
            }
            else if (message == settings.PauseCommand)//暂停
            {
                if (!isPlaying) return;
                if (!vp.isPaused) vp.Pause();
            }
            else if (message == settings.ContinueCommand)//暂停后的继续
            {
                if (!isPlaying) return;
                if (vp.isPaused) vp.Play();
            }
            else if (message == settings.StopCommand)//停止
            {
                idleImg.SetActive(true);
                MCoroutineManager.Instance.DelayNoRecord(() =>
                {
                    vp.Stop();
                    vp.Prepare();

                    isPlaying = false;
                }, 0.2f);
            }
            else if (message == settings.VolUpCommand)//音量+
            {
                audioSource.volume += 0.05f;
            }
            else if (message == settings.VolDownCommand)//音量-
            {
                audioSource.volume -= 0.05f;
            }
        };
    }
}
