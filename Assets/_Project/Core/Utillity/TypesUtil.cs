using UnityEngine;

using ProjectATLAS.Types;

namespace ProjectATLAS.Utility
{
    public static class TypesUtil
    {
        /// <summary> Converts a BoolEnum into a bool. </summary>
        public static bool AsBool(this BoolEnum boolEnum)
        {
            return boolEnum == BoolEnum.True;
        }
        
        /// <summary> Converts a BoolEnum into a bool. </summary>
        public static BoolEnum AsBoolEnum(this bool value)
        {
            return value ? BoolEnum.True : BoolEnum.False;
        }
    }
}
