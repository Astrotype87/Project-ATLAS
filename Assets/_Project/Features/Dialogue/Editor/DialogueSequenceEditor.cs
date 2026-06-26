using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ProjectATLAS.Dialogue
{
    [CustomEditor(typeof(DialogueSequence))]
    public class DialogueSequenceEditor : UnityEditor.Editor
    {
        private ReorderableList stepsList;
        
        private void OnEnable()
        {
            SerializedProperty stepsProperty = serializedObject.FindProperty("steps");
            
            stepsList = new ReorderableList(serializedObject, stepsProperty, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Dialogue Steps");
                },
                
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = stepsProperty.GetArrayElementAtIndex(index);
                    
                    var messageProp = element.FindPropertyRelative("message");
                    var useAnimationProp = element.FindPropertyRelative("useAnimation");
                    var startTimeProp = element.FindPropertyRelative("startTime");
                    var endTimeProp = element.FindPropertyRelative("endTime");
                    
                    // Append animation info if enabled
                    string animationInfo = "";
                    if (useAnimationProp.boolValue)
                        animationInfo = $" ({startTimeProp.floatValue * 60}-{endTimeProp.floatValue * 60})";
                    
                    // Get message preview (first line only)
                    string messagePreview = messageProp.stringValue;
                    if (!string.IsNullOrEmpty(messagePreview))
                    {
                        int newlineIndex = messagePreview.IndexOf('\n');
                        if (newlineIndex >= 0)
                            messagePreview = messagePreview.Substring(0, newlineIndex);
                    }
                    
                    // Draw element with formatted label
                    float indent = 10f;
                    Rect elementRect = rect;
                    elementRect.x += indent;
                    elementRect.width -= indent;
                    
                    EditorGUI.PropertyField(
                        elementRect,
                        element,
                        new GUIContent($"{index + 1}.{animationInfo} {messagePreview}"),
                        true
                    );
                },
                
                elementHeightCallback = index =>
                {
                    float indent = 0f;
                    return EditorGUI.GetPropertyHeight(stepsProperty.GetArrayElementAtIndex(index), true) + indent;
                }
            };
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            stepsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
