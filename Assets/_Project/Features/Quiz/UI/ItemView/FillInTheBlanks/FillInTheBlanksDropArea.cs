using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ProjectATLAS.UI;
using ProjectATLAS.Utility;

namespace ProjectATLAS.Quiz.UI
{
    public class FillInTheBlanksDropArea : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private UIDropArea dropArea;
        
        // PROPERTIES
        public event Action<int, string> OnChoiceDropped;
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            dropArea.OnDropped += DropArea_OnDropped;
            if (numberText) numberText.text = (transform.GetSiblingIndex() + 1).ToString();
        }
        
        private void OnValidate()
        {
            if (numberText) numberText.text = (transform.GetSiblingIndex() + 1).ToString();
        }
        
        // PUBLIC METHODS
        public string GetAssignedChoice()
        {
            if (dropArea)
            {
                GameObject gameObject = dropArea.GetFirstStored();
                if (gameObject == null) return string.Empty;
                
                if (gameObject.TryGetComponent(out FillInTheBlanksChoiceView fillInTheBlanksChoiceView))
                {
                    return fillInTheBlanksChoiceView.Choice;
                }
            }
            
            return string.Empty;
        }
        
        public void AssignChoice(FillInTheBlanksChoiceView choiceView)
        {
            dropArea.DropObject(choiceView.gameObject);
        }
        
        
        // EVENT LISTENER METHODS
        private void DropArea_OnDropped(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out FillInTheBlanksChoiceView fillInTheBlanksChoiceView))
            {
                int index = transform.GetSiblingIndex();
                string choice = fillInTheBlanksChoiceView.Choice;
                OnChoiceDropped?.Invoke(index, choice);
            }
        }
    }
}
