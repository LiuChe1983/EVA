using System.Collections;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
// 剧情脚本解释器
public class VisualNovelScriptInterpreter : MonoBehaviour
{
    public Image backgroundImage;
    public AudioSource bgmAudioSource;
    public AudioSource sfxAudioSource;
    public Image char1SpriteImage;
    public Image char2SpriteImage;
    public Image char3SpriteImage;
    public Image cgSpriteImage;

    public bool IsScriptEnd = false;

    public (string, string)? DialogueLine { get; set; } // 用于存储当前对话行的角色名和内容  
    private int currentLineIndex;
    private string[] scriptLines;
    private SaveData currentGameProgress;

    private void Start()
    {
        SaveSystem.CurrentGameProgress = new SaveData();
        currentGameProgress = SaveSystem.CurrentGameProgress;
    }
    public void Initialize(string filePath)
    {
        var textAsset = Resources.Load<TextAsset>(filePath);
        if(textAsset != null)
        {
            scriptLines = textAsset.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            currentLineIndex = 0;
        }
        else
        {
            Debug.LogError("Script file not found: " + filePath);
        }
    }


    public void InterpretNextLine()
    {
        DialogueLine = null; // 清空当前对话行

        while (currentLineIndex < scriptLines.Length)
        {
            string line = scriptLines[currentLineIndex].Trim();
            if (line.StartsWith("@"))
            {
                InterpretCommandLine(line);
            }
            else if (line.StartsWith("#"))
            {
                // 注释命令，输出注释内容
                Debug.Log(line.Substring(1).Trim());
            }
            else if (line.StartsWith("["))
            {
                InterpretDialogueLine(line);
                currentGameProgress.currentLineIndex = currentLineIndex; // 更新当前行索引到存档数据
                currentLineIndex++; // 保证下次能继续
                return; // 对话行需要等待玩家操作，不继续自动推进
            }
            currentLineIndex++;
        }
        IsScriptEnd = true;
        Debug.Log("ScriptEnd");
    }

    private void InterpretCommandLine(string commandLine)
    {
        string[] parts = commandLine.Split(':');
        string command = parts[0].Trim().Substring(1);
        string[] parameters = parts.Length > 1 ? parts[1].Split(',') : new string[0];

        switch (command)
        {
            case "BG":
            case "bg":
                HandleBackgroundChange(parameters);
                currentGameProgress.lastBGCommand = commandLine;
                break;
            case "CG":
            case "cg":
                HandleCGChange(parameters);
                currentGameProgress.lastCGCommand = commandLine;
                break;
            case "Char1":
            case "char1":
                HandleCharacterSpriteChange(char1SpriteImage, parameters);
                currentGameProgress.lastChar1Command = commandLine;
                currentGameProgress.isChar1Active = char1SpriteImage.gameObject.activeSelf; // 更新角色1的激活状态
                break;
            case "Char2":
            case "char2":
                HandleCharacterSpriteChange(char2SpriteImage, parameters);
                currentGameProgress.lastChar2Command = commandLine;
                currentGameProgress.isChar2Active = char2SpriteImage.gameObject.activeSelf; // 更新角色2的激活状态
                break;
            case "Char3":
            case "char3":
                HandleCharacterSpriteChange(char3SpriteImage, parameters);
                currentGameProgress.lastChar3Command = commandLine;
                currentGameProgress.isChar3Active = char3SpriteImage.gameObject.activeSelf; // 更新角色3的激活状态
                break;
            case "BGM":
            case "bgm":
                HandleBGMChange(parameters);
                currentGameProgress.lastBGMCommand = commandLine;
                break;
            case "SFX":
                HandleSFXPlay(parameters);
                currentGameProgress.lastSFXCommand = commandLine;
                break;
            case "save":
            case "Save":
            case "SAVE":
                // 标记检查点，自动保存游戏
                //GameManager.Instance.SaveGame();
                break;
            default:
                Debug.LogWarning("Unknown command: " + command);
                break;
        }
    }

    private void HandleBackgroundChange(string[] parameters)
    {
        if (parameters.Length > 0)
        {
            string bgPath = parameters[0].Trim();
            var bgFullPath = Consts.BACKGROUND_PATH + bgPath;
            UpdateImage(bgFullPath, backgroundImage);
        }
    }

