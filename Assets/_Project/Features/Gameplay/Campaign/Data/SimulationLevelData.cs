using Eflatun.SceneReference;
using UnityEngine;

namespace ProjectATLAS.Gameplay
{
    [CreateAssetMenu(fileName = "_Simulation", menuName = "Scriptable Objects/Campaign/Simulation Level Data")]
    public class SimulationLevelData : LevelData
    {
        [Header("Simulation Level Settings")]
        [SerializeField, TextArea(4, 8)] private string instructions;
        [SerializeField] private GameObject dialoguePrefab;
        [SerializeField] private GameObject simulationPrefab;
        
        // PROPERTIES
        public override string ID => $"LVL-{number:00}";
        public override string Name => $"Level {number:00}";
        public override LevelType Type => LevelType.Simulation;
        public override string Info => "Short Dialogue, Simulation";
        
        public override string Mechanics => instructions;
        
        public string Instructions => instructions;
        public GameObject DialoguePrefab => dialoguePrefab;
        public GameObject SimulationPrefab => simulationPrefab;
    }
}
