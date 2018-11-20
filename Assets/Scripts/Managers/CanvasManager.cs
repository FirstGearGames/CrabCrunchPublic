using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    #region Serialized.
    /// <summary>
    /// Use LevelCanvas.
    /// </summary>
    [Tooltip("Reference to the LevelCanvas script in this objects children.")]
    [SerializeField]
    private LevelCanvas m_levelCanvas;
    /// <summary>
    /// Reference to the LevelCanvas script in this objects children.
    /// </summary>
    public LevelCanvas LevelCanvas
    {
        get { return m_levelCanvas; }
    }
    /// <summary>
    /// Use LoseCanvas.
    /// </summary>
    [Tooltip("Reference to the LoseCanvas script in this objects children.")]
    [SerializeField]
    private LoseCanvas m_loseCanvas;
    /// <summary>
    /// Reference to the LoseCanvas script in this objects children.
    /// </summary>
    public LoseCanvas LoseCanvas
    {
        get { return m_loseCanvas; }
    }
    /// <summary>
    /// Use TitleCanvas.
    /// </summary>
    [Tooltip("Refernece to the TitleCanvas script in this objects children.")]
    [SerializeField]
    private TitleCanvas m_titleCanvas;
    /// <summary>
    /// Refernece to the TitleCanvas script in this objects children.
    /// </summary>
    public TitleCanvas TitleCanvas
    {
        get { return m_titleCanvas; }
    }
    /// <summary>
    /// Use MenuCanvas.
    /// </summary>
    [Tooltip("Reference to the MenuCanvas script in this objects children.")]
    [SerializeField]
    private MenuCanvas m_menuCanvas;
    /// <summary>
    /// Reference to the MenuCanvas script in this objects children.
    /// </summary>
    public MenuCanvas MenuCanvas
    {
        get { return m_menuCanvas; }
    }
    #endregion
}
