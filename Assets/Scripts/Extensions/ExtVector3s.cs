
using UnityEngine;

namespace FirstGearGames.Global.Structures
{

    public static class Vector3s
    {
        /// <summary>
        /// Returns square distance between two positions.
        /// </summary>
        /// <param name="startPosition">Start position.</param>
        /// <param name="endPosition">End position.</param>
        /// <returns>Distance between two positions.</returns>
        public static float SqrDistance(Vector3 startPosition, Vector3 endPosition)
        {
            return (endPosition - startPosition).sqrMagnitude;
        }

        /// <summary>
        /// Calculates the linear parameter t that produces the interpolant value within the range [a, b].
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
        {
            Vector3 AB = b - a;
            Vector3 AV = value - a;
            return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
        }

        /// <summary>
        /// Returns if two vectors are the same value so long as the difference total of all axis' are within below the tolerance level.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <param name="tolerance">Tolerance level. Must be a positive number.</param>
        /// <returns>True if both near each other within the tolerance level.</returns>
        public static bool ValuesMatch(Vector3 a, Vector3 b, float tolerance)
        {
            float differenceSum = Mathf.Abs(a.x - b.x)
                + Mathf.Abs(a.y - b.y)
                 + Mathf.Abs(a.z - b.z);

            return (differenceSum <= tolerance);
        }
    }

}