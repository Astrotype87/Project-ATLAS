using System;
using UnityEngine;

using ProjectATLAS.Utility;
using ProjectATLAS.Input;

namespace ProjectATLAS.Minigame.Mini03_HoverRacing
{
    public class HovercraftPhysics : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField, Range(0, 1)] private float hoverInput;
        [SerializeField, Range(-1, 1)] private float thrustInput;
        [SerializeField, Range(-1, 1)] private float airbrakeInput;
        [SerializeField] private InputSlider thrustSlider;
        [SerializeField] private InputButton airbrakeButton;
        
        [Header("Physics")]
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private MainSensors mainSensors;
        [SerializeField] private World world;
        [SerializeField] private Body body;
        [SerializeField] private Hover hover;
        [SerializeField] private Thrust thrust;
        [SerializeField] private Pitch pitch;
        [SerializeField] private float airbrakeDrag;
        
        [Header("Audio")]
        [SerializeField] private AudioSource engineSound;
        
        
        // EDITABLE PROPERTIES
        public Vector2 Gravity { get => world.gravity; set => world.gravity = value; }
        public float Drag { get => world.drag; set => world.drag = value; }
        public float Mass { get => body.mass; set => body.mass = value; }
        
        public float HoverForce
        {
            get => hover.force;
            set
            {
                hover.force = value;
                hover.pull = value;
            }
        }
        public float HoverDamping { get => hover.damping; set => hover.damping = value; }
        public float ThrustForce { get => thrust.force; set => thrust.force = value; }
        public float AirbrakeDrag { get => airbrakeDrag; set => airbrakeDrag = value; }
        
        public float HoverInput { get => hoverInput; set => hoverInput = value; }
        public float ThrustInput { get => thrustInput; set => thrustInput = value; }
        
        // AUTOMATIC/FIXED PROPERTIES
        public Vector2 Velocity => rigidbody2D.linearVelocity;
        public Vector2 VelocityKPH => rigidbody2D.linearVelocity * MPSToKPH;
        public Vector2 LocalVelocity => rigidbody2D.transform.TransformVector(rigidbody2D.linearVelocity);
        public Vector2 LocalVelocityKPH => rigidbody2D.transform.TransformVector(rigidbody2D.linearVelocity) * MPSToKPH;
        public Vector2 RightDirection => rigidbody2D.transform.right;
        public Vector2 UpDirection => rigidbody2D.transform.up;
        
        // CALCULATED PROPERTIES
        public Vector2 AppliedDrag { get; private set; }
        public Vector2 AppliedWeight { get; private set; }
        public Vector2 AppliedHover { get; private set; }
        public Vector2 AppliedPull { get; private set; }
        public Vector2 AppliedCounterSlip { get; private set; }
        public Vector2 AppliedThrust { get; private set; }
        
        public HovercraftDebug.DebugLiveData DebugLiveData {get; set;} = new();
        
        
        // PRIVATE GETTERS
        private float _hoverHitRatio => ((float) _hoverHitCount / hover.sensors.Length).NaNInfSafe();
        
        // PRIVATE VARIABLES: SENSORS
        private SensorInfo _frontSensorInfo;
        private SensorInfo _backSensorInfo;
        private bool isOnGround;
        
        // PRIVATE VARIABLES: HOVER
        private int _hoverHitCount;
        private Vector2[] _lastHoverPosition;
        
        // PRIVATE CONSTANTS
        private const float MPSToKPH = 3.6f;
        
        
        // MONOBEHAVIOUR METHODS
        private void Start()
        {
            _lastHoverPosition = new Vector2[hover.sensors.Length];
        }
        
        private void Update()
        {
            // thrustInput = 0;
            // if (Keyboard.current.dKey.isPressed) thrustInput += 1;
            // if (Keyboard.current.aKey.isPressed) thrustInput -= 1;
            
            thrustInput = thrustSlider.Value;
            airbrakeInput = airbrakeButton.Value;
        }
        
        private void FixedUpdate()
        {
            (Transform, Direction, float, LayerMask) settings
                = (rigidbody2D.transform, hover.hoverDirection, hover.range, mainSensors.groundLayer);
            
            _frontSensorInfo.UpdateSensor(mainSensors.front, settings);
            _backSensorInfo.UpdateSensor(mainSensors.back, settings);
            isOnGround = _frontSensorInfo.isHit || _backSensorInfo.isHit;
            
            ApplyGravity();
            ApplyDrag();
            ApplyAngularDrag();
            
            ApplyHover();
            ApplyPull();
            ApplyThrust();
            ApplyPitch();
        }
        
        private void LateUpdate()
        {
            engineSound.volume = thrustSlider.Value;
            engineSound.pitch = (thrustSlider.Value * 2f) + 1f;
        }
        
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                DebugLiveData ??= new();
            }
            
            DebugLiveData.mainSensors = mainSensors;
            DebugLiveData.hoverSensors = hover.sensors;
            DebugLiveData.hoverGroundLayer = hover.layer;
            
            DebugLiveData.hoverHeight = hover.height;
            DebugLiveData.hoverRange = hover.range;
            DebugLiveData.hoverPitchEffect = hover.pitchEffect;
            DebugLiveData.direction = hover.hoverDirection;
        }
        
        
        // PRIVATE METHODS
        private void ApplyGravity()
        {
            // UPDATE RIGIDBODY MASS
            rigidbody2D.mass = body.mass;
            
            // APPLY GRAVITY
            AppliedWeight = world.gravity * rigidbody2D.mass;
            rigidbody2D.AddForce(AppliedWeight, ForceMode2D.Force);
        }
        
        private void ApplyDrag()
        {
            // APPLY DRAG
            Vector2 velocity = rigidbody2D.linearVelocity;
            if (velocity.sqrMagnitude < 1e-6f) return;
            
            Vector2 localVelocity = rigidbody2D.transform.InverseTransformDirection(velocity);
            Vector2 scaledVelocity = localVelocity * body.dragArea;
            Vector2 worldVelocity = transform.TransformDirection(scaledVelocity);
            
            float airbrake = 1 + airbrakeInput * airbrakeDrag;
            float dragMagnitude = 0.5f * world.airDensity * world.drag * worldVelocity.sqrMagnitude;
            Vector2 dragForce = airbrake * dragMagnitude * -worldVelocity.normalized;
            
            // Avoid reverse velocity impulse (caused by extremely low values) and stop the object instead
            Vector2 impulse = dragForce * Time.fixedDeltaTime;
            Vector2 requiredImpulseToZero = -rigidbody2D.mass * velocity;
            
            if (impulse.magnitude >= requiredImpulseToZero.magnitude)
                dragForce = requiredImpulseToZero / Time.fixedDeltaTime;
            
            rigidbody2D.AddForce(AppliedDrag);
            
            AppliedDrag = dragForce;
        }
        
        private void ApplyAngularDrag()
        {
            // APPLY ANGULAR DRAG
            float angularVelocityRads = rigidbody2D.angularVelocity * Mathf.Deg2Rad;
            if (Mathf.Abs(angularVelocityRads) < 1e-4f) return;
            
            float torqueMagnitude = 0.5f * world.airDensity * world.angularDrag
                * body.angularArea * Mathf.Pow(body.effectiveRadius, 3)
                * angularVelocityRads * angularVelocityRads;
            float angularDrag = -Mathf.Sign(angularVelocityRads) * torqueMagnitude;
            
            // Avoid reverse velocity angular impulse (caused by extremely low values) and stop the object instead
            float angularImpulse = angularDrag * Time.fixedDeltaTime;
            float requiredAngularImpulseToZero = -rigidbody2D.inertia * angularVelocityRads;
            
            if (Mathf.Abs(angularImpulse) >= Mathf.Abs(requiredAngularImpulseToZero))
                angularDrag = requiredAngularImpulseToZero / Time.fixedDeltaTime;
            
            rigidbody2D.AddTorque(angularDrag);
        }
        
        private void ApplyHover()
        {
            AppliedHover = Vector2.zero;
            
            _hoverHitCount = 0;
            
            int hoverCount = hover.sensors.Length;
            for (int i = 0; i < hoverCount; i++)
            {
                Vector2 hoverOffset = rigidbody2D.transform.TransformVector(hover.sensors[i] * hover.hoverSpread);
                Vector2 forceOffset = rigidbody2D.transform.TransformVector(
                    Vector2.Scale(hover.sensors[i], new Vector2(hover.pitchEffect, 0)));
                
                Vector2 hoverPosition = rigidbody2D.position + hoverOffset;
                Vector2 hoverDirection =hover.hoverDirection switch
                {
                    Direction.World => Vector3.down,
                    Direction.Gravity => world.gravity.normalized,
                    _ => -rigidbody2D.transform.up
                };
                
                if (hover.hoverDirection == Direction.Ground)
                {
                    RaycastHit2D sensorHit = Physics2D.Raycast(hoverPosition, hoverDirection, hover.range, hover.layer);
                    if (sensorHit.collider) hoverDirection = -sensorHit.normal;
                }
                
                float hoverVelocity = Vector2.Dot(hoverPosition - _lastHoverPosition[i], -hoverDirection) / Time.fixedDeltaTime;
                
                RaycastHit2D hoverHit = Physics2D.Raycast(hoverPosition, hoverDirection, hover.range, hover.layer);
                if (hoverInput > 0 && hoverHit.collider != null)
                {
                    float ratio = Mathf.Clamp01(1 + (hover.height - hoverHit.distance) / hover.height);
                    float amount = hover.range > hover.height ? ratio : 1;
                    float pressure = Mathf.Max(hover.height - hoverHit.distance, 0) / hover.height;
                    
                    float force = hover.force * amount;
                    float suspension = hover.suspension * pressure * hover.force;
                    float damping = hover.damping * hoverVelocity;
                    
                    Vector2 appliedHover = (force + suspension - damping) / hoverCount * hoverInput * -hoverDirection;
                    Vector2 appliedPosition = rigidbody2D.position + forceOffset;
                    rigidbody2D.AddForceAtPosition(appliedHover, appliedPosition);
                    
                    AppliedHover += appliedHover;
                    
                    _hoverHitCount++;
                }
                
                _lastHoverPosition[i] = hoverPosition;
            }
        }
        
        private void ApplyPull()
        {
            AppliedPull = Vector2.zero;
            AppliedCounterSlip = Vector2.zero;
            
            int hoverCount = hover.sensors.Length;
            
            for (int i = 0; i < hover.sensors.Length; i++)
            {
                // Get pull and force offset
                Vector2 pullOffset = rigidbody2D.transform.TransformVector(hover.sensors[i]) * hover.pullSpread;
                Vector2 forceOffset = rigidbody2D.transform.TransformVector(
                    Vector2.Scale(hover.sensors[i], new Vector2(hover.pitchEffect, 0)));
                
                // Get pull position and direction
                Vector2 pullPosition = rigidbody2D.position + pullOffset;
                Vector2 pullDirection = hover.pullDirection switch
                {
                    Direction.World => Vector2.down,
                    Direction.Gravity => world.gravity.normalized,
                    _ => -rigidbody2D.transform.up};
                
                // Recalculate pull direction based on detected ground normal
                if (hover.pullDirection == Direction.Ground)
                {
                    RaycastHit2D sensorHit = Physics2D.Raycast(pullPosition, pullDirection, hover.range, hover.layer);
                    if (sensorHit.collider) pullDirection = -sensorHit.normal;
                }
                
                RaycastHit2D pullHit = Physics2D.Raycast(pullPosition, pullDirection, hover.range, hover.layer);
                if (hoverInput > 0 && pullHit.collider != null)
                {
                    // APPLY PULL
                    Vector2 gravityDirection = world.gravity.magnitude > -0.0001 ? Vector2.down : world.gravity.normalized;
                    float dotFactor = Mathf.Clamp01(Vector2.Dot(gravityDirection, pullDirection));
                    Vector2 appliedPull = hover.pull * dotFactor / hoverCount * hoverInput * pullDirection;
                    Vector2 appliedPosition = rigidbody2D.position + forceOffset;
                    rigidbody2D.AddForceAtPosition(appliedPull, appliedPosition);
                    
                    // APPLY COUNTER GRAVITY
                    Vector2 counterGravityVector = Vector3.Project(-world.gravity.normalized, -pullDirection);
                    Vector2 appliedCounterGravity = world.gravity.magnitude / hoverCount * hoverInput * rigidbody2D.mass * counterGravityVector;
                    rigidbody2D.AddForceAtPosition(appliedCounterGravity, appliedPosition);
                    
                    // APPLY COUNTER SLIP
                    Vector2 counterSlipVector = Vector3.ProjectOnPlane(-world.gravity.normalized, -pullDirection);
                    Vector2 appliedCounterSlip = world.gravity.magnitude * hover.counterSlip / hoverCount * hoverInput * rigidbody2D.mass * counterSlipVector;
                    rigidbody2D.AddForce(appliedCounterSlip);
                    
                    AppliedPull += appliedPull + appliedCounterGravity;
                    AppliedCounterSlip += appliedCounterSlip;
                }
            }
        }
        
        private void ApplyThrust()
        {
            Vector2 thrustDirection = rigidbody2D.transform.right;
            Vector2 appliedThrust = thrust.force * thrustInput * thrustDirection;
            rigidbody2D.AddForce(appliedThrust);
            
            AppliedThrust = appliedThrust;
        }
        
        private void ApplyPitch()
        {
            Vector2 groundForward = (_frontSensorInfo.endpoint - _backSensorInfo.endpoint).normalized;
            if (_frontSensorInfo.isHit ^ _backSensorInfo.isHit)
                if (_frontSensorInfo.isHit && pitch.alignFront)
                    groundForward = Vector3.ProjectOnPlane(rigidbody2D.transform.right, _frontSensorInfo.normal).normalized;
                else if (_backSensorInfo.isHit && pitch.alignBack)
                    groundForward = Vector3.ProjectOnPlane(rigidbody2D.transform.right, _backSensorInfo.normal).normalized;
            
            float currentAngle = Vector2.SignedAngle(groundForward, rigidbody2D.transform.right);
            float targetAngle = 0;
            float currentSpeed = rigidbody2D.angularVelocity;
            
            if (isOnGround)
            {
                float amount = pitch.trackForce * (targetAngle - currentAngle);
                float damping = pitch.trackDamping * currentSpeed;
                float appliedPitch = (amount - damping) * Mathf.Lerp(1, _hoverHitRatio, pitch.hoverRatio);
                rigidbody2D.AddTorque(appliedPitch);
            }
        }
        
        
        
        [Serializable] public class World
        {
            public Vector2 gravity;
            public float drag;
            public float angularDrag;
            public float airDensity = 1.225f;
        }
        
        [Serializable] public class Body
        {
            public float mass;
            public Vector2 dragArea; // x = height * width, y = length * width
            public float angularArea; // ((length * width) + (height * width)) / 2
            public float effectiveRadius; // (length + width + height) / 3 / 2
        }
        
        [Serializable] public class Hover
        {
            public LayerMask layer;
            public Vector2[] sensors;
            public float height;
            public float range;
            [Space(5)]
            public float force;
            public float pull;
            public float damping;
            public float suspension;
            [Range(0, 1)] public float counterSlip;
            [Space(5)]
            public Direction hoverDirection;
            public Direction pullDirection;
            [Range(0, 1)] public float hoverSpread;
            [Range(0, 1)] public float pullSpread;
            [Range(0, 1)] public float pitchEffect;
        }
        
        [Serializable] public class Thrust
        {
            public float force;
        }
        
        [Serializable] public struct Pitch
        {
            public float trackForce;
            public float trackDamping;
            [Range(0, 1)] public float hoverRatio;
            public bool alignFront;
            public bool alignBack;
        }
    }
    
    
    public enum Direction {Ground, Ship, Gravity, World}
    
    [Serializable] public struct MainSensors
    {
        public LayerMask groundLayer;
        public Vector2 center;
        public Vector2 front;
        public Vector2 back;
    }
    
    [Serializable] public struct SensorInfo
    {
        public bool isHit;
        public Vector3 position;
        public Vector3 direction;
        public Vector3 endpoint;
        public Vector3 normal;
        
        public void UpdateSensor(Vector2 sensor, (Transform parent, Direction direction, float range, LayerMask layer) settings)
        {
            this.position = settings.parent.position + settings.parent.TransformVector(sensor);
            this.direction = settings.direction == Direction.World ? Vector3.down : -settings.parent.up;
            
            if (settings.direction == Direction.Ground)
            {
                RaycastHit2D hit1 = Physics2D.Raycast(this.position, this.direction, settings.range, settings.layer);
                if (hit1.collider) this.direction = -hit1.normal;
            }
            
            RaycastHit2D hit2 = Physics2D.Raycast(this.position, this.direction, settings.range, settings.layer);
            isHit = hit2.collider != null;
            if (isHit)
            {
                this.endpoint = hit2.point;
                this.normal = hit2.normal;
            }
            else
            {
                this.endpoint = this.position + this.direction * settings.range;
                this.normal = -this.direction;
            }
        }
    }
    
}
