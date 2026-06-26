using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;

using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Menu
{
    public class TestButton : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Settings")]
        [SerializeField] private bool enable = true;
        [SerializeField] private TestData testData;
        
        [Header("Visuals")]
        [SerializeField] private Vector2 baseSize = new(1186, 1024);
        [SerializeField] private float height = 150;
        [SerializeField] private Sprite sprite;
        [SerializeField] private float fontSize = 50;
        
        [Header("Components")]
        [SerializeField, Self] private Button button;
        
        
        // PROPERTIES
        public TestData TestData => testData;
        public event Action<TestButton, TestData> OnClicked;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            button.onClick.AddListener(Button_onClick);
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
            
            // SET HEXAGON SIZE
            RectTransform rectTransform = gameObject.transform as RectTransform;
            rectTransform.sizeDelta = new Vector2(baseSize.x / baseSize.y * height, height);
            
            // SET IMAGE
            Image imageComponent = GetComponent<Image>();
            imageComponent.sprite = sprite;
            
            // SET LABEL TEXT
            TMP_Text textComponent = GetComponentInChildren<TMP_Text>();
            if (testData == null)
            {
                gameObject.name = "Test Button";
                textComponent.text = "0";
                textComponent.fontSize = fontSize;
            }
            else
            {
                gameObject.name = testData.Type switch {
                    LevelType.PreTest => "Pre Test " + testData.Chapter,
                    LevelType.PostTest => "Post Test " + testData.Chapter,
                    _ => "Test " + testData.Chapter
                };
                textComponent.text = testData.Type switch {
                    LevelType.PreTest => "Pre\nTest " + testData.Chapter,
                    LevelType.PostTest => "Post\nTest " + testData.Chapter,
                    _ => "Test " + testData.Chapter
                };
                textComponent.fontSize = fontSize;
            }
            
            // SET ENABLE
            SetEnable(enable);
        }
        
        
        // PUBLIC METHODS
        public void SetEnable(bool enable)
        {
            this.enable = enable;
            if (button) button.interactable = enable;
        }
        
        
        // PRIVATE METHODS
        private void Button_onClick()
        {
            OnClicked?.Invoke(this, testData);
        }
    }
}
