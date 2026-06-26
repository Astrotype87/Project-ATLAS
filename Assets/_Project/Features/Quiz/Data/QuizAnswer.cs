using System;
using System.Collections.Generic;
using UnityEngine;

using ProjectATLAS.Types;
using ProjectATLAS.Utility;
using static ProjectATLAS.Quiz.CategorizationItem;
using System.Linq;

namespace ProjectATLAS.Quiz
{
    [Serializable]
    public abstract class QuizAnswer
    {
        
    }
    
    [Serializable]
    public class MultipleChoiceAnswer : QuizAnswer
    {
        public string answer;
        
        public MultipleChoiceAnswer() { }
        public MultipleChoiceAnswer(string answer) => this.answer = answer;
        
        public override string ToString() => answer;
    }
    
    [Serializable]
    public class TrueOrFalseAnswer : QuizAnswer
    {
        public BoolEnum answer;
        
        public TrueOrFalseAnswer() { }
        public TrueOrFalseAnswer(BoolEnum answer) => this.answer = answer;
        
        public override string ToString() => answer.AsBool() ? "True" : "False";
    }
    
    [Serializable]
    public class MatchingAnswer : QuizAnswer
    {
        public string[] answers;
        
        public MatchingAnswer() { }
        public MatchingAnswer(string[] answers) => this.answers = answers;
        
        public override string ToString() => string.Join(", ", answers);
    }
    
    [Serializable]
    public class FillInTheBlanksAnswer : QuizAnswer
    {
        public string[] answers;
        
        public FillInTheBlanksAnswer() { }
        public FillInTheBlanksAnswer(string[] answers) => this.answers = answers;
        
        public override string ToString() => string.Join(", ", answers);
    }
    
    [Serializable]
    public class SequenceAnswer : QuizAnswer
    {
        public string[] answers;
        
        public SequenceAnswer() { }
        public SequenceAnswer(string[] answers) => this.answers = answers;
        
        public override string ToString() => string.Join(", ", answers);
    }
    
    [Serializable]
    public class CategorizationAnswer : QuizAnswer
    {
        public CategoryItems[] answers;
        
        public CategorizationAnswer(CategoryItems[] answers) => this.answers = answers;
        
        public string[] GetAnswersByCategory(string category) => answers.Where(a => a.category == category).First().items;
        
        public override string ToString()
        {
            string answerList = "";
            if (answers == null) return "(No Answer)";
            
            foreach (var answer in answers)
            {
                // Category: [Item1, item2, item3] \n 
                if (answer.items == null)
                    answerList += $"{answer.category} : (None)\n";
                else
                    answerList += $"{answer.category} : {string.Join(", ", answer.items)}\n";
            }
            return answerList;
        }
    }
    
    [Serializable]
    public class SolvingAnswer : QuizAnswer
    {
        public double answer;
        
        public SolvingAnswer() { }
        public SolvingAnswer(double answer) => this.answer = answer;
        
        public override string ToString() => answer.ToString();
    }
}
