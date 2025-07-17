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

    public SaveSystem saveSystem; // ��Ȼ����Inspector����
    public bool IsLoad = true;    // trueΪ������falseΪ�浵

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
            Debug.LogWarning("δ�ҵ�VisualNovelScriptInterpreterʵ����");
        }
    }

    public void ShowPanel(string title, bool isLoad)
    {
        titleText.text = title;
        IsLoad = isLoad;
        gameObject.SetActive(true);
        // ��ˢ�´浵��λ��ʾ����
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    void OnSaveSlotClicked(int slotIndex)
    {
        if (saveSystem == null || interpreter == null)
        {
            Debug.LogWarning("SaveSystem��Interpreterδ�����δ�ҵ���");
            return;
        }

        if (IsLoad)
        {
            bool result = saveSystem.LoadGame(slotIndex) != null;
            Debug.Log(result ? $"��ȡ�浵��λ {slotIndex + 1} �ɹ�" : $"��ȡ�浵��λ {slotIndex + 1} ʧ��");
        }
        else
        {
            saveSystem.SaveGame(interpreter, slotIndex);
            Debug.Log($"�ѱ��浽�浵��λ {slotIndex + 1}");
        }
    }
}
