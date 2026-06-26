using UnityEngine;

namespace ProjectATLAS.UI
{
    public class UIClosePageAction : UIAction
    {
        [SerializeField] private UIPage pageToClose;
        
        protected override void OnClick()
        {
            if (pageToClose) pageToClose.ClosePage();
        }
    }
}
