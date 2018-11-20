using FirstGearGames.Global.Structures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChainCollection : MonoBehaviour
{
    #region Public.
    /// <summary>
    /// Use TileChainHeld.
    /// </summary>
    private bool m_tileChainHeld = false;
    /// <summary>
    /// True if tile chain collider is being held.
    /// </summary>
    public bool TileChainHeld
    {
        get { return m_tileChainHeld; }
        private set { m_tileChainHeld = value; }
    }
    /// <summary>
    /// Use CurrentTileChain.
    /// </summary>
    private TileChain m_currentTileChain;
    /// <summary>
    /// Currently used tile chain.
    /// </summary>
    public TileChain CurrentTileChain
    {
        get { return m_currentTileChain; }
        private set { m_currentTileChain = value; }
    }
    #endregion

    #region Serialized.
    /// <summary>
    /// Tile chains for this collection.
    /// </summary>
    [Tooltip("Tile chains for this collection.")]
    [SerializeField]
    private List<TileChain> m_tileChains = new List<TileChain>();
    /// <summary>
    /// Numbered tile prefab object. The object which contains the numbered tile script.
    /// </summary>
    [Tooltip("Numbered tile prefab object. The object which contains the numbered tile script.")]
    [SerializeField]
    private GameObject m_numberedTilePrefab;
    /// <summary>
    /// Use RotateRate.
    /// </summary>
    [Tooltip("How quickly to rotate when chain is pressed.")]
    [SerializeField]
    private float m_rotateRate = 250f;
    /// <summary>
    /// How quickly to rotate when chain is pressed.
    /// </summary>
    public float RotateRate
    {
        get { return m_rotateRate; }
    }
    /// <summary>
    /// Played when the tile chain is rotated.
    /// </summary>
    [Tooltip("Played when the tile chain is rotated.")]
    [SerializeField]
    private AudioSource m_rotateAudio;
    /// <summary>
    /// Played when the tile chain is picked up.
    /// </summary>
    [Tooltip("Played when the tile chain is picked up.")]
    [SerializeField]
    private AudioSource m_pickupAudio;
    /// <summary>
    /// Played when the tile chain is dropped.
    /// </summary>
    [Tooltip("Played when the tile chain is dropped.")]
    [SerializeField]
    private AudioSource m_dropAudio;
    /// <summary>
    /// Played when the tile is placed.
    /// </summary>
    [Tooltip("Played when the tile is placed.")]
    [SerializeField]
    private AudioSource m_placedAudio;
    #endregion

    #region Private.
    /// <summary>
    /// Becomes true when the tile chain is lifted above it's origin at any point while being held.
    /// </summary>
    private bool m_tileChainPickedUp = false;
    /// <summary>
    /// Holds the Y value of the cursor in screen space where holding the tile begun.
    /// </summary>
    private Vector3 m_mouseDownPosition = Vector3.zero;
    #endregion


    private void Start()
    {
        LoadAudioSettings();
    }

    /// <summary>
    /// Loads audio settings into the audio source component.
    /// </summary>
    public void LoadAudioSettings()
    {
        //Since all audio sources on this are sound effects this is okay.
        AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].volume = MasterManager.GameManager.Settings.SoundEffectsVolume;
        }
    }

    private void Update()
    {
        //Only perform checks if current tile chain isn't null.
        if (CurrentTileChain != null)
        {
            CheckReleaseTileChain();
            CheckMoveTile();
            TraceForBoardTiles();
        }
    }

    /// <summary>
    /// Initializes the tile chain collection by spawning tiles and setting their values.
    /// </summary>
    /// <param name="chainType">Type of tile chain to setup.</param>
    /// <param name="tileValues">Values in order for chain.</param>
    public void Initialize(TileChainTypes chainType, TileValues[] tileValues)
    {
        int index = m_tileChains.FindIndex(x => x.TileChainType == chainType);

        //Chain type not found or supported.
        if (index == -1)
        {
            Debug.LogError(Debugs.ReturnErrorString(transform, " -> TileChainCollection -> Initialize -> Chain type not found."));
            return;
        }
        //Set tile chain that will be used.
        TileChain tileChain = m_tileChains[index];
        //List which will hold spawned number tiles.
        List<NumberedTile> numberedTiles = new List<NumberedTile>();

        //If tile values and chained positions do not match up.
        if (tileChain.ChainedTilePositions.Length != tileValues.Length)
        {
            Debug.LogError(Debugs.ReturnErrorString(transform, " -> TileChainCollection -> Initialize -> Chained tile positions and tile values length do not match."));
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < tileChain.ChainedTilePositions.Length; i++)
        {
            Transform positionTransform = tileChain.ChainedTilePositions[i];

            GameObject numberedTileObj = Instantiate(m_numberedTilePrefab, positionTransform.position, Quaternion.identity, positionTransform);
            //Find numbered tile script on spawned tile.
            NumberedTile numberedTile = numberedTileObj.GetComponent<NumberedTile>();

            //If not found.
            if (numberedTile == null)
            {
                Debug.LogError(Debugs.ReturnErrorString(transform, " -> TileChainCollection -> Initialize -> NumberedTile script not found."));
                //Destroy numbered tile obj and this object so no more tiles are attempted.
                Destroy(numberedTileObj);
                Destroy(gameObject);
                return;
            }

            //Set tile value to that of this index.
            numberedTile.TileValue = tileValues[i];
            //Set the tile chain reference.
            numberedTile.TileChain = tileChain;
            //Add to list.
            numberedTiles.Add(numberedTile);
        }

        //Set current tile chain.
        CurrentTileChain = tileChain;
        //Set tile chain's collection reference to this.
        tileChain.TileChainCollection = this;
        //Set number list reference.
        tileChain.NumberedTiles = numberedTiles;

        //Add the deciding tile effect.
        CheckSetDecidingTile();

        //Enable the tile chain.
        tileChain.gameObject.SetActive(true);
    }

    /// <summary>
    /// Checks if there should be a deciding tile and if so sets it.
    /// </summary>
    private void CheckSetDecidingTile()
    {
        //Not more than 1 tile, impossible for there to be a deciding tile.
        if (CurrentTileChain.NumberedTiles.Count < 2)
            return;

        //Go through each numbered tile skipping the last.
        for (int i = 0; i < (CurrentTileChain.NumberedTiles.Count - 1); i++)
        {
            //If tiles match there needs to be a deciding tile.
            if (CurrentTileChain.NumberedTiles[i].TileValue == CurrentTileChain.NumberedTiles[i + 1].TileValue)
            {
                CurrentTileChain.NumberedTiles[i].SetDecidingTile(true);
                break;
            }
        }
    }

    /// <summary>
    /// Checks if the tile chain needs to be released from being held.
    /// </summary>
    private void CheckReleaseTileChain()
    {
        //If tile chain isn't held.
        if (!TileChainHeld)
            return;

        //If mouse 0 isn't held.
        if (!Input.GetKey(KeyCode.Mouse0))
            TileChainReleased();
    }

    /// <summary>
    /// Called from TileChain when the collider is interacted with.
    /// </summary>
    public void TileChainClicked()
    {
        //If powerup selectors are shown do nothing.
        if (MasterManager.PowerupManager.PowerupSelectorsVisible)
            return;
        //If game input is blocked.
        if (MasterManager.GameManager.States.GameInputBlocked)
            return;

        Vector3 mouseWorldPos = MasterManager.CameraManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        //Start trace a little shallower where the tile chain is expected.
        float castDistance = 50f;
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, new Vector2(0f, 0f), castDistance, MasterManager.LayerManager.TileChainLayer, transform.position.z - 10f, transform.position.z + castDistance);
        //If raycast hits the tile chain.

        if (hit.collider != null)
        {
            m_mouseDownPosition = mouseWorldPos;
            TileChainHeld = true;
        }
    }

    /// <summary>
    /// Performs actions when a tile chain is released.
    /// </summary>
    private void TileChainReleased()
    {
        //Exit if tile chain isn't held, meaning already released.
        if (!TileChainHeld)
            return;

        //Becomes true if tile should be rotated.
        bool rotateTile = (!m_tileChainPickedUp && CurrentTileChain.NumberedTiles.Count > 1);

        //Unset held and moved state.
        TileChainHeld = false;
        m_tileChainPickedUp = false;

        /* If tile chain had not moved and if there is more than one numbered tile
        * rotate it by a set amount of degrees. */
        if (rotateTile)
        {
            RotateTiles();
            return;
        }

        //Get a list of highlighted board tiles.
        List<BoardTile> highlightedBoardTiles = ReturnHighlightedBoardTiles();
        //Remove the highlight on all board tiles. 
        SetBoardTileHighlight(highlightedBoardTiles, false);
        //Try to place the tiles on the board.
        TryPlaceTiles(highlightedBoardTiles);
    }

    /// <summary>
    /// Rotates the tiles.
    /// </summary>
    private void RotateTiles()
    {
        //Default rotation value.
        float rotationValue = 60f;

        /* Determine which way to rotate. */
        //Get the current mouse world position.
        Vector3 mouseWorldPos = MasterManager.CameraManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        /* Difference in position to be considered a swipe.
         * 7.5% of the screen should be okay. */
        float pixelDifferenceRequirement = MasterManager.CameraManager.OrthographicWidth * 0.075f;
        float mouseMoved = (m_mouseDownPosition.x - mouseWorldPos.x);


        /* If the mouse has moved past the difference requirement then
         * multiply the rotation value by the direction of movement
         * so that tiles move according to swipe direction. */
        if (Mathf.Abs(mouseMoved) > pixelDifferenceRequirement)
            rotationValue *= Mathf.Sign(mouseMoved);

        //Play audio and rotate.
        if (m_rotateAudio.enabled)
            m_rotateAudio.Play();

        CurrentTileChain.SetupRotate(rotationValue, m_rotateRate);
    }

    /// <summary>
    /// Returns BoardTIle scripts on the currently highlighted board tiles.
    /// </summary>
    /// <returns></returns>
    private List<BoardTile> ReturnHighlightedBoardTiles()
    {
        List<BoardTile> results = new List<BoardTile>();
        /* Go through every numbered tiles and get the highlighted object 
         * reference and extract the board tile script from it. */
        for (int i = 0; i < CurrentTileChain.NumberedTiles.Count; i++)
        {
            GameObject obj = CurrentTileChain.NumberedTiles[i].LastTracedBoardTile;

            //If obj isn't null.
            if (obj != null)
            {
                //Find board tile component.
                BoardTile boardTile = obj.GetComponent<BoardTile>();
                //If component doesn't exist. Shouldn't ever happen.
                if (boardTile == null)
                {
                    Debug.LogError(Debugs.ReturnErrorString(transform, " -> TileChainCollection -> ReturnHighlightedBoardTiles -> BoardTile script is null."));
                    continue;
                }
                results.Add(boardTile);
            }
        }

        return results;
    }

    /// <summary>
    /// Tells numbered tiles to trace for board tiles.
    /// </summary>
    private void TraceForBoardTiles()
    {
        //If tile chain isn't held.
        if (!TileChainHeld)
            return;

        for (int i = 0; i < CurrentTileChain.NumberedTiles.Count; i++)
        {
            CurrentTileChain.NumberedTiles[i].TraceForBoardTile();
        }
    }

    /// <summary>
    /// Checks where to move the tile.
    /// </summary>
    private void CheckMoveTile()
    {
        if (TileChainHeld)
        {
            Vector3 mouseWorldPos = MasterManager.CameraManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            //Pixel difference requirement is 10% of the screen height.
            float pixelDifferenceRequirement = MasterManager.CameraManager.OrthographicHeight * 0.05f;
            float mouseHeight = (mouseWorldPos.y - m_mouseDownPosition.y);
            /* Check if mouse is raised enough pixels above spawn point.
             * If so, move to input. Otherwise move to spawn. */
            if (mouseHeight > pixelDifferenceRequirement)
                MoveToInput();
            else
                MoveToSpawn();
        }
        else
        {
            MoveToSpawn();
        }
    }

    /// <summary>
    /// Moves the tile to where input is positioned.
    /// </summary>
    private void MoveToInput()
    {
        //Set that the tile has been picked up.
        TilesPickedUp();
        //Find where to put tile.
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 target = new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z);
        //Set rate.
        float distance = Vector3.Distance(transform.position, target);
        float rate = 25f * Time.deltaTime * distance;
        //Move to position ove rtime.
        transform.position = Vector3.MoveTowards(transform.position, target, rate);
    }

    /// <summary>
    /// Moves the tile to it's spawn point.
    /// </summary>
    private void MoveToSpawn()
    {
        Vector3 target = MasterManager.TileManager.TileSpawnPosition.position;
        //Set rate.
        float distance = Vector3.Distance(transform.position, target);
        float rate = 15f * Time.deltaTime * distance;
        //Move to position over time.
        transform.position = Vector3.MoveTowards(transform.position, target, rate);
    }

    /// <summary>
    /// Sets a hightlighted state of a board tile.
    /// </summary>
    /// <param name="boardTileObj">Object for the board tile.</param>
    /// <param name="highlight">Highlighted state.</param>
    public void SetBoardTileHighlight(GameObject boardTileObj, bool highlight)
    {
        //Null object passed in. No action can be taken.
        if (boardTileObj == null)
            return;

        //Find board tile component.
        BoardTile boardTile = boardTileObj.GetComponent<BoardTile>();
        //If component doesn't exist. Shouldn't ever happen.
        if (boardTile == null)
        {
            Debug.LogError(Debugs.ReturnErrorString(transform, " -> TileChainCollection -> SetBoardTileHighlight -> BoardTile script is null."));
            //Clear reference so that errors won't be spammed.
            boardTileObj = null;
            return;
        }

        //Remove highlight.
        SetBoardTileHighlight(boardTile, highlight);
    }

    /// <summary>
    /// Sets a hightlighted state of a board tile.
    /// </summary>
    /// <param name="boardTile">Board tile component reference.</param>
    /// <param name="highlight">Highlighted state.</param>
    private void SetBoardTileHighlight(BoardTile boardTile, bool highlight)
    {
        //Remove highlight.
        MasterManager.BoardManager.SetBoardTileHighlighted(boardTile, highlight);
    }

    /// <summary>
    /// Sets a hightlighted state of a list of board tiles.
    /// </summary>
    /// <param name="boardTiles">Board tile component references.</param>
    /// <param name="highlight">Highlighted state.</param>
    private void SetBoardTileHighlight(List<BoardTile> boardTiles, bool highlight)
    {
        for (int i = 0; i < boardTiles.Count; i++)
        {
            SetBoardTileHighlight(boardTiles[i], highlight);
        }
    }

    /// <summary>
    /// Called when the tiles are dropped.
    /// </summary>
    private void TilesDropped()
    {
        //Only play dropped audio if tile isn't already at it's drop position.
        if (m_dropAudio.enabled && !Vector3s.ValuesMatch(transform.position, MasterManager.TileManager.TileSpawnPosition.position, 0.1f))
            m_dropAudio.Play();
    }

    /// <summary>
    /// Called when the tiles are picked up.
    /// </summary>
    private void TilesPickedUp()
    {
        //If already picked up exit method.
        if (m_tileChainPickedUp)
            return;

        if (m_pickupAudio.enabled)
            m_pickupAudio.Play();

        m_tileChainPickedUp = true;
    }

    /// <summary>
    /// Called when the tiles are successfully placed.
    /// </summary>
    /// <param name="highlightedBoardTiles"></param>
    private void TilesPlaced(List<BoardTile> highlightedBoardTiles)
    {
        //Assign values for all highlighted board tiles.
        for (int i = 0; i < highlightedBoardTiles.Count; i++)
        {
            highlightedBoardTiles[i].TileValue = CurrentTileChain.NumberedTiles[i].TileValue;
        }

        //Tell tile manager that tiles have been placed.
        MasterManager.TileManager.TilePlaced();
        //Try to match placed tiles.
        MasterManager.BoardManager.TryMatchBoardTiles(highlightedBoardTiles);
        //Destroy the tile chain and let board manager handling spawning another.
        StartCoroutine(C_DestroyAfterPlacedAudio());
    }

    /// <summary>
    /// Plays the placed tile audio then destroys the object after audio has completed.
    /// </summary>
    private IEnumerator C_DestroyAfterPlacedAudio()
    {
        /* First disable the tile chain so it can no longer be interacted with,
         * and so that it becomes hidden. */
        CurrentTileChain.gameObject.SetActive(false);

        //Get length of audio.
        float audioLength = m_placedAudio.clip.length;

        //Play audio.
        if (m_placedAudio.enabled)
            m_placedAudio.Play();

        //Wait until audio finishes.
        yield return new WaitForSeconds(audioLength);
        //Destroy
        Destroy(gameObject);
    }

    /// <summary>
    /// Tries to place numbered tiles on the board. 
    /// </summary>
    private void TryPlaceTiles(List<BoardTile> highlightedBoardTiles)
    {

        /* Highlight count doesn't match numbered tiles count which means
         * not all tiles are over a board spot. */
        if (highlightedBoardTiles.Count != CurrentTileChain.NumberedTiles.Count)
        {
            TilesDropped();
            return;
        }

        /* Go through each board tile in order and make sure they're a neighbor
         * of one another. If being thorough we should make sure they're
         * a neighbor in the same pattern in which the tiles are rotated. But
         * it's virtually impossible to even have neighbors out of order, so
         * I'm going to be lazy and not check. */
        for (int i = 0; i < highlightedBoardTiles.Count; i++)
        {
            BoardTile boardTile = highlightedBoardTiles[i];
            //If the board tile isn't empty exit method. Can't place a on non empty board tile.
            if (boardTile.TileValue != TileValues.Empty)
            {
                TilesDropped();
                return;
            }

            //If not the last board tile make sure the next board tile is a neighbor.
            if (i != (highlightedBoardTiles.Count - 1))
            {
                //If the next board tile in list isn't a neighbor.
                if (!boardTile.Neighbors.Contains(highlightedBoardTiles[i + 1]))
                {
                    TilesDropped();
                    return;
                }
            }
        }

        /* If this far tile should be placed. */
        TilesPlaced(highlightedBoardTiles);
    }

}
