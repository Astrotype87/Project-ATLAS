using UnityEngine;
using TMPro;

public class AtmosphereLayerManager : MonoBehaviour
{
    [Header("References")]
    public Transform rocket;
    public TMP_Text atmosphereTMP;
    public Camera mainCamera;  // Drag your Main Camera here

    [Header("Background Settings")]
    public float backgroundHeight = 10f;   // height of 1 background piece
    public int backgroundsPerLayer = 6;    // how many backgrounds per atmosphere layer

    private string[] layers = { "Troposphere", "Stratosphere", "Mesosphere", "Thermosphere", "Exosphere" };

    [Header("Atmosphere Colors")]
    public Color troposphereColor = new Color(0.4f, 0.7f, 1f);     // light blue
    public Color stratosphereColor = new Color(0.1f, 0.5f, 0.9f);  // deeper blue
    public Color mesosphereColor = new Color(0.05f, 0.05f, 0.4f);  // dark blue
    public Color thermosphereColor = new Color(0.1f, 0f, 0.2f);    // space purple
    public Color exosphereColor = new Color(0f, 0f, 0f);           // black

    private Color[] layerColors;

    void Start()
    {
        layerColors = new Color[] {
            troposphereColor,
            stratosphereColor,
            mesosphereColor,
            thermosphereColor,
            exosphereColor
        };

        if (!mainCamera)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (!rocket) return;

        float layerHeight = backgroundHeight * backgroundsPerLayer;
        float altitude = Mathf.Max(0, rocket.position.y);

        // current and next layer index
        int currentLayer = Mathf.FloorToInt(altitude / layerHeight);
        currentLayer = Mathf.Clamp(currentLayer, 0, layerColors.Length - 1);

        int nextLayer = Mathf.Clamp(currentLayer + 1, 0, layerColors.Length - 1);

        // blend factor between current layer and next
        float blendFactor = (altitude % layerHeight) / layerHeight;

        // blend colors
        Color blendedColor = Color.Lerp(layerColors[currentLayer], layerColors[nextLayer], blendFactor);
        mainCamera.backgroundColor = blendedColor;

        // update text
        if (atmosphereTMP)
        {
            atmosphereTMP.text = "Atmosphere: " + layers[currentLayer];
        }
    }
}
