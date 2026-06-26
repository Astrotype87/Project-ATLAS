using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform submarine;
    public SubmarineAirSystem airSystem;

    [Header("Movement")]
    public float playerSpeed = 3f;

    // internal
    private bool leftDown = false;
    private bool rightDown = false;
    private float originalAutoSpeed = -1f;
    private bool isAlive = true; // 🚨 new

    void Start()
    {
        if (submarine == null) submarine = transform;
        if (airSystem != null) originalAutoSpeed = airSystem.autoMoveSpeed;
    }

    void Update()
    {
        if (!isAlive) return; // 🚨 stop controls when dead

        float axis = 0f;
        if (leftDown) axis -= 1f;
        if (rightDown) axis += 1f;

        if (Mathf.Abs(axis) > 0.01f)
        {
            if (airSystem != null) airSystem.autoMoveSpeed = 0f;
            Vector3 pos = submarine.position;
            pos.x += axis * playerSpeed * Time.deltaTime;
            submarine.position = pos;
        }
        else
        {
            if (airSystem != null && originalAutoSpeed >= 0f) airSystem.autoMoveSpeed = originalAutoSpeed;
        }
    }

    // called by ButtonPointer on pointer down/up
    public void SetLeft(bool down) { if (isAlive) leftDown = down; }
    public void SetRight(bool down) { if (isAlive) rightDown = down; }

    // 🚨 NEW: Kill the player
    public void Die()
    {
        isAlive = false;
        if (airSystem != null) airSystem.autoMoveSpeed = 0f; // stop auto-move too
        Debug.Log("💀 Submarine destroyed!");
    }
}
