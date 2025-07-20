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
        SaveSystem.LoadedSaveData = new SaveData(); // 初始化当前游戏进度
        SaveSystem.CurrentGameProgress = new SaveData(); // 设置当前游戏进度为新建的存档数据
        SceneController.Instance.SetState("PlayScene");
    }

    void OnLoadGame()
    {
        if (LoadGamePanel != null)
        {
            LoadGamePanel.ShowPanel("读取存档", true);
        }
    }

    void OnConfig()
    {
        if (ConfigPanel != null)
            ConfigPanel.ShowPanel();
    }
}
