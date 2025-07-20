using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Button StartGameBtn;
    public Button LoadGameBtn;
    public Button ConfigBtn;
    public Button QuitBtn;

    public LoadGamePanel LoadGamePanel;
    public ConfigPanel ConfigPanel;

    void Start()
    {
        StartGameBtn.onClick.AddListener(OnStartGame);
        LoadGameBtn.onClick.AddListener(OnLoadGame);
        ConfigBtn.onClick.AddListener(OnConfig);
        QuitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }


    void OnStartGame()
    {
        SaveSystem.LoadedSaveData = new SaveData(); // ��ʼ����ǰ��Ϸ����
        SaveSystem.CurrentGameProgress = new SaveData(); // ���õ�ǰ��Ϸ����Ϊ�½��Ĵ浵����
        SceneController.Instance.SetState("PlayScene");
    }

    void OnLoadGame()
    {
        if (LoadGamePanel != null)
        {
            LoadGamePanel.ShowPanel("��ȡ�浵", true);
        }
    }

    void OnConfig()
    {
        if (ConfigPanel != null)
            ConfigPanel.ShowPanel();
    }
}
