using System;
using UnityEngine;
using static ProjectATLAS.Minigame.Mini03_HoverRacing.HovercraftPhysics;

namespace ProjectATLAS.Minigame.Mini03_HoverRacing
{
    public class HovercraftDebug : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private HovercraftPhysics hovercraftPhysics2D;
        
        [Header("Debug")]
        public bool debugMainSensor;
        public bool debugHoverSensor;
        public bool debugPitch;
        
        [Header("Sensor Gizmos")]
        public float hitRadius;
        public float forceRadius;
        public float heightRoundUp;
        public float normalLength;
        
        [Header("Log")]
        public bool logPitchAngle;
        public bool logMainSensor;
        
        private MainSensors mainSensors         => hovercraftPhysics2D.DebugLiveData.mainSensors;
        private Vector2[] hoverSensors          => hovercraftPhysics2D.DebugLiveData.hoverSensors;
        private LayerMask groundLayer           => hovercraftPhysics2D.DebugLiveData.hoverGroundLayer;
        private float hoverHeight               => hovercraftPhysics2D.DebugLiveData.hoverHeight;
        private float hoverRange                => hovercraftPhysics2D.DebugLiveData.hoverRange;
        private float hoverPitchEffect          => hovercraftPhysics2D.DebugLiveData.hoverPitchEffect;
        private Direction direction             => hovercraftPhysics2D.DebugLiveData.direction;
        
        private bool _isOnGround;
        private SensorInfo _frontInfo;
        private SensorInfo _backInfo;
        
        private void OnDrawGizmos()
        {
            if (!rigidbody2D || !hovercraftPhysics2D || !this.enabled) return;
            
            if (debugMainSensor) DebugMainSensor();
            if (debugHoverSensor) DebugHoverSensor();
            if (debugPitch) DebugPitch();
        }
        
        private void DebugMainSensor()
        {
            Vector3 sensorPosition = rigidbody2D.transform.position;
            Vector3 sensorDirection = -rigidbody2D.transform.up;
            
            RaycastHit2D hitInfo = Physics2D.Raycast(sensorPosition, sensorDirection, hoverRange, groundLayer);
            if (hitInfo.collider)
            {
                DrawRay(sensorPosition, sensorDirection * hitInfo.distance, new Color(1, 1, 1, 0.25f));
                DrawSphere(hitInfo.point, hitRadius, new Color(1, 1, 1, 0.25f));
                
                Vector3 groundForward = Vector3.ProjectOnPlane(rigidbody2D.transform.forward, hitInfo.normal).normalized;
                Vector3 groundRight = Vector3.ProjectOnPlane(rigidbody2D.transform.right, hitInfo.normal).normalized;
                
                DrawRay(hitInfo.point, groundForward * normalLength, new Color(0, 1, 0, 0.5f));
                DrawRay(hitInfo.point, groundRight * normalLength, new Color(1, 0, 0, 0.5f));
                if (logMainSensor) Debug.Log("Ground Forward: " + groundForward + "  Ground Right: " + groundRight);
            }
            else
            {
                DrawRay(sensorPosition, sensorDirection * hoverRange, new Color(1, 1, 1, 0.125f));
            }
        }
        
        private void DebugHoverSensor()
        {
            if (!debugHoverSensor) return;
            
            for (int i = 0; i < hoverSensors.Length; i++)
            {
                Vector3 forceOffset = rigidbody2D.transform.TransformVector(
                    Vector3.Scale(hoverSensors[i], new Vector3(hoverPitchEffect, 0)));
                DrawSphere(transform.position + forceOffset, forceRadius, Color.white);
                
                Vector3 sensorOffset = rigidbody2D.transform.TransformVector(hoverSensors[i]);
                Vector3 hoverPosition = transform.position + sensorOffset;
                Vector3 hoverDirection = direction == Direction.World ? Vector3.down : -transform.up;
                
                if (direction == Direction.Ground)
                {
                    DrawSensor(hoverPosition, hoverDirection, hoverRange, new Color(1, 1, 1, 0.33f), Color.clear);
                    RaycastHit2D sensorHit = Physics2D.Raycast(hoverPosition, hoverDirection, hoverRange, groundLayer);
                    if (sensorHit.collider) hoverDirection = -sensorHit.normal;
                    DrawRay(sensorHit.point, sensorHit.normal * normalLength, new Color(1, 1, 0, 0.67f));
                }
                
                DrawSensor(hoverPosition, hoverDirection, hoverRange, Color.blue, Color.red);
                DrawSensor(hoverPosition, hoverDirection, hoverHeight + heightRoundUp, Color.blue, Color.green);
            }
            
            void DrawSensor(Vector3 position, Vector3 direction, float range, Color hitColor, Color notHitColor)
            {
                RaycastHit2D hitInfo = Physics2D.Raycast(position, direction, range, groundLayer);
                if (hitInfo.collider)
                {
                    DrawRay(position, direction * hitInfo.distance, hitColor);
                    DrawSphere(hitInfo.point, hitRadius, hitColor);
                }
                else
                {
                    DrawRay(position, direction * range, notHitColor);
                }
            }
        }
        
        private void DebugPitch()
        {
            // Group settings into tuple
            (Transform, Direction, float, LayerMask) settings
                = (rigidbody2D.transform, direction, hoverRange, mainSensors.groundLayer);
            
            // Update sensors
            _frontInfo.UpdateSensor(mainSensors.front, settings);
            _backInfo.UpdateSensor(mainSensors.back, settings);
            
            Vector3 pitchAxis = rigidbody2D.transform.right;
            Vector3 endpointForward = Vector3.ProjectOnPlane(_frontInfo.endpoint - _backInfo.endpoint, pitchAxis).normalized;
            float pitchAngle = Vector3.SignedAngle(endpointForward, rigidbody2D.transform.forward, pitchAxis);
            
            DrawSphere(_frontInfo.endpoint, hitRadius / 2, new Color(1, 1, 1, 0.5f));
            DrawSphere(_backInfo.endpoint, hitRadius / 2, new Color(1, 1, 1, 0.5f));
            DrawRay(_backInfo.endpoint, _frontInfo.endpoint - _backInfo.endpoint, new Color(1, 1, 0, 0.25f));
            DrawRay(rigidbody2D.transform.position, endpointForward * 3, Color.green);
            
            if (logPitchAngle) Debug.Log("Pitch Angle: " + pitchAngle);
            
            // string grdFwdTxt = "USE BOTH FRONT & BACK SENSOR";
            // if (_frontSensorInfo.isHit ^ _backSensorInfo.isHit)
            //     if (_frontSensorInfo.isHit && pitch.frontRecovery)
            //         grdFwdTxt = "USE FRONT SENSOR";
            //     else if (_backSensorInfo.isHit && pitch.backRecovery)
            //         grdFwdTxt = "USE BACK SENSOR";
            
            // Debug.Log($"LOG PITCH GROUND: {grdFwdTxt}");
        }
        
        // GIZMOS METHODS
        private void DrawRay(Vector3 from, Vector3 direction, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(from, direction);
        }
        
        private void DrawSphere(Vector3 center, float radius, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(center, radius);
        }
        
        
        [Serializable] public class DebugLiveData
        {
            public MainSensors mainSensors;
            public Vector2[] hoverSensors;
            public LayerMask hoverGroundLayer;
            public float hoverHeight;
            public float hoverRange;
            public float hoverPitchEffect;
            public Direction direction;
        }
    }
}
