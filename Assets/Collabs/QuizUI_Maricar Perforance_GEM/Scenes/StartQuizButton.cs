using UnityEngine;

public class StartQuizButton : MonoBehaviour
{
    public GameObject canvasToShow;

    public void ShowCanvas()
    {
        canvasToShow.SetActive(true);
    }

    // Optional: Hide current canvas or panel
    public GameObject canvasToHide;

    public void ShowNewCanvasAndHideOld()
    {
        Debug.Log("Button clicked! Switching canvases...");
        if (canvasToHide != null)
            canvasToHide.SetActive(false);

        if (canvasToShow != null)
            canvasToShow.SetActive(true);
    }
}
