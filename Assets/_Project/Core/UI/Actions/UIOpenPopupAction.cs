using UnityEngine;

namespace ProjectATLAS.UI
{
    public class UIOpenPopupAction : UIAction
    {
        [SerializeField] private UIPopup popupToOpen;
        
        protected override void OnClick()
        {
            if (popupToOpen) popupToOpen.OpenPopup();
        }
    }
}
