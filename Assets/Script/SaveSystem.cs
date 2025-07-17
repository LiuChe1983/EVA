using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    private static SaveData currentGameProgress;

    // ��̬�ֶΣ����ڴ洢���صĴ浵����
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
            Debug.LogError("��ǰ��Ϸ����Ϊ�գ��޷����棡");
            return false;
        }
        currentGameProgress.saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string jsonData = JsonUtility.ToJson(SaveSystem.currentGameProgress, true);
        string saveFilePath = GetSaveFilePath(slotIndex);
        File.WriteAllText(saveFilePath, jsonData);
        Debug.Log("��Ϸ�ѱ��浽: " + saveFilePath);
        return true;
    }

    // �޸ĺ��LoadGame������֧��ָ����λ
    public SaveData LoadGame(int slotIndex)
    {
        string saveFilePath = GetSaveFilePath(slotIndex);
        if (File.Exists(saveFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(saveFilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(jsonData);
                Debug.Log("�ɹ����ش浵: " + saveFilePath);
                LoadedSaveData = data;
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError("���ش浵ʧ��: " + e.Message);
                LoadedSaveData = null;
                return null;
            }
        }
        else
        {
            Debug.Log("û���ҵ��浵�ļ�: " + saveFilePath);
            LoadedSaveData = null;
            return null;
        }
    }
}

[System.Serializable]
public class SaveData
{
    public int currentLineIndex;

    // ��¼ÿ����������������
    public string lastBGCommand;
    public string lastChar1Command;
    public string lastChar2Command;
    public string lastChar3Command;
    public string lastBGMCommand;
    public string lastCGCommand;
    public string lastSFXCommand;

    // ��ɫ����״̬
    public bool isChar1Active;
    public bool isChar2Active;
    public bool isChar3Active;

    public string saveTime; // �浵ʱ��
}