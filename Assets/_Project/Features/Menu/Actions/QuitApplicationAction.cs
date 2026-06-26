using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using ProjectATLAS.GameData;
using ProjectATLAS.System;
using System.Threading.Tasks;

namespace ProjectATLAS.UI
{
    public class QuitApplicationAction : UIAction
    {
        protected override void OnClick()
        {
            StartCoroutine(ExitGame());
        }
        
        private IEnumerator ExitGame()
        {
            if (GameDataManager.Instance)
            {
                yield return GameDataManager.Instance.SaveDataAsync();
            }
            
#if UNITY_EDITOR
            if (Application.isEditor) EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
