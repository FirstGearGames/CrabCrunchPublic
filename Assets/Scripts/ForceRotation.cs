using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceRotation : MonoBehaviour
{
    private enum RotationSpaces
    {
        World,
        Local
    }

    /// <summary>
    /// The space which to apply rotationt to.
    /// </summary>
    [Tooltip("The space which to apply rotationt to.")]
    [SerializeField]
    private RotationSpaces m_rotationSpace = RotationSpaces.World;

    /// <summary>
    /// Rotation to apply.
    /// </summary>
    [Tooltip("Rotation to apply.")]
    [SerializeField]
    private Vector3 m_rotation = Vector3.zero;

    /// <summary>
    /// How quickly to rotate.
    /// </summary>
    [Tooltip("How quickly to rotate.")]
    [SerializeField]
    private float m_rate = 10f;

    /// <summary>
    /// True to rotate based on rotational distance remaining. Causes rotations to be quicker over greater distances.
    /// </summary>
    [Tooltip("True to rotate based on rotational distance remaining. Causes rotations to be quicker over greater distances.")]
    [SerializeField]
    private bool m_includeDistance = true;

    /// <summary>
    /// Vector3 m_rotation gets converted on awake for easier calculations.
    /// </summary>
    private Quaternion m_targetRotation;

    private void Awake()
    {
        m_targetRotation = Quaternion.Euler(m_rotation);
    }

    private void Update()
    {
        RotateObject();
    }

    /// <summary>
    /// Rotates the object using preset settings.
    /// </summary>
    private void RotateObject()
    {

        float speed;
        Quaternion startRotation;

        //If rotating in world space.
        if (m_rotationSpace == RotationSpaces.World)
            startRotation = transform.rotation;
        else
            startRotation = transform.localRotation;

        //Normal rate.
        speed = m_rate * Time.deltaTime;

        //If including distance in rotation speed.
        if (m_includeDistance)
        {
            float remainingAngle = Mathf.Max(m_rate, Quaternion.Angle(startRotation, m_targetRotation));
            speed *= remainingAngle;
        }

        //If world rotation.
        if (m_rotationSpace == RotationSpaces.World)
            transform.rotation = Quaternion.RotateTowards(startRotation, m_targetRotation, speed);
        else
            transform.localRotation = Quaternion.RotateTowards(startRotation, m_targetRotation, speed);
    }
}
