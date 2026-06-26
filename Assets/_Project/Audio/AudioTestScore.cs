using TMPro;
using UnityEngine;

namespace ProjectATLAS
{
    public class AudioTestScore : MonoBehaviour
    {
        public int score;
        public AudioClip winSFX;
        public TMP_Text text;
        
        public void AddScore()
        {
            score++;
            
            if (text) text.text = $"{score}";
            
            if (score > 10)
            {
                AudioManager.Instance.PlaySFX(winSFX);
            }
        }
    }
}
