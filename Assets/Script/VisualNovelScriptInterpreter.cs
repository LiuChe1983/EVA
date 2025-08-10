using System.Collections;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
// ����ű�������
public class VisualNovelScriptInterpreter : MonoBehaviour
{
    public Image backgroundImage;
    public AudioSource bgmAudioSource;
    public AudioSource sfxAudioSource;
    public Image char1SpriteImage;
    public Image char2SpriteImage;
    public Image char3SpriteImage;
    public Image cgSpriteImage;

    [HideInInspector]
    public bool IsScriptEnd = false;

    [HideInInspector]
    public bool IsSavePoint = false;

    public (string, string)? DialogueLine { get; set; } // ���ڴ洢��ǰ�Ի��еĽ�ɫ��������  
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
        if (textAsset != null)
        {
            scriptLines = textAsset.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            currentLineIndex = 0;
        }
        else
        {
            Debug.LogError("Script file not found: " + filePath);
        }
    }

    //ֱ�Ӷ�ȡtxt�ļ���InitializeDev����
    public void InitializeDev(string filePath)
    {
        scriptLines = File.ReadAllLines(filePath);
        currentLineIndex = 0;
    }


    public void InterpretNextLine()
    {
        DialogueLine = null; // ��յ�ǰ�Ի���

        while (currentLineIndex < scriptLines.Length)
        {
            string line = scriptLines[currentLineIndex].Trim();
            if (line.StartsWith("@"))
            {
                InterpretCommandLine(line);
            }
            else if (line.StartsWith("#"))
            {
                // ע��������ע������
                Debug.Log(line.Substring(1).Trim());
            }
            else if (line.StartsWith("["))
            {
                InterpretDialogueLine(line);
                currentGameProgress.currentLineIndex = currentLineIndex; // ���µ�ǰ���������浵����
                currentLineIndex++; // ��֤�´��ܼ���
                return; // �Ի�����Ҫ�ȴ���Ҳ������������Զ��ƽ�
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
                currentGameProgress.isChar1Active = char1SpriteImage.gameObject.activeSelf; // ���½�ɫ1�ļ���״̬
                break;
            case "Char2":
            case "char2":
                HandleCharacterSpriteChange(char2SpriteImage, parameters);
                currentGameProgress.lastChar2Command = commandLine;
                currentGameProgress.isChar2Active = char2SpriteImage.gameObject.activeSelf; // ���½�ɫ2�ļ���״̬
                break;
            case "Char3":
            case "char3":
                HandleCharacterSpriteChange(char3SpriteImage, parameters);
                currentGameProgress.lastChar3Command = commandLine;
                currentGameProgress.isChar3Active = char3SpriteImage.gameObject.activeSelf; // ���½�ɫ3�ļ���״̬
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
                // ��Ǽ��㣬�Զ�������Ϸ
                IsSavePoint = true;
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
            cgSpriteImage.gameObject.SetActive(false); // ���û�в���������
            return;
        }
        if (parameters.Length > 0)
        {
            if (string.IsNullOrEmpty(parameters[0]))
            {
                cgSpriteImage.gameObject.SetActive(false); // �������Ϊ�գ�����
                return;
            }
            var spriteFullPath = Consts.CG_PATH + parameters[0].Trim();
            UpdateImage(spriteFullPath, cgSpriteImage);
        }
    }

    private void HandleCharacterSpriteChange(Image spriteImage, string[] parameters)
    {
        if (parameters.Length == 0)
        {
            spriteImage.gameObject.SetActive(false); // ���û�в��������ؽ�ɫ����
            return;
        }
        if (parameters.Length > 0)
        {
            if (string.IsNullOrEmpty(parameters[0]))
            {
                spriteImage.gameObject.SetActive(false); // �������Ϊ�գ����ؽ�ɫ����
                return;
            }
            var spriteFullPath = Consts.CHAR_PATH + parameters[0].Trim();

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
                bgmAudioSource.loop = isLooping; // �����Ƿ�ѭ������
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
        Sprite sprite = LoadSprite(imagePath);
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
        // ���ν���ÿ����������������
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

        // ��ɫ����״̬
        char1SpriteImage.gameObject.SetActive(data.isChar1Active);
        char2SpriteImage.gameObject.SetActive(data.isChar2Active);
        char3SpriteImage.gameObject.SetActive(data.isChar3Active);

        // �ָ���ǰ������
        currentLineIndex = data.currentLineIndex;
    }

    public Sprite LoadSprite(string imagePath)
    {
#if DEV
        // �ȳ��Ա���·������
        // �Զ���ȫ .png ��չ��
        string pngRelativePath = imagePath;
        if (!imagePath.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase))
            pngRelativePath += ".png";

        Sprite s = LoadSpriteFromRelativePath(pngRelativePath);
        if(s != null)
        {
            return s;
        }
        // ������ؼ���ʧ�ܣ�����ʹ��.jpeg��׺����
        string jpegRelativePath = imagePath;
        if (!imagePath.EndsWith(".jpeg", System.StringComparison.OrdinalIgnoreCase))
            jpegRelativePath += ".jpeg";
        s = LoadSpriteFromRelativePath(jpegRelativePath);
        if (s != null)
        {
            return s;
        }

        // �������ʧ�ܣ���ȡ��Դimage/404
        Debug.LogWarning($"ͼƬ����ʧ��: {imagePath}�����Լ���Ĭ��404ͼƬ");
        return Resources.Load<Sprite>("image/404");

#else
        // imagePath ������չ����ֱ������ Resources.Load
        return Resources.Load<Sprite>(imagePath);
#endif
    }


    public Sprite LoadSpriteFromRelativePath(string relativePath, int pixelsPerUnit = 100)
    {


        // ��ȡִ���ļ�����Ŀ¼
        string executableDir = Path.GetDirectoryName(Application.dataPath);
        // ���������ִ���ļ�������ͼƬ·��
        string fullPath = Path.Combine(executableDir, "dev/" + relativePath);

        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"ͼƬ�ļ�������: {fullPath}");
            return null;
        }

        try
        {
            byte[] imageBytes = File.ReadAllBytes(fullPath);
            Texture2D texture = new Texture2D(2, 2);
            if (!texture.LoadImage(imageBytes))
            {
                Debug.LogError("�޷�����ͼƬ���ݵ�Texture2D: " + fullPath);
                return null;
            }

            // ����Sprite
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        }
        catch (IOException e)
        {
            Debug.LogError($"��ȡͼƬʱ��������: {e.Message}");
            return null;
        }
    }
}