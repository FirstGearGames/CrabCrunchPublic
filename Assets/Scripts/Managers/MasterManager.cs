using UnityEngine;

public class MasterManager : MonoBehaviour
{
    #region Public.
    [HideInInspector]
    public static MasterManager Instance;
    #endregion

    #region Serialized.
    /// <summary>
    /// Use FishManager.
    /// </summary>
    [Tooltip("Reference to the FishManager, within sub managers.")]
    [SerializeField]
    private FishManager m_fishManager;
    /// <summary>
    /// Reference to the FishManager, within sub managers.
    /// </summary>
    public static FishManager FishManager
    {
        get { return Instance.m_fishManager; }
    }
    /// <summary>
    /// Use TileManager.
    /// </summary>
    [Tooltip("Reference to the TileManager, within sub managers.")]
    [SerializeField]
    private TileManager m_tileManager;
    /// <summary>
    /// Reference to the TileManager, within sub managers.
    /// </summary>
    public static TileManager TileManager
    {
        get { return Instance.m_tileManager; }
    }
    /// <summary>
    /// Use BoardManager.
    /// </summary>
    [Tooltip("Reference to the BoardManager, within sub managers.")]
    [SerializeField]
    private BoardManager m_boardManager;
    /// <summary>
    /// Reference to the BoardManager, within sub managers.
    /// </summary>
    public static BoardManager BoardManager
    {
        get { return Instance.m_boardManager; }
    }
    /// <summary>
    /// Use LayerManager.
    /// </summary>
    [Tooltip("Reference to the LayerManager, within sub managers.")]
    [SerializeField]
    private LayerManager m_layerManager;
    /// <summary>
    /// Reference to the LayerManager, within sub managers.
    /// </summary>
    public static LayerManager LayerManager
    {
        get { return Instance.m_layerManager; }
    }
    /// <summary>
    /// Use GameManager.
    /// </summary>
    [Tooltip("Reference to the GameManager, within sub managers.")]
    [SerializeField]
    private GameManager m_gameManager;
    /// <summary>
    /// Reference to the GameManager, within sub managers.
    /// </summary>
    public static GameManager GameManager
    {
        get { return Instance.m_gameManager; }
    }
    /// <summary>
    /// Use EffeectsManager
    /// </summary>
    [Tooltip("Reference to the EffectsManager, within sub managers.")]
    [SerializeField]
    private EffectsManager m_effectsManager;
    /// <summary>
    /// Reference to the EffectsManager, within sub managers.
    /// </summary>
    public static EffectsManager EffectsManager
    {
        get { return Instance.m_effectsManager; }
    }
    /// <summary>
    /// Use CameraManager.
    /// </summary>
    [Tooltip("Reference to the CameraManger, within sub managers.")]
    [SerializeField]
    private CameraManager m_cameraManager;
    /// <summary>
    /// Reference to the CameraManger, within sub managers.
    /// </summary>
    public static CameraManager CameraManager
    {
        get { return Instance.m_cameraManager; }
    }
    /// <summary>
    /// Use CanvasManager.
    /// </summary>
    [Tooltip("Reference to the CanvasManager, within sub managers.")]
    [SerializeField]
    private CanvasManager m_canvasManager;
    /// <summary>
    /// Reference to the CanvasManager, within sub managers.
    /// </summary>
    public static CanvasManager CanvasManager
    {
        get { return Instance.m_canvasManager; }
    }
    /// <summary>
    /// Use PowerupManager.
    /// </summary>
    [Tooltip("Reference to the PowerupManager, within sub managers.")]
    [SerializeField]
    private PowerupManager m_powerupManager;
    /// <summary>
    /// Reference to the PowerupManager, within sub managers.
    /// </summary>
    public static PowerupManager PowerupManager
    {
        get { return Instance.m_powerupManager; }
    }
    /// <summary>
    /// Use SaveManager.
    /// </summary>
    [Tooltip("Reference to the SaveManager, within sub managers.")]
    [SerializeField]
    private SaveManager m_saveManager;
    /// <summary>
    /// Reference to the SaveManager, within sub managers.
    /// </summary>
    public static SaveManager SaveManager
    {
        get { return Instance.m_saveManager; }
    }
    /// <summary>
    /// Use AdManager.
    /// </summary>
    [Tooltip("Reference to the AdManager, within sub managers.")]
    [SerializeField]
    private AdManager m_adManager;
    /// <summary>
    /// Reference to the AdManager, within sub managers.
    /// </summary>
    public static AdManager AdManager
    {
        get { return Instance.m_adManager; }
    }
    #endregion

    private void Awake()
    {
        Instance = this;

        //Show the title screen.
        CanvasManager.TitleCanvas.Show();
    }

}
