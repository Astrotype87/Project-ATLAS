using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using PrimeTween;

namespace AstrotypeTools.PrimeTweenSequencer.Editor
{
    [CustomPropertyDrawer(typeof(SequenceCreator))]
    public class SequenceCreatorDrawer: PropertyDrawer
    {
        private readonly bool DrawBackground = false;
        private readonly bool IndentFoldout = false;
        private readonly float BGPadding = 5f; //5f
        
        private readonly Dictionary<string, ReorderableList> lists = new();
        
        private float LineHeight => EditorGUIUtility.singleLineHeight;
        private float Spacing => EditorGUIUtility.standardVerticalSpacing;
        private const float IndentSize = 15f;
        
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Steps_CreateReorderableListIfNotExist(property, label);
            
            // SERIALIZED PROPERTY REFERENCES
            SerializedProperty cyclesProp = property.FindPropertyRelative("cycles");
            SerializedProperty e_expandedProp = property.FindPropertyRelative("e_expanded");
            SerializedProperty e_cyclesExpandedProp = property.FindPropertyRelative("e_cyclesExpanded");
            
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
                Rect fieldRect = new Rect(rect.x, rect.y, rect.width, height);
                
                if (readOnly) EditorGUI.BeginDisabledGroup(true);
                if (labelOverride != null) EditorGUI.PropertyField(fieldRect, prop, labelOverride, true);
                else EditorGUI.PropertyField(fieldRect, prop, true);
                if (readOnly) EditorGUI.EndDisabledGroup();
                
                rect.y += height + Spacing;
            }
            
            void DrawCustomDropdown(string name, Action<SerializedProperty, Rect> callback,
                GUIContent label = null, bool readOnly = false, bool visible = true)
            {
                if (!visible) return;
                
                var prop = property.FindPropertyRelative(name);
                float height = EditorGUI.GetPropertyHeight(prop, true);
                float labelWidth = EditorGUIUtility.labelWidth + 2;
                Rect fieldRect = new Rect(rect.x, rect.y, rect.width, height);
                Rect buttonRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);
                
                if (readOnly) EditorGUI.BeginDisabledGroup(true);
                int id = GUIUtility.GetControlID(FocusType.Keyboard);
                EditorGUI.PrefixLabel(fieldRect, id, label?? new(prop.displayName));
                if (GUI.Button(buttonRect, prop.enumDisplayNames[prop.enumValueIndex], EditorStyles.popup))
                    callback?.Invoke(prop, buttonRect);
                
                if (readOnly) EditorGUI.EndDisabledGroup();
                
