
using UnityEngine;

namespace FirstGearGames.Global.Structures
{

    /// <summary>
    /// Various utility classes relating to floats.
    /// </summary>
    public static class Floats
    {
        /// <summary>
        /// Returns negative-one, zero, or postive-one of a value instead of just negative-one or positive-one.
        /// </summary>
        /// <param name="value">Value to sign.</param>
        /// <returns>Precise sign.</returns>
        public static float PreciseSign(float value)
        {
            if (value == 0)
                return 0f;
            else
                return (Mathf.Sign(value));
        }

        /// <summary>
        /// Returns if a float is within a range.
        /// </summary>
        /// <param name="value">Value of float.</param>
        /// <param name="rangeMin">Minimum of range.</param>
        /// <param name="rangeMax">Maximum of range.</param>
        /// <returns></returns>
        public static bool InRange(float value, float rangeMin, float rangeMax)
        {
            return (value >= rangeMin && value <= rangeMax);
        }

        /// <summary>
        /// Returns zero if a floats absolute value is at or below minimumValue. Otherwise returns value.
        /// </summary>
        /// <param name="value">Current value of float.</param>
        /// <param name="minimumValue">Threshold which float must exceed.</param>
        /// <returns></returns>
        public static float MinimumThreshold(float value, float minimumValue)
        {
            if (Mathf.Abs(value) <= minimumValue)
                return 0f;
            else
                return value;
        }

        public static bool ValuesMatch(float valueA, float valueB, float threshold)
        {
            return (Mathf.Abs(valueA - valueB) <= threshold);
        }
    }

}