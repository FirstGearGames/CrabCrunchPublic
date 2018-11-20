using UnityEngine;

namespace FirstGearGames.Global.Structures
{

    /// <summary>
    /// Various utility classes related to colliders.
    /// </summary>
    public static class Colliders
    {

        /// <summary>
        /// Get a collider's center offset if of supported collider types.
        /// </summary>
        /// <param name="collider">Collider to get center offset of.</param>
        /// <returns>Center offset.</returns>
        public static Vector3 CenterOffset(Collider collider)
        {
            object colliderType = collider.GetType();
            /* Check if collider is of various types 
             * and if of a known type return it's center.
             * Capsule collider will be the most popular
             * collider used when calling this function
             * so it is checked first. */
            if (colliderType == typeof(CapsuleCollider))
            {
                CapsuleCollider cc = (CapsuleCollider)collider;
                return cc.center;
            }
            else if (colliderType == typeof(BoxCollider))
            {
                BoxCollider bc = (BoxCollider)collider;
                return bc.center;
            }
            else if (colliderType == typeof(SphereCollider))
            {
                SphereCollider sc = (SphereCollider)collider;
                return sc.center;
            }
            else
            {
                Debug.LogWarning("Colliders -> CenterOffset -> Unhandled collider type. String value is " + colliderType.ToString());
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Get a rough estimate of a colliders half size.
        /// </summary>
        /// <param name="collider">Collider to check.</param>
        /// <returns></returns>
        public static Vector3 HalfSize(Collider collider)
        {
            object colliderType = collider.GetType();
            /* Check if collider is of various types 
             * and if of a known type return it's center.
             * Capsule collider will be the most popular
             * collider used when calling this function
             * so it is checked first. */
            if (colliderType == typeof(CapsuleCollider))
            {
                CapsuleCollider cc = (CapsuleCollider)collider;
                return new Vector3(cc.radius, cc.height / 2f, cc.radius);
            }
            else if (colliderType == typeof(BoxCollider))
            {
                BoxCollider bc = (BoxCollider)collider;
                return (bc.size / 2f);
            }
            else if (colliderType == typeof(SphereCollider))
            {
                SphereCollider sc = (SphereCollider)collider;
                return new Vector3(sc.radius, sc.radius, sc.radius);
            }
            else
            {
                Debug.LogWarning("Colliders -> HalfSize -> Unhandled collider type. String value is " + colliderType.ToString());
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Get a colliders volume.
        /// </summary>
        /// <param name="collider">Collider to check.</param>
        /// <returns></returns>
        public static float RelevantVolume(Collider collider)
        {
            object colliderType = collider.GetType();
            /* Check if collider is of various types 
             * and if of a known type return it's center.
             * Capsule collider will be the most popular
             * collider used when calling this function
             * so it is checked first. */
            //if (colliderType == typeof(CapsuleCollider))
            //{
            //    CapsuleCollider cc = (CapsuleCollider)collider;
            //    return (1.33f * 3.14f * Mathf.Pow(cc.radius + 1f, cc.radius + 1f) * (cc.height + 1f));
            //}
            //else if (colliderType == typeof(BoxCollider))
            //{
            //    BoxCollider bc = (BoxCollider)collider;
            //    return ((bc.size.z + 1f) * (bc.size.y + 1f) * (bc.size.x + 1f));
            //}
            //else if (colliderType == typeof(SphereCollider))
            //{
            //    SphereCollider sc = (SphereCollider)collider;
            //    return (1.33f * 3.14f * Mathf.Pow((sc.radius + 1f), (sc.radius + 1f)));
            //}
            if (colliderType == typeof(CapsuleCollider))
            {
                CapsuleCollider cc = (CapsuleCollider)collider;
                return (cc.bounds.extents.x * cc.bounds.extents.y * cc.bounds.extents.z);
            }
            else if (colliderType == typeof(BoxCollider))
            {
                BoxCollider bc = (BoxCollider)collider;
                return (bc.bounds.extents.x * bc.bounds.extents.y * bc.bounds.extents.z);
            }
            else if (colliderType == typeof(SphereCollider))
            {
                SphereCollider sc = (SphereCollider)collider;
                return (sc.bounds.extents.x * sc.bounds.extents.y * sc.bounds.extents.z);
            }
            else
            {
                Debug.LogWarning("Colliders -> Volume -> Unhandled collider type. String value is " + colliderType.ToString());
                return 0f;
            }
        }
    }

}