using System;

namespace ProjectATLAS.Quiz
{
    [Serializable]
    public class CustomQuizSettings
    {
        public string topicName;
        public QuizData[] quizDatas;
        public int itemCount;
        public ItemTypes itemTypes;
    }
}
