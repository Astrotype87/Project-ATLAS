using KBCore.Refs;
using TMPro;
using UnityEngine;

namespace ProjectATLAS
{
    public class ChapterLabel : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private int chapterID;
        [SerializeField] private string chapterName;
        
        [Header("Components")]
        [SerializeField, Child] private TMP_Text textComponent;
        
        private void OnValidate()
        {
            this.name = $"Chapter {chapterID} Label";
            if (textComponent) textComponent.text = $"CHAPTER {chapterID}: {chapterName}";
        }
    }
}
