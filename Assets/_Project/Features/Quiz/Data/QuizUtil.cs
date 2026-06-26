using ProjectATLAS.Gameplay;
using UnityEngine;

namespace ProjectATLAS.Quiz
{
    public static class QuizUtil
    {
        public static readonly char[] alphabet = {'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};
        public static char Alphabet(int index) => alphabet[index];
    }
}
