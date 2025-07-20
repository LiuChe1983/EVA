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
    public Button skipButton;
    public Button saveButton;
    public Button loadButton;
    public Button configButton;

    //public string scriptFilePath;
    public VisualNovelScriptInterpreter scriptInterpreter;
    public TypewriterEffect typewriterEffect;
    public LoadGamePanel LoadGamePanel;
    public ConfigPanel ConfigPanel;
    private bool isAutoPlay = false;
    private bool isSkip = false;

    private void Start()
    {
        autoButton.onClick.AddListener(OnAutoButtonClick);
        saveButton.onClick.AddListener(() =>
        {
            StopAutoAndSkip();
            LoadGamePanel.ShowPanel("Save Game", false);
        });

        skipButton.onClick.AddListener(() =>
        {
            if (!isSkip)
            {
                StopAutoAndSkip();
                //skipButton.GetComponentInChildren<TextMeshProUGUI>().text = "停止跳过";
                //更换skipbutton的图标
                skipButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/skip_active");
                StartSkip();
            }
            else
            {
                StopCoroutine(SkipToSavePoint());
                EndSkip();
            }
        });
        loadButton.onClick.AddListener(() =>
        {
            StopAutoAndSkip();
            LoadGamePanel.ShowPanel("Load Game", true);
        });

        configButton.onClick.AddListener(() =>
        {
            StopAutoAndSkip();
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

    private void StopAutoAndSkip()
    {
        if (isSkip)
        {
            StopCoroutine(SkipToSavePoint());
            EndSkip();
        }
        if (isAutoPlay)
        {
            StopAutoPlay();
        }
    }

    private void EndSkip()
    {
        isSkip = false;
        //换回跳过按钮的图标
        skipButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/skip");
    }

    private IEnumerator SkipToSavePoint()
    {
        while (isSkip)
        {
            if (scriptInterpreter.IsSavePoint)
            {
                Debug.Log("SavePoint:EndSkip");
                EndSkip();
            }
            else if (scriptInterpreter.IsScriptEnd)
            {
                Debug.Log("ScriptEnd:EndSkip");
                EndSkip();
                yield break;
            }
            else
            {
                scriptInterpreter.InterpretNextLine();
            }
            yield return new WaitForSeconds(Consts.DEFAULT_SKIP_WAITING_SECONDS);
        }
    }

    private void StartSkip()
    {
        Debug.Log("StartSkip");
        isSkip = true;
        // typewriterEffect.typingSpeed = Consts.SKIP_MODE_TYPING_SPEED;
        StartCoroutine(SkipToSavePoint());
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
            scriptInterpreter.IsSavePoint = false; // 点击或空格键时，重置存档点状态
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
        if (isSkip)
        {
            // 如果正在跳过，先结束跳过
            StopCoroutine(SkipToSavePoint());
            EndSkip();
        }
        isAutoPlay = !isAutoPlay;

        //autoButton.GetComponentInChildren<TextMeshProUGUI>().text = isAutoPlay ? "停止自动" : "自动播放";

        if (isAutoPlay)
        {
            // 更换autoButton的图标
            autoButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/auto_active");
            StartCoroutine(StartAutoPlay());
        }
        else
        {
            StopAutoPlay();
        }
    }

    private IEnumerator StartAutoPlay()
    {
        while (isAutoPlay)
        {
            if (scriptInterpreter.IsSavePoint)
            {
                // 如果遇到存档点，停止自动播放
                StopAutoPlay();
                yield break;
            }
            if (scriptInterpreter.IsScriptEnd)
            {
                // 如果脚本已经结束，停止自动播放
                StopAutoPlay();
                yield break;
            }
            if (!typewriterEffect.IsTyping)
            {
                scriptInterpreter.InterpretNextLine();
            }
            yield return new WaitForSeconds(Consts.DEFAULT_AUTO_WAITING_SECONDS);
        }
    }

    void StopAutoPlay()
    {
        if (isAutoPlay)
        {
            StopCoroutine(StartAutoPlay());
            isAutoPlay = false;
            // 更换autoButton的图标
            autoButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/auto");
        }
    }

    void DisplayDialogueLine(string content)
    {
        if (isSkip)
        {
            // 如果正在跳过，直接显示完整内容
            typewriterEffect.CompleteLine(content);
        }
        else
        {
            // 使用打字机效果 显示对话内容
            typewriterEffect.StartTyping(content, Consts.DEFAULT_TYPING_SPEED);
        }
        
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
        if(ConfigPanel.gameObject.activeSelf)
        {
            // 如果配置面板正在显示，忽略鼠标点击
            return false;
        }
        return true;
    }
}