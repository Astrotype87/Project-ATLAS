using UnityEngine;
using KBCore.Refs;

namespace ProjectATLAS.UI
{
    public class UIBackPageAction : UIAction
    {
        [SerializeField, Parent(Flag.Editable)] private UIPageGroup pageGroup;
        
        protected override void OnClick()
        {
            if (pageGroup) pageGroup.BackPage();
        }
    }
}
