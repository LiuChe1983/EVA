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
        if (!IsLoad)
        {
            interpreter = FindObjectOfType<VisualNovelScriptInterpreter>();
            if (interpreter == null)
            {
                Debug.LogWarning("δ�ҵ�VisualNovelScriptInterpreterʵ����");
            }
        }
    }

    public void ShowPanel(string title, bool isLoad)
    {
        titleText.text = title;
        IsLoad = isLoad;
        gameObject.SetActive(true);
        // ��ˢ�´浵��λ��ʾ����
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
            Debug.LogWarning("SaveSystemδ�����δ�ҵ���");
            return;
        }

        if (IsLoad)
        {
            var data = saveSystem.LoadGame(slotIndex);
            if (data != null)
            {
                // ����Ƕ������ָ���Ϸ״̬
                SaveSystem.LoadedSaveData = data;
                Debug.Log($"��ȡ�浵��λ {slotIndex + 1} �ɹ�");
                SceneController.Instance.SetState("PlayScene");

            }
            else
            {
                Debug.LogError($"��ȡ�浵��λ {slotIndex + 1} ʧ��");
            }
        }
        else
        {
            if (saveSystem.SaveGame(slotIndex))
            {
                UpdateSaveSlot(slotIndex);
                Debug.Log($"�ѱ��浽�浵��λ {slotIndex + 1}");
            }
            else
            {
                Debug.LogError($"���浽�浵��λ {slotIndex + 1} ʧ��");
            }
        }
    }

    public void UpdateSaveSlot(int slotIndex)
    {
        if (saveSystem == null)
        {
            Debug.LogWarning("SaveSystemδ�����δ�ҵ���");
            return;
        }
        SaveData data = saveSystem.LoadGame(slotIndex);
        if (data != null)
        {
            // ����UI��ʾ�浵��Ϣ
            var texts = saveSlotButtons[slotIndex].GetComponentsInChildren<TMP_Text>();
            if (texts.Length > 0)
            {
                texts[0].text = $"�浵 {slotIndex + 1}";
                texts[1].text = data.saveTime;
            }
        }
        else
        {
            saveSlotButtons[slotIndex].GetComponentInChildren<TMP_Text>().text = $"�浵 {slotIndex + 1} - ��";
        }
    }
}