                rect.y += height + Spacing;
            }
            
            bool DrawFoldout(bool foldout, bool readOnly = false, bool includeHeight = true, GUIContent labelOverride = null)
            {
                bool isLabelClickable = labelOverride != GUIContent.none;
                float offset = 0f; //3f
                Rect foldoutRect = new(rect.x - offset, rect.y, isLabelClickable ? rect.width + offset : 15, LineHeight);
                
                if (readOnly) EditorGUI.BeginDisabledGroup(true);
                foldout = EditorGUI.Foldout(foldoutRect, foldout,
                    labelOverride ?? new GUIContent("Unnamed Foldout"), isLabelClickable);
                if (readOnly) EditorGUI.EndDisabledGroup();
                
                if (includeHeight) rect.y += LineHeight + Spacing;
                
                return foldout;
            }
            
            float GetHeight(string name, bool visible = true, bool hasNoLabel = false)
            {
                if (!visible) return 0f;
                var prop = property.FindPropertyRelative(name);
                float propHeight = EditorGUI.GetPropertyHeight(prop, true);
                
                return propHeight + Spacing;
            }
            
            // void AddSpace(float height = 4f) => rect.y += height;
            void IncreaseIndent() => EditorGUI.indentLevel++;
            void DecreaseIndent() => EditorGUI.indentLevel--;
            
            
            // DRAW ALL PROPERTIES
            
            // Draw background
            if (DrawBackground)
            {
                // Calculate box height
                float boxHeight = EditorGUIUtility.singleLineHeight;
                if (e_expandedProp.boolValue)
                {
                    boxHeight += lists[property.propertyPath].GetHeight();
                    boxHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    if (e_cyclesExpandedProp.boolValue)
                    {
                        boxHeight += GetHeight("cycleMode", visible: !isZeroOrOneCycle);
                        boxHeight += GetHeight("sequenceEase");
                        boxHeight += GetHeight("useUnscaledTime");
                        boxHeight += GetHeight("updateType");
                    }
                    boxHeight += EditorGUIUtility.standardVerticalSpacing;
                }
                
                // Print box
                Rect boxRect = new(rect.x, rect.y, rect.width, boxHeight + BGPadding * 2f);
                GUIStyle boxStyle = new("FrameBox"); // FrameBox
                GUI.BeginGroup(boxRect, boxStyle);
                GUI.EndGroup();
                
                rect.x += BGPadding;
                rect.y += BGPadding;
                rect.width -= BGPadding * 2f;
            }
            
            // Draw foldout
            float extraIndent = DrawBackground || IndentFoldout ? IndentSize : 0f;
            Rect foldoutRect = new(rect.x + extraIndent, rect.y, rect.width - extraIndent, rect.height);
            e_expandedProp.boolValue = EditorGUI.PropertyField(foldoutRect, property, false);
            rect.y += LineHeight + Spacing;
            
            if (e_expandedProp.boolValue)
            {
                // Draw reorderable steps
                float indent = EditorGUI.indentLevel * IndentSize;
                Rect listRect = new(rect.x + indent, rect.y, rect.width - indent, rect.height);
                
                EditorGUIUtility.labelWidth -= IndentSize + 6f;
                lists[property.propertyPath].DoList(listRect);
                EditorGUIUtility.labelWidth += IndentSize + 6f;
                
                rect.y += lists[property.propertyPath].GetHeight() + Spacing;
                
                // Increase indent
                IncreaseIndent();
                
                // Draw sequence properties
                e_cyclesExpandedProp.boolValue = DrawFoldout(e_cyclesExpandedProp.boolValue, includeHeight: false, labelOverride: GUIContent.none);
                DrawProperty("cycles");
                if (e_cyclesExpandedProp.boolValue)
                {
                    DrawProperty("cycleMode", visible: !isZeroOrOneCycle);
                    DrawCustomDropdown("sequenceEase", ShowEaseMenu);
                    DrawProperty("useUnscaledTime");
                    DrawProperty("updateType");
                }
                
                // Decrease indent
                DecreaseIndent();
                
                // Validate fields
                ValidateValues(property);
            }
            
            // END PROPERTY
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Steps_CreateReorderableListIfNotExist(property, label);
            
            // SERIALIZED PROPERTY REFERENCES
            SerializedProperty cyclesProp = property.FindPropertyRelative("cycles");
            SerializedProperty e_expandedProp = property.FindPropertyRelative("e_expanded");
            SerializedProperty e_cyclesExpandedProp = property.FindPropertyRelative("e_cyclesExpanded");
            
            int cyclesVal = cyclesProp.intValue;
            bool isZeroOrOneCycle = cyclesVal >= 0 && cyclesVal <= 1;
            
            
            // BEGIN PROPERTY
            float height = 0f;
            
            // LOCAL METHODS
            void AddProperty(string name, bool visible = true, bool hasNoLabel = false)
            {
                if (!visible) return;
                var prop = property.FindPropertyRelative(name);
                float propHeight = EditorGUI.GetPropertyHeight(prop, true);
                
                height += propHeight + Spacing;
            }
            
            // void AddSpace(float addedHeight = 4f) => height += addedHeight;
            
            
            // CALCULATE HEIGHTS
            height += EditorGUIUtility.singleLineHeight; // + Spacing;
            if (DrawBackground) height += BGPadding * 2f;
            
            if (e_expandedProp.boolValue)
            {
                // Add height for steps list
                height += lists[property.propertyPath].GetHeight();
                
                // Add height for sequence properties
                AddProperty("cycles");
                if (e_cyclesExpandedProp.boolValue)
                {
                    AddProperty("cycleMode", visible: !isZeroOrOneCycle);
                    AddProperty("sequenceEase");
                    AddProperty("useUnscaledTime");
                    AddProperty("updateType");
                }
                height += Spacing;
            }
            
            return height;
        }
        
        
        // REORDERABLE LIST
        private void Steps_CreateReorderableListIfNotExist(SerializedProperty property, GUIContent label)
        {
            if (lists.ContainsKey(property.propertyPath)) return;
            
            SerializedProperty stepsProp = property.FindPropertyRelative("steps");
            var list = new ReorderableList(property.serializedObject, stepsProp, true, true, true, true);
            
            list.drawHeaderCallback = rect => {
                float xOffset = IndentSize * -EditorGUI.indentLevel;
                Rect labelRect = new(rect.x + xOffset, rect.y, rect.width - xOffset, rect.height);
                EditorGUI.LabelField(labelRect, "Steps");
            };
            
            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty element = stepsProp.GetArrayElementAtIndex(index);
                
                float indentSize = 20f;
                float width = rect.width + indentSize + 6f;
                float headerHeight = LineHeight + Spacing * 2;
                
                // Draw body background
                float bodyHeight = EditorGUI.GetPropertyHeight(element, true) + Spacing;
                Rect bodyRect = new(rect.x - indentSize, rect.y + headerHeight, width, bodyHeight - headerHeight);
                EditorGUI.DrawRect(bodyRect, new Color(0f, 0f, 0f, 0.1f));
                
                // Draw header background
                Rect headerRect = new(rect.x - indentSize, rect.y, width, headerHeight);
                EditorGUI.DrawRect(headerRect, new Color(1f, 1f, 1f, 0.025f));
                
                // Draw outline
                Rect outlineRect = new(rect.x - indentSize, rect.y, width, 1f);
                EditorGUI.DrawRect(outlineRect, new Color(0.2f, 0.2f, 0.2f, 1f));
                outlineRect.y += headerHeight;
                EditorGUI.DrawRect(outlineRect, new Color(0.2f, 0.2f, 0.2f, 1f));
                
                // Draw step property
                float xOffset = 0f, yOffset = 2f;
                Rect stepRect = new(rect.x + xOffset, rect.y + yOffset, rect.width - xOffset, rect.height);
                EditorGUI.PropertyField(stepRect, element, label, true);
            };
            
            list.elementHeightCallback = index =>
            {
                SerializedProperty element = stepsProp.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true);
            };
            
            list.onAddDropdownCallback = (rect, list) =>
            {
                AddStepGenericMenu(property);
            };
            
            lists[property.propertyPath] = list;
        }
        
        private void AddStepGenericMenu(SerializedProperty property)
        {
            var menu = new GenericMenu();
            
            menu.AddDisabledItem(new GUIContent("Add Step"));
            menu.AddSeparator("");
            
            menu.AddItem(new GUIContent("Tween"), false, () => AddStep(property, new TweenStep() { e_expanded = true }));
            menu.AddItem(new GUIContent("Sequence"), false, () => AddStep(property, new SequenceStep() { e_expanded = true }));
            menu.AddItem(new GUIContent("Play Sequencer"), false, () => AddStep(property, new PlaySequencerStep() { e_expanded = true }));
            menu.AddItem(new GUIContent("Delay"), false, () => AddStep(property, new DelayStep() { e_expanded = true }));
            menu.AddItem(new GUIContent("Callback"), false, () => AddStep(property, new CallbackStep() { e_expanded = true }));
            
            menu.ShowAsContext();
        }
        
        private void ShowEaseMenu(SerializedProperty easeProp, Rect rect)
        {
            var menu = new GenericMenu();
            
            AddItem(menu, "Default", easeProp, Ease.Default);
            AddItem(menu, "Linear", easeProp, Ease.Linear);
            string[] groups = { "Sine", "Quad", "Cubic", "Quart", "Quint", "Expo", "Circ", "Elastic", "Back", "Bounce" };
            
            int easeIndex = (int)Ease.InSine;
            foreach (string group in groups)
            {
                AddItem(menu, $"{group}/In {group}", easeProp, (Ease)easeIndex++);
                AddItem(menu, $"{group}/Out {group}", easeProp, (Ease)easeIndex++);
                AddItem(menu, $"{group}/In Out {group}", easeProp, (Ease)easeIndex++);
            }
            // AddItem(menu, "Custom", easeProp, Ease.Custom);
            menu.DropDown(rect);
            
            void AddItem(GenericMenu menu, string path, SerializedProperty prop, Ease ease)
            {
                menu.AddItem(new GUIContent(path), prop.intValue == (int)ease, () =>
                {
                    prop.intValue = (int)ease;
                    prop.serializedObject.ApplyModifiedProperties();
                });
            }
        }
        
        
        // FUNCTIONAL METHODS
        private void ValidateValues(SerializedProperty property)
        {
            var cycles = property.FindPropertyRelative("cycles");
            cycles.intValue = Math.Max(-1, cycles.intValue);
            if (cycles.intValue == 0) cycles.intValue = 1;
        }
        
        private void AddStep(SerializedProperty property, StepBase newStep)
        {
            if (property.serializedObject.targetObject is Component component)
            {
                GameObject gameObject = component.gameObject;
                if (newStep is TweenStep tweenStep)
                {
                    tweenStep.SetGameObject(gameObject);
                }
            }
            
            SerializedProperty stepsProp = property.FindPropertyRelative("steps");
            stepsProp.arraySize++;
            // stepsProp.InsertArrayElementAtIndex(stepsProp.arraySize); // if you want to duplicate last array element.
            
            SerializedProperty newElement = stepsProp.GetArrayElementAtIndex(stepsProp.arraySize - 1);
            newElement.managedReferenceValue = newStep;
            
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
        
        
    }
}
