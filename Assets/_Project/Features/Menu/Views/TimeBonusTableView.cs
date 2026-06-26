using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;

using ProjectATLAS.Gameplay;

namespace ProjectATLAS
{
    public class TimeBonusTableView : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private TimeBonus timeBonus;
        
        [Header("Components")]
        [SerializeField, Child] private List<TimeBonusRow> timeBonusRows;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            DisplayTimeBonus(timeBonus);
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }
        
        
        // PUBLIC METHODS
        public void DisplayTimeBonus(TimeBonus timeBonus)
        {
            this.timeBonus = timeBonus;
            
            // Handle time bonus row duplication logic
            int targetChildCount = Mathf.FloorToInt(timeBonus.timeBonuses.Count) + 1;
            int interval = targetChildCount - transform.childCount;
            
            if (interval > 0)
            {
                for (int i = 0; i < interval; i++)
                {
                    AddTimeBonusRow();
                }
            }
            else if (interval < 0)
            {
                interval = -interval; // flip the sign
                for (int i = 0; i < interval; i++)
                {
                    RemoveTimeBonusRow();
                }
            }
            
            // Update each points item view
            for (int i = 0; i < timeBonus.timeBonuses.Count; i++)
            {
                TimeBonusEntry timeBonusEntry = timeBonus.timeBonuses[i];
                timeBonusRows[i].SetTimeBonus(timeBonusEntry.seconds, timeBonusEntry.score);
            }
            
            // Last row
            timeBonusRows[^1].SetTimeBonusLowest(timeBonus.lowestScore);
        }
        
        // PRIVATE METHODS
        private void AddTimeBonusRow()
        {
            GameObject gameObject = transform.GetChild(0).gameObject;
            GameObject newGameObject = Instantiate(gameObject, transform);
            
            if (newGameObject.TryGetComponent(out TimeBonusRow timeBonusRow))
            {
                timeBonusRows.Add(timeBonusRow);
            }
        }
        
        private void RemoveTimeBonusRow()
        {
            int index = transform.childCount - 1;
            GameObject gameObject = transform.GetChild(index).gameObject;
            
            if (gameObject.TryGetComponent(out TimeBonusRow timeBonusRow))
            {
                timeBonusRows.Remove(timeBonusRow);
            }
            
            if (index <= 0) return;
            Destroy(gameObject);
        }
    }
}
