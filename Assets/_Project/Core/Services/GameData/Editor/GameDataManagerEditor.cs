using UnityEngine;
using UnityEditor;
using ProjectATLAS.GameData;

namespace ProjectATLAS.Editor
{
    [CustomEditor(typeof(GameDataManager))]
    public class GameDataManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GameDataManager script = target as GameDataManager;
            
            base.OnInspectorGUI();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Manage Data", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Save Data"))
            {
                script.SaveData();
            }
            if (GUILayout.Button("Load Data"))
            {
                Undo.RecordObject(script, "Load Game Data");
                script.LoadData();
                EditorUtility.SetDirty(script);
            }
            if (GUILayout.Button("Unload Data"))
            {
                Undo.RecordObject(script, "Unload Game Data");
                script.UnloadData();
                EditorUtility.SetDirty(script);
            }
            
            EditorGUILayout.LabelField("File Path");
            GUIStyle style = new(EditorStyles.textArea) { wordWrap = true };
            GUI.enabled = false;
            EditorGUILayout.TextArea(script.GetFilePath(), style);
            GUI.enabled = true;
        }
    }
}
