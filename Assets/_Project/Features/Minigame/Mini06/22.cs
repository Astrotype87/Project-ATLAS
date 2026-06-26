using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public float fallSpeed = 2f;
    public float destroyY = -5f;

    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}
