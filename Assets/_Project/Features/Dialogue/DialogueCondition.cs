using UnityEngine;

namespace ProjectATLAS.Dialogue
{
    public abstract class DialogueCondition : MonoBehaviour
    {
        public bool IsConditionMet { get; protected set; }
    }
}
