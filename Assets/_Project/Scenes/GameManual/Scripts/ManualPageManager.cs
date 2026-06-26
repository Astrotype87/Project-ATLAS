using UnityEngine;

public class ManualPageManager : MonoBehaviour
{
    public Transform pageParent;
    public GameObject[] pagePrefabs;

    private GameObject currentPage;

    public void LoadPage(int pageIndex)
    {
        // Clear current page
        if (currentPage != null)
            Destroy(currentPage);

        // Instantiate new page
        currentPage = Instantiate(pagePrefabs[pageIndex], pageParent);
    }

    void Start()
    {
        LoadPage(0); // Load the first page (e.g., Basics) on start
    }


}