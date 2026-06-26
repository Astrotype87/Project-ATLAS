using System;
using System.Collections;
using UnityEngine;

public class CongratsTrigger : MonoBehaviour
{
    public event Action OnGameWin;
    
    [Header("Congrats Settings")]
    public GameObject astronaut; // i-drag mo dito yung astronaut sa Inspector

    private bool hasWon = false;

    [System.Obsolete]
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<MonoBehaviourPlayerController>();
        if (player == null || hasWon) return;

        if (CompareTag("Congrats"))
        {
            hasWon = true;

            // Ipakita astronaut
            if (astronaut != null)
                astronaut.SetActive(true);

            Debug.Log("Congrats, you win!");

            // Ipakita UI text kung may UIMessageManager ka
            UIMessageManager.Instance.ShowMessage("Congrats, you win!");
                
            StartCoroutine(TriggerGameWin());
        }
    }
    
    public IEnumerator TriggerGameWin()
    {
        yield return new WaitForSeconds(1f);
        OnGameWin?.Invoke();
    }
}
