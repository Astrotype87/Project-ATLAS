using UnityEngine;
using KBCore.Refs;

namespace ProjectATLAS.Minigame.Mini01_HovercraftAssembly
{
    public class AssemblyResultsView : MonoBehaviour
    {
        [SerializeField, Child] private AssemblyGradeView[] assemblyGradeViews;
        
        public void DisplayResults((string partName, Vector2 correct, Vector2 answer)[] results)
        {
            for (int i = 0; i < assemblyGradeViews.Length; i++)
            {
                if (i < results.Length)
                {
                    var (partName, correct, answer) = results[i];
                    assemblyGradeViews[i].gameObject.SetActive(true);
                    assemblyGradeViews[i].SetData(partName, correct, answer);
                }
                else
                {
                    // Hide unused rows if there are fewer results than views
                    assemblyGradeViews[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
