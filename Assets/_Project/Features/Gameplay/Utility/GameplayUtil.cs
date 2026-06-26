using UnityEngine;

namespace ProjectATLAS.Gameplay
{
    public static class GameplayUtil
    {
        /// <summary> Returns Easy = 1, medium = 2, hard = 3. </summary>
        public static float AsMultiplier(this Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => 1f,
                Difficulty.Medium => 2f,
                Difficulty.Hard => 3f,
                _ => 1f
            };
        }
    }
}
