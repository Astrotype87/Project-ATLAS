using UnityEngine;

namespace ProjectATLAS.UI
{
    public class UIOpenDialogueAction : UIAction
    {
        [SerializeField] private UIDialog dialog;
        
        protected override void OnClick()
        {
            dialog.OpenDialog();
        }
    }
}
