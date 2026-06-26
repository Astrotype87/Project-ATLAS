using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectATLAS.UI
{
    // Credits: https://github.com/PandaArcade/UIOutline
    public class UIOutline : MaskableGraphic
    {
        [SerializeField] private Texture texture;
        [SerializeField] [Range(0f, 500f)] private float outlineWidth = 100f;
        [SerializeField] [Range(0f, 500f)] private float cornerRadius = 50f;
        [SerializeField] [Range(1, 20)] private int cornerSegments = 1;
        [SerializeField] [Range(0f, 1f)] private float mappingBias = 0.5f;
        [SerializeField] private bool fillCenter;
        
        private Vector3[] _corners = new Vector3[4];
        private List<UIVertex> _verts = new List<UIVertex>();
        
        public override Texture mainTexture => texture == null ? s_WhiteTexture : texture;
        
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
            SetMaterialDirty();
        }
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            
            // Clamp corner radius
            Rect rect = rectTransform.rect;
            float halfSize = Mathf.Min(rect.width, rect.height) / 2f;
            float clampedCornerRadius = Mathf.Min(halfSize, cornerRadius);
            
            // Offset corner based on clamped corner radius
            rectTransform.GetLocalCorners(_corners);
            _corners[0] += new Vector3(clampedCornerRadius, clampedCornerRadius, 0f);
            _corners[1] += new Vector3(clampedCornerRadius, -clampedCornerRadius, 0f);
            _corners[2] += new Vector3(-clampedCornerRadius, -clampedCornerRadius, 0f);
            _corners[3] += new Vector3(-clampedCornerRadius, clampedCornerRadius, 0f);
            
            // Calculate dimensions
            float height = _corners[1].y - _corners[0].y;
            float width = _corners[2].x - _corners[1].x;
            float[] edgeLengths = { height, width, height, width };
            
            float outlinedRadius = Mathf.Lerp(clampedCornerRadius, clampedCornerRadius + outlineWidth, mappingBias);
            float circumference = 2f * Mathf.PI * outlinedRadius;
            float around = height * 2f + width * 2f + circumference;
            
            float cornerLength = circumference / 4f;
            float segmentLength = cornerLength / cornerSegments;
            
            UIVertex vert = new(){ color = color };
            _verts.Clear();
            
            
            // Create corners
            float u = 0f;
            for (var c = 0; c < 4; c++)
            {
                // Create verts
                var origin = _corners[c];
                for (var i = 0; i < cornerSegments + 1; i++)
                {
                    var angle = (float)i / cornerSegments * Mathf.PI / 2f + Mathf.PI * 0.5f - Mathf.PI * c * 1.5f;
                    var direction = new Vector3(Mathf.Cos(-angle), Mathf.Sin(-angle), 0f);
                    
                    vert.position = origin + direction * clampedCornerRadius;
                    vert.uv0 = new Vector2(u, 0f);
                    _verts.Add(vert);
                    
                    vert.position = origin + direction * (clampedCornerRadius + outlineWidth);
                    vert.uv0 = new Vector2(u, 1f);
                    _verts.Add(vert);
                    
                    if (fillCenter)
                    {
                        vert.position = rect.center;
                        vert.uv0 = new Vector2(u, 0f);
                        _verts.Add(vert);
                    }
                    
                    if (i < cornerSegments)
                        u += segmentLength / around;
                    else
                        u += edgeLengths[c] / around;
                }
            }
            
            // Add end verts
            vert = _verts[0];
            vert.uv0 = new Vector2(1f, 0f);
            _verts.Add(vert);
            
            vert = _verts[1];
            vert.uv0 = new Vector2(1f, 1f);
            _verts.Add(vert);
            
            if (fillCenter)
            {
                vert = _verts[2];
                vert.uv0 = new Vector2(1f, 1f);
                _verts.Add(vert);
            }
            
            // Add verts to VertexHelper
            foreach (var vertex in _verts)
                vh.AddVert(vertex);
            
            // Add triangles to VertexHelper 
            if (fillCenter)
            {
                for (var v = 0; v < vh.currentVertCount - 3; v += 3)
                {
                    vh.AddTriangle(v, v + 1, v + 4);
                    vh.AddTriangle(v, v + 4, v + 3);
                    
                    vh.AddTriangle(v + 2, v, v + 3);
                    vh.AddTriangle(v + 2, v + 3, v + 5);
                }
            }
            else
            {
                for (var v = 0; v < vh.currentVertCount - 2; v += 2)
                {
                    vh.AddTriangle(v, v + 1, v + 3);
                    vh.AddTriangle(v, v + 3, v + 2);
                }
            }
        }
    }
}
