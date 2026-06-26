using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;

using ProjectATLAS.UI;

namespace ProjectATLAS.Menu
{
    public class LoadSceneAction : UIAction
    {
        [SerializeField] private SceneReference sceneToLoad;
        
        protected override void OnClick()
        {
            SceneManager.LoadScene(sceneToLoad.BuildIndex);
        }
    }
}
