using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectATLAS
{
    public class Selectable2D : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
