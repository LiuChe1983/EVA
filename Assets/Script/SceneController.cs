using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ��������������
public class SceneController : MonoBehaviour
{
    // ����ʵ��
    private static SceneController _instance;
    public static SceneController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SceneController>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("SceneController");
                    _instance = singletonObject.AddComponent<SceneController>();
                }
            }
            return _instance;
        }
    }

    // ��ǰ����״̬
    private SceneState currentState;

    // ����״̬�ֵ�
    private Dictionary<string, SceneState> sceneStates = new Dictionary<string, SceneState>();

    private void Awake()
    {
        // ȷ������Ψһ��
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // ȷ�������л�ʱ��������

        // ��ʼ������״̬
        InitializeSceneStates();
    }

    private void InitializeSceneStates()
    {
        // ע�᳡��״̬
        sceneStates.Add("StartScene", new StartSceneState(this));
        sceneStates.Add("PlayScene", new PlaySceneState(this));

        // ���ó�ʼ����
        SetState("StartScene");
    }

    // ���õ�ǰ����״̬
    public void SetState(string sceneName)
    {
        if (sceneStates.ContainsKey(sceneName))
        {
            currentState?.Exit();
            currentState = sceneStates[sceneName];
            currentState.Enter();
        }
        else
        {
            Debug.LogError($"Scene state not found: {sceneName}");
        }
    }

    // ���س���
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

// ����״̬������
public abstract class SceneState
{
    protected SceneController controller;

    public SceneState(SceneController controller)
    {
        this.controller = controller;
    }

    public abstract void Enter();
    public abstract void Exit();
    public virtual void Update() { }
}

// ��ʼ����״̬
public class StartSceneState : SceneState
{
    public StartSceneState(SceneController controller) : base(controller) { }

    public override void Enter()
    {
        controller.LoadScene("StartScene");
        Debug.Log("Entering Start Scene");
        // ע��UI�¼������ȳ�ʼ������
    }

    public override void Exit()
    {
        Debug.Log("Exiting Start Scene");
        // ������Դ���¼�����
    }
}

// ��Ϸ����״̬
public class PlaySceneState : SceneState
{
    public PlaySceneState(SceneController controller) : base(controller) { }

    public override void Enter()
    {
        controller.LoadScene("PlayScene");
        Debug.Log("Entering Play Scene");
        // ��ʼ����Ϸ�߼�
    }

    public override void Exit()
    {
        Debug.Log("Exiting Play Scene");
        // ������Ϸ���ȵȲ���
    }
}