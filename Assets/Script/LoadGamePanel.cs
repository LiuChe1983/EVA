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
        if (!IsLoad)
        {
            interpreter = FindObjectOfType<VisualNovelScriptInterpreter>();
            if (interpreter == null)
            {
                Debug.LogWarning("未找到VisualNovelScriptInterpreter实例！");
            }
        }
    }

    public void ShowPanel(string title, bool isLoad)
    {
        titleText.text = title;
        IsLoad = isLoad;
        gameObject.SetActive(true);
        // 可刷新存档栏位显示内容
        for(int i = 0; i < saveSlotButtons.Length; i++)
        {
            UpdateSaveSlot(i);
        }

    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    void OnSaveSlotClicked(int slotIndex)
    {
        if (saveSystem == null)
        {
            Debug.LogWarning("SaveSystem未分配或未找到！");
            return;
        }

        if (IsLoad)
        {
            var data = saveSystem.LoadGame(slotIndex);
            if (data != null)
            {
                // 如果是读档，恢复游戏状态
                SaveSystem.LoadedSaveData = data;
                Debug.Log($"读取存档栏位 {slotIndex + 1} 成功");
                SceneController.Instance.SetState("PlayScene");

            }
            else
            {
                Debug.LogError($"读取存档栏位 {slotIndex + 1} 失败");
            }
        }
        else
        {
            if (saveSystem.SaveGame(slotIndex))
            {
                UpdateSaveSlot(slotIndex);
                Debug.Log($"已保存到存档栏位 {slotIndex + 1}");
            }
            else
            {
                Debug.LogError($"保存到存档栏位 {slotIndex + 1} 失败");
            }
        }
    }

    public void UpdateSaveSlot(int slotIndex)
    {
        if (saveSystem == null)
        {
            Debug.LogWarning("SaveSystem未分配或未找到！");
            return;
        }
        SaveData data = saveSystem.LoadGame(slotIndex);
        if (data != null)
        {
            // 更新UI显示存档信息
            var texts = saveSlotButtons[slotIndex].GetComponentsInChildren<TMP_Text>();
            if (texts.Length > 0)
            {
                texts[0].text = $"存档 {slotIndex + 1}";
                texts[1].text = data.saveTime;
            }
        }
        else
        {
            saveSlotButtons[slotIndex].GetComponentInChildren<TMP_Text>().text = $"存档 {slotIndex + 1} - 空";
        }
    }
}
