using System;
using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;
using ProjectATLAS.UI.Layout;
using System.Collections;

namespace ProjectATLAS.Quiz.UI
{
    public class MultipleChoicesPanel : MonoBehaviour
    {
        [SerializeField, Child] private List<MultipleChoiceAnswerButton> answerButtons;
        
        public event Action<string> OnAnswerButtonClicked;
        
        // private bool isInitialized;
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            for (int i = 0; i < answerButtons.Count; i++)
            {
                answerButtons[i].OnClicked += AnswerButton_OnClicked;
            }
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }
        
        
        // PUBLIC METHODS
        public void SetAnswersPanelDisplay(string[] choices, string answer)
        {
            // Handle answer button duplication logic
            int targetChildCount = Mathf.FloorToInt(choices.Length);
            int interval = targetChildCount - answerButtons.Count;
            
            if (interval > 0)
            {
                for (int i = 0; i < interval; i++)
                {
                    AddAnswerButton();
                }
            }
            else if (interval < 0)
            {
                interval = -interval; // flip the sign
                for (int i = 0; i < interval; i++)
                {
                    RemoveAnswerButton();
                }
            }
            
            // Update answer button text and selected state
            char[] letter = {'A', 'B', 'C', 'D', 'E'};
            for (int i = 0; i < answerButtons.Count; i++)
            {
                answerButtons[i].SetAnswerDisplay(letter[i].ToString(), choices[i]);
                
                if (answerButtons[i].Answer == answer)
                    answerButtons[i].SetSelected(true);
                else
                    answerButtons[i].SetSelected(false);
            }
            
            // if (!isInitialized)
            //     StartCoroutine(UpdateViewportHeightAfterFrame());
            // else
            //     verticalContentExpander.UpdateViewportHeight();
        }
        
        
        
        
        // PRIVATE METHODS
        // private IEnumerator UpdateViewportHeightAfterFrame()
        // {
        //     yield return null;
        //     yield return null;
        //     verticalContentExpander.UpdateViewportHeight();
        //     isInitialized = true;
        // }
        
        private void AddAnswerButton()
        {
            MultipleChoiceAnswerButton template = answerButtons[0];
            GameObject newGameObject = Instantiate(template.gameObject, transform);
            
            if (newGameObject.TryGetComponent(out MultipleChoiceAnswerButton answerButton))
            {
                answerButton.OnClicked += AnswerButton_OnClicked;
                answerButtons.Add(answerButton);
            }
        }
        
        private void RemoveAnswerButton()
        {
            int lastIndex = answerButtons.Count - 1;
            if (lastIndex <= 0) return;
            
            MultipleChoiceAnswerButton viewToRemove = answerButtons[lastIndex];
            
            viewToRemove.OnClicked -= AnswerButton_OnClicked;
            answerButtons.Remove(viewToRemove);
            
            Destroy(viewToRemove.gameObject);
        }
        
        private void AnswerButton_OnClicked(string answer)
        {
            foreach (var button in answerButtons)
            {
                button.SetSelected(false);
            }
            
            OnAnswerButtonClicked?.Invoke(answer);
        }
    }
}
