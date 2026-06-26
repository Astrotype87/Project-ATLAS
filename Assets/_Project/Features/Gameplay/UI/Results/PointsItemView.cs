using TMPro;
using UnityEngine;

namespace ProjectATLAS.Gameplay.UI
{
    public class PointsItemView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PointsEntry pointsEntry;
        
        [Header("Components")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text scoreText;
        
        // MONOBEHAVIOUR METHODS
        private void OnValidate()
        {
            DisplayPoints(pointsEntry);
        }
        
        // PUBLIC METHODS
        public void DisplayPoints(PointsEntry pointsEntry)
        {
            this.pointsEntry = pointsEntry;
            
            if (nameText) nameText.text = pointsEntry.name;
            if (scoreText) scoreText.text = pointsEntry.points.ToString();
        }
    }
}
