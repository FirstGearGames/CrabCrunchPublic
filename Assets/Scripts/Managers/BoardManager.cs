using FirstGearGames.Global.Structures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    #region SubClasses.
    [System.Serializable]
    private class GameModeGridSizes
    {
        #region Serialized.
        /// <summary>
        /// Use GameMode.
        /// </summary>
        [Tooltip("GameMode this grid size is for.")]
        [SerializeField]
        private GameModes m_gameMode;
        /// <summary>
        /// GameMode this grid size is for.
        /// </summary>
        public GameModes GameMode
        {
            get { return m_gameMode; }
        }
        /// <summary>
        /// Use GridSize.
        /// </summary>
        [Tooltip("The maximum tiles wide and number of rows count.")]
        [SerializeField]
        private int m_gridSize = 5;
        /// <summary>
        /// The maximum tiles wide and number of rows count.
        /// </summary>
        public int GridSize
        {
            get { return m_gridSize; }
        }
        #endregion
    }
    #endregion

    #region Public.
    /// <summary>
    /// Grid containing tile holders for board. Some entries may be null.
    /// </summary>
    [HideInInspector]
    public BoardTile[][] Grid;
    /// <summary>
    /// Use BoardTileScale.
    /// </summary>
    private float m_boardTileScale = 1f;
    /// <summary>
    /// Scale of the board tiles. Is set on board generation.
    /// </summary>
    public float BoardTileScale
    {
        get { return m_boardTileScale; }
        private set { m_boardTileScale = value; }
    }
    /// <summary>
    /// Use PreviousBoardSave.
    /// </summary>
    private string m_previousBoardSave = string.Empty;
    /// <summary>
    /// The previous board save state. Used for the undo powerup.
    /// </summary>
    public string PreviousBoardSave
    {
        get { return m_previousBoardSave; }
        set { m_previousBoardSave = value; }
    }
    #endregion

    #region Serialized.
    /// <summary>
    /// Tiles scale to fill this board size.
    /// </summary>
    [Tooltip("Tiles scale to fill this board size.")]
    [SerializeField]
    private Vector2 m_boardSize = new Vector2(600f, 600f);
    /// <summary>
    /// Amount of spacing between tiles. Negative values can be used to bring tiles closer together. This value is automatically adjusted with tile scale.
    /// </summary>
    [Tooltip("Amount of spacing between tiles. Negative values can be used to bring tiles closer together. This value is automatically adjusted with tile scale.")]
    [SerializeField]
    private Vector2 m_spacing = new Vector2(0f, 0f);
    /// <summary>
    /// Prefab for the board tile object to spawn.
    /// </summary>
    [Tooltip("Prefab for the board tile object to spawn.")]
    [SerializeField]
    private GameObject m_boardTilePrefab;
    /// <summary>
    /// Grid sizes for each game mode.
    /// </summary>
    [Tooltip("Grid sizes for each game mode.")]
    [SerializeField]
    private GameModeGridSizes[] m_gridSizes;
    #endregion

    #region Private.
    /// <summary>
    /// Houses the board tiles when they are generated.
    /// </summary>
    private GameObject m_boardTilesParent = null;
    #endregion

    #region Const and readonly.
    /// <summary>
    /// String to look up for the board tiles save.
    /// </summary>
    private const string BOARD_SAVE = "BoardLayout";
    #endregion

    private void Awake()
    {
        ResizeBoardWidth();
    }

    /// <summary>
    /// Resizes the board width to 90% of the screen width.
    /// </summary>
    private void ResizeBoardWidth()
    {
        float orthographicWidth = MasterManager.CameraManager.OrthographicWidth;
        m_boardSize = new Vector2(orthographicWidth * 0.90f, m_boardSize.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, new Vector3(m_boardSize.x, m_boardSize.y, 1f));
    }

    /// <summary>
    /// Loads the grid from last saved file for game mode.
    /// </summary>
    public void LoadSavedBoard()
    {
        /* Hide all powerup selectors and drop all powerups.
        * Some may be shown/raised if the user tried to
        * change game modes while powerup selectors were
         * enabled or powerups were raised. */
        MasterManager.PowerupManager.HidePowerupSelectors();


        //Clear out previously saved.
        PreviousBoardSave = string.Empty;
        //Load saved grid.
        LoadGrid();
    }

    /// <summary>
    /// Clears the tile holder grid objects.
    /// </summary>
    private void ClearGrid()
    {
        if (m_boardTilesParent != null)
            Destroy(m_boardTilesParent);
    }

    /// <summary>
    /// Returns the grid size for the current game mode.
    /// </summary>
    /// <returns></returns>
    private int ReturnGridSize()
    {
        //Go through each game mode grid size.
        for (int i = 0; i < m_gridSizes.Length; i++)
        {
            //If game mode if grid size entry matches game mode in settings.
            if (MasterManager.GameManager.Settings.GameMode == m_gridSizes[i].GameMode)
                return m_gridSizes[i].GridSize;
        }

        //If here no matches were found.
        Debug.LogError("BoardManager -> ReturnGridSize -> GameMode not found within grid sizes.");
        return -1;
    }

    /// <summary>
    /// Builds the tile holder grid objects
    /// </summary>
    public void BuildGrid()
    {
        ClearGrid();
        //Make a new parent and assign it to board tiles parent.
        GameObject parent = new GameObject();
        parent.name = "Board Tiles Parent";
        m_boardTilesParent = parent;

        //Get the grid size.
        int gridSize = ReturnGridSize();

        //Generate a new grid.
        Grid = new BoardTile[gridSize][];
        for (int i = 0; i < Grid.Length; i++)
        {
            Grid[i] = new BoardTile[gridSize];
        }

        //Reference to sprite renderer component on prefab background tile prefab.
        SpriteRenderer spriteRenderer = m_boardTilePrefab.GetComponent<SpriteRenderer>();

        //Sets scale to smallest of both axis to ensure both sides fit within the box.
        BoardTileScale = ReturnScale(m_spacing, spriteRenderer.bounds.size, gridSize);

        //Set sprite size in accordance to scale.
        Vector2 spriteSize = new Vector2(spriteRenderer.bounds.size.x * BoardTileScale, spriteRenderer.bounds.size.y * BoardTileScale);
        //Set spacing in accordance to scale.
        Vector2 spacing = new Vector2(m_spacing.x * BoardTileScale, m_spacing.y * BoardTileScale);

        //Half amount of rows needed. Used to determine how many tiles are skipped per row.
        int halfSize = Mathf.FloorToInt(gridSize / 2);

        for (int column = 0; column < gridSize; column++)
        {
            for (int row = 0; row < gridSize; row++)
            {
                //Position in a spherical sense.
                VirtualPosition virtualPosition = new VirtualPosition(column - halfSize, row - halfSize);

                //If should be dropped off from grid.
                if (virtualPosition.Column + virtualPosition.Row > halfSize || virtualPosition.Column + virtualPosition.Row < -halfSize)
                    continue;

                //Amount to shift the current row.
                float shiftX = (virtualPosition.Row * spriteSize.x * 0.5f);

                //Position to spawn the tile holder.
                float positionX = (virtualPosition.Column * spriteSize.x) + shiftX;
                float positionY = (-virtualPosition.Row * spriteSize.y);

                //The y axis doesn't shift so padding is easy to calculate as -row * spacing.y;
                positionY += -virtualPosition.Row * spacing.y;
                //Padding on the x axis isn't yet supported.

                Vector3 spawnPosition = transform.position + new Vector3(positionX, positionY, 0f);

                //Instantiate and name the object.
                GameObject obj = Instantiate(m_boardTilePrefab, spawnPosition, Quaternion.identity, m_boardTilesParent.transform);
                obj.name = virtualPosition.Column + ", " + virtualPosition.Row;
                //Set it's local scale to the scale required to fit within the bounds.
                obj.transform.localScale = new Vector3(BoardTileScale, BoardTileScale, 1f);
                //Setup the tile holder script.
                BoardTile boardTile = obj.GetComponent<BoardTile>();
                boardTile.VirtualPosition = virtualPosition;
                boardTile.TileValue = TileValues.Empty;

                //Add to grid. Not used for much, just out of bounds checks on finding neighbors at the moment.
                Grid[column][row] = boardTile;
            }
        }

        //Cycle through all tile holders and set each's neighbor.
        SetNeighbors(gridSize);
    }

    /// <summary>
    /// Cycles through spawned tile holders and sets their neighbors.
    /// </summary>
    private void SetNeighbors(int gridSize)
    {
        for (int column = 0; column < Grid.Length; column++)
        {
            for (int row = 0; row < Grid[column].Length; row++)
            {
                BoardTile tileHolder = Grid[column][row];
                //If a tile holder was set for this grid position then set it's neighbors.
                if (tileHolder != null)
                    tileHolder.Neighbors = ReturnNeighbors(tileHolder, gridSize);
            }
        }
    }

    /// <summary>
    /// Returns a float which all sprite sides must be multiplied by to fit within the desired grid size.
    /// </summary>
    /// <param name="paddingSpacing">Spacing between tiles.</param>
    /// <param name="spriteSize">Size of sprite bounds.</param>
    /// <param name="tileSets">Number of sprites in either direction.</param>
    /// <returns></returns>
    private float ReturnScale(Vector2 paddingSpacing, Vector2 spriteSize, int tileSets)
    {
        /* Get the total amount of pixels needed for largest padding.
         * Do the same except with rows. */
        float horizontalPadding = paddingSpacing.x * (tileSets - 1);
        float verticalPadding = paddingSpacing.y * (tileSets - 1);

        Vector2 requiredSpace = new Vector2(
            (spriteSize.x * tileSets) + horizontalPadding
            , (spriteSize.y * tileSets) + verticalPadding
            );

        return Mathf.Min(m_boardSize.x / requiredSpace.x, m_boardSize.y / requiredSpace.y);
    }


    /// <summary>
    /// Returns neighboring tiles from a specific grid position.
    /// </summary>
    /// <param name="tileHolder">Tile holder to find neighbors for.</param>
    /// <returns></returns>
    private List<BoardTile> ReturnNeighbors(BoardTile tileHolder, int gridSize)
    {
        List<BoardTile> results = new List<BoardTile>();

        for (int column = -1; column < 2; column++)
        {
            for (int row = -1; row < 2; row++)
            {
                int neighborColumn = column + tileHolder.VirtualPosition.Column + Mathf.FloorToInt(gridSize / 2);
                int neighborRow = row + tileHolder.VirtualPosition.Row + Mathf.FloorToInt(gridSize / 2);

                //Don't add self.
                if (column == 0 && row == 0)
                    continue;
                //If out of bounds on columns.
                if (neighborColumn < 0 || neighborColumn >= Grid.Length)
                    continue;
                //If out of bounds on rows.
                if (neighborRow < 0 || neighborRow >= Grid[0].Length)
                    continue;
                //If not adjacent negative.
                if (row == -1 && column == -1)
                    continue;
                //If not adjacent positive.
                if (row == 1 && column == 1)
                    continue;
                //If neighbor is null.
                if (Grid[neighborColumn][neighborRow] == null)
                    continue;

                results.Add(Grid[neighborColumn][neighborRow]);
            }
        }

        return results;
    }

    /// <summary>
    /// Sets highlighted state for a board tile. Also stores references of which tiles are highlighted.
    /// </summary>
    /// <param name="boardTile">Board tile to highlight.</param>
    /// <param name="highlighted">Highlight state.</param>
    public void SetBoardTileHighlighted(BoardTile boardTile, bool highlighted)
    {
        //If not bypassing can set check.
        if (!CanSetHighlightOnBoardTile(boardTile))
            return;

        //Set highlight state on tile.
        boardTile.SwitchHighlight(highlighted);
    }

    /// <summary>
    /// Returns if a board tile is allowed to have it's highlighted state set to a passed in value.
    /// </summary>
    /// <param name="boardTile">Board tile to check.</param>
    /// <returns></returns>
    private bool CanSetHighlightOnBoardTile(BoardTile boardTile)
    {
        TileValues currentValue = boardTile.TileValue;

        //If empty tile board tile can have it's highlight state changed.
        if (currentValue == TileValues.Empty)
            return true;
        //Anything but empty.
        else
            return false;
    }

    /// <summary>
    /// Clears highlight on all board tiles.
    /// </summary>
    public void ClearTileHighlights()
    {
        for (int column = 0; column < Grid.Length; column++)
        {
            for (int row = 0; row < Grid[column].Length; row++)
            {
                Grid[column][row].SwitchHighlight(false);
            }
        }
    }

    /// <summary>
    /// Tries to match tiles adjacent to the provided board tiles.
    /// </summary>
    /// <param name="boardTiles">Board tiles to find matches of, using nearby tiles.</param>
    /// <param name="throughPowerup">True if called through a powerup. Used to prevent some actions from executing, such as saving, turn ended, new tile, and more.</param>
    public void TryMatchBoardTiles(List<BoardTile> boardTiles)
    {
        //Start matching coroutine.
        StartCoroutine(C_TryMatchBoardTiles(boardTiles, false));
    }

    /// <summary>
    /// Tries to match adjacent tiles to a tile which PlusOne was used on.
    /// </summary>
    /// <param name="boardTiles">Board tiles to find matches of, using nearby tiles.</param>
    public IEnumerator C_TryMatchPowerupTile(List<BoardTile> boardTiles)
    {
        //Start matching coroutine.
        yield return StartCoroutine(C_TryMatchBoardTiles(boardTiles, true));
    }

    /// <summary>
    /// Tries to match tiles around specified board tiles.
    /// </summary>
    /// <param name="placedBoardTiles">Board tiles to match around.</param>
    private IEnumerator C_TryMatchBoardTiles(List<BoardTile> placedBoardTiles, bool throughPowerup)
    {
        //Set that matches are trying to be made.
        MasterManager.GameManager.States.Matching = true;

        //Becomes true when a match is made.
        bool matchMade = false;
        //Combo multiplier to apply to score per match made.
        int comboMultiplier = 1;
        //Total score during match attempt.
        int totalScore = 0;

        //Holds tiles which match the starting tile.
        List<BoardTile> matchingTiles = new List<BoardTile>();

        //Go through the tiles which have had their values changed.
        for (int startingTileIndex = 0; startingTileIndex < placedBoardTiles.Count; startingTileIndex++)
        {
            //Set starting score as 0.
            int score = 0;

            BoardTile startingBoardTile = placedBoardTiles[startingTileIndex];
            int startingTileNumeric = (int)startingBoardTile.TileValue;
            /* If board tile has an empty or blocked value.
             * Can happen if tile is destroyed in a match. */
            if (startingBoardTile.TileValue == TileValues.Blocked || startingBoardTile.TileValue == TileValues.Empty)
                continue;

            //Reset list.
            matchingTiles.Clear();
            //Add starting tile to matching tiles.
            matchingTiles.Add(startingBoardTile);

            //Iterate through matching tiles. The list count will increases as matches are found.
            for (int i = 0; i < matchingTiles.Count; i++)
            {
                //Go through each neighbor in the current matching tile.
                foreach (BoardTile neighbor in matchingTiles[i].Neighbors)
                {
                    //If already in matching tiles.
                    if (matchingTiles.Contains(neighbor))
                        continue;

                    //If neighbor tile matches starting tile value.
                    if (neighbor.TileValue == startingBoardTile.TileValue)
                        matchingTiles.Add(neighbor);
                }
            }

            //If enough matching tiles.
            if (matchingTiles.Count > 2)
            {
                score += (int)startingBoardTile.TileValue * matchingTiles.Count;
                float interval = 0.2f;

                //If a 7 tile was matched then crunch it and nearby tiles.
                if (startingBoardTile.TileValue == TileValues.Seven)
                {
                    yield return StartCoroutine(C_CrunchAroundMatches(matchingTiles, interval));
                    //Clear matching tiles as tiles nearby 
                }
                //Anything less than 7 matches.
                else
                {
                    yield return StartCoroutine(C_ExplodeMatches(matchingTiles, interval, true));
                    //Reduce index so matched tile is checked again.
                }

                //Reduce the index so the same tile is checked.
                startingTileIndex--;

                if (score != 0)
                {
                    float matchingTilesCreditBonus = matchingTiles.Count * 0.02f;
                    //Chance to get credits is a base value times combo mutliplier.
                    float creditsChance = (0.04f + matchingTilesCreditBonus) * comboMultiplier;
                    //If a roll is witihin creditChance then grant credits equal to the score.
                    if (UnityEngine.Random.Range(0f, 1f) <= creditsChance)
                        MasterManager.GameManager.AddCredits(score);

                    //Multiply score by combo count.
                    score *= comboMultiplier;

                    //Setup floating text for score.
                    float distance = MasterManager.CameraManager.OrthographicHeight * 0.2f;
                    float speed = distance * 0.75f;
                    Color32 color = MasterManager.TileManager.ReturnTileColor(startingTileNumeric);
                    MasterManager.EffectsManager.SpawnFloatingText(score.ToString(), speed, new Vector3(0f, 1f, 0f), startingBoardTile.transform.position - new Vector3(0f, 0f, 10f), distance, 67f, color);

                    //Add score onto current score.
                    MasterManager.GameManager.AddCurrentScore(score, true);
                    //Add to total score and reset score for this match cycle.
                    totalScore += score;
                    score = 0;
                    //Increase combo multiplier.
                    comboMultiplier++;
                    //Set that a match has been made
                    matchMade = true;
                }

            }
        }

        //Unset that matches are trying to be made.
        MasterManager.GameManager.States.Matching = false;

        /* Update the highest placed tile. It's safe to check against the
         * tiles that were just placed since it's impossible for tiles
         * to level outside of what was placed. */
        MasterManager.GameManager.UpdateHighestPlacedTile(placedBoardTiles);

        //Only process the match made logic if not through a powerup.
        if (!throughPowerup)
        {
            if (matchMade)
                MatchMade(totalScore, comboMultiplier);
            else
                NoMatchMade();
        }

        yield break;
    }


    /// <summary>
    /// Called when no matches are made, after attempting to make matches.
    /// </summary>
    private void NoMatchMade()
    {
        //Try to spawn a new random tile.
        StartCoroutine(MasterManager.TileManager.C_SpawnRandomNewTile());
        //Call turn ended.
        MasterManager.GameManager.TurnEnded();
    }

    /// <summary>
    /// Called when a match has successfully been made.
    /// </summary>
    private void MatchMade(int totalScore, int comboMultiplier)
    {
        //Try to spawn a new random tile.
        StartCoroutine(MasterManager.TileManager.C_SpawnRandomNewTile());
        //Call turn ended.
        MasterManager.GameManager.TurnEnded();
    }

    private IEnumerator C_CrunchAroundMatches(List<BoardTile> matches, float interval)
    {
        //Match tiles normally.
        yield return StartCoroutine(C_ExplodeMatches(matches, interval, false));

        //Now explode around the first tile.
        foreach (BoardTile neighbor in matches[0].Neighbors)
        {
            //Spawn the FX.
            MasterManager.EffectsManager.SpawnMatchEffect(neighbor);
            //Set the tile value to empty.
            neighbor.TileValue = TileValues.Empty;
        }

        //Empty out the first tile as well, as it exploded.
        matches[0].TileValue = TileValues.Empty;
    }

    private IEnumerator C_ExplodeMatches(List<BoardTile> matches, float interval, bool playPopAudio)
    {
        //Start bubbling audio.
        MasterManager.EffectsManager.StartBubblingAudio();

        int nextValue = (int)matches[0].TileValue + 1;
        //Reverse
        for (int i = (matches.Count - 1); i > -1; i--)
        {
            //Spawn the FX.
            MasterManager.EffectsManager.SpawnMatchEffect(matches[i]);

            //If on last tile.
            if (i == 0)
            {
                //Stop bubbling sound.
                MasterManager.EffectsManager.StopBubblingAudio();
                //If to play pop audio.
                if (playPopAudio)
                    MasterManager.EffectsManager.PlayPopAudio(matches[i].TileValue);
            }

            //Set the tile value to empty.
            matches[i].TileValue = TileValues.Empty;
            //Wait an interval before showing the next effect.
            yield return new WaitForSeconds(interval);
        }

        //Increased matched tiles value.
        matches[0].TileValue = (TileValues)nextValue;
    }

    /// <summary>
    /// Loads information about the grid from a the save file for the current game type.
    /// </summary>
    private void LoadGrid()
    {
        //Get save name for gamemode type.
        int gameModeNumeric = (int)MasterManager.GameManager.Settings.GameMode;
        string saveName = BOARD_SAVE + gameModeNumeric.ToString();
        string boardLayout = PlayerPrefs.GetString(saveName, string.Empty);

        //There is no saved data.
        if (boardLayout == string.Empty)
        {
            Debug.Log("BoardManager -> LoadGrid -> Grid save is empty.");
            //Set all board tiles as empty.
            for (int column = 0; column < Grid.Length; column++)
            {
                for (int row = 0; row < Grid[column].Length; row++)
                {
                    if (Grid[column][row] != null)
                        Grid[column][row].TileValue = TileValues.Empty;
                }
            }

            return;
        }



        //If data is invalid length.
        if (boardLayout.Length % 2 != 0)
        {
            Debug.LogError("BoardManager -> LoadGrid -> Invalid board layout length. Length of " + boardLayout.Length);
            return;
        }

        //Index within board layout string to start reading from.
        int tileIndex = 0;
        //Go through every column and row to build the board layout string.
        for (int column = 0; column < Grid.Length; column++)
        {
            for (int row = 0; row < Grid[column].Length; row++)
            {
                BoardTile boardTile = Grid[column][row];
                //Some entries may be null since grids are a hexagon.
                if (boardTile == null)
                    continue;

                string text = boardLayout.Substring(tileIndex, 2);

                int result;
                if (Int32.TryParse(text, out result))
                {
                    TileValues tileValue = (TileValues)result;
                    //If not a possible tile value.
                    if (!Enum.IsDefined(typeof(TileValues), tileValue))
                    {
                        Debug.LogError("BoardManager -> LoadGrid -> Invalid tile value range. Value of " + result.ToString() + ", " + text);
                        return;
                    }
                    //Within value range, set value.
                    else
                    {
                        boardTile.TileValue = tileValue;
                        //Update highest placed tile.
                        MasterManager.GameManager.UpdateHighestPlacedTile(boardTile);
                        //Increase tile index by 2 to load next value.
                        tileIndex += 2;
                    }
                }
                //Couldn't parse text.
                else
                {
                    Debug.LogError("BoardManager -> LoadGrid -> Unable to parse text: " + text);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Saves the grid to file for the current game type.
    /// </summary>
    /// <param name="forceSave">True to save the file immediately regardless of last file save.</param>
    public void SaveGrid(SaveTypes saveType, bool gameOver = false)
    {
        StringBuilder boardLayout = new StringBuilder();
        string result;

        //Game over, overwrite grid save with empty string.
        if (gameOver)
        {
            PreviousBoardSave = string.Empty;
            result = string.Empty;
        }
        //Game not over.
        else
        {
            //Go through every column and row to build the board layout string.
            for (int column = 0; column < Grid.Length; column++)
            {
                for (int row = 0; row < Grid[column].Length; row++)
                {
                    BoardTile boardTile = Grid[column][row];
                    //Some entries may be null since grids are a hexagon.
                    if (boardTile == null)
                        continue;

                    //Get numeric and string of tile value.
                    int numeric = (int)boardTile.TileValue;
                    string textValue = numeric.ToString();
                    //Padd the string.
                    textValue = Strings.PadLeft(textValue, 2, "0");

                    //Add value onto string builder.
                    boardLayout.Append(textValue);
                }
            }
            //Set result from string builder.
            result = boardLayout.ToString();
        }

        string saveName = ReturnBoardSavePath();
        //Get the current save before saving and apply it as previous save.
        PreviousBoardSave = PlayerPrefs.GetString(saveName);
        //Set player prefs string and save.
        PlayerPrefs.SetString(saveName, result);

        //Send to save type handler.
        MasterManager.SaveManager.HandleSaveType(saveType);
    }

    /// <summary>
    /// Returns the save path, or save name for the board. Varies based on current game mode.
    /// </summary>
    /// <returns></returns>
    public string ReturnBoardSavePath()
    {
        //Get save name for gamemode type.
        int gameModeNumeric = (int)MasterManager.GameManager.Settings.GameMode;
        return (BOARD_SAVE + gameModeNumeric.ToString());
    }

    /// <summary>
    /// Returns the highest number of empty connected board tiles up to a specified value.
    /// </summary>
    /// <param name="maxConnected">Maximum number allowed before exiting method with the maximum result.</param>
    /// <returns></returns>
    public int ReturnHighestEmptyConnectedTilesCount(int maxConnected)
    {
        //Highest found connected.
        int highestConnected = 0;

        //Go through every board spot until results are found.
        for (int column = 0; column < Grid.Length; column++)
        {
            for (int row = 0; row < Grid[column].Length; row++)
            {
                //Reference for easy accessing.
                BoardTile boardTile = Grid[column][row];

                //If current board tile is null.
                if (boardTile == null)
                    continue;
                //Board tile isn't empty. Can't check neighbors.
                if (boardTile.TileValue != TileValues.Empty)
                    continue;

                //Reset empty neighbor count but include self in result.
                int emptyNeighborCount = 1;

                //Go through each neighbor in board tile.
                foreach (BoardTile neighbor in boardTile.Neighbors)
                {
                    //If neighbor is empty increase value.
                    if (neighbor.TileValue == TileValues.Empty)
                        emptyNeighborCount++;
                    //If at desired target break from this foreach.
                    if (emptyNeighborCount == maxConnected)
                        break;
                }

                //If empty neighbor count equals maxConnected return as max connected.
                if (emptyNeighborCount == maxConnected)
                    return maxConnected;

                //Set highestConnected to whichever is larger, itself or empty neighbors.
                highestConnected = Math.Max(emptyNeighborCount, highestConnected);
            }
        }

        //If here went through the entire loop. Return whatever was the highest result.
        return highestConnected;
    }


}
