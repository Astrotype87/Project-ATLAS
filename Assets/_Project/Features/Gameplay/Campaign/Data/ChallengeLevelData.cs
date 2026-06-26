using UnityEngine;

namespace ProjectATLAS.Gameplay
{
    [CreateAssetMenu(fileName = "_Challenge", menuName = "Scriptable Objects/Campaign/Challenge Level Data")]
    public class ChallengeLevelData : LevelData
    {
        [Header("Challenge Level Settings")]
        [SerializeField, TextArea(4, 8)] private string instructions;
        [SerializeField] private GameObject dialoguePrefab;
        [SerializeField] private GameObject minigamePrefab;
        
        // PROPERTIES
        public override string ID => $"LVL-{number:00}";
        public override string Name => $"Level {number:00}";
        public override LevelType Type => LevelType.Challenge;
        public override string Info => "Short Dialogue, Simulation";
        
        public override string Mechanics => instructions;
        
        public string Instructions => instructions;
        public GameObject DialoguePrefab => dialoguePrefab;
        public GameObject MinigamePrefab => minigamePrefab;
    }
}
