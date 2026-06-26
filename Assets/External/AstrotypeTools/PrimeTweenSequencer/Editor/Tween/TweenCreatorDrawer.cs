using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace AstrotypeTools.PrimeTweenSequencer.Editor
{
    [CustomPropertyDrawer(typeof(TweenCreator))]
    public class TweenCreatorDrawer: PropertyDrawer
    {
        private float LineHeight => EditorGUIUtility.singleLineHeight;
        private float Spacing => EditorGUIUtility.standardVerticalSpacing;
        
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty gameObjectProp = property.FindPropertyRelative("gameObject");
            SerializedProperty tweenBaseProp = property.FindPropertyRelative("tweenBase");
            
            GameObject gameObject = gameObjectProp.objectReferenceValue as GameObject;
            TweenBase tweenBase = tweenBaseProp.managedReferenceValue as TweenBase;
            
            
            // REUSABLE RECT
            Rect rect = new Rect(position.x, position.y, position.width, LineHeight);
            float labelWidth = EditorGUIUtility.labelWidth;
            float dropdownWidth = rect.width - labelWidth;
            
            
            // DRAW GAME OBJECT
            Rect gameObjectRect = new(rect.x, rect.y, labelWidth, rect.height);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(gameObjectRect, gameObjectProp, GUIContent.none);
            if (EditorGUI.EndChangeCheck() && tweenBase != null)
            {
                GameObject newGameObject = gameObjectProp.objectReferenceValue as GameObject;
                Component newTarget = newGameObject != null ? newGameObject.GetComponent(tweenBase.TargetType) : null;
                
                SerializedProperty targetProp = tweenBaseProp.FindPropertyRelative("target");
                if (targetProp != null) targetProp.objectReferenceValue = newTarget;
                
                tweenBaseProp.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(tweenBaseProp.serializedObject.targetObject);
            }
            
            // DRAW SELECTOR
            string text = tweenBase == null ? "No Tween" : tweenBase.GetType().Name;
            Rect dropdownRect = new(rect.x + labelWidth + 2f, rect.y, dropdownWidth - 2f, rect.height);
            if (EditorGUI.DropdownButton(dropdownRect, new(text), FocusType.Keyboard, EditorStyles.popup))
            {
                SelectTweenGenericMenu(property);
            }
            rect.y += LineHeight + Spacing;
            
            // DRAW WARNING IF NO TARGET
            if (tweenBase != null && !tweenBase.IsTargetSet)
            {
                Rect warningRect = new(rect.x, rect.y, rect.width, LineHeight * 2 + Spacing);
                string warningText = gameObject == null
                    ? $"No GameObject assigned."
                    : $"{tweenBase.TargetType.Name} component not found on the assigned GameObject.";
                
                EditorGUI.HelpBox(warningRect, new(warningText), MessageType.Warning);
                rect.y += LineHeight * 2 + Spacing + Spacing;
            }
            
            // ADD TINY SPACING
            // rect.y += Spacing;
            
            // DRAW TWEEN SETTINGS
            if (tweenBase != null)
            {
                EditorGUI.PropertyField(rect, tweenBaseProp, true);
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty gameObjectProp = property.FindPropertyRelative("gameObject");
            SerializedProperty tweenBaseProp = property.FindPropertyRelative("tweenBase");
            
            GameObject gameObject = gameObjectProp.objectReferenceValue as GameObject;
            TweenBase tweenBase = tweenBaseProp.managedReferenceValue as TweenBase;
            
            float height = 0;
            
            height += LineHeight; // gameObject and tween selector
            
            if (tweenBase != null && !tweenBase.IsTargetSet)
                height += (LineHeight + Spacing) * 2;   // warning when gameobject doesn't have expected component
            
            if (tweenBase != null)
            {
                // height += Spacing;  // add tiny space between target and tween
                height += EditorGUI.GetPropertyHeight(tweenBaseProp, label, true);  // tween settings
            }
            
            return height;
        }
        
        
        // GENERIC SELECTOR
        private void SelectTweenGenericMenu(SerializedProperty property)
        {
            SerializedProperty gameObjectProp = property.FindPropertyRelative("gameObject");
            SerializedProperty tweenBaseProp = property.FindPropertyRelative("tweenBase");
            
            GameObject gameObject = gameObjectProp.objectReferenceValue as GameObject;
            TweenBase tweenBase = tweenBaseProp.managedReferenceValue as TweenBase;
            
            
            // New Generic Menu
            var menu = new GenericMenu();
            
            // Add menu title
            string menuTitle = gameObject == null ? "No Target Selected" : gameObject.name;
            menu.AddDisabledItem(new GUIContent(menuTitle));
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("None"), false, () => SetTweenBase(property, null));
            
            // Get list of components and add available tweens for each component in the generic menu
            Component[] components = gameObject != null ? gameObject.GetComponents<Component>() : new Component[0];
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                Type[] tweenTypes = GetComponentTweenTypes(component.GetType());
                if (tweenTypes.Length <= 0) continue;
                
                for (int j = 0; j < tweenTypes.Length; j++)
                {
                    Type tweenType = tweenTypes[j];
                    string path = $"{component.GetType().Name}/{tweenType.Name}";
                    bool isSelected = tweenType == tweenBase?.GetType();
                    
                    menu.AddItem(new GUIContent(path), isSelected, () => SetTweenBase(property, tweenType));
                }
            }
            
            // Show generic menu
            menu.ShowAsContext();
        }
        
        private static Type[] GetComponentTweenTypes(Type componentType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type => InheritsFromTweenBase(type, componentType))
                .ToArray();
            
            static bool InheritsFromTweenBase(Type type, Type componentType)
            {
                while (type != null && type != typeof(object))
                {
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(TweenBase<>))
                    {
                        var tweenTargetType = type.GetGenericArguments()[0];
                        return tweenTargetType.IsAssignableFrom(componentType);
                    }
                    type = type.BaseType;
                }
                return false;
            }
        }
        
        
        // MODIFY PROPERTY METHODS
        private void SetTweenBase(SerializedProperty property, Type type)
        {
            SerializedProperty gameObjectProp = property.FindPropertyRelative("gameObject");
            SerializedProperty tweenBaseProp = property.FindPropertyRelative("tweenBase");
            
            if (type == null)
            {
                tweenBaseProp.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
                return;
            }
            
            TweenBase newTweenBase = Activator.CreateInstance(type) as TweenBase;
            GameObject gameObject = gameObjectProp.objectReferenceValue as GameObject;
            newTweenBase.SetTarget(gameObject);
            
            tweenBaseProp.managedReferenceValue = newTweenBase;
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
        
        
    }
}
