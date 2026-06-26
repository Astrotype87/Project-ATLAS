using System.Collections.Generic;
using UnityEngine;

namespace ProjectATLAS.Beta.Quiz
{
    [CreateAssetMenu(fileName = "QuizData", menuName = "Scriptable Objects/Backup/QuizData2")]
    public class QuizData : ScriptableObject
    {
        [SerializeReference]
        public List<QuizItem> quizItems;


    }
    
}
