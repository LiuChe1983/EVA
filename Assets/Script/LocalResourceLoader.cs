using System.IO;
using UnityEngine;

public static class LocalResourceLoader
{
    /// <summary>
    /// 从本地dev目录加载图片，支持png/jpeg
    /// </summary>
    public static Sprite LoadSprite(string relativePath, int pixelsPerUnit = 100)
    {
        // 支持png和jpeg
        string[] exts = { ".png", ".jpeg" };
        foreach (var ext in exts)
        {
            string pathWithExt = relativePath.EndsWith(ext, System.StringComparison.OrdinalIgnoreCase)
                ? relativePath
                : relativePath + ext;

            string executableDir = Path.GetDirectoryName(Application.dataPath);
            string fullPath = Path.Combine(executableDir, "dev/" + pathWithExt);

            if (!File.Exists(fullPath))
                continue;

            try
            {
                byte[] imageBytes = File.ReadAllBytes(fullPath);
                Texture2D texture = new Texture2D(2, 2);
                if (!texture.LoadImage(imageBytes))
                {
                    Debug.LogError("无法加载图片数据到Texture2D: " + fullPath);
                    continue;
                }
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            }
            catch (IOException e)
            {
                Debug.LogError($"读取图片时发生错误: {e.Message}");
            }
        }
        Debug.LogWarning($"本地图片未找到: {relativePath}");
        return null;
    }

    /// <summary>
    /// 从本地dev目录加载音频，支持wav/mp3/ogg
    /// </summary>
    public static AudioClip LoadAudioClip(string relativePath)
    {
        string[] exts = { ".wav", ".mp3", ".ogg" };
        foreach (var ext in exts)
        {
            string pathWithExt = relativePath.EndsWith(ext, System.StringComparison.OrdinalIgnoreCase)
                ? relativePath
                : relativePath + ext;

            string executableDir = Path.GetDirectoryName(Application.dataPath);
            string fullPath = Path.Combine(executableDir, "dev/" + pathWithExt);

            if (!File.Exists(fullPath))
                continue;

#pragma warning disable 0618
            WWW www = new WWW("file://" + fullPath);
            while (!www.isDone) { }
            if (string.IsNullOrEmpty(www.error) && www.GetAudioClip() != null)
            {
                return www.GetAudioClip();
            }
#pragma warning restore 0618
        }
        Debug.LogWarning($"本地音频未找到: {relativePath}");
        return null;
    }
}