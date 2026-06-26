using UnityEngine;

namespace ProjectATLAS.Graphics
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteBlend : MonoBehaviour
    {
        [SerializeField] private BlendMode blendMode;
        
        private Material materialInstance;
        private SpriteRenderer spriteRenderer;
        
        private static readonly int BlendModeID = Shader.PropertyToID("_BlendMode");
        private static Shader cachedBlendShader;
        
        
        private void OnEnable()
        {
            RefreshMaterial();
        }
        
        private void OnDisable()
        {
            ResetMaterialToDefault();
            CleanupMaterial();
        }
        
        private void OnValidate()
        {
            if (enabled)
                RefreshMaterial();
            else
                ResetMaterialToDefault();
        }
        
        
        
        private void RefreshMaterial()
        {
            if (!spriteRenderer)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
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
            spriteRenderer.sharedMaterial = materialInstance;
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
            if (spriteRenderer)
                spriteRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
        }
        
    }
}
