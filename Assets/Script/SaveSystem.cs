using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    private static SaveData currentGameProgress;

    // 静态字段，用于存储加载的存档数据
    public static SaveData LoadedSaveData { get; set; }

    public static SaveData CurrentGameProgress { get => currentGameProgress; set => currentGameProgress = value; }
    private string GetSaveFilePath(int slotIndex)
    {
        return Application.persistentDataPath + $"/savegame_{slotIndex}.json";
    }

    public bool SaveGame(int slotIndex)
    {
        if (SaveSystem.currentGameProgress == null)
        {
            Debug.LogError("当前游戏进度为空，无法保存！");
            return false;
        }
        currentGameProgress.saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string jsonData = JsonUtility.ToJson(SaveSystem.currentGameProgress, true);
        string saveFilePath = GetSaveFilePath(slotIndex);
        File.WriteAllText(saveFilePath, jsonData);
        Debug.Log("游戏已保存到: " + saveFilePath);
        return true;
    }

    // 修改后的LoadGame方法，支持指定栏位
    public SaveData LoadGame(int slotIndex)
    {
        string saveFilePath = GetSaveFilePath(slotIndex);
        if (File.Exists(saveFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(saveFilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(jsonData);
                Debug.Log("成功加载存档: " + saveFilePath);
                LoadedSaveData = data;
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError("加载存档失败: " + e.Message);
                LoadedSaveData = null;
                return null;
            }
        }
        else
        {
            Debug.Log("没有找到存档文件: " + saveFilePath);
            LoadedSaveData = null;
            return null;
        }
    }
}

[System.Serializable]
public class SaveData
{
    public int currentLineIndex;

    // 记录每种命令的最后命令行
    public string lastBGCommand;
    public string lastChar1Command;
    public string lastChar2Command;
    public string lastChar3Command;
    public string lastBGMCommand;
    public string lastCGCommand;
    public string lastSFXCommand;

    // 角色激活状态
    public bool isChar1Active;
    public bool isChar2Active;
    public bool isChar3Active;

    public string saveTime; // 存档时间
}