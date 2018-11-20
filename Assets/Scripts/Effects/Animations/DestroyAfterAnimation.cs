using UnityEngine;


[RequireComponent(typeof(Animator))]
public class DestroyAfterAnimation : MonoBehaviour
{
    #region Serialized.
    /// <summary>
    /// Animation clip which is considered to be played on start.
    /// </summary>
    [Tooltip("Animation clip which is considered to be played on start.")]
    [SerializeField]
    private AnimationClip m_playedClip;
    #endregion

    #region Private.
    /// <summary>
    /// Once Time.time passes this value this scripts object is destroyed.
    /// </summary>
    private float m_destroyTime = 0f;
    #endregion

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        CheckDestroy();
    }

    /// <summary>
    /// Initializes script.
    /// </summary>
	private void Initialize()
    {
        Animator animator = GetComponent<Animator>();
        //If animator isn't found.
        if (animator == null)
        {
            Debug.LogError(Debugs.ReturnErrorString(transform, "-> Initialize -> Animator component not found."));
            Destroy(gameObject);
        }
        //Animator found.
        else
        {
            //Set destroy time and make sure animator is enabled.
            m_destroyTime = Time.time + m_playedClip.length;
            animator.enabled = true;
        }
    }

    /// <summary>
    /// Checks if destroy time has passed.
    /// </summary>
    private void CheckDestroy()
    {
        if (Time.time > m_destroyTime)
            Destroy(gameObject);
    }
}
