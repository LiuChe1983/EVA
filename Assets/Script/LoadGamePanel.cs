using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadGamePanel : MonoBehaviour
{
    public TMP_Text titleText;
    public Button closeButton;
    public Button[] saveSlotButtons = new Button[6];

    public SaveSystem saveSystem; // 依然可在Inspector分配
    public bool IsLoad = true;    // true为读档，false为存档

    private VisualNovelScriptInterpreter interpreter;

    void Start()
    {
        closeButton.onClick.AddListener(HidePanel);

        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            int index = i;
            saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotClicked(index));
        }

        interpreter = FindObjectOfType<VisualNovelScriptInterpreter>();
        if (interpreter == null)
        {
            Debug.LogWarning("未找到VisualNovelScriptInterpreter实例！");
        }
    }

    public void ShowPanel(string title, bool isLoad)
    {
        titleText.text = title;
        IsLoad = isLoad;
        gameObject.SetActive(true);
        // 可刷新存档栏位显示内容
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    void OnSaveSlotClicked(int slotIndex)
    {
        if (saveSystem == null || interpreter == null)
        {
            Debug.LogWarning("SaveSystem或Interpreter未分配或未找到！");
            return;
        }

        if (IsLoad)
        {
            bool result = saveSystem.LoadGame(slotIndex) != null;
            Debug.Log(result ? $"读取存档栏位 {slotIndex + 1} 成功" : $"读取存档栏位 {slotIndex + 1} 失败");
        }
        else
        {
            saveSystem.SaveGame(interpreter, slotIndex);
            Debug.Log($"已保存到存档栏位 {slotIndex + 1}");
        }
    }
}
