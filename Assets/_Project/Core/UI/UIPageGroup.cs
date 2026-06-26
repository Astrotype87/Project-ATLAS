using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ProjectATLAS.UI
{
    public class UIPageGroup : MonoBehaviour
    {
        [SerializeField] private bool displayFirstPageOnStart = true;
        [SerializeField] private List<UIPage> pages;
        [SerializeField] private bool dontUseStack;
        
        private UIPage currentPage;
        private readonly Stack<UIPage> pageStack = new();
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            if (pages != null && pages.Count != 0)
            {
                foreach (var page in pages)
                {
                    if (page)
                    {
                        page.SetPageGroup(this);
                        page.ClosePage();
                    }
                }
            }
            
            if (displayFirstPageOnStart && pages[0] != null)
            {
                pages[0].OpenPageInGroup();
            }
        }
        
        
        // PUBLIC METHODS
        /// <summary> Returns true if the page is part of this page group. </summary>
        public bool ContainsPage(UIPage page)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i] == page) return true;
            }
            
            return false;
        }
        
        /// <summary> Shows the next page and store the previous page to stack. </summary>
        public void OpenPage(UIPage page)
        {
            if (page == null) return;
            if (!ContainsPage(page)) return;
            
            bool isAlreadyOpened = page.IsVisible;
            bool isCurrentPage = currentPage == page;
            
            if (currentPage && !isCurrentPage)
            {
                currentPage.ClosePage();
                
                if (!dontUseStack)
                    pageStack.Push(currentPage);
            }
            
            currentPage = page;
            if (!isAlreadyOpened) currentPage.OpenPage();
        }
        
        /// <summary> Shows the previous page from stack. </summary>
        public void BackPage()
        {
            if (dontUseStack)
            {
                Debug.LogWarning($"{name}: BackPage() called but 'dontUseStack' is enabled.");
                return;
            }
            
            if (currentPage)
                currentPage.ClosePage();
            
            if (pageStack.Count > 0)
            {
                currentPage = pageStack.Pop();
                currentPage.OpenPage();
            }
            else
            {
                Debug.LogWarning($"{name}: No previous pages in stack.");
            }
        }
        
        /// <summary> Clears the page stack. </summary>
        public void ClearPageStack()
        {
            pageStack.Clear();
        }
        
        /// <summary> Loads the first page. </summary>
        public void LoadFirstPage()
        {
            if (pages != null && pages.Count > 0 && pages[0] != null)
            {
                OpenPage(pages[0]);
            }
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(UIPageGroup))]
    public class UIPageGroupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            UIPageGroup script = (UIPageGroup)target;
            
            if (GUILayout.Button("Get All Pages"))
            {
                Undo.RecordObject(script, "Get All Pages");
                
                // Get all UIPage components under this group
                List<UIPage> allPages = script.GetComponentsInChildren<UIPage>(true).ToList();
                
                // Get all nested UIPageGroups under this one (children, grandchildren, etc.)
                List<UIPageGroup> childGroups = script
                    .GetComponentsInChildren<UIPageGroup>(true)
                    .Where(g => g != script) // exclude this group itself
                    .ToList();
                
                // Collect all pages belonging to those child groups
                HashSet<UIPage> pagesInChildGroups = new();
                foreach (var childGroup in childGroups)
                {
                    var childPages = childGroup.GetComponentsInChildren<UIPage>(true);
                    foreach (var p in childPages)
                        pagesInChildGroups.Add(p);
                }
                
                // Filter out pages that belong to nested groups
                List<UIPage> filteredPages = allPages
                    .Where(p => !pagesInChildGroups.Contains(p))
                    .ToList();
                
                // Assign filtered list to the private field
                script.GetType()
                    .GetField("pages", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.SetValue(script, filteredPages);
                
                EditorUtility.SetDirty(script);
            }
        }
    }
#endif
}
