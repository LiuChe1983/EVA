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

        // ʾ����Ϊÿ���浵��λ��ӵ���¼�
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            int index = i; // ����հ�����
            saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotClicked(index));
        }
    }

    public void ShowPanel(string title)
    {
        titleText.text = title;
        gameObject.SetActive(true);
        // �������ˢ�´浵��λ����ʾ����
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    void OnSaveSlotClicked(int slotIndex)
    {
        // ���ﴦ�����浵��λ���߼�
        Debug.Log($"����˴浵��λ {slotIndex + 1}");
        // �ɸ�������ʵ�ּ��ػ򱣴湦��
    }
}
