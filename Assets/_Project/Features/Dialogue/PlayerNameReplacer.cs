using UnityEngine;
using TMPro;
using KBCore.Refs;
using ProjectATLAS.GameData;
using System.Collections;

namespace ProjectATLAS
{
    public class PlayerNameReplacer : MonoBehaviour
    {
        [SerializeField, Self] private TMP_Text textComponent;
        
        private void Awake()
        {
            GameDataManager.Instance.OnGameDataLoaded += HandleGameDataLoaded;
            
            // if (textComponent)
            // {
            //     string displayName = GameDataService.Instance.DetailsData.displayName;
            //     string currentText = textComponent.text;
                
            //     Debug.Log($"{displayName}");
                
            //     textComponent.text = currentText.Replace("{player}", displayName);
            // }
        }
        
        private void HandleGameDataLoaded(GameData.GameData gameData)
        {
            if (textComponent)
            {
                string displayName = GameDataManager.Instance.DetailsData.displayName;
                string currentText = textComponent.text;
                
                Debug.Log($"{displayName}");
                
                textComponent.text = currentText.Replace("{player}", displayName);
            }
        }
    }
}
