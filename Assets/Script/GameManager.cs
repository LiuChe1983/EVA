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
                //skipButton.GetComponentInChildren<TextMeshProUGUI>().text = "ֹͣ����";
                //����skipbutton��ͼ��
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
        // ͨ����̬�ֶ��ж��Ƿ��д浵����
        if (SaveSystem.LoadedSaveData != null)
        {
            // �ָ���Ϸ״̬
            scriptInterpreter.RestoreSceneByCommands(SaveSystem.LoadedSaveData);
           
            Debug.Log("��Ϸ�ѴӴ浵����");
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
        //����������ť��ͼ��
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
        // �����������Ϸ�߼�
        if(scriptInterpreter.IsScriptEnd)
        {
            // ����ű��Ѿ���������ʾ��������������߼�
           SceneController.Instance.LoadScene("StartScene");
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) || IsMouseHitting())
        {
            scriptInterpreter.IsSavePoint = false; // �����ո��ʱ�����ô浵��״̬
            scriptInterpreter.InterpretNextLine();
        }
        
        if (scriptInterpreter.DialogueLine != null)
        {
            var dialogue = scriptInterpreter.DialogueLine.Value;
            scriptInterpreter.DialogueLine = null; // �����ǰ�Ի��У������ظ�����
            speakerName.text = dialogue.Item1;
            DisplayDialogueLine(dialogue.Item2);
        }

    }

    void OnAutoButtonClick()
    {
        if (isSkip)
        {
            // ��������������Ƚ�������
            StopCoroutine(SkipToSavePoint());
            EndSkip();
        }
        isAutoPlay = !isAutoPlay;

        //autoButton.GetComponentInChildren<TextMeshProUGUI>().text = isAutoPlay ? "ֹͣ�Զ�" : "�Զ�����";

        if (isAutoPlay)
        {
            // ����autoButton��ͼ��
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
                // ��������浵�㣬ֹͣ�Զ�����
                StopAutoPlay();
                yield break;
            }
            if (scriptInterpreter.IsScriptEnd)
            {
                // ����ű��Ѿ�������ֹͣ�Զ�����
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
            // ����autoButton��ͼ��
            autoButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/auto");
        }
    }

    void DisplayDialogueLine(string content)
    {
        if (isSkip)
        {
            // �������������ֱ����ʾ��������
            typewriterEffect.CompleteLine(content);
        }
        else
        {
            // ʹ�ô��ֻ�Ч�� ��ʾ�Ի�����
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
            // ����������������ʾ�����������
            return false;
        }
        if(ConfigPanel.gameObject.activeSelf)
        {
            // ����������������ʾ�����������
            return false;
        }
        return true;
    }
}