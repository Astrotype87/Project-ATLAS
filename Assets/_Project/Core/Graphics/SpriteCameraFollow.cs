using System;
using UnityEditor;
using UnityEngine;

namespace ProjectATLAS.Graphics
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteCameraFollow : MonoBehaviour
    {
        public enum FollowMode { None = 0, Horizontal = 1 << 0, Vertical = 1 << 1, All = Horizontal | Vertical }
        public enum StretchMode { None = 0, Horizontal = 1 << 0, Vertical = 1 << 1, Full = Horizontal | Vertical }
        
        [Header("Camera")]
        [SerializeField] private Camera targetCamera;
        [SerializeField] private FollowMode followMode;
        [SerializeField] private StretchMode stretchMode;
        
        private SpriteRenderer spriteRenderer;
        
        
        private void OnValidate()
        {
            if (!isActiveAndEnabled) return;
            
            if (!targetCamera) targetCamera = Camera.main;
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            FollowCameraPosition();
            StretchToCameraView();
        }
        
        private void Start()
        {
            if (!targetCamera) targetCamera = Camera.main;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        private void LateUpdate()
        {
            FollowCameraPosition();
            StretchToCameraView();
        }
        
        
        public void Refresh()
        {
            if (!targetCamera) targetCamera = Camera.main;
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            FollowCameraPosition();
            StretchToCameraView();
        }
        
        
        private void FollowCameraPosition()
        {
            if (!targetCamera) return;
            
            Vector3 spritePos = transform.position;
            Vector3 cameraPos = targetCamera.transform.position;
            
            float x = followMode.HasFlag(FollowMode.Horizontal) ? cameraPos.x : spritePos.x;
            float y = followMode.HasFlag(FollowMode.Vertical) ? cameraPos.y : spritePos.y;
            
            transform.position = new(x, y, spritePos.z);
        }
        
        private void StretchToCameraView()
        {
            if (!targetCamera || !spriteRenderer || !spriteRenderer.sprite) return;
            
            float screenHeight = targetCamera.orthographicSize * 2f;
            float screenWidth = screenHeight * targetCamera.aspect;
            
            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
            Vector3 scale = transform.localScale;
            
            if (stretchMode.HasFlag(StretchMode.Horizontal)) scale.x = screenWidth / spriteSize.x;
            if (stretchMode.HasFlag(StretchMode.Vertical)) scale.y = screenHeight / spriteSize.y;
            
            transform.localScale = scale;
        }
        
        
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(SpriteCameraFollow))]
    public class SpriteCameraFollowEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Refresh"))
            {
                SpriteCameraFollow script = target as SpriteCameraFollow;
                script.Refresh();
            }
        }
    }
    #endif
}
