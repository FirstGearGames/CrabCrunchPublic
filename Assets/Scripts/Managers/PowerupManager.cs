using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{

    #region Public.
    /// <summary>
    /// Use PowerupSelectorsVisible.
    /// </summary>
    private bool m_powerupSelectorsVisible = false;
    /// <summary>
    /// True if a powerup is raised. A raised powerup indicates tiles are queued for powerup selection.
    /// </summary>
    public bool PowerupSelectorsVisible
    {
        get { return m_powerupSelectorsVisible; }
        private set { m_powerupSelectorsVisible = value; }
    }
    #endregion

    #region Serialized.
    /// <summary>
    /// Collection of all powerups and their data.
    /// </summary>
    [Tooltip("Collection of all powerups and their cost.")]
    [SerializeField]
    private List<PowerupData> m_powerupDatas = new List<PowerupData>();
    #endregion

    #region Private.
    /// <summary>
    /// Powerup scripts found in scene on load.
    /// </summary>
    private Powerup[] m_powerups;
    /// <summary>
    /// Time in which powerup interactions should be blocked until.
    /// </summary>
    private float m_globalUnblockTime = 0f;
    #endregion

    #region Const and readonly.
    /// <summary>
    /// Holds the hash value for the powerup being blocked from use.
    /// </summary>
    private readonly int m_powerupBlockedHash;
    /// <summary>
    /// Holds the hash value for the powerup being raised, or prompted trigger.
    /// </summary>
    private readonly int m_powerupRaisedHash;
    /// <summary>
    /// Holds the hash value for the powerup being dropped, no longer prompted trigger.
    /// </summary>
    private readonly int m_powerupDroppedHash;
    /// <summary>
    /// Holds the hash value for the powerup being used immediately.
    /// </summary>
    private readonly int m_powerupImmediatelyUsedHash;
    #endregion

    /// <summary>
    /// Basic constructor.
    /// </summary>
    public PowerupManager()
    {
        m_powerupBlockedHash = Animator.StringToHash("PowerupBlocked");
        m_powerupRaisedHash = Animator.StringToHash("PowerupRaised");
        m_powerupDroppedHash = Animator.StringToHash("PowerupDropped");
        m_powerupImmediatelyUsedHash = Animator.StringToHash("PowerupImmediatelyUsed");
    }

    private void Awake()
    {
        //Find all powerups in the scene.
        m_powerups = GameObject.FindObjectsOfType<Powerup>();
    }

    /// <summary>
    /// Sets a time in which a global block is in affect.
    /// </summary>
    /// <param name="duration">Duration to add onto current game time.</param>
    public void SetGlobalBlockedTime(float duration)
    {
        //If the new duration would be less than the current global unblock time then exit method.
        if (Time.time + duration < m_globalUnblockTime)
            return;

        m_globalUnblockTime = Time.time + duration;
    }

    /// <summary>
    /// Returns the animator trigger hash for interaction states.
    /// </summary>
    /// <param name="interactionState"></param>
    /// <returns></returns>
    public int ReturnPowerupInteractionStateHash(PowerupInteractionStates interactionState)
    {
        switch (interactionState)
        {
            //None is not supported within the animator.
            case PowerupInteractionStates.None:
                return -1;
            case PowerupInteractionStates.Blocked:
                return m_powerupBlockedHash;
            case PowerupInteractionStates.Dropped:
                return m_powerupDroppedHash;
            case PowerupInteractionStates.Raised:
                return m_powerupRaisedHash;
            case PowerupInteractionStates.ImmediatelyUsed:
                return m_powerupImmediatelyUsedHash;
            //Unhandled.
            default:
                return -1;
        }
    }

    /// <summary>
    /// Returns if the powerup icons can be interacted with. Not to be mistaken for the tiles, after using a powerup.
    /// </summary>
    /// <param name="powerup">The powerup which is attempting to be interacted with.</param>
    /// <returns></returns>
    public bool CanInteractWithPowerups(Powerup powerup)
    {
        //Still on global cooldown.
        if (Time.time < m_globalUnblockTime)
            return false;
        //If a menu is up block input.
        if (MasterManager.GameManager.States.GameInputBlocked)
            return false;
        //If tile chain is currently held.
        if (MasterManager.TileManager.CurrentTileChainCollection != null)
        {
            if (MasterManager.TileManager.CurrentTileChainCollection.TileChainHeld)
                return false;
        }

        //Check each state of all powerups to see if any are active.
        for (int i = 0; i < m_powerups.Length; i++)
        {
            /* Allow interaction if the powerup being checked to interact with
             * is the same as the current array index.
             * If the index is not the same return that the powerup
             * cannot be interacted with if the current index interaction state
             * is not set to None. */
            if (m_powerups[i] != powerup && m_powerups[i].InteractionState != PowerupInteractionStates.None)
                return false;
        }

        //Otherwise return true.
        return true;
    }

    /// <summary>
    /// Returns the cost to use a specified powerup.
    /// </summary>
    /// <param name="powerupType">Type of powerup to return cost for.</param>
    /// <returns></returns>
    public PowerupData ReturnPowerupData(PowerupTypes powerupType)
    {
        //Find the index within powerup costs.
        int index = m_powerupDatas.FindIndex(x => x.Type == powerupType);
        //If index isn't found.
        if (index == -1)
        {
            Debug.LogError("PowerupManager -> ReturnPowerupCost -> Couldn't find index for powerup type " + powerupType);
            return null;
        }
        //If found.
        else
        {
            return m_powerupDatas[index];
        }
    }

    /// <summary>
    /// Goes through all powerups and drops them.
    /// </summary>
    public void DropPowerups()
    {
        for (int i = 0; i < m_powerups.Length; i++)
        {
            m_powerups[i].DropPowerup();
        }
    }

    /// <summary>
    /// Shows a specified powerup selector on all supporting board tiles.
    /// </summary>
    /// <param name="type"></param>
    public void ShowPowerupSelectors(PowerupTypes type)
    {
        //Set that powerup selectors are visible.
        PowerupSelectorsVisible = true;

        BoardTile[][] grid = MasterManager.BoardManager.Grid;
        //Go through every board tile.
        for (int column = 0; column < grid.Length; column++)
        {
            for (int row = 0; row < grid[column].Length; row++)
            {
                //If not null show the powerup selector for the specified powerup.
                if (grid[column][row] != null)
                    grid[column][row].ShowPowerupSelector(type);
            }
        }
    }

    /// <summary>
    /// Shows or hides the powerup selectors for a specified powerup type.
    /// </summary>
    public void HidePowerupSelectors()
    {
        //Set that powerup selectors are no longer visible.
        PowerupSelectorsVisible = false;

        BoardTile[][] grid = MasterManager.BoardManager.Grid;
        //Go through every board tile.
        for (int column = 0; column < grid.Length; column++)
        {
            for (int row = 0; row < grid[column].Length; row++)
            {
                //If not null call hide powerup selectors.
                if (grid[column][row] != null)
                    grid[column][row].HidePowerupSelectors();
            }
        }
    }

    /// <summary>
    /// Tells all powerups to check their conditions and to locally set conditions met. Needs to be called anytime the board, tiles, or credits change.
    /// </summary>
    public void CheckPowerupConditions(bool forceConditionUpdate)
    {
        for (int i = 0; i < m_powerups.Length; i++)
        {
            m_powerups[i].CheckPowerupConditions(forceConditionUpdate);
        }
    }

    /// <summary>
    /// Performs a specified powerup.
    /// </summary>
    /// <param name="type">The type of powerup being performed.</param>
    /// <param name="boardTileAffected">Board tile affected by powerup. May be null.</param>
    public IEnumerator C_PerformPowerup(PowerupTypes type, BoardTile boardTileAffected = null)
    {
        switch (type)
        {
            case PowerupTypes.Undo:
                PerformUndo();
                break;
            case PowerupTypes.NewTile:
                StartCoroutine(C_PerformNewTile());
                break;
            case PowerupTypes.Destroy:
                PerformDestroy(boardTileAffected);
                break;
            case PowerupTypes.PlusOne:
                StartCoroutine(C_PerformPlusOne(boardTileAffected));
                break;
        }

        yield break;
    }

    /// <summary>
    /// Called when the undo powerup is used.
    /// </summary>
    private void PerformUndo()
    {
        PowerupData data = ReturnPowerupData(PowerupTypes.Undo);
        if (data == null)
        {
            Debug.LogError("PowerupManager -> PerformUndo -> Couldn't retrieve PowerupData.");
            return;
        }

        //Get previous save files for board and tile.
        string previousBoardSave = MasterManager.BoardManager.PreviousBoardSave;
        string previousTileSave = MasterManager.TileManager.PreviousTileSave;
        //If all previous save datas are empty there is nothing to undo.
        if (previousBoardSave == string.Empty && previousTileSave == string.Empty)
        {
            Debug.Log("PowerupManager -> PerformUndo -> Board and Tile save are both empty.");
            return;
        }

        //Remove the amount of credits the powerup cost using write only saving.
        MasterManager.GameManager.AddCredits(-data.Cost, SaveTypes.WriteOnly);

        if (previousBoardSave != string.Empty)
        {
            //Get the save location for board.
            string boardSaveName = MasterManager.BoardManager.ReturnBoardSavePath();
            //Save the board.
            PlayerPrefs.SetString(boardSaveName, previousBoardSave);
            //Load the set board save.
            MasterManager.BoardManager.LoadSavedBoard();
        }

        if (previousTileSave != string.Empty)
        {
            //Get the save location for tile.
            string tileSaveName = MasterManager.TileManager.ReturnTileSavePath();

            //Destroy current tile.
            if (MasterManager.TileManager.CurrentTileChainCollection != null)
                DestroyImmediate(MasterManager.TileManager.CurrentTileChainCollection.gameObject);

            PlayerPrefs.SetString(tileSaveName, previousTileSave);
            //Load the set board save.
            MasterManager.TileManager.LoadSavedTiles();
        }

        //Save changes immediately.
        MasterManager.SaveManager.HandleSaveType(SaveTypes.ForceSave);

        //After performing the powerup check conditions.
        CheckPowerupConditions(false);
    }

    /// <summary>
    /// Called when the NewTile powerup is used.
    /// </summary>
    private IEnumerator C_PerformNewTile()
    {
        PowerupData data = ReturnPowerupData(PowerupTypes.NewTile);
        if (data == null)
        {
            Debug.LogError("PowerupManager -> PerformNewTile -> Couldn't retrieve PowerupData.");
            yield break;
        }

        //Remove the amount of credits the powerup cost using write only saving.
        MasterManager.GameManager.AddCredits(-data.Cost, SaveTypes.WriteOnly);

        //Clear the board previous save.
        MasterManager.BoardManager.PreviousBoardSave = string.Empty;
        //Get the save location for tile.
        string tileSaveName = MasterManager.TileManager.ReturnTileSavePath();
        MasterManager.TileManager.PreviousTileSave = PlayerPrefs.GetString(tileSaveName, string.Empty);


        //Spawn a new tile.
        yield return StartCoroutine(MasterManager.TileManager.C_SpawnRandomNewTile(true));
        //Write to player prefs.
        MasterManager.TileManager.SaveTile(SaveTypes.WriteOnly);
        //Force save since credits were spent.
        MasterManager.SaveManager.HandleSaveType(SaveTypes.ForceSave);

        //After performing the powerup check conditions.
        CheckPowerupConditions(false);
    }


    private void PerformDestroy(BoardTile boardTile)
    {
        PowerupData data = ReturnPowerupData(PowerupTypes.Destroy);
        if (data == null)
        {
            Debug.LogError("PowerupManager -> PerformNewTile -> Couldn't retrieve PowerupData.");
            return;
        }

        //Remove the amount of credits the powerup cost using write only saving.
        MasterManager.GameManager.AddCredits(-data.Cost, SaveTypes.WriteOnly);

        //Hide all powerup selectors.
        HidePowerupSelectors();
        //Drop all powerups.
        DropPowerups();

        //Clear out previous tile save so that if Undo was used it would only reset the board.
        MasterManager.TileManager.PreviousTileSave = string.Empty;
        //Save current tile to player prefs.
        MasterManager.TileManager.SaveTile(SaveTypes.WriteOnly);

        //Spawn the FX.
        MasterManager.EffectsManager.SpawnMatchEffect(boardTile);
        //Play the audio.
        MasterManager.EffectsManager.PlayPopAudio(boardTile.TileValue);
        //Set the tile value to empty.
        boardTile.TileValue = TileValues.Empty;

        //Set the global cooldown.
        SetGlobalBlockedTime(data.InteractedBlockDuration);

        //Save grid and credit changes immediately.
        MasterManager.BoardManager.SaveGrid(SaveTypes.ForceSave);

        //After performing the powerup check conditions.
        CheckPowerupConditions(false);
    }

    private IEnumerator C_PerformPlusOne(BoardTile boardTile)
    {
        //Don't process if tile value is invalid.
        if (boardTile.TileValue == TileValues.Blocked || boardTile.TileValue == TileValues.Empty || boardTile.TileValue == TileValues.Seven)
            yield break;

        PowerupData data = ReturnPowerupData(PowerupTypes.PlusOne);
        if (data == null)
        {
            Debug.LogError("PowerupManager -> PerformPlusOne -> Couldn't retrieve PowerupData.");
            yield break;
        }

        //Remove the amount of credits the powerup cost using write only saving.
        MasterManager.GameManager.AddCredits(-data.Cost, SaveTypes.WriteOnly);

        //Clear previous tile save.
        MasterManager.TileManager.PreviousTileSave = string.Empty;
        //Set the current board save as previous board save.
        string boardSaveName = MasterManager.BoardManager.ReturnBoardSavePath();
        MasterManager.BoardManager.PreviousBoardSave = PlayerPrefs.GetString(boardSaveName, string.Empty);
        //Hide all powerup selectors.
        HidePowerupSelectors();
        //Drop all powerups.
        DropPowerups();

        //Increase the tile value by one.
        int tileValueNumeric = (int)boardTile.TileValue;
        boardTile.TileValue = (TileValues)(tileValueNumeric + 1);
        //Check for matches around the tile. Wait until matches complete.
        yield return StartCoroutine(MasterManager.BoardManager.C_TryMatchPowerupTile(new List<BoardTile>() { boardTile }));

        //Set the global cooldown.
        SetGlobalBlockedTime(data.InteractedBlockDuration);
        //Force save grid, causing credits to also save.
        MasterManager.BoardManager.SaveGrid(SaveTypes.ForceSave);

        //After performing the powerup check conditions.
        CheckPowerupConditions(false);
    }
}
