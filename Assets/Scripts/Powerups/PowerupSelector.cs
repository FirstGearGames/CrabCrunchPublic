using UnityEngine;

public class PowerupSelector : MonoBehaviour
{

    #region Serialized.
    /// <summary>
    /// Use PowerupType.
    /// </summary>
    [Tooltip("Type of powerup for this selector.")]
    [SerializeField]
    private PowerupTypes m_powerupType;
    /// <summary>
    /// Type of powerup for this selector.
    /// </summary>
    public PowerupTypes PowerupType
    {
        get { return m_powerupType; }
    }
    #endregion

    #region Private.
    /// <summary>
    /// Board tile this powerup selector belongs to.
    /// </summary>
    private BoardTile m_boardTile = null;
    #endregion

    /// <summary>
    /// Initializes important data for this powerup selector.
    /// </summary>
    /// <param name="boardTile"></param>
    public void Initialize(BoardTile boardTile)
    {
        m_boardTile = boardTile;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when the mouse clicks the collider of this object.
    /// </summary>
    private void OnMouseDown()
    {
        StartCoroutine(MasterManager.PowerupManager.C_PerformPowerup(PowerupType, m_boardTile));
    }

}
