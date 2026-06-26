using UnityEngine;

namespace ProjectATLAS.Dialogue
{
    [CreateAssetMenu(fileName = "StoryData", menuName = "Scriptable Objects/StoryData")]
    public class StoryData : ScriptableObject
    {
        [SerializeField] private int chapter;
        [SerializeField] private string title;
        [SerializeField, TextArea(20, 40)] private string story;
        
        public int Chapter => chapter;
        public string Title => title;
        public string Story => story;
    }
}
