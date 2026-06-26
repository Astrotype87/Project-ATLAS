using UnityEngine;

public class LibraryPage : MonoBehaviour
{
    public GameObject guidebooksListPage;

    public void Open()
    {
        guidebooksListPage.SetActive(true);
    }
    public void Close()
    {
        guidebooksListPage.SetActive(false);
    }

}
