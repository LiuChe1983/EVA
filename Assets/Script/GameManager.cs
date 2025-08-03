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
    public Image arrow;
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

    void Update()
    {
        // 处理输入和游戏逻辑
        if (scriptInterpreter.IsScriptEnd)
        {
            // 如果脚本已经结束，显示结束界面或其他逻辑
            SceneController.Instance.LoadScene("StartScene");
            return;
        }
        // 如果正在自动播放或跳过，且没有面板激活，则继续处理输入
        if (!IsPanelActive() &&( Input.GetKeyDown(KeyCode.Space) || IsMouseHitting()))
        {
            // 如果是自动播放或跳过状态，停止它们
            if (isAutoPlay)
            {
                StopAutoPlay();
            }
            if (isSkip)
            {
                StopCoroutine(SkipToSavePoint());
                EndSkip();
            }
            if (typewriterEffect.IsTyping)
            {
                // 如果正在打字，直接完成当前行
                typewriterEffect.CompleteLine(speakContent.text);
            }
            else
            {
                // 如果不是自动播放或跳过状态，直接解释下一行
                arrow.gameObject.SetActive(false);
                scriptInterpreter.IsSavePoint = false; // 点击或空格键时，重置存档点状态
                scriptInterpreter.InterpretNextLine();
            }
            // 如果是自动播放或跳过状态，停止它们


            //if (typewriterEffect.IsTyping) {
            //    // 如果正在打字，直接完成当前行
            //    typewriterEffect.CompleteLine(speakContent.text);
            //}
            //else if (isAutoPlay)
            //{
            //    // 如果是自动播放状态，停止自动播放
            //    StopAutoPlay();
            //}
            //else if (isSkip)
            //{
            //    // 如果是跳过状态，结束跳过
            //    StopCoroutine(SkipToSavePoint());
            //    EndSkip();
            //}
            //else
            //{
            //    // 如果不是自动播放或跳过状态，直接解释下一行
            //    arrow.gameObject.SetActive(false);
            //    scriptInterpreter.IsSavePoint = false; // 点击或空格键时，重置存档点状态
            //    scriptInterpreter.InterpretNextLine();
            //}

        }

        if (scriptInterpreter.DialogueLine != null)
        {
            var dialogue = scriptInterpreter.DialogueLine.Value;
            scriptInterpreter.DialogueLine = null; // 清除当前对话行，避免重复处理
            speakerName.text = dialogue.Item1;
            DisplayDialogueLine(dialogue.Item2);
        }

    }
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

        //DEV标识符下使用InitializeDev方法
#if DEV
        Debug.Log("DEV mode is enabled. Initializing with full story path.");
        scriptInterpreter.InitializeDev(Consts.DEV_STORY_PATH);
#else
        string scriptFilePath = Consts.STORY_PATH + "01";
        scriptInterpreter.Initialize(scriptFilePath);
#endif


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



    void OnAutoButtonClick()
    {
        if (isSkip)
        {
            // 如果正在跳过，先结束跳过
            StopCoroutine(SkipToSavePoint());
            EndSkip();
        }
       // isAutoPlay = !isAutoPlay;

        //autoButton.GetComponentInChildren<TextMeshProUGUI>().text = isAutoPlay ? "停止自动" : "自动播放";

        if (isAutoPlay)
        {
            StopAutoPlay();
        }
        else
        {
            isAutoPlay = true;
            // 更换autoButton的图标
            autoButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/auto_active");
            StartCoroutine(StartAutoPlay());
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
            arrow.gameObject.SetActive(true);
            // 使用打字机效果 显示对话内容
            typewriterEffect.StartTyping(content, Consts.DEFAULT_TYPING_SPEED);
        }
        
    }

    bool IsPanelActive()
    {
        // 检查是否有面板正在显示
        return LoadGamePanel.gameObject.activeSelf || ConfigPanel.gameObject.activeSelf;
    }
    bool IsMouseHitting()
    {
        if(!Input.GetMouseButtonDown(0)) return false;
        if (RectTransformUtility.RectangleContainsScreenPoint(
            bottomButtons.GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
        { return false; }
        return true;
    }
}