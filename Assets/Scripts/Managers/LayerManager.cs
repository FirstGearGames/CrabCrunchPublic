using UnityEngine;

public class LayerManager : MonoBehaviour
{

    #region Serialized.
    /// <summary>
    /// Use BoardTileLayer.
    /// </summary>
    [Tooltip("Layer used for board tiles.")]
    [SerializeField]
    private LayerMask m_boardTileLayer;
    /// <summary>
    /// Layer used for board tiles.
    /// </summary>
    public LayerMask BoardTileLayer
    {
        get { return m_boardTileLayer; }
    }
    /// <summary>
    /// Use TileChainLayer.
    /// </summary>
    [Tooltip("Layer used for tile chains.")]
    [SerializeField]
    private LayerMask m_tileChainLayer;
    /// <summary>
    /// Layer used for tile chains.
    /// </summary>
    public LayerMask TileChainLayer
    {
        get { return m_tileChainLayer; }
    }
    #endregion

}
