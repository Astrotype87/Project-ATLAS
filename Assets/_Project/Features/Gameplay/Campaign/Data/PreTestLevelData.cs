using UnityEngine;

using ProjectATLAS.Quiz;

namespace ProjectATLAS.Gameplay
{
    [CreateAssetMenu(fileName = "C_PreTest", menuName = "Scriptable Objects/Campaign/Pre-Test Data")]
    public class PreTestData : TestData
    {
        [Header("Pre-Test Settings")]
        [SerializeField] private GameObject dialoguePrefab;
        [SerializeField] private QuizData[] chapterQuizData;
        
        // PROPERTIES
        public override string ID => $"PRE-{chapter:00}";
        public override string Name => $"Pre-Test {chapter:00}";
        public override LevelType Type => LevelType.PreTest;
        public override string Info => "Intro Dialogue, Quiz";
        
        public GameObject DialoguePrefab => dialoguePrefab;
        public QuizData[] ChapterQuizData => chapterQuizData;
    }
}
