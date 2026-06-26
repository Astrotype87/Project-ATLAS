using UnityEngine;

namespace ProjectATLAS.UI
{
    public class UIOpenPageAction : UIAction
    {
        [SerializeField] private UIPage pageToOpen;
        [SerializeField] private bool openIsolated;
        
        protected override void OnClick()
        {
            if (pageToOpen)
            {
                if (openIsolated) pageToOpen.OpenPage();
                else pageToOpen.OpenPageInGroup();
            }
        }
        
        public void SetPage(UIPage pageToOpen)
        {
            this.pageToOpen = pageToOpen;
        }
    }
}
