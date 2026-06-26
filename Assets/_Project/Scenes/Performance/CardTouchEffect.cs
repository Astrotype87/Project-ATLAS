using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CardTouchEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("Touch Animation Settings")]
    public float scaleAmount = 1.1f;
    public float animationSpeed = 8f;
    public string sceneName;

    private Vector3 originalScale;
    private bool isPressed = false;

    void Start() => originalScale = transform.localScale;

    void Update()
    {
        Vector3 targetScale = isPressed ? originalScale * scaleAmount : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
    }
}
