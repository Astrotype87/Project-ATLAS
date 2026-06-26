using System;
using UnityEngine;
using UnityEngine.UI;
using KBCore.Refs;
using PrimeTween;
using UnityEngine.Serialization;

namespace ProjectATLAS.Simulation.Sim01_PartsMeasurements
{
    public class MeasurementsPanel : MonoBehaviour
    {
        [SerializeField] private bool isOn;
        
        [Header("Components")]
        [SerializeField] private Button showHideButton;
        [SerializeField] private RectTransform showHideImage;
        [SerializeField] private Button checkButton;
        [SerializeField, Child(Flag.Editable)] private UnitField[] unitFields;
        
        [Header("Animation")]
        [SerializeField, FormerlySerializedAs("openAnimation")] private TweenSettings<Vector2> openPanel;
        [SerializeField, FormerlySerializedAs("closeAnimation")] private TweenSettings<Vector2> closePanel;
        [SerializeField] private TweenSettings<Vector3> openArrow;
        [SerializeField] private TweenSettings<Vector3> closeArrow;
        
        private RectTransform rectTransform;
        private Tween openPanelTween;
        private Tween closePanelTween;
        private Tween openArrowTween;
        private Tween closeArrowTween;
        
        
        // PROPERTIES
        public event Action OnCheckClicked;
        
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            showHideButton.onClick.AddListener(ShowHideButton_onClick);
            checkButton.onClick.AddListener(CheckButton_onClick);
            
            rectTransform = transform as RectTransform;
        }
        
        
        // PUBLIC METHODS
        public void HighlightField(string id, bool isHighlighted)
        {
            foreach (var field in unitFields)
            {
                if (field.LabelText == id)
                {
                    field.SetHighlight(isHighlighted);
                    return;
                }
            }
        }
        
        
        // PRIVATE METHODS
        public (string, float)[] GetAnswers()
        {
            (string variable, float answer)[] answers = new (string, float)[unitFields.Length];
            
            for (int i = 0; i < unitFields.Length; i++)
            {
                answers[i].variable = unitFields[i].LabelText;
                answers[i].answer = unitFields[i].InputText;
            }
            
            return answers;
        }
        
        
        
        // EVENT LISTENER METHODS
        private void ShowHideButton_onClick()
        {
            if (openPanelTween.isAlive) openPanelTween.Complete();
            if (closePanelTween.isAlive) closePanelTween.Complete();
            if (openArrowTween.isAlive) openArrowTween.Complete();
            if (closeArrowTween.isAlive) closeArrowTween.Complete();
            
            isOn = !isOn;
            
            if (isOn)
            {
                openPanelTween = Tween.UISizeDelta(rectTransform, openPanel);
                openArrowTween = Tween.EulerAngles(showHideImage, openArrow);
            }
            else
            {
                closePanelTween = Tween.UISizeDelta(rectTransform, closePanel);
                closeArrowTween = Tween.EulerAngles(showHideImage, closeArrow);
            }
        }
        
        private void CheckButton_onClick()
        {
            OnCheckClicked?.Invoke();
        }
    }
}
