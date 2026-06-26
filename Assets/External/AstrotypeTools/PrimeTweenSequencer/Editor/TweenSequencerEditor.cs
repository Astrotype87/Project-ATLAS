using UnityEngine;
using UnityEditor;

namespace AstrotypeTools.PrimeTweenSequencer.Editor
{
    [CustomEditor(typeof(TweenSequencer))]
    public class TweenSequencerEditor : UnityEditor.Editor
    {
        private SerializedProperty autoPlayModeProp;
        private SerializedProperty sequenceProp;
        private SerializedProperty e_expandPlayerProp;
        private SerializedProperty e_progressProp;
        private SerializedProperty e_smoothRefreshProp;
        
        private Texture2D playIcon;
        private Texture2D pauseIcon;
        private Texture2D nextIcon;
        private Texture2D previousIcon;
        private Texture2D stopIcon;
        
        private PlayerState previousPlayerState;
        
        
        private void OnEnable()
        {
            EditorApplication.update += EditorApplication_update;
            
            autoPlayModeProp = serializedObject.FindProperty("autoPlayMode");
            sequenceProp = serializedObject.FindProperty("sequence");
            
            e_expandPlayerProp = serializedObject.FindProperty("e_expandPlayer");
            e_progressProp = serializedObject.FindProperty("e_progress");
            e_smoothRefreshProp = serializedObject.FindProperty("e_smoothRefresh");
            
            playIcon = EditorGUIUtility.IconContent("play").image as Texture2D;
            pauseIcon = EditorGUIUtility.IconContent("pause").image as Texture2D;
            nextIcon = EditorGUIUtility.IconContent("animation.nextKey").image as Texture2D;
            previousIcon = EditorGUIUtility.IconContent("animation.prevKey").image as Texture2D;
            stopIcon = EditorGUIUtility.IconContent("stop").image as Texture2D;
        }
        
        private void OnDisable()
        {
            EditorApplication.update -= EditorApplication_update;
        }
        
        public override void OnInspectorGUI()
        {
            // UPDATE SERIALIZED OBJECT
            serializedObject.Update();
            
            using (var v = new EditorGUILayout.VerticalScope("FrameBox"))
            {
                DrawPlayerButtons();
                if (DrawFoldout())
                {
                    DrawExpandedControls();
                }
            }
            
            // DRAW SEQUENCE PANEL
            using (var v = new EditorGUILayout.VerticalScope("FrameBox"))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(sequenceProp);
                EditorGUILayout.Space(-2f);
                EditorGUI.indentLevel--;
            }
            // APPLY CHANGES TO SERIALIZED OBJECT
            serializedObject.ApplyModifiedProperties();
        }
        
        
        private void DrawPlayerButtons()
        {
            TweenSequencer script = target as TweenSequencer;
            PlayerState _playerState = script.PlayerState;
            
            // START HORIZONTAL CENTERED SPACE
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // BUTTON SETTINGS
            GUILayoutOption[] buttonSize = {
                GUILayout.Width(EditorGUIUtility.singleLineHeight * 1.75f),
                GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.75f),
            };
            
            Texture2D playIcon = _playerState == PlayerState.Playing ? pauseIcon : this.playIcon;
            string playTooltip = _playerState switch {
                PlayerState.Playing => "Pause",
                PlayerState.Paused => "Resume",
                _ => "Play"
            };
            
            // GUILayoutOption[] emptySize = {
            //     GUILayout.Width(EditorGUIUtility.singleLineHeight * 1.75f / 2f),
            //     GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.75f),
            // };
            // GUILayout.Label("", emptySize);
            
            // DRAW BUTTONS
            if (GUILayout.Button(new GUIContent(previousIcon, "Restart"), buttonSize))
            {
                script.Restart();
                e_progressProp.floatValue = 0;
            }
            if (GUILayout.Button(new GUIContent(playIcon, playTooltip), buttonSize))
            {
                switch (_playerState)
                {
                    case PlayerState.Playing : script.Pause(); break;
                    case PlayerState.Paused : script.Resume(); break;
                    default: script.Play(); break;
                }
            }
            if (GUILayout.Button(new GUIContent(nextIcon, "Complete"), buttonSize))
            {
                script.Complete();
                e_progressProp.floatValue = 1;
            }
            GUI.enabled = _playerState != PlayerState.Inactive;
            if (GUILayout.Button(new GUIContent(stopIcon, "Stop"), buttonSize))
            {
                script.Stop();
                script.RestoreState();
                e_progressProp.floatValue = 0;
            }
            GUI.enabled = true;
            
