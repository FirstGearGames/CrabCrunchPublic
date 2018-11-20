using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Public.
    /// <summary>
    /// Returns the orthographic width, not the same thing as orthographicSize.
    /// </summary>
    public float OrthographicWidth
    {
        get
        {
            return (OrthographicHeight * MainCamera.aspect);
        }
    }
    /// <summary>
    /// Returns the orthographic height.
    /// </summary>
    /// <returns></returns>
    public float OrthographicHeight
    {
        get
        {
            return (MainCamera.orthographicSize * 2f);
        }
    }
    /// <summary>
    /// Returns half of the orthographic height.
    /// </summary>
    /// <returns></returns>
    public float HalfOrthographicHeight
    {
        get
        {
            return (OrthographicHeight / 2f);
        }
    }
    #endregion

    #region Serialized.
    /// <summary>
    /// Use MainCamera.
    /// </summary>
    [Tooltip("Reference to the main camera for the scene.")]
    [SerializeField]
    private Camera m_mainCamera;
    /// <summary>
    /// Reference to the main camera for the scene.
    /// </summary>
    public Camera MainCamera
    {
        get { return m_mainCamera; }
    }
    #endregion

    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Initializes the camera resolution. Only used for testing. //TODO/
    /// </summary>
    private void Initialize()
    {
        //if (!Application.isEditor)
        //    Screen.SetResolution(360, 640, false);

    }
}
