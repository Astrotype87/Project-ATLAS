using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Box : MonoBehaviour
{
    [Range(1f, 5f)]
    public float weight = 1f;
}
