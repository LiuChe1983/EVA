using CorePro.ButtonPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{

    public ButtonPro StartBtnPro;

    public ButtonPro LoadGameBtn;
    public ButtonPro ConfigBtn;
    public ButtonPro QuitBtn;

    public LoadGamePanel LoadGamePanel;
    public ConfigPanel ConfigPanel;

    public AudioSource MainAudioSource;

    void Start()
    {
        StartBtnPro.onClick.AddListener(OnStartGame);
        LoadGameBtn.onClick.AddListener(OnLoadGame);
        ConfigBtn.onClick.AddListener(OnConfig);
        QuitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        PlayMainMusic();
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

    public void PlayMainMusic()
    {
        AudioClip clip = null;
        var bgmFullPath = Consts.MUSIC_PATH + "main_menu_bgm";
#if DEV
        // 优先本地加载
        
        clip = LocalResourceLoader.LoadAudioClip(bgmFullPath);
        if (clip == null)
        {
            Debug.LogWarning("本地加载主菜单音乐失败，尝试Resources加载。");
            clip = Resources.Load<AudioClip>(bgmFullPath);
        }
#else
        clip = Resources.Load<AudioClip>(bgmFullPath);
#endif
        if (clip != null)
        {
            MainAudioSource.clip = clip;
            MainAudioSource.loop = true;
            MainAudioSource.Play();
        }
        else
        {
            Debug.LogError("主菜单音乐加载失败！");
        }
    }
}
