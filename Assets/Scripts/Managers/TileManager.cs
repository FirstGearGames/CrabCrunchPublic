using FirstGearGames.Global.Structures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    #region Public.
    /// <summary>
    /// Use CurrentTileChainCollection.
    /// </summary>
    private TileChainCollection m_currentTileChainCollection;
    /// <summary>
    /// Currently spawned tile chain collection.
    /// </summary>
    public TileChainCollection CurrentTileChainCollection
    {
        get { return m_currentTileChainCollection; }
        private set { m_currentTileChainCollection = value; }
    }
    /// <summary>
    /// The previous tile save state. Used for undo powerup.
    /// </summary>
    public string PreviousTileSave = string.Empty;
    #endregion

    #region Serialized.
    /// <summary>
    /// Use TileSpawnPosition.
    /// </summary>
    [Tooltip("Transform reference marking the position where tiles spawn.")]
    [SerializeField]
    private Transform m_tileSpawnPosition;
    /// <summary>
    /// Transform reference marking the position where tiles spawn.
    /// </summary>
    public Transform TileSpawnPosition
    {
        get { return m_tileSpawnPosition; }
    }
    /// <summary>
    /// Tile chain collection prefab. The prefab which is used for spawned tiles.
    /// </summary>
    [Tooltip("Tile chain collection prefab. The prefab which is used for spawned tiles.")]
    [SerializeField]
    private GameObject m_tileChainCollectionPrefab;
    /// <summary>
    /// Color of tiles for each number, in order. Tile number 0 is for an empty tile.
    /// </summary>
    [Tooltip("Color of tiles for each number, in order. Tile number 0 is for an empty tile.")]
    [SerializeField]
    private Color32[] m_tileColors = new Color32[8];
    /// <summary>
    /// "Color of the text for each tile number, in order. Tile number 0 is for empty tile.
    /// </summary>
    [Tooltip("Color of the text for each tile number, in order. Tile number 0 is for empty tile.")]
    [SerializeField]
    private Color32[] m_textColors = new Color32[8];
    #endregion

    #region Const and readonly.
    /// <summary>
    /// String to look up for the tiles save.
    /// </summary>
    private const string TILES_SAVE = "CurrentTiles";
    #endregion

    /// <summary>
    /// Loads the last saved tile for the current game mode.
    /// </summary>
    public void LoadSavedTiles()
    {
        //Clear any previously saved tiles.
        PreviousTileSave = string.Empty;

        if (!LoadTiles())
            StartCoroutine(C_SpawnRandomNewTile());
    }

    /// <summary>
    /// Loads save tiles or spawns a new tile if nothing is saved.
    /// </summary>
    /// <returns>Returns true if successful in loading a saved tile.</returns>
    private bool LoadTiles()
    {
        //Get save name for gamemode type.
        int gameModeNumeric = (int)MasterManager.GameManager.Settings.GameMode;
        string saveName = TILES_SAVE + gameModeNumeric.ToString();
        string tiles = PlayerPrefs.GetString(saveName, string.Empty);

        //There is no saved data.
        if (tiles == string.Empty)
        {
            Debug.Log("TilesManager -> LoadTiles -> Tiles save is empty.");
            return false;
        }


        //If data is invalid length or impossible length to be a tile.
        if (tiles.Length % 2 != 0 || tiles.Length < 4)
        {
            Debug.LogError("TileManager -> LoadTiles -> Invalid tiles length. Length of " + tiles.Length);
            return false;
        }

        int result;
        string text;
        TileChainTypes tileChainType = TileChainTypes.None;

        text = tiles.Substring(0, 2);
        if (Int32.TryParse(text, out result))
        {
            TileChainTypes type = (TileChainTypes)result;
            //If type isn't defined.
            if (!Enum.IsDefined(typeof(TileChainTypes), type))
            {
                Debug.LogError("TileManager -> LoadTiles -> Tile chain type isn't defined.");
                return false;
            }

            //Set type.
            tileChainType = type;
        }
        //Couldn't parse tile chain type.
        else
        {
            Debug.LogError("TileManager -> LoadTiles -> Couldn't parse tile chain type.");
            return false;
        }

        if (tileChainType == TileChainTypes.None)
        {
            Debug.LogError("TileManager -> LoadTiles -> Cannot use None for a tile chain type.");
            return false;
        }

        //Make a new list for tile values.
        List<TileValues> tileValues = new List<TileValues>();
        /* Go through the rest of the tiles skipping the first two characters
         * as those were reserved for tile chain type. */
        for (int i = 2; i < tiles.Length; i += 2)
        {
            text = tiles.Substring(i, 2);
            if (Int32.TryParse(text, out result))
            {
                TileValues tileValue = (TileValues)result;
                if (result < 1 || !Enum.IsDefined(typeof(TileValues), tileValue))
                {
                    Debug.LogError("TileManager -> LoadTiles -> Impossible tile value.");
                    return false;
                }

                //If here tile value is good. Add it to tile values list.
                tileValues.Add(tileValue);

            }
            //Couldn't parse tile value numeric.
            else
            {
                Debug.LogError("TileManager -> LoadTiles -> Couldn't parse tile value numeric.");
                return false;
            }
        }

        //If tile chain type is none
        if (tileValues.Count != ReturnTileChainNumbersCount(tileChainType))
        {
            Debug.LogError("TileManager -> LoadTiles -> tileValues count doesn't match expected count for tile chain type.");
            return false;
        }
        //Is right length of tile values.
        else
        {
            SpawnTile(tileChainType, tileValues.ToArray());
            return true;
        }
    }

    /// <summary>
    /// Returns the tile color for a specified tile number.
    /// </summary>
    /// <param name="tileNumber">Tile value number to return color for.</param>
    /// <returns></returns>
    public Color32 ReturnTileColor(int tileNumber)
    {
        if (tileNumber < 0 || tileNumber >= m_tileColors.Length)
            return m_tileColors[0];
        else
            return m_tileColors[tileNumber];
    }
    /// <summary>
    /// Returns the tile color for a specified tile value.
    /// </summary>
    /// <param name="tileValue">Tile value enum to return color for.</param>
    /// <returns></returns>
    public Color32 ReturnTileColor(TileValues tileValue)
    {
        return ReturnTileColor((int)tileValue);
    }

    /// <summary>
    /// Returns the text color for a specified tile number.
    /// </summary>
    /// <param name="tileNumber"></param>
    /// <returns></returns>
    public Color32 ReturnTextColor(int tileNumber)
    {
        if (tileNumber < 0 || tileNumber >= m_tileColors.Length)
            return m_textColors[0];
        else
            return m_textColors[tileNumber];
    }
    /// <summary>
    /// Returns the text color for a specified tile value.
    /// </summary>
    /// <param name="tileValue"></param>
    /// <returns></returns>
    public Color32 ReturnTextColor(TileValues tileValue)
    {
        return ReturnTextColor((int)tileValue);
    }

    /// <summary>
    /// Spawns a tile if possible and returns the NumberedTile script.
    /// </summary>
    /// <param name="tileValues">Values of the tiles. May be single or multiple.</param>
    public void SpawnTile(TileChainTypes chainType, TileValues[] tileValues)
    {
        /* If a tile is already spawned. Can only have one spawned tile at a time.
         * Placed tiles are not considered spawned tiles. */
        if (CurrentTileChainCollection != null)
        {
            Debug.Log("TileManager -> SpawnTile -> A tile is already spawned.");
            return;
        }

        //Instantiate at tile spawn with no parent.
        GameObject obj = Instantiate(m_tileChainCollectionPrefab, m_tileSpawnPosition.transform.position, Quaternion.identity, null);
        //Set scale to match board tiles.
        obj.transform.localScale = new Vector3(MasterManager.BoardManager.BoardTileScale, MasterManager.BoardManager.BoardTileScale, 1f);
        //Find associated script with prefab.
        TileChainCollection tileChainCollection = obj.GetComponent<TileChainCollection>();

        //Null check.
        if (tileChainCollection == null)
        {
            Debug.LogError("TileManager -> SpawnTile -> Couldn't find TileChainCollection script.");
            Destroy(obj);
            return;
        }

        //Set as spawned tile chain collection.
        CurrentTileChainCollection = tileChainCollection;
        //Initialize tile chain.
        tileChainCollection.Initialize(chainType, tileValues);
    }

    /// <summary>
    /// Called when a tile is placed on the board.
    /// </summary>
    public void TilePlaced()
    {
        //Clear current tile chain.
        CurrentTileChainCollection = null;
    }

    /// <summary>
    /// Generates a new random tile and spawns it.
    /// </summary>
    public IEnumerator C_SpawnRandomNewTile(bool deleteCurrentTile = false)
    {
        //If to delete current tile first.
        if (deleteCurrentTile)
        {
            //Destroy the current tile.
            if (CurrentTileChainCollection != null)
            {
                //Save tile in player prefs, which is the tile being removed, as previous tile save.
                string saveName = ReturnTileSavePath();
                //Get the current save before saving and apply it as previous save.
                PreviousTileSave = PlayerPrefs.GetString(saveName);
                //Wait until the tile is removed before continuing.
                yield return StartCoroutine(C_RemoveCurrentTileChainCollection());
            }
        }

        TileChainTypes chainType;
        int[] numbers;
        TileValues[] results;

        //Get chain type and number count.
        chainType = ReturnRandomTileChainType();

        //If no chain type was returned or number count is 0 a tile cannot be spawned..
        if (chainType == TileChainTypes.None)
        {
            Debug.Log("TileManager -> C_SpawnRandomNewTile -> chainType is none or numberCount is 0.");
            yield break;
        }

        //Set numbers array length to that of numbers count.
        numbers = new int[ReturnTileChainNumbersCount(chainType)];

        //Fill the numbers array.
        for (int i = 0; i < numbers.Length; i++)
        {
            //Pick a random number between 1 and highest possible number.
            int randomTileNumber = Ints.RandomInclusiveRange(1, MasterManager.GameManager.CurrentGame.HighestPlacedTile);
            //Maximum value tile numbers can be reduced by. If greater than 0, the tile can be reduced.
            int maximumReduction = (randomTileNumber - 1);

            /* Flat 50% chance reduction rate regardless of difficulty.
             * If can be reduced and to reduce. */
            if (maximumReduction > 0 && Bools.RandomBool())
            {
                int reductionAmount;
                //Use different reduction logic based on game difficulty.
                switch (MasterManager.GameManager.Settings.GameMode)
                {
                    //Hard game mode.
                    case GameModes.Hard:
                        reductionAmount = Ints.RandomInclusiveRange(1, maximumReduction);
                        randomTileNumber -= reductionAmount;
                        break;
                    //Normal game mode.
                    case GameModes.Normal:
                        //Only reduce if the tile is 3 or higher.
                        if (randomTileNumber >= 4)
                        {
                            reductionAmount = Ints.RandomInclusiveRange(1, maximumReduction);
                            randomTileNumber -= reductionAmount;
                        }
                        break;
                }
            }

            //Set number.
            numbers[i] = randomTileNumber;
        }

        //Sort numbers so they're in order lowest to highest.
        Array.Sort(numbers);

        /* Perform additional operations on tiles of 3 length
         * to make chances of performing a match more difficult. */
        if (numbers.Length == 3)
        {
            //If all values match.
            if (Ints.ValuesMatch(numbers))
            {
                //Get the number which all numbers are.
                int matchingNumber = numbers[0];
                /* If the number is greater than 1
                 * reduce the first two numbers. */
                if (matchingNumber > 1)
                {
                    numbers[0]--;
                    numbers[1]--;
                }
                /* If the number is less than or equal to 1 than
                 * increase only the last number. */
                else if (matchingNumber <= 1)
                {
                    numbers[2]++;
                }
            }
            //If not all the numbers are the same.
            else
            {
                //If the last two numbers are the same.
                if (numbers[2] == numbers[1])
                {
                    /* Since the numbers are in order of lowest to highest
                     * and since if within this nested code there's no way that
                     * all the numbers are the same, it's safe to assume that
                     * the 2nd and 3rd numbers are greater than 1. As a result
                     * the only thing which must be done is reduce the middle number
                     * by 1. */
                    numbers[1]--;
                }
            }
        }

        //Convert numbers to enums.
        results = new TileValues[numbers.Length];
        for (int i = 0; i < numbers.Length; i++)
        {
            results[i] = (TileValues)numbers[i];
        }

        //Spawn the generated tile type.
        MasterManager.TileManager.SpawnTile(chainType, results);
    }


    /// <summary>
    /// Animates and removes the current tile chain type collection.
    /// </summary>
    /// <returns></returns>
    private IEnumerator C_RemoveCurrentTileChainCollection()
    {
        DestroyImmediate(CurrentTileChainCollection.gameObject);
        yield break;
    }

    /// <summary>
    /// Returns a random tile chain type. Checks the board tiles grid and only returns possible chain types.
    /// </summary>
    /// <returns></returns>
    private TileChainTypes ReturnRandomTileChainType()
    {
        //Length the chain should be. This data has been checked to be possible.
        int chainLength = ReturnRandomPossibleConnectedTileCount();

        //If chain length is 0 then the grid is full.
        if (chainLength == 0)
            return TileChainTypes.None;

        /* The shapes for chain lengths of two or less are always
         * the same so there's no need to go through the grid to find a
         * shape that will fit. Can safely shortcut out of the method. */
        if (chainLength <= 2)
            return ReturnTileChainType(chainLength, null);

        /* Convert to a 1 dimensional array for easier shuffling. Shuffling
        * is done to randomize shape of spawned tile as the spawned tile
        * will assume first available shape. */
        BoardTile[] shuffledBoardTiles = MasterManager.BoardManager.Grid.SelectMany(X => X).ToArray();
        //Shuffle array.
        Generics.Shuffle(ref shuffledBoardTiles);

        //list of tiles which match the required chain length.
        List<BoardTile> matchingTiles = new List<BoardTile>();

        for (int i = 0; i < shuffledBoardTiles.Length; i++)
        {
            BoardTile boardTile = shuffledBoardTiles[i];

            //If current board tile is null.
            if (boardTile == null)
                continue;

            //Board tile isn't empty. Can't check neighbors.
            if (boardTile.TileValue != TileValues.Empty)
                continue;

            //Reset matching tiles.
            matchingTiles.Clear();
            //Add self to neighbors to include as a connected board tile.
            matchingTiles.Add(boardTile);

            //Shuffle the neighbors so the order in which they're checked is random.
            Generics.Shuffle(ref boardTile.Neighbors);
            //Go through each neighbor.
            foreach (BoardTile neighbor in boardTile.Neighbors)
            {
                //If neighbor is empty increase value.
                if (neighbor.TileValue == TileValues.Empty)
                    matchingTiles.Add(neighbor);

                //If matching tiles count is chain length break from foreach.
                if (matchingTiles.Count == chainLength)
                    break;
            }

            //If matching tiles count is chain length break from for loop.
            if (matchingTiles.Count == chainLength)
                break;
        }

        //Get the chain type result from the found matching tiles.
        return ReturnTileChainType(chainLength, matchingTiles);
    }

    /// <summary>
    /// Returns the number of connected tiles preferred for a tile spawn. Does not neccesarily mean that this value is achievable. The number is random based on GameMode and chance.
    /// </summary>
    /// <returns></returns>
    private int ReturnRandomPossibleConnectedTileCount()
    {
        int maxAllowedConnected;
        //Set max allowed connected based on game mode.
        GameModes gameMode = MasterManager.GameManager.Settings.GameMode;
        if (gameMode == GameModes.Normal)
            maxAllowedConnected = 2;
        else
            maxAllowedConnected = 3;

        //Set the max number of connected tiles found on the grid.
        int availableMaxConnected = MasterManager.BoardManager.ReturnHighestEmptyConnectedTilesCount(maxAllowedConnected);

        //No available connected. Return that value.
        if (availableMaxConnected == 0)
            return 0;

        //Holds connected count and odds of getting that count.
        List<KeyValuePair<int, float>> connectedOdds = new List<KeyValuePair<int, float>>();

        //Changes the odds chances. Used to make adjustments based on game difficulty.
        float oddsMultiplier = 1f;
        /* If on hard mode set the odds multiplier to 0.5f. This causes
         * less chance of getting smaller tile chain collections. */
        if (MasterManager.GameManager.Settings.GameMode == GameModes.Hard)
            oddsMultiplier = 0.5f;
        /* Add an opportunity of 33% of each connected count. */
        //Add 1.
        if (availableMaxConnected > 0)
            connectedOdds.Add(new KeyValuePair<int, float>(1, 0.33f * oddsMultiplier));
        //Add 2.
        if (availableMaxConnected > 1)
            connectedOdds.Add(new KeyValuePair<int, float>(2, 0.33f * oddsMultiplier));
        //Add 3.
        if (availableMaxConnected > 2)
            connectedOdds.Add(new KeyValuePair<int, float>(3, 0.33f * oddsMultiplier));

        int result = 0;
        //Go through the odds and pick one.
        float roll = UnityEngine.Random.Range(0f, 1f);
        float total = 0f;
        //Go through odds list.
        foreach (KeyValuePair<int, float> odds in connectedOdds)
        {
            //Add value of this odds onto total.
            total += odds.Value;
            //See if roll is equal or less to, if so odds are met.
            if (roll <= total)
            {
                //Set result and break from loop.
                result = odds.Key;
                break;
            }
        }

        /* If result wasn't set. Happens when odds total doesn't equal 1f
         * or higher. When this occurs select the value of the last added key,
         * which should be the most difficult. */
        if (result < 1)
            result = connectedOdds[connectedOdds.Count - 1].Key;

        return result;
    }

    /// <summary>
    /// Returns the expected number of tiles to spawn for a specified tile chain type.
    /// </summary>
    /// <param name="tileChainType"></param>
    /// <returns></returns>
    private int ReturnTileChainNumbersCount(TileChainTypes tileChainType)
    {
        switch (tileChainType)
        {
            case TileChainTypes.None:
                return 0;
            case TileChainTypes.Single:
                return 1;
            case TileChainTypes.Double:
                return 2;
            case TileChainTypes.Hook:
                return 3;
            case TileChainTypes.Line:
                return 3;
            case TileChainTypes.Triangle:
                return 3;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Returns the tile chain type for the specified connected and shape fo the joined board tiles.
    /// </summary>
    /// <param name="connected">Tiles connected.</param>
    /// <param name="boardTiles">Board tiles to check the shape of. Isn't required for single and double connected (1 or 2).</param>
    /// <returns></returns>
    private TileChainTypes ReturnTileChainType(int connected, List<BoardTile> boardTiles = null)
    {
        switch (connected)
        {
            case 0:
                return TileChainTypes.None;
            case 1:
                return TileChainTypes.Single;
            case 2:
                return TileChainTypes.Double;
            case 3:
                return ReturnThreeCountTileChainType(boardTiles);
            default:
                return TileChainTypes.None;
        }
    }

    private TileChainTypes ReturnThreeCountTileChainType(List<BoardTile> boardTiles)
    {
        if (boardTiles == null || boardTiles.Count != 3)
        {
            Debug.LogError("TileManager -> ReturnThreeCountTileChainType -> Passed in board tiles are null or the count doesn't equal 3.");
            return TileChainTypes.None;
        }

        //Sort the board tiles left to right, then bottom to top of their world space values.
        boardTiles = boardTiles.OrderBy(x => x.transform.position.x).ThenBy(y => y.transform.position.y).ToList();

        //First check for a line in any direction.
        if (MeetsLineRequirements(boardTiles))
            return TileChainTypes.Line;

        //Check if a triangle.
        if (MeetsTriangleRequirements(boardTiles))
            return TileChainTypes.Triangle;

        //Only possibility left is hook.
        return TileChainTypes.Hook;
    }

    /// <summary>
    /// Checks if three board tiles are in a line. Should not be called without first verifying the board tiles list is of proper length.
    /// </summary>
    /// <param name="boardTiles"></param>
    /// <returns></returns>
    private bool MeetsLineRequirements(List<BoardTile> boardTiles)
    {
        /* To match a line all tiles must be the same direction from one another on a single axis.
         * Since board tiles are sorted from left to right, from bottom to top it's reliable
         * to check the direction between first and second tile, then second and third. */
        Vector3 directionA = boardTiles[1].transform.position - boardTiles[0].transform.position;
        Vector3 directionB = boardTiles[2].transform.position - boardTiles[1].transform.position;
        return Vector3s.ValuesMatch(directionA, directionB, 1f);
    }

    /// <summary>
    /// Checks if three board tiles are in a triangle. Should not be called without first verifying the board tiles list is of proper length.
    /// </summary>
    /// <param name="boardTiles"></param>
    /// <returns></returns>
    private bool MeetsTriangleRequirements(List<BoardTile> boardTiles)
    {
        /* A triangle is expected to have a different Y value between the middle
         * tile and the edge tiles, while the edge tiles should be on the same Y. */
        //bool edgesMatchX = !Floats.ValuesMatch(boardTiles[0].transform.position.x, boardTiles[2].transform.position.x, 1f);
        bool edgesMatchY = Floats.ValuesMatch(boardTiles[0].transform.position.y, boardTiles[2].transform.position.y, 1f);
        bool middleVaries = !Floats.ValuesMatch(boardTiles[0].transform.position.y, boardTiles[1].transform.position.y, 1f);

        if (edgesMatchY && middleVaries)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Saves the current tile to player prefs.
    /// </summary>
    public void SaveTile(SaveTypes saveType, bool gameOver = false)
    {
        string result;

        //If trying to save an empty tile chain collection or game is over.
        if (CurrentTileChainCollection == null || gameOver)
        {
            result = string.Empty;
        }
        //Not game over and current tile chain collection is not null.
        else
        {
            //If current tile chain is null; shouldn't be possible.
            if (CurrentTileChainCollection.CurrentTileChain == null)
            {
                Debug.LogError("TileManager -> SaveTile -> CurrentTileChain is null.");
                return;
            }

            StringBuilder tileNumbers = new StringBuilder();

            string text;
            //The first text in tileNumbers should be the chain type.
            int chainTypeNumeric = (int)CurrentTileChainCollection.CurrentTileChain.TileChainType;
            text = Strings.PadLeft(chainTypeNumeric.ToString(), 2, "0");
            //Append chain type to string builder.
            tileNumbers.Append(text);
            //Go through each numbered tile in order.
            for (int i = 0; i < CurrentTileChainCollection.CurrentTileChain.NumberedTiles.Count; i++)
            {
                NumberedTile numberedTile = CurrentTileChainCollection.CurrentTileChain.NumberedTiles[i];
                int numericValue = (int)numberedTile.TileValue;
                //Pad the numbered tile into text.
                text = Strings.PadLeft(numericValue.ToString(), 2, "0");
                //Add onto string builder.
                tileNumbers.Append(text);
            }

            //Set result.
            result = tileNumbers.ToString();
        }

        string saveName = ReturnTileSavePath();
        //Get the current save before saving and apply it as previous save.
        PreviousTileSave = PlayerPrefs.GetString(saveName);
        //Write result to player prefs.
        PlayerPrefs.SetString(saveName, result);

        MasterManager.SaveManager.HandleSaveType(saveType);
    }

    /// <summary>
    /// Returns the save path, or save name for the tile. Varies based on current game mode.
    /// </summary>
    /// <returns></returns>
    public string ReturnTileSavePath()
    {
        //Get save name for gamemode type.
        int gameModeNumeric = (int)MasterManager.GameManager.Settings.GameMode;
        return (TILES_SAVE + gameModeNumeric.ToString());
    }

}
