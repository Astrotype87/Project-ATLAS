using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPointer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum Dir { Left, Right }
    public Dir direction = Dir.Left;
    public PlayerController playerController; // assign in inspector

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerController == null) return;
        if (direction == Dir.Left) playerController.SetLeft(true);
        else playerController.SetRight(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (playerController == null) return;
        if (direction == Dir.Left) playerController.SetLeft(false);
        else playerController.SetRight(false);
    }
}

