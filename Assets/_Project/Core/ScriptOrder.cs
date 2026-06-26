using System;
using UnityEngine;

namespace ProjectATLAS
{
    /// <summary> Set up script execution order here with matching class name. </summary>
    public static class ScriptOrder
    {
        public const int GameManager    = -50;
        public const int OtherManager   = GameManager + 1;
    }
}
