using UnityEngine;
using CustomInspector;

namespace ProjectATLAS.Avatar
{
    [CreateAssetMenu(fileName = "AvatarData", menuName = "Scriptable Objects/AvatarData")]
    public class AvatarProfile : ScriptableObject
    {
        public int characterIndex;
        public string characterName;
        [Preview] public Sprite characterSprite;
        [Preview] public Sprite characterHead;
    }
}
