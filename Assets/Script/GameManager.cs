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
        // �����������Ϸ�߼�
        if (scriptInterpreter.IsScriptEnd)
        {
            // ����ű��Ѿ���������ʾ��������������߼�
            SceneController.Instance.LoadScene("StartScene");
            return;
        }
        // ��������Զ����Ż���������û����弤��������������
        if (!IsPanelActive() &&( Input.GetKeyDown(KeyCode.Space) || IsMouseHitting()))
        {
            // ������Զ����Ż�����״̬��ֹͣ����
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
                // ������ڴ��֣�ֱ����ɵ�ǰ��
                typewriterEffect.CompleteLine(speakContent.text);
            }
            else
            {
                // ��������Զ����Ż�����״̬��ֱ�ӽ�����һ��
                arrow.gameObject.SetActive(false);
                scriptInterpreter.IsSavePoint = false; // �����ո��ʱ�����ô浵��״̬
                scriptInterpreter.InterpretNextLine();
            }
            // ������Զ����Ż�����״̬��ֹͣ����


            //if (typewriterEffect.IsTyping) {
            //    // ������ڴ��֣�ֱ����ɵ�ǰ��
            //    typewriterEffect.CompleteLine(speakContent.text);
            //}
            //else if (isAutoPlay)
            //{
            //    // ������Զ�����״̬��ֹͣ�Զ�����
            //    StopAutoPlay();
            //}
            //else if (isSkip)
            //{
            //    // ���������״̬����������
            //    StopCoroutine(SkipToSavePoint());
            //    EndSkip();
            //}
            //else
            //{
            //    // ��������Զ����Ż�����״̬��ֱ�ӽ�����һ��
            //    arrow.gameObject.SetActive(false);
            //    scriptInterpreter.IsSavePoint = false; // �����ո��ʱ�����ô浵��״̬
            //    scriptInterpreter.InterpretNextLine();
            //}

        }

        if (scriptInterpreter.DialogueLine != null)
        {
            var dialogue = scriptInterpreter.DialogueLine.Value;
            scriptInterpreter.DialogueLine = null; // �����ǰ�Ի��У������ظ�����
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

        //DEV��ʶ����ʹ��InitializeDev����
#if DEV
        Debug.Log("DEV mode is enabled. Initializing with full story path.");
        scriptInterpreter.InitializeDev(Consts.DEV_STORY_PATH);
#else
        string scriptFilePath = Consts.STORY_PATH + "01";
        scriptInterpreter.Initialize(scriptFilePath);
#endif


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



    void OnAutoButtonClick()
    {
        if (isSkip)
        {
            // ��������������Ƚ�������
            StopCoroutine(SkipToSavePoint());
            EndSkip();
        }
       // isAutoPlay = !isAutoPlay;

        //autoButton.GetComponentInChildren<TextMeshProUGUI>().text = isAutoPlay ? "ֹͣ�Զ�" : "�Զ�����";

        if (isAutoPlay)
        {
            StopAutoPlay();
        }
        else
        {
            isAutoPlay = true;
            // ����autoButton��ͼ��
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
            arrow.gameObject.SetActive(true);
            // ʹ�ô��ֻ�Ч�� ��ʾ�Ի�����
            typewriterEffect.StartTyping(content, Consts.DEFAULT_TYPING_SPEED);
        }
        
    }

    bool IsPanelActive()
    {
        // ����Ƿ������������ʾ
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