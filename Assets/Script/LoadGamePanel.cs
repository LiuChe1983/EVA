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

    void Start()
    {
        closeButton.onClick.AddListener(HidePanel);

        // 示例：为每个存档栏位添加点击事件
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            int index = i; // 避免闭包问题
            saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotClicked(index));
        }
    }

    public void ShowPanel(string title)
    {
        titleText.text = title;
        gameObject.SetActive(true);
        // 这里可以刷新存档栏位的显示内容
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    void OnSaveSlotClicked(int slotIndex)
    {
        // 这里处理点击存档栏位的逻辑
        Debug.Log($"点击了存档栏位 {slotIndex + 1}");
        // 可根据需求实现加载或保存功能
    }
}
