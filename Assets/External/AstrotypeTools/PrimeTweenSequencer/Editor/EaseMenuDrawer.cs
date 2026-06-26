using UnityEngine;
using UnityEditor;
using PrimeTween;

namespace AstrotypeTools.PrimeTweenSequencer.Editor
{
    [CustomPropertyDrawer(typeof(EaseMenuAttribute))]
    public class EaseMenuDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            float labelWidth = EditorGUIUtility.labelWidth + 2f;
            Rect labelRect = new(position.x, position.y, labelWidth, position.height);
            Rect fieldRect = new (position.x + labelWidth, position.y, position.width - labelWidth, position.height);
            GUIContent content = new(property.enumDisplayNames[property.enumValueIndex]);
            
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard, position);
            bool isActive = GUIUtility.hotControl == controlID;
            bool isKeyboardFocused = GUIUtility.keyboardControl == controlID;
            
            Event evt = Event.current;
            bool isClicked = evt.type == EventType.MouseDown && fieldRect.Contains(evt.mousePosition);
            bool isEntered = evt.type == EventType.KeyDown && evt.keyCode == KeyCode.Return;
            
            EditorGUI.PrefixLabel(labelRect, controlID, label);
            if (evt.type == EventType.Repaint)
            {
                // Draw dropdown with hover and keyboard focus
                EditorStyles.popup.Draw(fieldRect, content, controlID, isActive, isKeyboardFocused);
            }
            if (isClicked || (isEntered && isKeyboardFocused))
            {
                // Show ease menu when you dropdown is clicked or you press enter
                GUIUtility.keyboardControl = controlID;
                GUIUtility.hotControl = controlID;
                evt.Use();
                ShowEaseMenu(property, fieldRect);
            }
            
            EditorGUI.EndProperty();
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
            AddItem(menu, "Custom", easeProp, Ease.Custom);
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
    }
}
