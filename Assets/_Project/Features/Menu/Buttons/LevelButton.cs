using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KBCore.Refs;

using ProjectATLAS.Gameplay;

namespace ProjectATLAS.Menu
{
    public class LevelButton : MonoBehaviour
    {
        // INSPECTOR FIELDS
        [Header("Settings")]
        [SerializeField] private bool enable = true;
        [SerializeField] private LevelData levelData;
        [SerializeField] private bool bronze;
        [SerializeField] private bool silver;
        [SerializeField] private bool gold;
        
        [Header("Visuals")]
        [SerializeField] private Vector2 baseSize = new(1186, 1024);
        [SerializeField] private float height = 150;
        [SerializeField] private Sprite sprite;
        [SerializeField] private float fontSize = 50;
        
        [Header("Components")]
        [SerializeField, Self] private Button button;
        [SerializeField] private Image bronzeImage;
        [SerializeField] private Image silverImage;
        [SerializeField] private Image goldImage;
        
        
        // PROPERTIES
        public LevelData LevelData => levelData;
        public event Action<LevelButton, LevelData> OnClicked;
        
        
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
            if (levelData == null)
            {
                gameObject.name = "Level Button";
                textComponent.text = "0";
                textComponent.fontSize = fontSize;
            }
            else
            {
                gameObject.name = "Level " + levelData.Number;
                textComponent.text = levelData.Number.ToString();
                textComponent.fontSize = fontSize;
            }
            
            // SET ENABLE
            SetEnable(enable);
            
            // SET MEDALS
            SetMedals(bronze, silver, gold);
        }
        
        
        // PUBLIC METHODS
        public void SetMedals(bool bronze, bool silver, bool gold)
        {
            this.bronze = bronze;
            this.silver = silver;
            this.gold = gold;
            
            if (bronzeImage) bronzeImage.gameObject.SetActive(bronze);
            if (silverImage) silverImage.gameObject.SetActive(silver);
            if (goldImage) goldImage.gameObject.SetActive(gold);
        }
        
        public void SetEnable(bool enable)
        {
            this.enable = enable;
            if (button) button.interactable = enable;
        }
        
        
        // PRIVATE METHODS
        private void Button_onClick()
        {
            OnClicked?.Invoke(this, levelData);
        }
    }
}
