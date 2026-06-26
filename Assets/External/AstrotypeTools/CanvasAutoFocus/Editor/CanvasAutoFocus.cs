using UnityEngine;
using UnityEditor;

namespace AstrotypeTools.CanvasAutoFocus
{
    /// <summary>
    /// Automatically focuses the Scene view on the root Canvas of the selected UI GameObject.
    /// </summary>
    [InitializeOnLoad]
    public static class CanvasAutoFocus
    {
        private const float ZoomFactor = 0.6f;
        
        private const string AutoFocusMenuPath = "Tools/Astrotype Tools/Canvas Auto Focus/Auto Focus";
        private const string InstantFocusMenuPath = "Tools/Astrotype Tools/Canvas Auto Focus/Instant Focus";
        private const string KeepFocusOnDeselectMenuPath = "Tools/Astrotype Tools/Canvas Auto Focus/Keep Focus on Canvas Deselect";
        
        private static bool IsAutoFocus
        {
            get => EditorPrefs.GetBool("AstrotypeTools.CanvasAutoFocus.AutoFocus", true);
            set => EditorPrefs.SetBool("AstrotypeTools.CanvasAutoFocus.AutoFocus", value);
        }
        private static bool IsInstantFocus
        {
            get => EditorPrefs.GetBool("AstrotypeTools.CanvasAutoFocus.InstantFocus", false);
            set => EditorPrefs.SetBool("AstrotypeTools.CanvasAutoFocus.InstantFocus", value);
        }
        private static bool IsKeepFocusOnDeselect
        {
            get => EditorPrefs.GetBool("AstrotypeTools.CanvasAutoFocus.KeepFocusOnDeselect", false);
            set => EditorPrefs.SetBool("AstrotypeTools.CanvasAutoFocus.KeepFocusOnDeselect", value);
        }
        
        private static Canvas focusedRootCanvas = null;
        private static SceneViewState? previousState = null;
        
        static CanvasAutoFocus()
        {
            Selection.selectionChanged += OnSelectionChanged;
            EditorApplication.delayCall += () =>
            {
                Menu.SetChecked(AutoFocusMenuPath, IsAutoFocus);
                Menu.SetChecked(InstantFocusMenuPath, IsInstantFocus);
                Menu.SetChecked(KeepFocusOnDeselectMenuPath, IsKeepFocusOnDeselect);
            };
        }
        
        
        [MenuItem(AutoFocusMenuPath)]
        private static void AutoFocus()
        {
            IsAutoFocus = !IsAutoFocus;
            Menu.SetChecked(AutoFocusMenuPath, IsAutoFocus);
        }
        
        [MenuItem(InstantFocusMenuPath)]
        private static void InstantFocus()
        {
            IsInstantFocus = !IsInstantFocus;
            Menu.SetChecked(InstantFocusMenuPath, IsInstantFocus);
        }
        
        [MenuItem(KeepFocusOnDeselectMenuPath)]
        private static void KeepFocusOnDeselect()
        {
            IsKeepFocusOnDeselect = !IsKeepFocusOnDeselect;
            Menu.SetChecked(KeepFocusOnDeselectMenuPath, IsKeepFocusOnDeselect);
        }
        
        
        private static void OnSelectionChanged()
        {
            if (!IsAutoFocus)
            {
                previousState = null;
                return;
            }
            
            GameObject selectedGameObject = Selection.activeGameObject;
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null) return;
            
            Canvas parentCanvas = selectedGameObject == null ? null : selectedGameObject.GetComponentInParent<Canvas>(true);
            Canvas selectedRootCanvas = parentCanvas == null ? null : parentCanvas.rootCanvas;
            
            if (selectedRootCanvas)
            {
                if (selectedRootCanvas == focusedRootCanvas) return;
                focusedRootCanvas = selectedRootCanvas;
                
                if (previousState == null)
                    previousState = new SceneViewState(sceneView);
                
                sceneView.in2DMode = true;
                sceneView.orthographic = true;
                
                Bounds bounds = CalculateBounds(selectedRootCanvas.gameObject);
                bounds.size *= ZoomFactor;
                sceneView.Frame(bounds, IsInstantFocus);
            }
            else if (focusedRootCanvas && (selectedGameObject || !IsKeepFocusOnDeselect))
            {
                if (previousState != null)
                {
                    previousState.Value.Restore(sceneView, IsInstantFocus);
                    previousState = null;
                }
                focusedRootCanvas = null;
            }
        }
        
        private static Bounds CalculateBounds(GameObject root)
        {
            var renderers = root.GetComponentsInChildren<Renderer>();
            var rectTransforms = root.GetComponentsInChildren<RectTransform>();
            
            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                    bounds.Encapsulate(renderers[i].bounds);
                return bounds;
            }
            else if (rectTransforms.Length > 0)
            {
                Vector3 min = rectTransforms[0].position;
                Vector3 max = rectTransforms[0].position;
                
                foreach (var rt in rectTransforms)
                {
                    Vector3[] corners = new Vector3[4];
                    rt.GetWorldCorners(corners);
                    foreach (var c in corners)
                    {
                        min = Vector3.Min(min, c);
                        max = Vector3.Max(max, c);
                    }
                }
                
                Bounds bounds = new Bounds();
                bounds.SetMinMax(min, max);
                return bounds;
            }
            
            return new Bounds(root.transform.position, Vector3.one);
        }
        
        
        private struct SceneViewState
        {
            public Vector3 pivot;
            public float size;
            public bool is2D;
            public Quaternion rotation;
            public bool orthographic;
            
            public SceneViewState(SceneView view)
            {
                pivot = view.pivot;
                size = view.size;
                is2D = view.in2DMode;
                rotation = view.rotation;
                orthographic = view.orthographic;
            }
            
            public void Restore(SceneView view, bool instant)
            {
                view.in2DMode = is2D;
                view.LookAt(pivot, rotation, size, orthographic, instant);
            }
        }
    }
}
