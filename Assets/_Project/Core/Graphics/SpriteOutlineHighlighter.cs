using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectATLAS.Graphics
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteOutlineHighlighter : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
    {
        [Header("Outline Colors")]
        [SerializeField] private Color normalColor = Color.white;  
        [SerializeField] private Color pressedColor = Color.green;
        [SerializeField] private Color selectedColor = Color.yellow;
        
        [Header("Outline Settings")]
        [SerializeField] [Range(0f, 10f)] private float normalWidth = 2f;
        [SerializeField] [Range(0f, 10f)] private float pressedWidth = 2f;
        [SerializeField] [Range(0f, 10f)] private float selectedWidth = 2f;
        
        private SpriteRenderer sr;
        private MaterialPropertyBlock mpb;
        private bool isSelected = false;
        
        // Shader property IDs
        private static readonly int SelectedID = Shader.PropertyToID("_Selected");
        private static readonly int OutlineColorID = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineWidthID = Shader.PropertyToID("_OutlineWidth");
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            mpb = new MaterialPropertyBlock();
            SetOutline(normalColor, normalWidth, true);
        }
        
        private void OnValidate()
        {
            // Ensure SpriteRenderer and MPB exist, even in edit mode
            if (sr == null) sr = GetComponent<SpriteRenderer>();
            
            if (mpb == null) mpb = new MaterialPropertyBlock();
            
            // When editing in Inspector, always set to normal state unless "selected"
            if (isSelected)
                SetOutline(selectedColor, selectedWidth, true);
            else
                SetOutline(normalColor, normalWidth, true);
        }
        
        private void SetOutline(Color color, float width, bool enabled)
        {
            sr.GetPropertyBlock(mpb);
            mpb.SetFloat(SelectedID, enabled ? 1f : 0f);
            mpb.SetColor(OutlineColorID, color);
            mpb.SetFloat(OutlineWidthID, width);
            sr.SetPropertyBlock(mpb);
        }
        
        
        // POINTER METHODS
        public void OnPointerDown(PointerEventData eventData)
        {
            SetOutline(pressedColor, pressedWidth, true);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (isSelected)
                SetOutline(selectedColor, selectedWidth, true);
            else
                SetOutline(normalColor, normalWidth, true);
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            isSelected = true;
            SetOutline(selectedColor, selectedWidth, true);
        }
        
        public void OnDeselect(BaseEventData eventData)
        {
            isSelected = false;
            SetOutline(normalColor, normalWidth, true);
        }
    }
}
