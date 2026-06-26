using UnityEngine;

public class WaterManager : MonoBehaviour
{
    [Header("Water Backgrounds")]
    public Transform[] waters;   // Assign 4 water backgrounds here
    public float spacing = 17.72f; // Distance between each water sprite

    void Start()
    {
        if (waters.Length != 4)
        {
            Debug.LogError("Please assign exactly 4 water backgrounds in the array!");
            return;
        }

        // Position waters correctly
        for (int i = 0; i < waters.Length; i++)
        {
            Vector3 pos = waters[i].position;
            pos.x = i * spacing;
            waters[i].position = pos;
        }
    }
}

