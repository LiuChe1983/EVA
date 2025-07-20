using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 场景控制器单例
public class SceneController : MonoBehaviour
{
    public SceneEnum StartScene = SceneEnum.StartScene;
    public SceneEnum CurrentScene = SceneEnum.StartScene;
    // 单例实现
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

    // 当前场景状态
    private SceneState currentState;

    // 场景状态字典
    private Dictionary<string, SceneState> sceneStates = new Dictionary<string, SceneState>();

    private void Awake()
    {
        // 确保单例唯一性
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // 初始化窗口设置
        int winMode = PlayerPrefs.GetInt("WinMode", 0);
        bool isFullScreen = winMode == 1;
        Screen.fullScreen = isFullScreen;

        int savedWidth = PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width);
        Resolution[] resolutions = Screen.resolutions;
        Resolution selectedResolution = resolutions[0];
        foreach (var res in resolutions)
        {
            // 只保留16:9且宽度>=1920的分辨率
            if (Mathf.Abs((float)res.width / res.height - 16f / 9f) > 0.01f || res.width < 1920)
                continue;
            if (res.width == savedWidth)
            {
                selectedResolution = res;
                break;
            }
        }
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, isFullScreen);

        // 初始化场景状态
        InitializeSceneStates();
    }

    private void InitializeSceneStates()
    {
        // 注册场景状态
        sceneStates.Add("StartScene", new StartSceneState(this));
        sceneStates.Add("PlayScene", new PlaySceneState(this));

        // 设置初始场景
        SetState(StartScene.ToString());
    }

    public void SetState(SceneEnum sceneEnum)
    {
        SetState(sceneEnum.ToString());
    }
    // 设置当前场景状态
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

    // 加载场景
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

// 场景状态抽象类
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

// 开始场景状态
public class StartSceneState : SceneState
{
    public StartSceneState(SceneController controller) : base(controller) { }

    public override void Enter()
    {
        SceneController.Instance.CurrentScene = SceneEnum.StartScene;
        controller.LoadScene("StartScene");
        Debug.Log("Entering Start Scene");
        // 注册UI事件监听等初始化操作
    }

    public override void Exit()
    {
        Debug.Log("Exiting Start Scene");
        // 清理资源和事件监听
    }
}

// 游戏场景状态
public class PlaySceneState : SceneState
{
    public PlaySceneState(SceneController controller) : base(controller) { }

    public override void Enter()
    {
        SceneController.Instance.CurrentScene = SceneEnum.PlayScene;
        controller.LoadScene("PlayScene");
        Debug.Log("Entering Play Scene");
        // 初始化游戏逻辑
    }

    public override void Exit()
    {
        Debug.Log("Exiting Play Scene");
        // 保存游戏进度等操作
    }
}

public enum SceneEnum
{
    StartScene,
    PlayScene
}