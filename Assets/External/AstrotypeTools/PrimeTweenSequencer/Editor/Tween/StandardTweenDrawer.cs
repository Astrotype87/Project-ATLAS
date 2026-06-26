using System;
using UnityEngine;
using UnityEditor;
using PrimeTween;

namespace AstrotypeTools.PrimeTweenSequencer.Editor
{
    [CustomPropertyDrawer(typeof(StandardTween<,>), true)]
    public class StandardTweenDrawer: PropertyDrawer
    {
        private float LineHeight => EditorGUIUtility.singleLineHeight;
        private float Spacing => EditorGUIUtility.standardVerticalSpacing;
        
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ManagedReference) {
                if (property.managedReferenceValue == null) {
                    EditorGUI.LabelField(position, label.text, "No object set.");
                    return;
                }
            }
            
            // SERIALIZED PROPERTY REFERENCES
            SerializedProperty startModeProp = property.FindPropertyRelative("startMode");
            SerializedProperty endModeProp = property.FindPropertyRelative("endMode");
            SerializedProperty easeProp = property.FindPropertyRelative("ease");
            SerializedProperty customEaseProp = property.FindPropertyRelative("customEase");
            SerializedProperty cyclesProp = property.FindPropertyRelative("cycles");
            
            SerializedProperty e_durationFoldoutProp = property.FindPropertyRelative("e_durationFoldout");
            SerializedProperty e_cyclesFoldoutProp = property.FindPropertyRelative("e_cyclesFoldout");
            
            bool isStartModeCurrent = startModeProp.intValue == (int)StartMode.Current;
            bool isEndModeFollow = endModeProp.intValue == (int)EndMode.Follow || endModeProp.intValue == (int)EndMode.Follow;
            bool isEaseCustom = easeProp.intValue == (int)Ease.Custom;
            bool isCustomCurve = customEaseProp.intValue == (int)CustomEase.Curve;
            bool isCustomElastic = customEaseProp.intValue == (int)CustomEase.Elastic;
            bool isCustomBounceExact = customEaseProp.intValue == (int)CustomEase.BounceExact;
            int cyclesVal = cyclesProp.intValue;
            bool isZeroOrOneCycle = cyclesVal >= 0 && cyclesVal <= 1;
            
            
            // BEGIN PROPERTY
            EditorGUI.BeginProperty(position, label, property);
            Rect rect = new Rect(position.x, position.y, position.width, LineHeight);
            
            // LOCAL METHODS
            void DrawProperty(string name, bool readOnly = false, bool visible = true, GUIContent labelOverride = null)
            {
                if (!visible) return;
                
                var prop = property.FindPropertyRelative(name);
                float height = EditorGUI.GetPropertyHeight(prop, true);
                // if (labelOverride == GUIContent.none)
                // {
                //     height = prop.propertyType switch {
                //         SerializedPropertyType.Vector2 => LineHeight,
                //         SerializedPropertyType.Vector3 => LineHeight,
                //         SerializedPropertyType.Quaternion => LineHeight,
                //         SerializedPropertyType.Vector4 => height - Spacing,
                //         SerializedPropertyType.Rect => (LineHeight + Spacing) * 2 - Spacing,
                //     _ => height
                //     };
                // }
                Rect fieldRect = new Rect(rect.x, rect.y, rect.width, height);
                
                if (readOnly) EditorGUI.BeginDisabledGroup(true);
                if (labelOverride != null) EditorGUI.PropertyField(fieldRect, prop, labelOverride, true);
                else EditorGUI.PropertyField(fieldRect, prop, true);
                if (readOnly) EditorGUI.EndDisabledGroup();
                
                rect.y += height + Spacing;
            }
            
            bool DrawFoldout(bool foldout, bool readOnly = false, bool includeHeight = true, GUIContent labelOverride = null)
            {
                bool isLabelClickable = labelOverride != GUIContent.none;
                Rect toggleRect = new Rect(rect.x - 3, rect.y, isLabelClickable ? rect.width : 15, LineHeight);
                
                if (readOnly) EditorGUI.BeginDisabledGroup(true);
                foldout = EditorGUI.Foldout(toggleRect, foldout,
                    labelOverride ?? new GUIContent("Unnamed Foldout"), isLabelClickable);
                if (readOnly) EditorGUI.EndDisabledGroup();
                
                if (includeHeight) rect.y += LineHeight + Spacing;
                
                return foldout;
            }
            
            void AddSpace(float height = 4f) => rect.y += height;
            void IncreaseIndent() => EditorGUI.indentLevel++;
            void DecreaseIndent() => EditorGUI.indentLevel--;
            
            
            // DRAW ALL PROPERTIES
            
            // Draw start and end settings
            DrawProperty("startMode");
            DrawProperty("startValue", visible: !isStartModeCurrent);
            DrawProperty("endMode");
            DrawProperty("followTarget", visible: isEndModeFollow);
            DrawProperty("endValue", labelOverride: new(isEndModeFollow ? "Follow Offset" : "End Value"));
            AddSpace(2f);
            
            // Increase indent
            IncreaseIndent();
            
            // Draw duration section
            e_durationFoldoutProp.boolValue = DrawFoldout(e_durationFoldoutProp.boolValue,
                includeHeight: false, labelOverride: GUIContent.none);
            DrawProperty("duration"); // FIXME: AtSpeed tweens name duration as "averageSpeed" instead. do this in BaseTweenDrawer
            if (e_durationFoldoutProp.boolValue)
            {
                string strengthLabel = isCustomBounceExact ? "Amplitude" : "Strength";
                
                DrawProperty("ease");
                DrawProperty("customEase", visible: isEaseCustom);
                DrawProperty("curve", visible: isEaseCustom && isCustomCurve);
                DrawProperty("strength", visible: isEaseCustom && !isCustomCurve, labelOverride: new(strengthLabel));
                DrawProperty("period", visible: isEaseCustom && isCustomElastic);
                DrawProperty("direction");
                AddSpace();
            }
            
