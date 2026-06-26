using UnityEngine;

using ProjectATLAS.Quiz;

namespace ProjectATLAS.Gameplay
{
    [CreateAssetMenu(fileName = "C_PostTest", menuName = "Scriptable Objects/Campaign/Post-Test Level Data")]
    public class PostTestData : TestData
    {
        [Header("Post-Test Settings")]
        [SerializeField] private GameObject dialoguePrefab;
        [SerializeField] private QuizData[] chapterQuizData;
        
        // PROPERTIES
        public override string ID => $"POST-{chapter:00}";
        public override string Name => $"Post-Test {chapter:00}";
        public override LevelType Type => LevelType.PostTest;
        public override string Info => "Quiz, Ending Dialogue";
        
        public GameObject DialoguePrefab => dialoguePrefab;
        public QuizData[] ChapterQuizData => chapterQuizData;
    }
}
