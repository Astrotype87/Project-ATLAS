using System;
using UnityEngine;

using CustomInspector;

namespace ProjectATLAS.Minigame.Mini02_DesertNavigation
{
    public class FinishLine : MonoBehaviour
    {
        [SerializeField, Tag] private string hovercraftTag;
        
        public event Action OnFinishLineTriggered;
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag(hovercraftTag))
            {
                OnFinishLineTriggered?.Invoke();
            }
        }
    }
}
