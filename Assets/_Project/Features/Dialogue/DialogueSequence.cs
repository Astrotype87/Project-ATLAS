using System;
using System.Collections.Generic;
using UnityEngine;

using AstrotypeTools.InspectorAttributes;

namespace ProjectATLAS.Dialogue
{
    public class DialogueSequence : MonoBehaviour
    {
        [SerializeField] private List<DialogueStep> steps;
        
        public int StepCount => steps.Count;
        
        public DialogueStep GetDialogueStep(int index)
        {
            if (index < 0 || index >= steps.Count)
                return null;
            
            return steps[index];
        }
    }
    
    [Serializable]
    public class DialogueStep
    {
        [Indent(-1)] [TextArea(1, 4)]
        [SerializeField] private string message;
        
        [Indent(-1)] [InlineEditor]
        [SerializeField] private DialogueCharacter character;
        
        [Indent(-1)]
        [SerializeField] private bool dontUseTTS;
        
        [Indent(-1)]
        [SerializeField] private bool useAnimation;
        
        [CustomInspector.ShowIf(nameof(useAnimation))]
        [SerializeField] private float startTime;
        
        [CustomInspector.ShowIf(nameof(useAnimation))]
        [SerializeField] private float endTime;
        
        [SerializeField] private DialogueCondition dialogueCondition;
        
        
        public Sprite Image => character ? character.image : null;
        public string Name => character ? character.name : "Unknown Character";
        public float Pitch => character.pitch;
        
        public string Message => message;
        
        public bool UseTTS => !dontUseTTS;
        public bool UseAnimation => useAnimation;
        
        public float StartTime => startTime;
        public float EndTime => endTime;
        
        public DialogueCondition DialogueCondition => dialogueCondition;
    }
}
