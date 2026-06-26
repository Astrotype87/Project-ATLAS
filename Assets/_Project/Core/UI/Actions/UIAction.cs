using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;

namespace ProjectATLAS.UI
{
    public abstract class UIAction : MonoBehaviour
    {
        [SerializeField, Self(Flag.Editable)] private Button button;
        
        protected virtual void Awake()
        {
            if (button) button.onClick.AddListener(OnClick);
        }
        
        protected virtual void OnValidate()
        {
            this.ValidateRefs();
        }
        
        protected abstract void OnClick();
    }
}
