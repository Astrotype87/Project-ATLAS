using UnityEngine;

using ProjectATLAS.Quiz;

namespace ProjectATLAS.Gameplay
{
    [CreateAssetMenu(fileName = "_Lesson", menuName = "Scriptable Objects/Campaign/Lesson Level Data")]
    public class LessonLevelData : LevelData
    {
        [Header("Lesson Level Settings")]
        [SerializeField] private GameObject dialoguePrefab;
        [SerializeField] private GameObject guidebookPrefab;
        [SerializeField] private QuizData quizData;
        
        // PROPERTIES
        public override string ID => $"LVL-{number:00}";
        public override string Name => $"Level {number:00}";
        public override LevelType Type => LevelType.Lesson;
        public override string Info => "Lesson Dialogue, Quiz";
        
        public override string Mechanics => Objectives;
        
        public GameObject DialoguePrefab => dialoguePrefab;
        public GameObject GuidebookPrefab => guidebookPrefab;
        public QuizData QuizData => quizData;
    }
}
