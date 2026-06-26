using UnityEngine;
using UnityEditor;

namespace ProjectATLAS.Types.Editor
{
    [CustomPropertyDrawer(typeof(Range))]
    public class RangeDrawer: PropertyDrawer
    {
        private static readonly GUIContent[] labels = { new("Min"), new("Max") };
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty minProp = property.FindPropertyRelative("min");
            SerializedProperty maxProp = property.FindPropertyRelative("max");
            
            float[] values = { minProp.floatValue, maxProp.floatValue };
            
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            
            EditorGUI.MultiFloatField(position, label, labels, values);
            
            if (EditorGUI.EndChangeCheck())
            {
                minProp.floatValue = values[0];
                maxProp.floatValue = values[1];
            }
            
            EditorGUI.EndProperty();
        }
    }
}