    private void HandleCGChange(string[] parameters)
    {
        if (parameters.Length == 0)
        {
            cgSpriteImage.gameObject.SetActive(false); // 如果没有参数，隐藏
            return;
        }
        if (parameters.Length > 0)
        {
            if (string.IsNullOrEmpty(parameters[0]))
            {
                cgSpriteImage.gameObject.SetActive(false); // 如果参数为空，隐藏
                return;
            }
            var spriteFullPath = Consts.CHAR_PATH + parameters[0].Trim();
            string charPath = parameters[0].Trim();
            UpdateImage(spriteFullPath, cgSpriteImage);
        }
    }

    private void HandleCharacterSpriteChange(Image spriteImage, string[] parameters)
    {
        if(parameters.Length == 0)
        {
            spriteImage.gameObject.SetActive(false); // 如果没有参数，隐藏角色立绘
            return;
        }
        if (parameters.Length > 0)
        {
            if(string.IsNullOrEmpty(parameters[0])) 
            {
                spriteImage.gameObject.SetActive(false); // 如果参数为空，隐藏角色立绘
                return;
            }
            var spriteFullPath = Consts.CHAR_PATH + parameters[0].Trim();
            string charPath = parameters[0].Trim();
            UpdateImage(spriteFullPath, spriteImage);
        }
    }

    private void HandleBGMChange(string[] parameters)
    {
        if (parameters.Length > 0)
        {
            string bgmPath = parameters[0].Trim();
            bool isLooping = parameters.Length > 1 && parameters[1].Trim().ToLower() == "true";
            var bgmFullPath = Consts.MUSIC_PATH + bgmPath;
            AudioClip bgmClip = Resources.Load<AudioClip>(bgmFullPath);
            if (bgmClip != null)
            {
                bgmAudioSource.clip = bgmClip;
                bgmAudioSource.Play();
                bgmAudioSource.loop = isLooping; // 设置是否循环播放
            }
            else
            {
                Debug.LogError("BGM clip not found: " + bgmPath);
            }
        }
    }

    private void HandleSFXPlay(string[] parameters)
    {
        if (parameters.Length > 0)
        {
            string sfxPath = parameters[0].Trim();
            AudioClip sfxClip = Resources.Load<AudioClip>(sfxPath);
            if (sfxClip != null)
            {
                sfxAudioSource.clip = sfxClip;
                sfxAudioSource.Play();
            }
            else
            {
                Debug.LogError("SFX clip not found: " + sfxPath);
            }
        }
    }

    private void InterpretDialogueLine(string dialogueLine)
    {
        int endBracketIndex = dialogueLine.IndexOf(']');
        if (endBracketIndex > 0)
        {
            string characterName = dialogueLine.Substring(1, endBracketIndex - 1);
            string dialogue = dialogueLine.Substring(endBracketIndex + 1).Trim();
            //dialogueText.text = characterName + ": " + dialogue;
            DialogueLine = (characterName, dialogue);
        }
        else
        {
            Debug.LogWarning("Invalid dialogue line format: " + dialogueLine);
        }
    }
    void UpdateImage(string imagePath, Image img)
    {
        Sprite sprite = Resources.Load<Sprite>(imagePath);
        if (sprite != null)
        {
            img.sprite = sprite;
            img.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("IMAGE_LOAD_FAILED: " + imagePath);
        }
    }

    public void RestoreSceneByCommands(SaveData data)
    {
        // 依次解释每种命令的最后命令行
        if (!string.IsNullOrEmpty(data.lastBGCommand))
            InterpretCommandLine(data.lastBGCommand);
        if (!string.IsNullOrEmpty(data.lastChar1Command))
            InterpretCommandLine(data.lastChar1Command);
        if (!string.IsNullOrEmpty(data.lastChar2Command))
            InterpretCommandLine(data.lastChar2Command);
        if (!string.IsNullOrEmpty(data.lastChar3Command))
            InterpretCommandLine(data.lastChar3Command);
        if (!string.IsNullOrEmpty(data.lastBGMCommand))
            InterpretCommandLine(data.lastBGMCommand);
        if (!string.IsNullOrEmpty(data.lastCGCommand))
            InterpretCommandLine(data.lastCGCommand);
        if (!string.IsNullOrEmpty(data.lastSFXCommand))
            InterpretCommandLine(data.lastSFXCommand);

        // 角色激活状态
        char1SpriteImage.gameObject.SetActive(data.isChar1Active);
        char2SpriteImage.gameObject.SetActive(data.isChar2Active);
        char3SpriteImage.gameObject.SetActive(data.isChar3Active);

        // 恢复当前行索引
        currentLineIndex = data.currentLineIndex;
    }


}