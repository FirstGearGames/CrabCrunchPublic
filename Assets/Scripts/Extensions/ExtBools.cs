using UnityEngine;

namespace FirstGearGames.Global.Structures
{

    /// <summary>
    /// Various utility classes relating to bools.
    /// </summary>
    public static class Bools
    {

        /// <summary>
        /// Returns a random bool.
        /// </summary>
        /// <returns></returns>
        public static bool RandomBool()
        {
            if (Ints.RandomInclusiveRange(0, 1) == 0)
                return true;
            else
                return false;
        }
    }
}