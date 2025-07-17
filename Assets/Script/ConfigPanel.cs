using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConfigPanel : MonoBehaviour
{
    public Button closeButton;
    public Button backToStartButton;

    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.AddListener(HidePanel);
        backToStartButton.onClick.AddListener(OnBackToStart);
        if(SceneController.Instance.CurrentScene == SceneEnum.StartScene)
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
