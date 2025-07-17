using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{

    public TextMeshProUGUI textDisplay;
    public float typingSpeed = Consts.DEFAULT_TYPING_SPEED;

    private Coroutine typingCoroutine;
    private bool isTyping;
    public bool IsTyping { get => isTyping; set => isTyping = value; }

    public void StartTyping(string text)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(text));
    }

    public void StartTyping(string text, float speed)
    {
        typingSpeed = speed;
        StartTyping(text);
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;
        textDisplay.text = text;
        textDisplay.maxVisibleCharacters = 0;

        for (int i = 0; i <= text.Length; i++)
        {
            textDisplay.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

    }

    public void CompleteLine()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        textDisplay.maxVisibleCharacters = textDisplay.text.Length;
        isTyping = false;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
