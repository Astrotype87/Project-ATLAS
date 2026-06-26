using UnityEngine;
using UnityEngine.UI;

namespace ProjectATLAS.Input
{
    public class InputSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        
        public float Value => slider.value;
        
        public void SetValue(float value)
        {
            slider.value = value;
        }
    }
}
