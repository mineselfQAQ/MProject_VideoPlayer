using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class CaptureWindow
    {
        [MenuItem("MFramework/CaptureGameWindow _F6", priority = 906)]
        public static void CaptureGameWindow()
        {
            string savePath = GetSavePath();

            ScreenCapture.CaptureScreenshot(savePath);

            if (!Application.isPlaying)//δ�������
            {
                EditorDelayExecute.Instance.DelayDo(() =>
                {
                    MLog.Print($"�ѽ�ͼ��·����<{savePath}>");
                    savePath = savePath.Replace("/", "\\");
                    System.Diagnostics.Process.Start("explorer", "/select,\"" + savePath + "\"");
                });
            }
            else//�������
            {
                MCoroutineManager.Instance.DelayNoRecord(() =>
                {
                    MLog.Print($"�ѽ�ͼ��·����<{savePath}>");
                    savePath = savePath.Replace("/", "\\");
                    System.Diagnostics.Process.Start("explorer", "/select,\"" + savePath + "\"");
                }, 0.2f);
            }
        }

        [MenuItem("MFramework/CaptureSceneWindow _F7", priority = 907)]
        public static void CaptureSceneWindow()
        {
            string savePath = GetSavePath();

            SceneView sceneView = SceneView.lastActiveSceneView;

            if (sceneView != null)
            {
                RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
                sceneView.camera.targetTexture = renderTexture;
                sceneView.camera.Render();

                Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                RenderTexture.active = renderTexture;//�����renderTexture(������ReadPixels()�ͻ��ȡ)
                screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                screenshot.Apply();

                byte[] bytes = screenshot.EncodeToPNG();
                File.WriteAllBytes(savePath, bytes);

                sceneView.camera.targetTexture = null;
                RenderTexture.active = null;
                GameObject.DestroyImmediate(renderTexture);
                GameObject.DestroyImmediate(screenshot);

                MLog.Print($"�ѽ�ͼ��·����<{savePath}>");
                savePath = savePath.Replace("/", "\\");
                System.Diagnostics.Process.Start("explorer", "/select,\"" + savePath + "\"");
            }
            else
            {
                MLog.Print("��Scene���ڣ�����", MLogType.Warning);
            }
        }

        private static string GetSavePath()
        {
            string time = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"Screenshot_{time}.png";
            string saveFolder = $"{MSettings.TempRootPath}/Screenshots";
            MPathUtility.CreateFolderIfNotExist(saveFolder);
            string savePath = $"{saveFolder}/{fileName}";

            return savePath;
        }
    }
}
