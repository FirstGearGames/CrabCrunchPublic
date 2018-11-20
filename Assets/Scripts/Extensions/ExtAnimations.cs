using UnityEngine;

namespace FirstGearGames.Global.Structures
{

    /// <summary>
    /// Various utility classes related to animations.
    /// </summary>
    public static class Animatings
    {
        /// <summary>
        /// Provides an animation transition time based on distance between two input values.
        /// </summary>
        /// <param name="lastInputValue">Last input value.</param>
        /// <param name="currentInputValue">Current input value.</param>
        /// <param name="minimumTransitionTime">Minimum time for transition to take.</param>
        /// <param name="maximumTransitionTime">Maximum time for transition to take.</param>
        /// <returns></returns>
        public static float ReturnTransitionTime(float lastInputValue, float currentInputValue, float maximumInputRange, float minimumTransitionTime, float maximumTransitionTime)
        {
            float range = (Mathf.Abs(lastInputValue - currentInputValue) / (maximumInputRange * 2));
            return Mathf.Lerp(minimumTransitionTime, maximumTransitionTime, range);
        }
    }

}