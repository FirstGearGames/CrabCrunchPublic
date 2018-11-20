using TMPro;
using UnityEngine;

public class NumberedTile : MonoBehaviour
{
    /// <summary>
    /// TextMeshPro component reference for this numbered tile.
    /// </summary>
    [Tooltip("TextMeshPro component reference for this numbered tile.")]
    [SerializeField]
    private TextMeshPro m_textMeshPro;

    /// <summary>
    /// Sprite renderer component reference.
    /// </summary>
    [Tooltip("Sprite renderer component reference.")]
    [SerializeField]
    private SpriteRenderer m_spriteRenderer;

    /// <summary>
    /// Object to be enabled if this tile becomes the deciding tile.
    /// </summary>
    [Tooltip("Object to be enabled if this tile becomes the deciding tile.")]
    [SerializeField]
    private GameObject m_decidingTileObject;

    /// <summary>
    /// Use TileValue.
    /// </summary>
    [Tooltip("Current value of the tile.")]
    [SerializeField]
    private TileValues m_tileValue;
    /// <summary>
    /// Current value of the tile.
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

    /// <summary>
    /// Use LastTracedBoardTile.
    /// </summary>
    private GameObject m_lastTracedBoardTile = null;
    /// <summary>
    /// If not null then this tile is hovering over a board tile.
    /// </summary>
    public GameObject LastTracedBoardTile
    {
        get { return m_lastTracedBoardTile; }
        private set { m_lastTracedBoardTile = value; }
    }
    /// <summary>
    /// Tile chain collection this numbered tile belongs to.
    /// </summary>
    [HideInInspector]
    public TileChain TileChain;

    /// <summary>
    /// Traces for board tiles which sit underneath this tile.
    /// </summary>
    public void TraceForBoardTile()
    {
        float castDistance = 50f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(0f, 0f), castDistance, MasterManager.LayerManager.BoardTileLayer, transform.position.z - 10f, transform.position.z + castDistance);
        //If a hit is successful.
        if (hit.collider != null)
        {
            //If same hit collider as last time.
            if (hit.collider.gameObject == m_lastTracedBoardTile)
            {
                return;
            }
            //Not same hit collider.
            else
            {
                RemoveBoardTileHighlight();
            }

            //Set last hit board tile.
            LastTracedBoardTile = hit.collider.gameObject;
            //Highlight board tile.
            TileChain.TileChainCollection.SetBoardTileHighlight(LastTracedBoardTile, true);
        }
        //Nothing hit.
        else
        {
            RemoveBoardTileHighlight();
        }
    }

    /// <summary>
    /// Sets a tiles text and colors it accordingly.
    /// </summary>
    private void PaintTile()
    {
        int tileValue = (int)TileValue;
        //Set text value and color.
        m_textMeshPro.text = tileValue.ToString();
        m_textMeshPro.color = MasterManager.TileManager.ReturnTextColor(tileValue);
        //Set tile color.
        m_spriteRenderer.color = MasterManager.TileManager.ReturnTileColor(tileValue);
    }

    /// <summary>
    /// Tells the tile chain collection to remove board tile highlight for this numbered tile.
    /// </summary>
    private void RemoveBoardTileHighlight()
    {
        TileChain.TileChainCollection.SetBoardTileHighlight(LastTracedBoardTile, false);
        //Unset last board tile hit.
        LastTracedBoardTile = null;
    }

    /// <summary>
    /// Sets the deciding tile state of this tile.
    /// </summary>
    /// <param name="decidingTile">True if is deciding tile.</param>
    public void SetDecidingTile(bool decidingTile)
    {
        m_decidingTileObject.SetActive(decidingTile);
    }
}
