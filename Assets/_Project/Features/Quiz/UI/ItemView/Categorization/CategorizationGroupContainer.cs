using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using ProjectATLAS.UI;
using ProjectATLAS.UI.Layout;

namespace ProjectATLAS.Quiz.UI
{
    public class CategorizationGroupContainer : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private string category;
        
        [Header("Components")]
        [SerializeField] private TMP_Text categoryText;
        [SerializeField] private UIDropArea dropArea;
        
        
        // PROPERTIES
        public string Category => category;
        public event Action<string, string[]> OnAnswerDropped; // <string category, string answer>
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            dropArea.OnDropped += DropArea_OnDropped;
        }
        
        private void OnValidate()
        {
            if (categoryText) categoryText.text = category;
        }
        
        
        // PUBLIC METHODS
        public void DisplayCategory(string category)
        {
            this.category = category;
            
            if (categoryText) categoryText.text = category;
        }
        
        public string[] GetAssignedAnswers()
        {
            if (dropArea)
            {
                List<GameObject> gameObjects = dropArea.StoredGameObjects;
                if (gameObjects == null || gameObjects.Count == 0) return null;
                
                List<string> answers = new();
                foreach (var gameObject in gameObjects)
                {
                    if (gameObject.TryGetComponent(out CategorizationAnswerView categorizationAnswerView))
                    {
                        answers.Add(categorizationAnswerView.Answer);
                    }
                }
                
                return answers.ToArray();
            }
            
            return null;
        }
        
        public void AssignAnswer(CategorizationAnswerView answerView)
        {
            dropArea.DropObject(answerView.gameObject);
        }
        
        
        // EVENT LISTENER METHODS
        private void DropArea_OnDropped(GameObject gameObject)
        {
            OnAnswerDropped?.Invoke(category, GetAssignedAnswers());
        }
    }
}
