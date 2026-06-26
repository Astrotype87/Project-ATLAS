using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [System.Serializable]
    public class PopupData
    {
        public GameObject popupObject;   // The popup panel
        public MonoBehaviour popupScript; // The script controlling this popup
    }

    public PopupData[] popups; // Assign all popups and their scripts in the inspector

    /// <summary>
    /// Activate the popup at the given index and disable all others.
    /// </summary>
    /// <param name="index">Index of the popup to activate</param>
    public void ShowPopup(int index)
    {
        for (int i = 0; i < popups.Length; i++)
        {
            bool isActive = (i == index);
            // Activate/deactivate popup GameObject
            if (popups[i].popupObject != null)
                popups[i].popupObject.SetActive(isActive);

            // Enable/disable the script
            if (popups[i].popupScript != null)
                popups[i].popupScript.enabled = isActive;
        }
    }
}