            // Draw cycles section
            e_cyclesFoldoutProp.boolValue = DrawFoldout(e_cyclesFoldoutProp.boolValue,
                includeHeight: false, labelOverride: GUIContent.none);
            DrawProperty("cycles");
            if (e_cyclesFoldoutProp.boolValue)
            {
                DrawProperty("cycleMode", visible: !isZeroOrOneCycle);
                DrawProperty("startDelay");
                DrawProperty("endDelay");
                DrawProperty("useUnscaledTime");
                DrawProperty("updateType");
                DrawProperty("target", readOnly: true);
            }
            
            // Decrease indent
            DecreaseIndent();
            
            // Validate fields
            ValidateValues(property);
            
            // END PROPERTY
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ManagedReference) {
                if (property.managedReferenceValue == null) {
                    return LineHeight;
                }
            }
            
            // SERIALIZED PROPERTY REFERENCES
            SerializedProperty startModeProp = property.FindPropertyRelative("startMode");
            SerializedProperty endModeProp = property.FindPropertyRelative("endMode");
            SerializedProperty easeProp = property.FindPropertyRelative("ease");
            SerializedProperty customEaseProp = property.FindPropertyRelative("customEase");
            SerializedProperty cyclesProp = property.FindPropertyRelative("cycles");
            
            SerializedProperty e_durationFoldoutProp = property.FindPropertyRelative("e_durationFoldout");
            SerializedProperty e_cyclesFoldoutProp = property.FindPropertyRelative("e_cyclesFoldout");
            
            bool isStartModeCurrent = startModeProp.intValue == (int)StartMode.Current;
            bool isEndModeFollow = endModeProp.enumValueIndex == (int)EndMode.Follow || endModeProp.enumValueIndex == (int)EndMode.Follow;
            bool isEaseCustom = easeProp.intValue == (int)Ease.Custom;
            bool isCustomCurve = customEaseProp.intValue == (int)CustomEase.Curve;
            bool isCustomElastic = customEaseProp.intValue == (int)CustomEase.Elastic;
            int cyclesVal = cyclesProp.intValue;
            bool isZeroOrOneCycle = cyclesVal >= 0 && cyclesVal <= 1;
            
            
            // BEGIN PROPERTY
            float totalHeight = 0f;
            
            // LOCAL METHODS
            void AddProperty(string name, bool visible = true, bool hasNoLabel = false)
            {
                if (!visible) return;
                var prop = property.FindPropertyRelative(name);
                float propHeight = EditorGUI.GetPropertyHeight(prop, true);
                // if (hasNoLabel)
                // {
                //     propHeight = prop.propertyType switch {
                //         SerializedPropertyType.Vector2 => LineHeight,
                //         SerializedPropertyType.Vector3 => LineHeight,
                //         SerializedPropertyType.Vector4 => propHeight - Spacing,
                //         SerializedPropertyType.Rect => (LineHeight + Spacing) * 2 - Spacing,
                //         SerializedPropertyType.Quaternion => LineHeight,
                //     _ => propHeight
                //     };
                // }
                
                totalHeight += propHeight + Spacing;
            }
            
            void AddSpace(float height = 4f)
            {
                totalHeight += height;
            }
            
            AddProperty("startMode");
            AddProperty("startValue", visible: !isStartModeCurrent, hasNoLabel: true);
            AddProperty("endMode");
            AddProperty("endValue", hasNoLabel: true);
            AddProperty("followTarget", visible: isEndModeFollow);
            AddSpace(2f);
            
            // AddSpace(LineHeight + Spacing); // durationFoldout header
            AddProperty("duration");
            if (e_durationFoldoutProp.boolValue)
            {
                AddProperty("ease");
                AddProperty("customEase", visible: isEaseCustom);
                AddProperty("curve", visible: isEaseCustom && isCustomCurve);
                AddProperty("strength", visible: isEaseCustom && !isCustomCurve);
                AddProperty("period", visible: isEaseCustom && isCustomElastic);
                AddProperty("direction");
                AddSpace();
            }
            
            // AddSpace(LineHeight + Spacing); // cyclesFoldout header
            AddProperty("cycles");
            if (e_cyclesFoldoutProp.boolValue)
            {
                AddProperty("cycleMode", visible: !isZeroOrOneCycle);
                AddProperty("startDelay");
                AddProperty("endDelay");
                AddProperty("useUnscaledTime");
                AddProperty("updateType");
                AddProperty("target");
            }
            
            // END PROPERTY
            return totalHeight;
        }
        
        
        // FUNCTIONAL METHODS
        private void ValidateValues(SerializedProperty property)
        {
            var duration = property.FindPropertyRelative("duration");
            var startDelay = property.FindPropertyRelative("startDelay");
            var endDelay = property.FindPropertyRelative("endDelay");
            var cycles = property.FindPropertyRelative("cycles");
            
            // Clamp durations to non-negative
            duration.floatValue = Mathf.Max(0f, duration.floatValue);
            startDelay.floatValue = Mathf.Max(0f, startDelay.floatValue);
            endDelay.floatValue = Mathf.Max(0f, endDelay.floatValue);
            
            cycles.intValue = Math.Max(-1, cycles.intValue);
            if (cycles.intValue == 0) cycles.intValue = 1;
        }
    }
}
