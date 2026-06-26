using UnityEngine;
using TMPro;
using System.Collections;

using ProjectATLAS.Architecture;

public class UIMessageManager : SceneSingleton<UIMessageManager>
{
    public TextMeshProUGUI messageText; // drag mo dito yung KeyMessageText
    public float displayTime = 2f;

    private Coroutine hideRoutine;

    public void ShowMessage(string message)
    {
        if (hideRoutine != null) StopCoroutine(hideRoutine);
        messageText.gameObject.SetActive(true);
        messageText.text = message;
        hideRoutine = StartCoroutine(HideAfterSeconds());
    }

    private IEnumerator HideAfterSeconds()
    {
        yield return new WaitForSeconds(displayTime);
        messageText.gameObject.SetActive(false);
    }
}
