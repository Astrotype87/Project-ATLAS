using UnityEngine;
using UnityEngine.UI;
using ProjectATLAS.Graphics;

namespace ProjectATLAS.UI
{
    [RequireComponent(typeof(Graphic))]
    public class UIBlend : MonoBehaviour
    {
        [SerializeField] private BlendMode blendMode;
        private Material materialInstance;
        private Graphic graphic;
        
        private static readonly int BlendModeID = Shader.PropertyToID("_BlendMode");
        private static Shader cachedBlendShader;
        
        
        private void OnEnable()
        {
            RefreshMaterial();
        }
        
        private void OnValidate()
        {
            if (enabled)
                RefreshMaterial();
            else
                ResetMaterialToDefault();
        }
        
        private void OnDisable()
        {
            ResetMaterialToDefault();
            CleanupMaterial();
        }
        
        
        
        private void RefreshMaterial()
        {
            if (!graphic)
                graphic = GetComponent<Graphic>();
            
            if (!materialInstance)
            {
                cachedBlendShader = Shader.Find("Custom/Blend");
                if (!cachedBlendShader)
                {
                    Debug.LogError("Shader 'Custom/Blend' not found.");
                    return;
                }
                
                materialInstance = new Material(cachedBlendShader)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
            }
            
            materialInstance.SetInt(BlendModeID, (int)blendMode);
            graphic.material = materialInstance;
        }
        
        private void CleanupMaterial()
        {
            if (materialInstance)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(materialInstance);
                else
                    Destroy(materialInstance);
#else
                Destroy(materialInstance);
#endif
                materialInstance = null;
            }
        }
        
        private void ResetMaterialToDefault()
        {
            if (graphic)
                graphic.material = null;
        }
    }
    
}
