using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using ProjectATLAS.Utility;

namespace ProjectATLAS.Menu.Background
{
    using ParallaxLayer = ParallaxPreset.ParallaxLayer;
    
    public class ParallaxBackground : MonoBehaviour
    {
        [Header("Preset")]
        [SerializeField] private ParallaxPreset parallaxPreset;
        // [SerializeField] private bool randomStart;
        
        [Header("Settings")]
        [SerializeField] private float speed = 0.1f;
        [SerializeField] private Transform template;
        
        [Header("Debug")]
        [SerializeField] private double time;
        [SerializeField] private List<Transform> transformLayers = new();
        
        /// The background's aspect is 16:9, which is equal to 17.77778:10.
        public const float BG_LENGTH =  17.77778f;
        public const float BG_LENGTH_HALF = 8.888889f;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            InitializeBackgroundLayers();
        }
        
        private void Update()
        {
            time += Time.deltaTime / BG_LENGTH * speed;
            
            for (int i = 0; i < parallaxPreset.layers.Length; i++)
            {
                UpdateBackgroundLayer(transformLayers[i], parallaxPreset.layers[i]);
            }
        }
        
        
        // PRIVATE METHODS
        private void InitializeBackgroundLayers()
        {
            if (!template) return;
            
            // Delete all child game objects except the template
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child != template) Destroy(child.gameObject);
            }
            
            // Get child gameobject template
            transformLayers = new List<Transform> { template };
            
            // Clone template (parallaxPreset.layers.Length - 1) times
            for (int i = 0; i < parallaxPreset.layers.Length - 1; i++)
            {
                GameObject clone = Instantiate(template.gameObject, transform);
                clone.name = $"Layer {i + 2}";
                
                transformLayers.Add(clone.transform);
            }
            
            // Get reference to SpriteRenderer and assign sprites
            for (int i = 0; i < parallaxPreset.layers.Length; i++)
            {
                if (transformLayers[i].TryGetComponent(out SpriteRenderer spriteRenderer))
                {
                    spriteRenderer.sprite = parallaxPreset.layers[i].background;
                    spriteRenderer.sortingOrder = i;
                    spriteRenderer.size = new(spriteRenderer.size.x, spriteRenderer.size.y);
                }
            }
        }
        
        private void UpdateBackgroundLayer(Transform transformLayer, ParallaxLayer parallaxLayer)
        {
            double time = this.time * parallaxLayer.speed;
            float phase = 0.5f + parallaxLayer.phase;
            
            float progress = (float)Repeat(time + phase, 1);
            float position = progress * -BG_LENGTH + BG_LENGTH_HALF;
            transformLayer.SetLocalPositionX(position);
        }
        
        
        
        
        // STATIC METHODS
        /// <summary> The double version of UnityEngine.Mathf.Repeat(). </summary>
        public static double Repeat(double t, double length)
        {
            return Math.Clamp(t - Math.Floor(t / length) * length, 0, length);
        }
    }
}
