using UnityEngine;

namespace ProjectATLAS.Minigame.Mini04_CargoAirshipStartup
{
    public class GoalTrigger : MonoBehaviour
    {
        public CargoLevelManager levelManager;
        private SpriteRenderer goalRenderer;
        private bool isActivated = false;

        private void Start()
        {
            goalRenderer = GetComponent<SpriteRenderer>();

            if (levelManager == null)
                levelManager = FindObjectOfType<CargoLevelManager>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isActivated) return;

            if (other.CompareTag("Box"))
            {
                if (levelManager != null && goalRenderer != null)
                {
                    levelManager.AddGoal(goalRenderer);
                    other.gameObject.SetActive(false);
                    isActivated = true;
                }
            }
        }

        public void ResetGoal()
        {
            isActivated = false;
            if (goalRenderer != null)
                goalRenderer.color = Color.white;
        }
    }
    
}
