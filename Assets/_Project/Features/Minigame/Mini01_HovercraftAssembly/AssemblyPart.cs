using UnityEngine;
using UnityEngine.EventSystems;
using KBCore.Refs;

#if UNITY_EDITOR
using UnityEditor;
#endif

using ProjectATLAS.Input;

namespace ProjectATLAS.Minigame.Mini01_HovercraftAssembly
{
    public class AssemblyPart : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private new string name;
        [SerializeField] private Vector2 currentPosition;
        [SerializeField] private Vector2 startPosition;
        [SerializeField] private Vector2 assembledPosition;
        [SerializeField, Self] private Draggable2D draggable2D;
        
        
        // PROPERTIES
        public string Name => name;
        public Vector2 CurrentPosition => currentPosition;
        public Vector2 AssembledPosition => assembledPosition;
        
        
        // MONOBEHAVIOUR METHODS
        private void Awake()
        {
            currentPosition = transform.position;
        }
        
        private void OnValidate()
        {
            this.ValidateRefs();
            
            if (!Application.isPlaying)
            {
                currentPosition = transform.position;
            }
        }
        
        // PUBLIC METHODS
        public void MoveToStartPosition() => transform.position = startPosition;
        public void MoveToAssembledPosition() => transform.position = assembledPosition;
        
        public void RecordStartPosition() => startPosition = transform.position;
        public void RecordAssembledPosition() => assembledPosition = transform.position;
        
        public void SetGridSnap(Vector2 gridSnap) => draggable2D.SetGridSnap(gridSnap);
        
        // HANDLER METHODS
        public void OnBeginDrag(PointerEventData eventData)
        {
            currentPosition = transform.position;
        }
        public void OnDrag(PointerEventData eventData)
        {
            currentPosition = transform.position;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            currentPosition = transform.position;
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(AssemblyPart))]
    public class AssemblyPartEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            AssemblyPart script = target as AssemblyPart;
            
            if (script && GUILayout.Button("Move To Start Position"))
            {
                script.MoveToStartPosition();
            }
            if (script && GUILayout.Button("Move To Assembled Position"))
            {
                script.MoveToAssembledPosition();
            }
            
            if (script && GUILayout.Button("Record Start Position"))
            {
                script.RecordStartPosition();
            }
            if (script && GUILayout.Button("Record Assembled Position"))
            {
                script.RecordAssembledPosition();
            }
        }
    }
#endif
}
