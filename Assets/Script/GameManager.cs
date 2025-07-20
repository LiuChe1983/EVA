using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI speakContent;
    public GameObject bottomButtons;
    public Button autoButton;
    public Button saveButton;
    public Button loadButton;
    public Button configButton;

    //public string scriptFilePath;
    public VisualNovelScriptInterpreter scriptInterpreter;
    public TypewriterEffect typewriterEffect;
    public LoadGamePanel LoadGamePanel;
    public ConfigPanel ConfigPanel;
    private bool isAutoPlay = false;

    private void Start()
    {
        autoButton.onClick.AddListener(OnAutoButtonClick);
        saveButton.onClick.AddListener(() =>
        {
            //SaveGame();
            LoadGamePanel.ShowPanel("Save Game", false);
        });

        loadButton.onClick.AddListener(() =>
        {
            //LoadGame();
            LoadGamePanel.ShowPanel("Load Game", true);
        });

        configButton.onClick.AddListener(() =>
        {
            ConfigPanel.ShowPanel();
        });
        string scriptFilePath = Consts.STORY_PATH + "01";
        scriptInterpreter.Initialize(scriptFilePath);
        // 通过静态字段判断是否有存档数据
        if (SaveSystem.LoadedSaveData != null)
        {
            // 恢复游戏状态
            scriptInterpreter.RestoreSceneByCommands(SaveSystem.LoadedSaveData);
           
            Debug.Log("游戏已从存档加载");
        }
        scriptInterpreter.InterpretNextLine();


    }

    void Update()
    {
        // 处理输入和游戏逻辑
        if(scriptInterpreter.IsScriptEnd)
        {
            // 如果脚本已经结束，显示结束界面或其他逻辑
           SceneController.Instance.LoadScene("StartScene");
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) || IsMouseHitting())
        {
            scriptInterpreter.InterpretNextLine();
        }
        
        if (scriptInterpreter.DialogueLine != null)
        {
            var dialogue = scriptInterpreter.DialogueLine.Value;
            scriptInterpreter.DialogueLine = null; // 清除当前对话行，避免重复处理
            speakerName.text = dialogue.Item1;
            DisplayDialogueLine(dialogue.Item2);
        }

    }

    void OnAutoButtonClick()
    {
        isAutoPlay = !isAutoPlay;

        //autoButton.GetComponentInChildren<TextMeshProUGUI>().text = isAutoPlay ? "停止自动" : "自动播放";

        if (isAutoPlay) StartCoroutine(StartAutoPlay());
    }

    private IEnumerator StartAutoPlay()
    {
        while (isAutoPlay)
        {
            if (!typewriterEffect.IsTyping)
            {
                scriptInterpreter.InterpretNextLine();
            }
            yield return new WaitForSeconds(Consts.DEFAULT_AUTO_WAITING_SECONDS);
        }
    }

    void DisplayDialogueLine(string content)
    {

        // 显示对话内容
        typewriterEffect.StartTyping(content, Consts.DEFAULT_TYPING_SPEED);
        
    }

    bool IsMouseHitting()
    {
        if(!Input.GetMouseButtonDown(0)) return false;
        if (RectTransformUtility.RectangleContainsScreenPoint(
            bottomButtons.GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
        { return false; }
        if(LoadGamePanel.gameObject.activeSelf)
        {
            // 如果读档面板正在显示，忽略鼠标点击
            return false;
        }
        return true;
    }
    //public void SaveGame()
    //{
    //    saveSystem.SaveGame(scriptInterpreter);
    //}

    //public void LoadGame()
    //{
    //    // 重新加载存档
    //    saveSystem.LoadGame();

    //    if (SaveSystem.LoadedSaveData != null)
    //    {
    //        scriptInterpreter.RestoreState(SaveSystem.LoadedSaveData);
    //        Debug.Log("游戏已从存档加载");
    //    }
    //    else
    //    {
    //        Debug.Log("没有找到存档，开始新游戏");
    //        scriptInterpreter.Initialize(scriptFilePath);
    //        scriptInterpreter.InterpretNextLine();
    //    }
    //}
}