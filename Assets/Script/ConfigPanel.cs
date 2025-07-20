using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConfigPanel : MonoBehaviour
{
    public TMP_Dropdown winModeDropdown;
    public TMP_Dropdown resolutionDropdown;
    public Button resetButton;

    public Button closeButton;
    public Button backToStartButton;

    private Resolution defaultResolution;

    private Dictionary<int, Resolution> resolutionMap = new Dictionary<int, Resolution>();

    private void Awake()
    {
        defaultResolution = Screen.currentResolution;
        // ��ʼ�������˵�
        InitializeWinModeDropdown();
        InitializeResolutionDropdown();
    }
    private void InitializeWinModeDropdown()
    {
        winModeDropdown.ClearOptions();
        List<string> options = new List<string> { "����ģʽ", "ȫ��ģʽ" };
        winModeDropdown.AddOptions(options);
        winModeDropdown.value = PlayerPrefs.GetInt("WinMode", 1); // Ĭ��ȫ��
        winModeDropdown.onValueChanged.AddListener(OnWinModeChanged);
    }


    private void InitializeResolutionDropdown()
    {
        Debug.Log("Current Resolution:" + Screen.currentResolution.ToString());
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutionMap.Clear();

        // ȥ�ز�ɸѡ�ֱ���
        foreach (Resolution res in Screen.resolutions)
        {
            // ֻ����16:9�ҿ��>=1920�ķֱ���
            if (Mathf.Abs((float)res.width / res.height - 16f / 9f) > 0.01f || res.width < 1920)
                continue;
            if (!resolutionMap.ContainsKey(res.width))
            {
                resolutionMap.Add(res.width, res);
                options.Add($"{res.width} x {res.height}");
            }
        }

        resolutionDropdown.AddOptions(options);

        // ��ȡ��Ȳ���λ�����˵�
        int savedWidth = PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width);
        int selectedIndex = 0;
        int idx = 0;
        foreach (var kvp in resolutionMap)
        {
            if (kvp.Key == savedWidth)
            {
                selectedIndex = idx;
                break;
            }
            idx++;
        }
        resolutionDropdown.value = selectedIndex;
        resolutionDropdown.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }


    private void OnWinModeChanged(int index)
    {
        bool isFullScreen = index == 1; // 1��ʾȫ��ģʽ
        Screen.fullScreen = isFullScreen;
        PlayerPrefs.SetInt("WinMode", index);
        PlayerPrefs.Save();
    }

    private void OnResolutionChanged(int index)
    {
        if (index < 0 || index >= resolutionMap.Count) return;
        int selectedWidth = new List<int>(resolutionMap.Keys)[index];
        Resolution selectedResolution = resolutionMap[selectedWidth];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionWidth", selectedResolution.width);
        PlayerPrefs.Save();
    }


    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.AddListener(HidePanel);
        backToStartButton.onClick.AddListener(OnBackToStart);
        //resetButton����¼������ֱ��ʺʹ���ģʽ����ΪĬ��ֵ
        resetButton.onClick.AddListener(() =>
        {
            PlayerPrefs.DeleteKey("ResolutionWidth");
            PlayerPrefs.DeleteKey("WinMode");
            Screen.SetResolution(defaultResolution.width, defaultResolution.height, false);
            winModeDropdown.value = 1; // ����Ϊȫ��ģʽ
            resolutionDropdown.value = 0; // ����ΪĬ�Ϸֱ���
            PlayerPrefs.SetInt("WinMode", 1);
            PlayerPrefs.SetInt("ResolutionWidth", defaultResolution.width);
            PlayerPrefs.Save();
            InitializeResolutionDropdown(); // ���³�ʼ���ֱ��������˵�
        });
        if (SceneController.Instance.CurrentScene == SceneEnum.StartScene)
        {
            backToStartButton.gameObject.SetActive(false);
        }
        else
        {
            backToStartButton.gameObject.SetActive(true);
        }
    }

    private void OnBackToStart()
    {
        HidePanel();

        SceneController.Instance.SetState(SceneEnum.StartScene);
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
