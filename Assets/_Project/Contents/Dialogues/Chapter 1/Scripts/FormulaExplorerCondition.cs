using UnityEngine;

using ProjectATLAS.Dialogue;
using ProjectATLAS.Lesson;

namespace ProjectATLAS
{
    public class FormulaExplorerCondition : DialogueCondition
    {
        [SerializeField] private FormulaExplorer formulaExplorer;
        [SerializeField] private float targetResultValue;
        
        private void Update()
        {
            IsConditionMet = Mathf.Approximately((float)formulaExplorer.ResultValue, targetResultValue);
        }
    }
}
