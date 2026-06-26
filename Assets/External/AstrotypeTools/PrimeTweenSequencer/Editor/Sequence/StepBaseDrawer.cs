using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AstrotypeTools.PrimeTweenSequencer.Editor
{
    [CustomPropertyDrawer(typeof(StepBase))]
    public class StepBaseDrawer: PropertyDrawer
    {
        private const float IndentSize = 15f;
        
        private float LineHeight => EditorGUIUtility.singleLineHeight;
        private float Spacing => EditorGUIUtility.standardVerticalSpacing;
        
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            StepBase step = property.managedReferenceValue as StepBase;
            SerializedProperty enabledProp = property.FindPropertyRelative("enabled");
            SerializedProperty stepModeProp = property.FindPropertyRelative("stepMode");
            SerializedProperty insertTimeProp = property.FindPropertyRelative("insertTime");
            SerializedProperty e_expandedProp = property.FindPropertyRelative("e_expanded");
            
            bool isEnabled = step != null && enabledProp.boolValue;
            bool isStepModeGroup = step != null && stepModeProp.intValue == (int)StepMode.Group;
            bool isStepModeInsert = step != null && stepModeProp.intValue == (int)StepMode.Insert;
            
            
            // REUSABLE RECT
            Rect rect = new Rect(position.x, position.y, position.width, LineHeight);
            float indentSpace = EditorGUI.indentLevel * IndentSize;
            float halfSpacing = 1f;
            
            // INCREASE INDENT IF STEP MODE IS GROUP
            if (isStepModeGroup) EditorGUI.indentLevel++;
            
            
            // DRAW FOLDOUT
            // Get foldout name and icon
            string name = step == null ? " Null" : " " + GetStepName(step);
            Texture2D icon = GetStepIcon(step);
            
            // Get foldout style
            GUIContent content = new(name, icon);
            // GUIStyle style = new(EditorStyles.label);
            // style.fixedWidth = EditorGUIUtility.currentViewWidth;
            // style.fixedHeight = LineHeight - Spacing;
            // style.contentOffset = new(0f, 2f);
            GUIStyle originalStyle = new(EditorStyles.foldout);
            GUIStyle style = EditorStyles.foldout;
            
            // Calculate foldout rect
            float xOffset = 8.5f;
            float widthOffset = -indentSpace - IndentSize - xOffset;
            Rect foldoutRect = new(rect.x + IndentSize - 3f, rect.y + halfSpacing, rect.width + widthOffset, rect.height);
            // Rect foldoutRect = new(rect.x + xOffset, rect.y - 4f + halfSpacing, rect.width + widthOffset, rect.height + 2f * 3);
            
            // Override foldout style
            style.normal.background = null;
            style.fixedWidth = EditorGUIUtility.currentViewWidth;
            style.fixedHeight = LineHeight - Spacing;
            style.contentOffset = new(0f, 0f);
            
            // Draw foldout
            EditorGUI.BeginDisabledGroup(!isEnabled);
            if (step != null)
            {
                e_expandedProp.boolValue = EditorGUI.PropertyField(foldoutRect, property, content, false);
                // e_expandedProp.boolValue = EditorGUI.Foldout(foldoutRect, e_expandedProp.boolValue, content, true, style);
            }
            else
            {
                foldoutRect.height -= 2f;
                EditorGUI.LabelField(foldoutRect, content);
                // EditorGUI.Foldout(foldoutRect, false, content, true, style);
            }
            EditorGUI.EndDisabledGroup();
            
            // Restore foldout style
            style.normal.background = originalStyle.normal.background;
            style.fixedWidth = originalStyle.fixedWidth;
            style.fixedHeight = originalStyle.fixedHeight;
            style.contentOffset = originalStyle.contentOffset;
            
            
            
            // DRAW ENABLED CHECKBOX
            if (step != null)
            {
                // Get enabled rect
                float enabledX = rect.x + rect.width - LineHeight + 4;
                if (isStepModeGroup) enabledX -= IndentSize;
                Rect enabledRect = new(enabledX, rect.y - 0.5f + halfSpacing, LineHeight, rect.height);
                
                // Draw enabled
                EditorGUI.PropertyField(enabledRect, enabledProp, GUIContent.none);
                rect.y += LineHeight + Spacing;
            }
            
            
            // DRAW STEP PROPERTIES
            if (step != null && e_expandedProp.boolValue)
            {
                rect.y += Spacing * 2; // Add another spacing
                
                // BEGIN DISABLED if enabled is false
                EditorGUI.BeginDisabledGroup(!isEnabled);
                
                // Draw step mode
                EditorGUI.PropertyField(rect, stepModeProp);
                rect.y += LineHeight + Spacing;
                
                // Draw insert time
                if (isStepModeInsert)
                {
                    EditorGUI.PropertyField(rect, insertTimeProp);
                    rect.y += LineHeight + Spacing; 
                }
                
                // Draw additional properties
                SerializedProperty iterator = property.Copy();
                SerializedProperty endProperty = iterator.GetEndProperty();
                bool enterChildren = true;
                
                while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
                {
                    if (iterator.name is "enabled" or "stepMode" or "insertTime" or "e_expanded")
                    {
                        enterChildren = false;
                        continue;
                    }
                    
                    float propHeight = EditorGUI.GetPropertyHeight(iterator, true);
                    Rect propRect = new(position.x, rect.y, position.width, propHeight);
                    EditorGUI.PropertyField(propRect, iterator, true);
                    rect.y += propHeight + Spacing;
                    
                    enterChildren = false;
                }
                
                // END DISABLED
                EditorGUI.EndDisabledGroup();
            }
            
            // DECREASE INDENT IF STEP MODE IS GROUP
            if (isStepModeGroup) EditorGUI.indentLevel--;
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty enabledProp = property.FindPropertyRelative("enabled");
            SerializedProperty stepModeProp = property.FindPropertyRelative("stepMode");
            SerializedProperty insertTimeProp = property.FindPropertyRelative("insertTime");
            SerializedProperty e_expandedProp = property.FindPropertyRelative("e_expanded");
            
            StepBase step = property.managedReferenceValue as StepBase;
            bool isStepModeInsert = step != null && stepModeProp.intValue == (int)StepMode.Insert;
            
            
            // START HEIGHT CALCULATION
            float height = LineHeight + Spacing;  // Add height for foldout
            if (step == null) return height; // Return height if step is null
            
            if (step != null && e_expandedProp.boolValue)
            {
                height += Spacing * 2; // Add another spacing
                
                // Add height for stepMode
                height += LineHeight + Spacing;
                
                // Add height for insertTime
                if (isStepModeInsert) height += LineHeight + Spacing;
                
                // Add height for additional properties
                SerializedProperty iterator = property.Copy();
                SerializedProperty endProperty = iterator.GetEndProperty();
                
                bool enterChildren = true;
                
                while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
                {
                    if (iterator.name is "enabled" or "stepMode" or "insertTime" or "e_expanded")
                    {
                        enterChildren = false;
                        continue;
                    }
                    
                    height += EditorGUI.GetPropertyHeight(iterator, true) + Spacing;
                    enterChildren = false;
                }
                
                // Add final spacing
                height += Spacing * 2;
            }
            
            return height;
        }
        
        
        // EDITOR UI METHODS
        private string GetStepName(StepBase step)
        {
            if (step is TweenStep tweenStep)
            {
                string name = tweenStep.TweenType == null ? "(Empty)" : tweenStep.TweenType.Name;
                return $"Tween {name}";
            }
            else if (step is SequenceStep sequenceStep)
            {
                return "Sequence";
            }
            else if (step is PlaySequencerStep playSequencerStep)
            {
                TweenSequencer sequencer = playSequencerStep.Sequencer;
                string name = sequencer == null ? "None" : sequencer.gameObject.name;
                return $"Play Sequencer ({name})";
            }
            else if (step is DelayStep delayStep)
            {
                return $"Delay ({delayStep.Delay} s)";
            }
            else if (step is CallbackStep callbackStep)
            {
                return "Callback";
            }
            
            return "Step";
        }
        
        private Texture2D GetStepIcon(StepBase step)
        {
            if (step is TweenStep tweenStep)
            {
                return IconDictionary.GetIcon(tweenStep.TargetType ?? tweenStep.GetType());
            }
            else
            {
                return IconDictionary.GetIcon(step?.GetType());
            }
        }
        
        
    }
}
