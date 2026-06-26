using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace ProjectATLAS.Beta.Quiz
{
    
    [CustomPropertyDrawer(typeof(QuizItem))]
    public class QuizItemDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (property.managedReferenceValue == null)
            {
                var buttonRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(buttonRect, $"Select Item Type"))
                {
                    ShowTypeSelectionMenu(property);
                }
            }
            else
            {
                var typeLabel = new GUIContent(PascalToTitleCase(property.managedReferenceValue.GetType().Name));
                var height = EditorGUI.GetPropertyHeight(property, true);
                var fullRect = new Rect(position.x, position.y, position.width, height);
                EditorGUI.PropertyField(fullRect, property, typeLabel, true);
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.managedReferenceValue == null)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private void ShowTypeSelectionMenu(SerializedProperty property)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new("Multiple Choice Item"), false, () => SetQuizItem(property, new MultipleChoiceItem()));
            menu.AddItem(new("True Or False Item"), false, () => SetQuizItem(property, new TrueOrFalseItem()));
            menu.AddItem(new("Matching Item"), false, () => SetQuizItem(property, new MatchingItem()));
            menu.AddItem(new("Fill In The Blanks Item"), false, () => SetQuizItem(property, new FillInTheBlanksItem()));
            menu.AddItem(new("Sequence Item"), false, () => SetQuizItem(property, new SequencingItem()));
            menu.AddItem(new("Categorization Item"), false, () => SetQuizItem(property, new CategorizationItem()));
            menu.AddItem(new("Solving Item"), false, () => SetQuizItem(property, new SolvingItem()));

            menu.ShowAsContext();
        }

        private void SetQuizItem(SerializedProperty property, QuizItem quizItem)
        {
            property.serializedObject.Update();
            property.managedReferenceValue = quizItem;
            property.serializedObject.ApplyModifiedProperties();
        }

        static string PascalToTitleCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            string withSpaces = Regex.Replace(input, "(?<!^)([A-Z])", " $1");
            return withSpaces.Trim();
        }
}
}
