using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Button StartGameBtn;
    public Button LoadGameBtn;
    public Button ConfigBtn;

    public GameObject LoadGamePanel;
    public GameObject ConfigPanel;

    void Start()
    {
        StartGameBtn.onClick.AddListener(OnStartGame);
        LoadGameBtn.onClick.AddListener(OnLoadGame);
        ConfigBtn.onClick.AddListener(OnConfig);
    }

    void OnStartGame()
    {
        SceneController.Instance.SetState("PlayScene");
    }

    void OnLoadGame()
    {
        if (LoadGamePanel != null)
            LoadGamePanel.SetActive(true);
    }

    void OnConfig()
    {
        if (ConfigPanel != null)
            ConfigPanel.SetActive(true);
    }
}
