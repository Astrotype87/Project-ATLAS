using UnityEngine;
using KBCore.Refs;

namespace ProjectATLAS.Simulation.Sim01_PartsMeasurements
{
    public class MeasurementResultsView : CustomResultsView
    {
        [SerializeField, Child] private BreakdownView[] breakdownViews;
        
        public void DisplayResults((string partName, float correct, float answer)[] results)
        {
            for (int i = 0; i < breakdownViews.Length; i++)
            {
                if (i < results.Length)
                {
                    var (partName, correct, answer) = results[i];
                    breakdownViews[i].gameObject.SetActive(true);
                    breakdownViews[i].SetData(partName, correct, answer);
                }
                else
                {
                    // Hide unused rows if there are fewer results than views
                    breakdownViews[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
