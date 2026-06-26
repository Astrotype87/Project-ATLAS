using KBCore.Refs;
using ProjectATLAS.Input;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectATLAS
{
    public class Ship2D : MonoBehaviour
    {
        [Header("Physics")]
        [SerializeField, Self] private new Rigidbody2D rigidbody2D;
        [SerializeField] private float thrust;
        [SerializeField] private float steer;
        [SerializeField] private float grip;
        [SerializeField, Range(0, 1)] private float brakePercent;
        
        [Header("Input")]
        [SerializeField] private InputButton thrustButton;
        [SerializeField] private InputButton steerLeftButton;
        [SerializeField] private InputButton steerRightButton;
        [SerializeField] private InputButton brakeButton;
        
        private void FixedUpdate()
        {
            // Apply thrust force
            float thrustInput = thrustButton.Value;
            Vector2 forward = transform.up;
            Vector2 appliedThrust = thrust * thrustInput * forward;
            rigidbody2D.AddForce(appliedThrust);
            
            // Apply steer torque
            float steerInput = -steerLeftButton.Value + steerRightButton.Value;
            float appliedSteer = steer * -steerInput;
            rigidbody2D.AddTorque(appliedSteer);
            
            Debug.Log($"steerInput: {steerInput}  appliedSteer: {appliedSteer}");
            
            // Apply grip
            Vector2 side = transform.right;
            float sideSpeed = Vector2.Dot(rigidbody2D.linearVelocity, side);
            Vector2 appliedGrip = -sideSpeed * grip * side;
            rigidbody2D.AddForce(appliedGrip);
            
            // Apply brake
            float brakeInput = brakeButton.Value;
            rigidbody2D.linearVelocity *= 1f - brakePercent * brakeInput;
        }
    }
}
