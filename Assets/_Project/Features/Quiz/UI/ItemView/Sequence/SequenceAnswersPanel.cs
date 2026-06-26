using System;
using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;

using ProjectATLAS.UI.DragAndDrop;
using UnityEngine.UI;
using System.Linq;

namespace ProjectATLAS
{
    public class SequenceAnswersPanel : MonoBehaviour
    {
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
        [SerializeField] private List<SequenceAnswerView> orderedAnswerViews;
        [SerializeField] private UIOrderedDropContainer orderedDropContainer;
        
        // PROPERTIES
        public event Action<string[]> OnContainedAnswersChanged;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            orderedDropContainer.OnIndexUpdated += OrderedDropContainer_OnIndexUpdated;
        }
        
        
        // PUBLIC METHODS
        public void DisplayAnswers(string[] answers)
        {
            // Handle answer button duplication logic
            int targetChildCount = Mathf.FloorToInt(answers.Length);
            int interval = targetChildCount - transform.childCount;
            
            if (interval > 0)
            {
                for (int i = 0; i < interval; i++)
                {
                    AddAnswerView();
                }
            }
            else if (interval < 0)
            {
                interval = -interval; // flip the sign
                for (int i = 0; i < interval; i++)
                {
                    RemoveAnswerView();
                }
            }
            
            // Update ordered drop container
            orderedDropContainer.SetItemsList(orderedAnswerViews.Select(o => o.gameObject).ToList());
            
            // Get sequence answer views components by transform order
            orderedAnswerViews = orderedAnswerViews
                .OrderBy(view => view.transform.GetSiblingIndex())
                .ToList();
            
            // orderedAnswerViews.Clear();
            // for (int i = 0; i < targetChildCount; i++)
            // {
            //     if (transform.GetChild(i).TryGetComponent(out SequenceAnswerView answerView))
            //     {
            //         orderedAnswerViews.Add(answerView);
            //     }
            // }
            
            // Update answer view text
            for (int i = 0; i < targetChildCount; i++)
            {
                orderedAnswerViews[i].DisplayAnswer(answers[i]);
            }
        }
        
        
        // PRIVATE METHODS
        private void AddAnswerView()
        {
            SequenceAnswerView template = orderedAnswerViews[0];
            GameObject newGameObject = Instantiate(template.gameObject, transform);
            
            if (newGameObject.TryGetComponent(out SequenceAnswerView answerView))
            {
                orderedAnswerViews.Add(answerView);
            }
        }
        
        private void RemoveAnswerView()
        {
            int lastIndex = orderedAnswerViews.Count - 1;
            if (lastIndex <= 0) return;
            
            SequenceAnswerView viewToRemove = orderedAnswerViews[lastIndex];
            
            orderedAnswerViews.Remove(viewToRemove);
            
            Destroy(viewToRemove.gameObject);
        }
        
        // EVENT LISTENER METHODS
        private void OrderedDropContainer_OnIndexUpdated()
        {
            string[] answers = new string[orderedAnswerViews.Count];
            
            for (int i = 0; i < answers.Length; i++)
            {
                SequenceAnswerView answerView = transform.GetChild(i).GetComponent<SequenceAnswerView>();
                answers[i] = answerView.Answer;
            }
            
            OnContainedAnswersChanged?.Invoke(answers);
        }
    }
}