            // END HORIZONTAL CENTERED SPACE
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        
        private bool DrawFoldout()
        {
            // DRAW FOLDOUT
            // Get name and icon
            // string name = " Player";
            // Texture2D icon = IconDictionary.GetIcon(typeof(TweenSequencePlayer));
            
            // Get foldout style
            GUIContent playFoldContent = new("Player");
            GUIStyle playFoldStyle = new(EditorStyles.foldout);
            playFoldStyle.fixedWidth = EditorGUIUtility.currentViewWidth;
            playFoldStyle.fixedHeight = EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing;
            
            // Draw foldout
            EditorGUI.indentLevel++;
            Rect playFoldRect = GUILayoutUtility.GetLastRect();
            playFoldRect.y += EditorGUIUtility.singleLineHeight * 1.75f / 4f;
            e_expandPlayerProp.boolValue = EditorGUI.Foldout(playFoldRect, e_expandPlayerProp.boolValue, playFoldContent, true, playFoldStyle);
            EditorGUI.indentLevel--;
            
            // DRAW EXPANDED PLAYER CONTROLS
            return e_expandPlayerProp.boolValue;
        }
        
        private void DrawExpandedControls()
        {
            TweenSequencer script = target as TweenSequencer;
            
            // CONDITIONS
            PlayerState _playerState = script.PlayerState;
            bool _isInfiniteCycle = script.IsInfiniteCycle;
            float _progress = script.GetProgress();
            float _elapsedTime = _playerState == PlayerState.Completed ? script.GetDuration() : script.GetElapsedTime();
            float _duration = script.GetDuration();
            
            // UPDATE PROGRESS
            e_progressProp.floatValue = _progress;
            
            // DRAW PROGRESS SLIDER
            GUI.enabled = !_isInfiniteCycle;
            Rect rect = EditorGUILayout.GetControlRect();
            Rect progressRect = new(rect.x + 2, rect.y, rect.width - 4, rect.height);
            float newProgress = GUI.HorizontalSlider(progressRect, e_progressProp.floatValue, 0, 1);
            GUI.enabled = true;
            
            if (!Mathf.Approximately(newProgress, e_progressProp.floatValue))
            {
                script.Seek(newProgress);
                e_progressProp.floatValue = newProgress;
            }
            
            
            // LABEL STYLES
            GUIStyle leftLabelStyle = new(GUI.skin.label) { alignment = TextAnchor.MiddleLeft };
            GUIStyle centerLabelStyle = new(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            GUIStyle rightLabelStyle = new(GUI.skin.label) { alignment = TextAnchor.MiddleRight };
            
            string durationText = _duration == float.PositiveInfinity ?
                "\u221E" : _duration.ToString("00.00");
            
            // DRAW TIMER LABELS
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(_elapsedTime.ToString("00.00"), leftLabelStyle, GUILayout.Width(70));
            GUILayout.FlexibleSpace();
            GUILayout.Label(_playerState.ToString(), centerLabelStyle, GUILayout.Width(70));
            GUILayout.FlexibleSpace();
            GUILayout.Label(durationText, rightLabelStyle, GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();
            
            // DRAW REMAINING PROPERTIES
            EditorGUILayout.PropertyField(autoPlayModeProp);
            EditorGUILayout.PropertyField(e_smoothRefreshProp, new GUIContent("Smooth Refresh"));
        }
        
        
        private void EditorApplication_update()
        {
            TweenSequencer script = target as TweenSequencer;
            
            bool _isPlaying = script.PlayerState == PlayerState.Playing
                || previousPlayerState == PlayerState.Playing;
            
            previousPlayerState = script.PlayerState;
            
            if (e_smoothRefreshProp.boolValue && _isPlaying)
            {
                Repaint();
            }
        }
        
        
    }
}
