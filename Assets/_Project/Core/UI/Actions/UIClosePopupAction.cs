using UnityEngine;

namespace ProjectATLAS.UI
{
    public class UIClosePopupAction : UIAction
    {
        [SerializeField] private UIPopup popupToClose;
        
        protected override void OnClick()
        {
            if (popupToClose) popupToClose.ClosePopup();
        }
    }
}
