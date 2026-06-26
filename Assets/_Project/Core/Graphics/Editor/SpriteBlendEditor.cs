using UnityEngine;
using UnityEditor;

namespace ProjectATLAS.Graphics.Editor
{
    [CustomEditor(typeof(SpriteBlend))]
    public class SpriteBlendEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
        }
    }
}
