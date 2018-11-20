using UnityEngine;

namespace FirstGearGames.Global.Structures
{


    public static class Layers
    {
        /// <summary>
        /// Checks if an int layer is contained within a LayerMask.
        /// </summary>
        /// <param name="layer">Layer mask may contain.</param>
        /// <param name="mask">LayerMask to check for layer within.</param>
        /// <returns>True if layer is within mask.</returns>
        public static bool MaskContainsLayer(int layer, LayerMask mask)
        {
            if (((1 << layer) & mask) != 0)
                return true;
            else
                return false;
        }

    }

}