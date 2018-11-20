using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardTile : MonoBehaviour
{

    #region Public.
    /// <summary>
    /// Position where tile resides in the multi-dimensional grid, within TilesBoard.
    /// </summary>
    [HideInInspector]
    public GridPosition GridPosition;
    /// <summary>
    /// Position where tile resides in the spiral grid.
    /// </summary>
    [HideInInspector]
    public VirtualPosition VirtualPosition;
    /// <summary>
    /// Neighbor board tiles.
    /// </summary>
    [HideInInspector]
    public List<BoardTile> Neighbors = new List<BoardTile>();
    /// <summary>
    /// Use TileValue.
    /// </summary>
    private TileValues m_tileValue = TileValues.Empty;
    /// <summary>
    /// The current value of this board tile.
    /// </summary>
    public TileValues TileValue
    {
        get { return m_tileValue; }
        set
        {
            m_tileValue = value;
            PaintTile();
        }
    }
    #endregion

    #region Serialized.
    /// <summary>
    /// Reference to the sprite renderer which displays the value of the tile on this board piece.
    /// </summary>
    [SerializeField]
    private SpriteRenderer m_tileValueSpriteRenderer;
    /// <summary>
    /// Child object to enable when tile is highlighted.
    /// </summary>
    [Tooltip("Child object to enable when tile is highlighted.")]
    [SerializeField]
    private GameObject m_selectedHighlight;
    /// <summary>
    /// TextMeshPro component used to display the tile value text.
    /// </summary>
    [Tooltip("TextMeshPro component used to display the tile value text.")]
    [SerializeField]
    private TextMeshPro m_tileValueText;
    #endregion

    #region Private.
    /// <summary>
    /// PowerupSelector(s) which are used to handle selecting powerups on this board tile.
    /// </summary>
    private PowerupSelector[] m_powerupSelectors;
    #endregion

    private void Awake()
    {
        FindPowerupSelectors();
    }

    /// <summary>
    /// Finds and assigns all powerup selectors, as well initializes them.
    /// </summary>
    private void FindPowerupSelectors()
    {
        m_powerupSelectors = GetComponentsInChildren<PowerupSelector>();
        for (int i = 0; i < m_powerupSelectors.Length; i++)
        {
            m_powerupSelectors[i].Initialize(this);
        }
    }

    /// <summary>
    /// Hides all powerup selectors from view.
    /// </summary>
    public void HidePowerupSelectors()
    {
        for (int i = 0; i < m_powerupSelectors.Length; i++)
        {
            m_powerupSelectors[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Shows the powerup selector for a specified powerup type.
    /// </summary>
    /// <param name="type"></param>
    public void ShowPowerupSelector(PowerupTypes type)
    {
        /* Currently only tiles with values support showing powerups.
         * Exit method if the tile has no value. */
        if (TileValue == TileValues.Blocked || TileValue == TileValues.Empty)
            return;

        //If powerup type is plus one.
        if (type == PowerupTypes.PlusOne)
        {
            //If tile value is 7 don't show selector as it cannot be used with plus one.
            if (TileValue == TileValues.Seven)
                return;
        }

        for (int i = 0; i < m_powerupSelectors.Length; i++)
        {
            //If power up type matches.
            if (m_powerupSelectors[i].PowerupType == type)
            {
                m_powerupSelectors[i].gameObject.SetActive(true);
                return;
            }
        }
    }

    /// <summary>
    /// Sets the highlight state on the board tile.
    /// </summary>
    /// <param name="highlighted"></param>
    public void SwitchHighlight(bool highlighted)
    {
        m_selectedHighlight.SetActive(highlighted);
    }

    /// <summary>
    /// Sets a tiles text and colors it accordingly.
    /// </summary>
    private void PaintTile()
    {
        int tileValue = (int)TileValue;

        //If tile value should be hidden because of empty or blocked.
        if (TileValue <= 0)
        {
            //Simply just hide sprite renderer.
            m_tileValueSpriteRenderer.gameObject.SetActive(false);
        }
        //Tile value should be visible.
        else
        {
            //Set tile color.
            m_tileValueSpriteRenderer.color = MasterManager.TileManager.ReturnTileColor(tileValue);
            //Set textmeshpro text and color.
            m_tileValueText.text = tileValue.ToString();
            m_tileValueText.color = MasterManager.TileManager.ReturnTextColor(tileValue);

            //Set tile value renderer object active.
            m_tileValueSpriteRenderer.gameObject.SetActive(true);
        }
    }

}
