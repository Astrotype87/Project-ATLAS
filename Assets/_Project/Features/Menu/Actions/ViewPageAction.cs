using UnityEngine;
using KBCore.Refs;
using ProjectATLAS.UI;

namespace ProjectATLAS.Menu
{
    public class ViewPageAction : UIAction
    {
        [SerializeField, Parent(Flag.Editable)] private UIPage pageToClose;
        [SerializeField] private UIPage pageToOpen;
        
        protected override void OnClick()
        {
            pageToClose.ClosePage();
            pageToOpen.OpenPageInGroup();
        }
    }
}
