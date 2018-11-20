using UnityEngine;

public class GizmoDraw : MonoBehaviour
{
    /// <summary>
    /// Color of the gizmo to draw.
    /// </summary>
    [Tooltip("Color of the gizmo to draw.")]
    [SerializeField]
    private Color32 m_gizmoColor = Color.red;
    /// <summary>
    /// Radius of gizmo drawn if not using a box gizmo.
    /// </summary>
    [Tooltip("Radius of gizmo drawn if not using a box gizmo.")]
    [SerializeField]
    private float m_circleRadius;
    /// <summary>
    /// Size of the box if using a box gizmo.
    /// </summary>
    [Tooltip("Size of the box if using a box gizmo.")]
    [SerializeField]
    private Vector2 m_boxSize;
    /// <summary>
    /// True to draw a box gizmo, false to draw a sphere.
    /// </summary>
    [Tooltip("True to draw a box gizmo, false to draw a sphere.")]
    [SerializeField]
    private bool m_useBox = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = m_gizmoColor;
        if (m_useBox)
            Gizmos.DrawWireCube(transform.position, new Vector3(m_boxSize.x, m_boxSize.y, 1f));
        else
            Gizmos.DrawWireSphere(transform.position, m_circleRadius);
    }

}
