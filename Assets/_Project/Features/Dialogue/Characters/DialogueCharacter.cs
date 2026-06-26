using UnityEngine;

namespace ProjectATLAS.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueCharacter", menuName = "Scriptable Objects/Dialogue Character")]
    public class DialogueCharacter : ScriptableObject
    {
        public new string name;
        public Sprite image;
        public float pitch;
    }
}
