using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectATLAS.Simulation.Sim02_ProjectileMotion
{
    public class GoalTrigger : MonoBehaviour
    {
        public TMP_Text messageText; // drag MessageText here
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (messageText != null) messageText.text = "Reached Goal (trigger)!";
                Debug.Log("Goal reached by " + other.name);
            }
        }
    }
}
