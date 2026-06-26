using UnityEngine;
using KBCore.Refs;

using AstrotypeTools.InspectorAttributes;

using ProjectATLAS.UI;
using static ProjectATLAS.Menu.Header;

namespace ProjectATLAS.Menu
{
    public class PageHeader : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private bool visible = true;
        [SerializeField, EnableIf(nameof(visible))] private bool getPageName = true;
        [SerializeField, EnableIf(nameof(EnableHeaderTextField))] private string headerText;
        [SerializeField, EnableIf(nameof(visible))] private bool allCaps;
        [SerializeField, EnableIf(nameof(visible))] private BarMode barMode;
        
        [Header("Components")]
        [SerializeField, Scene] private Header header;
        [SerializeField] private CustomHeader customHeader;
        [SerializeField, Self] private UIPage uIPage;
        
        private bool EnableHeaderTextField => visible && !getPageName;
        
        private void OnValidate()
        {
            this.ValidateRefs();
            if (getPageName && uIPage) headerText = uIPage.PageName;
            
            if (gameObject.activeInHierarchy)
            {
                UIPage_OnOpened();
            }
        }
        
        private void OnEnable()
        {
            uIPage.OnOpened += UIPage_OnOpened;
            uIPage.OnClosed += UIPage_OnClosed;
        }
        
        private void OnDisable()
        {
            uIPage.OnOpened -= UIPage_OnOpened;
            uIPage.OnClosed -= UIPage_OnClosed;
        }
        
        private void UIPage_OnOpened()
        {
            if (header)
            {
                header.SetVisible(visible);
                header.SetText(headerText, allCaps);
                header.SetBarMode(barMode);
            }
            if (customHeader) customHeader.Show();
        }
        
        private void UIPage_OnClosed()
        {
            if (customHeader) customHeader.Hide();
        }
    }
}
