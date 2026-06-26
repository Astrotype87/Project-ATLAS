using UnityEngine;

namespace ProjectATLAS.Graphics
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteGradient : MonoBehaviour
    {
        public enum GradientType { Vertical, Horizontal }
        public enum BlendMode { Multiply, Add, Overlay, Replace }
        
        [SerializeField] private Sprite baseSprite;
        [SerializeField] private Gradient gradient = new();
        [SerializeField] private GradientType gradientType;
        [SerializeField] private BlendMode blendMode = BlendMode.Multiply;
        [SerializeField] private bool flip = false;
        [SerializeField] private int pixelsPerUnit = 100;
        
        private SpriteRenderer spriteRenderer;
        
        
        private void Start()
        {
            ApplyGradient();
        }
        
        private void OnValidate()
        {
            ApplyGradient();
        }
        
        
        public void ApplyGradient()
        {
            if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
            pixelsPerUnit = Mathf.Clamp(pixelsPerUnit, 1, 200);
            
            if (baseSprite == null)
                GenerateGradientSprite();
            else 
                GenerateGradientOverlay();
        }
        
        private void GenerateGradientSprite()
        {
            int width = pixelsPerUnit;
            int height = pixelsPerUnit;
            
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float t = gradientType == GradientType.Vertical
                        ? (float)y / Mathf.Max(height - 1, 1)
                        : (float)x / Mathf.Max(width - 1, 1);
                    
                    if (flip) t = 1f - t;
                    
                    Color color = gradient.Evaluate(t);
                    texture.SetPixel(x, y, color);
                }
            }
            
            texture.Apply();
            
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            sprite.name = "Generated_Gradient_Only";
            
            spriteRenderer.sprite = sprite;
        }
        
        private void GenerateGradientOverlay()
        {
            Texture2D originalTex = DuplicateSpriteTexture(baseSprite);
            Rect spriteRect = baseSprite.textureRect;
            
            Texture2D newTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height, TextureFormat.ARGB32, false);
            newTexture.filterMode = originalTex.filterMode;
            newTexture.wrapMode = TextureWrapMode.Clamp;
            
            int spriteWidth = (int)spriteRect.width;
            int spriteHeight = (int)spriteRect.height;
            
            Color32[] colors = new Color32[spriteWidth * spriteHeight];
            for (int y = 0; y < spriteHeight; y++)
            {
                for (int x = 0; x < spriteWidth; x++)
                {
                    float t = gradientType == GradientType.Vertical
                        ? (float)y / Mathf.Max(spriteHeight - 1, 1)
                        : (float)x / Mathf.Max(spriteWidth - 1, 1);
                    
                    if (flip) t = 1f - t;
                    
                    Color32 originalColor = originalTex.GetPixel((int)spriteRect.x + x, (int)spriteRect.y + y);
                    Color32 gradientColor = gradient.Evaluate(t);
                    Color32 finalColor = Blend(originalColor, gradientColor);
                    if (Mathf.Approximately(originalColor.a, 0f)) finalColor.a = originalColor.a;
                    
                    colors[y * spriteWidth + x] = finalColor;
                }
            }
            newTexture.SetPixels32(colors);
            newTexture.Apply();
            
            Sprite newSprite = Sprite.Create(
                newTexture,
                new Rect(0, 0, newTexture.width, newTexture.height),
                new Vector2(0.5f, 0.5f),
                baseSprite.pixelsPerUnit
            );
            
            newSprite.name = "GradientOverlay_" + baseSprite.name;
            spriteRenderer.sprite = newSprite;
        }
        
        
        private Color Blend(Color baseColor, Color gradientColor)
        {
            return new Color(
                BlendChannel(baseColor.r, gradientColor.r, blendMode),
                BlendChannel(baseColor.g, gradientColor.g, blendMode),
                BlendChannel(baseColor.b, gradientColor.b, blendMode),
                BlendChannel(baseColor.a, gradientColor.a, blendMode)
            );
        }
        
        private float BlendChannel(float baseVal, float blendVal, BlendMode mode)
        {
            return mode switch
            {
                BlendMode.Multiply => baseVal * blendVal,
                BlendMode.Add => Mathf.Clamp01(baseVal + blendVal),
                BlendMode.Overlay => OverlayChannel(baseVal, blendVal),
                BlendMode.Replace => blendVal,
                _ => baseVal,
            };
        }
        
        private float OverlayChannel(float baseChannel, float blendChannel)
        {
            return baseChannel < 0.5f
                ? 2f * baseChannel * blendChannel
                : 1f - 2f * (1f - baseChannel) * (1f - blendChannel);
        }
        
        
        public static Texture2D DuplicateSpriteTexture(Sprite sprite)
        {
            if (sprite == null) return null;
            
            Rect spriteRect = sprite.textureRect;
            int spriteWidth = (int)spriteRect.width;
            int spriteHeight = (int)spriteRect.height;
            
            // Create render texture of full source texture size
            RenderTexture renderTexture = new(sprite.texture.width, sprite.texture.height, 0);
            RenderTexture.active = renderTexture;
            
            // Blit the entire texture through the material
            UnityEngine.Graphics.Blit(sprite.texture, renderTexture);
            
            // Create texture result
            int readX = (int)spriteRect.x;
            int readY = (int)spriteRect.y;
            Texture2D result = new(spriteWidth + readX, spriteHeight + readY, TextureFormat.ARGB32, false);
            
            // Read pixels and apply to texture result
            Rect readRect = new(0, 0, spriteWidth + readX, spriteHeight + readY);
            result.ReadPixels(readRect, 0, 0);
            result.Apply();
            
            // Apply modes
            result.wrapMode = sprite.texture.wrapMode;
            result.filterMode = sprite.texture.filterMode;
            
            // Release render texture
            RenderTexture.active = null;
            renderTexture.Release();
            
            Debug.Log($"[Copied texture] result.width {result.width}, result.height {result.height}");
            
            return result;
        }
    }
}
