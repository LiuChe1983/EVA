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
        // ͨ����̬�ֶ��ж��Ƿ��д浵����
        if (SaveSystem.LoadedSaveData != null)
        {
            // �ָ���Ϸ״̬
            scriptInterpreter.RestoreSceneByCommands(SaveSystem.LoadedSaveData);
           
            Debug.Log("��Ϸ�ѴӴ浵����");
        }
        scriptInterpreter.InterpretNextLine();


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
        isAutoPlay = !isAutoPlay;

        //autoButton.GetComponentInChildren<TextMeshProUGUI>().text = isAutoPlay ? "ֹͣ�Զ�" : "�Զ�����";

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

        // ��ʾ�Ի�����
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
            // ����������������ʾ�����������
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
    //    // ���¼��ش浵
    //    saveSystem.LoadGame();

    //    if (SaveSystem.LoadedSaveData != null)
    //    {
    //        scriptInterpreter.RestoreState(SaveSystem.LoadedSaveData);
    //        Debug.Log("��Ϸ�ѴӴ浵����");
    //    }
    //    else
    //    {
    //        Debug.Log("û���ҵ��浵����ʼ����Ϸ");
    //        scriptInterpreter.Initialize(scriptFilePath);
    //        scriptInterpreter.InterpretNextLine();
    //    }
    //}
}