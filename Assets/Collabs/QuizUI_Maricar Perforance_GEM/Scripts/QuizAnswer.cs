using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectATLAS.Beta.Quiz
{
    public abstract class QuizAnswer
    {
        public int index;
    }

    public class MultipleChoiceAnswer : QuizAnswer
    {
        public string answer;
    }

    public class TrueOrFalseAnswer : QuizAnswer
    {
        public bool answer;
    }

    public class MatchingAnswer : QuizAnswer
    {
        public string[] answers;
    }
    public class FillInTheBlanksAnswer : QuizAnswer
    {
        public string[] answers;
    }
    public class SequenceAnswer : QuizAnswer
    {
        public string[] orderedAnswers;
    }

    public class CategorizationAnswer : QuizAnswer
    {
        public List<CategoryItem> categoryAnswers;
        
        public class CategoryItem
        {
            
        }
    }

    public class SolvingAnswer : QuizAnswer
    {
        public double answer;

    }
}


