using System.Collections.Generic;
using UnityEngine;

public class TileChain : MonoBehaviour
{

    #region Public.
    /// <summary>
    /// List of numbered tiles for this tile chain.
    /// </summary>
    [HideInInspector]
    public List<NumberedTile> NumberedTiles = new List<NumberedTile>();
    /// <summary>
    /// Tile chain collection this numbered tile belongs to.
    /// </summary>
    [HideInInspector]
    public TileChainCollection TileChainCollection;
    /// <summary>
    /// Becomes true if tile chain is rotating.
    /// </summary>
    [HideInInspector]
    public bool Rotating;
    #endregion

    #region Serialized.
    /// <summary>
    /// Use TileChainType.
    /// </summary>
    [Tooltip("Type of tile chain.")]
    [SerializeField]
    private TileChainTypes m_tileChainType;
    /// <summary>
    /// Type of tile chain.
    /// </summary>
    public TileChainTypes TileChainType
    {
        get { return m_tileChainType; }
    }
    /// <summary>
    /// Use ChainedTilePositions.
    /// </summary>
    [Tooltip("Gameobjects representing where tiles would spawn for the tile chain. Recommended to place in order.")]
    [SerializeField]
    private Transform[] m_chainedTilePositions = new Transform[0];
    /// <summary>
    /// Gameobjects representing where tiles would spawn for the tile chain. Recommended to place in order.
    /// </summary>
    public Transform[] ChainedTilePositions
    {
        get { return m_chainedTilePositions; }
    }
    #endregion

    #region Private.
    /// <summary>
    /// How quickly to rotate. Set through SetupRotate.
    /// </summary>
    private float m_rotateRate = 1f;
    /// <summary>
    /// Target to rotate towards.
    /// </summary>
    private Quaternion m_targetRotation;
    #endregion

    private void Update()
    {
        Rotate();
    }

    /// <summary>
    /// Sets up information about how a tile chain should be rotated and enables Rotating.
    /// </summary>
    /// <param name="degrees"></param>
    /// <param name="rate"></param>
    public void SetupRotate(float degrees, float rate)
    {
        if (Rotating)
            return;

        //Set as rotating so no more rotates can be called until this completes.
        Rotating = true;

        //Set rate.
        m_rotateRate = rate;
        //Set the target rotation.
        m_targetRotation = transform.rotation * Quaternion.Euler(new Vector3(0f, 0f, degrees));
    }

    /// <summary>
    /// Rotates the tile chain over time. Used to animate a tile rotation.
    /// </summary>
    private void Rotate()
    {
        if (!Rotating)
            return;

        //Rotate towards.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, m_targetRotation, m_rotateRate * Time.deltaTime);

        //If at goal rotation.
        if (transform.rotation == m_targetRotation)
            Rotating = false;
    }

    /// <summary>
    /// Called when the tile chain is interacted with.
    /// </summary>
    private void OnMouseDown()
    {
        TileChainCollection.TileChainClicked();
    }
}
