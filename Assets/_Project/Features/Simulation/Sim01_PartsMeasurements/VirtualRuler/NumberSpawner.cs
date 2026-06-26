using TMPro;
using UnityEngine;

namespace ProjectATLAS.Simulation.Sim01
{
    public class NumberSpawner : MonoBehaviour
    {
        [Tooltip("The parent to store the duplicated numbers.")]
        [SerializeField] private Transform container;
        [Tooltip("The GameObject to copy.")]
        [SerializeField] private GameObject template;
        
        public void UpdateNumber(float length)
        {
            int targetChildCount = Mathf.FloorToInt(length) + 2;
            int interval = targetChildCount - container.childCount;
            
            if (interval > 0)
            {
                for (int i = 0; i < interval; i++)
                {
                    AddNumber();
                }
            }
            else if (interval < 0)
            {
                interval = -interval; // flip the sign
                for (int i = 0; i < interval; i++)
                {
                    RemoveNumber();
                }
            }
        }
        
        private void AddNumber()
        {
            GameObject gameObject = container.GetChild(0).gameObject;
            GameObject newGameObject = Instantiate(gameObject, container);
            
            int index = container.childCount - 1;
            
            RectTransform rectTransform = newGameObject.transform as RectTransform;
            Vector2 position = rectTransform.anchoredPosition;
            position.x = index;
            
            rectTransform.anchoredPosition = position;
            
            newGameObject.name = index.ToString();
            if (newGameObject.TryGetComponent(out TMP_Text component))
                component.text = index.ToString();
        }
        
        private void RemoveNumber()
        {
            int index = container.childCount - 1;
            GameObject gameObject = container.GetChild(index).gameObject;
            
            if (index <= 0) return;
            Destroy(gameObject);
        }
    }
}
