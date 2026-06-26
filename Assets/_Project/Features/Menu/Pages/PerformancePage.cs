using UnityEngine;
using ProjectATLAS.UI;

namespace ProjectATLAS.Menu
{
    public class PerformancePage : UIPage
    {
        [Header("Pages")]
        [SerializeField] private SummaryPage summaryPage;
        [SerializeField] private UIToggleButton summaryToggle;
        
        public override void OpenPage()
        {
            base.OpenPage();
            
            summaryPage.ClosePage();
            summaryToggle.SetValue(true);
            summaryPage.OpenPage();
        }
    }
}
