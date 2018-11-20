using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Public.
    /// <summary>
    /// Reference to a settings class which holds user preferences about the game.
    /// </summary>
    [HideInInspector]
    public Settings Settings = new Settings();
    /// <summary>
    /// Reference to GameStates class which Holds various data about the game and it's activity states.
    /// </summary>
    [HideInInspector]
    public GameStates States = new GameStates();
    /// <summary>
    /// Data relating to the current game.
    /// </summary>
    private CurrentGame m_currentGame = new CurrentGame();
    [HideInInspector]
    ///Information about the current game.
    public CurrentGame CurrentGame
    {
        get { return m_currentGame; }
        private set { m_currentGame = value; }
    }
    #endregion

    #region Const and readonly.
    /// <summary>
    /// Minimum time between saving files when not using force save.
    /// </summary>
    private const float SAVE_INTERVAL = 5f;
    /// <summary>
    /// String to look up for highscore saves.
    /// </summary>
    private const string HIGHSCORE_SAVE = "Highscore";
    /// <summary>
    /// String to look up for current score saves.
    /// </summary>
    private const string CURRENTSCORE_SAVE = "Currentscore";
    /// <summary>
    /// String to look up for credits save.
    /// </summary>
    private const string CREDITS_SAVE = "Credits";
    /// <summary>
    /// String to look up for music enabled save.
    /// </summary>
    public const string MUSIC_ENABLED_SAVE = "MusicEnabled";
    /// <summary>
    /// String to look up for sound effects enabled save.
    /// </summary>
    public const string SOUND_EFFECTS_ENABLED_SAVE = "SoundEffectsSave";
    #endregion


    private void Awake()
    {
        LoadAudioSettings();
        LoadCredits();
        PreserveAlphaTester();
    }

    /// <summary>
    /// Loads the last saved game for the current. Can be called when pressing retry after losing, or when a game initially starts.
    /// </summary>
    public void LoadLastSaved()
    {
        print("Loading last saved.");
        //Reset current game information.
        CurrentGame = new CurrentGame();
        //Destroy current tile.
        if (MasterManager.TileManager.CurrentTileChainCollection != null)
            DestroyImmediate(MasterManager.TileManager.CurrentTileChainCollection.gameObject);
        //Rebuild the grid.
        MasterManager.BoardManager.BuildGrid();
        //Set the highscore for the current game mode.        
        LoadHighScore();
        //Set the currentscore for the current game mode.
        LoadCurrentScore();
        MasterManager.BoardManager.LoadSavedBoard();
        MasterManager.TileManager.LoadSavedTiles();
    }
    /// <summary>
    /// Set that the player is an alpha tester. Will give them free upgrades later.
    /// </summary>
    private void PreserveAlphaTester()
    {
        PlayerPrefs.SetInt("AlphaTester", 1);
        MasterManager.SaveManager.SavePlayerPrefs(true);
    }

    /// <summary>
    /// Loads all settings related to audio.
    /// </summary>
    private void LoadAudioSettings()
    {
        print("Loading audio settings.");
        //Load enabled state.
        Settings.MusicEnabled = Convert.ToBoolean(PlayerPrefs.GetInt(MUSIC_ENABLED_SAVE, 1));
        Settings.SoundEffectsEnabled = Convert.ToBoolean(PlayerPrefs.GetInt(SOUND_EFFECTS_ENABLED_SAVE, 1));
    }

    /// <summary>
    /// Loads the players highscore for the selected game mode type.
    /// </summary>
    private void LoadHighScore()
    {
        print("Loading highscore.");
        States.Highscore = PlayerPrefs.GetInt(HIGHSCORE_SAVE + (int)Settings.GameMode, 0);
        MasterManager.CanvasManager.LevelCanvas.SetHighScore(States.Highscore);
    }

    /// <summary>
    /// Loads the players current score for the selected game mode type.
    /// </summary>
    private void LoadCurrentScore()
    {
        print("Loading current score.");
        CurrentGame.Score = PlayerPrefs.GetInt(CURRENTSCORE_SAVE + (int)Settings.GameMode, 0);
        MasterManager.CanvasManager.LevelCanvas.SetCurrentScore(CurrentGame.Score, 0f);
    }

    /// <summary>
    /// Loads the players credits.
    /// </summary>
    private void LoadCredits()
    {
        States.Credits = PlayerPrefs.GetInt(CREDITS_SAVE, 0);
        MasterManager.CanvasManager.LevelCanvas.SetCredits(States.Credits);
    }

    /// <summary>
    /// Changes audio for objects which may be persistent and affected by audio settings.
    /// </summary>
    public void AssignLoadedAudioSettings()
    {
        //Find all audio settings appliers in scene.
        AudioSettingsApplier[] audioSettingsAppliers = GameObject.FindObjectsOfType<AudioSettingsApplier>();
        //Go through each found and apply settings.
        foreach (AudioSettingsApplier applier in audioSettingsAppliers)
        {
            applier.ApplyAudioSettings();
        }
    }

    /// <summary>
    /// Updates the players highscore to file.
    /// </summary>
    /// <param name="value">New high score.</param>
    public void SetHighScore(int value, bool savePlayerPrefs, bool forceSave)
    {
        States.Highscore = value;
        int gameModeNumeric = (int)MasterManager.GameManager.Settings.GameMode;
        PlayerPrefs.SetInt(HIGHSCORE_SAVE + gameModeNumeric.ToString(), States.Highscore);
        //If to save prefs, save while passing in force save.
        if (savePlayerPrefs)
            MasterManager.SaveManager.SavePlayerPrefs(forceSave);
    }

    /// <summary>
    /// Updates the players currentscore to file.
    /// </summary>
    /// <param name="value">New current score.</param>
    public void SetCurrentScore(int value, bool savePlayerPrefs, bool forceSave)
    {
        int gameModeNumeric = (int)MasterManager.GameManager.Settings.GameMode;
        PlayerPrefs.SetInt(CURRENTSCORE_SAVE + gameModeNumeric.ToString(), value);
        //If to save prefs, save while passing in force save.
        if (savePlayerPrefs)
            MasterManager.SaveManager.SavePlayerPrefs(forceSave);
    }

    /// <summary>
    /// Adds onto the players current score.
    /// </summary>
    /// <param name="value">Value to add onto the current score.</param>
    /// <param name="roll">True to roll text, false to set it immediately.</param>
    /// <param name="compareToHighScore">True to compare to high score.</param>
    public void AddCurrentScore(int value, bool compareToHighScore)
    {
        float rollTime = 1f;
        //Add value to current score.
        CurrentGame.Score += value;

        /* If current score is higher than high score then set high score
         * and is then set high score and save. */
        if (compareToHighScore && CurrentGame.Score > States.Highscore)
        {
            //Set highscore in settings while also default saving.
            SetHighScore(CurrentGame.Score, true, false);
            //Roll highscore.
            MasterManager.CanvasManager.LevelCanvas.RollHighScore(States.Highscore, rollTime);
        }

        //Set current score in settings.
        SetCurrentScore(CurrentGame.Score, true, false);
        //Roll new score.
        MasterManager.CanvasManager.LevelCanvas.RollCurrentScore(CurrentGame.Score, rollTime);
    }

    /// <summary>
    /// Adds to the players credits. Can also subtract when using negative numbers. Will never go below 0.
    /// </summary>
    /// <param name="value">Value to add onto credits.</param>
    public void AddCredits(int value, SaveTypes saveType = SaveTypes.ForceSave)
    {
        //Add to states so that the new credits may be accessed globally.
        States.Credits += value;
        //Make sure credits don't go below 0. Shouldn't ever be possible anyway.
        if (States.Credits < 0)
            States.Credits = 0;
        //Save to player prefs.
        SaveCredits(saveType);
        //Roll credits.
        MasterManager.CanvasManager.LevelCanvas.RollCredts(States.Credits, 1f);
    }

    /// <summary>
    /// Saves credits to player prefs.
    /// </summary>
    /// <param name="saveType">Type of way to handle saving.</param>
    private void SaveCredits(SaveTypes saveType)
    {
        //Write to player prefs.
        PlayerPrefs.SetInt(CREDITS_SAVE, States.Credits);
        //Handle the save.
        MasterManager.SaveManager.HandleSaveType(saveType);
    }

    /// <summary>
    /// Updates the highest placed tile to the max of it's current value verus values of specified board tiles.
    /// </summary>
    /// <param name="boardTiles">Board tiles to compare highest placed tile against.</param>
    public void UpdateHighestPlacedTile(List<BoardTile> boardTiles)
    {
        for (int i = 0; i < boardTiles.Count; i++)
        {
            UpdateHighestPlacedTile(boardTiles[i]);
        }
    }

    /// <summary>
    /// Updates the highest placed tile to the max of it's current value verus value of the specified board tile.
    /// </summary>
    /// <param name="boardTile">Board tile to compare highest placed tile against.</param>
    public void UpdateHighestPlacedTile(BoardTile boardTile)
    {
        CurrentGame.HighestPlacedTile = Math.Max((int)boardTile.TileValue, CurrentGame.HighestPlacedTile);
    }


    /// <summary>
    /// Called when a turn has ended and after a new tile has been requested.
    /// </summary>
    public void TurnEnded()
    {
        /* If not matching and there is no current tile chain collection
         * then a tile could not be spawned, which means the game
         * has ended due to a full board. */
        if (MasterManager.TileManager.CurrentTileChainCollection == null && !States.Matching)
            TurnEndedGameEnded();
        //Game hasn't ended yet.
        else
            TurnEndedGameContinues();

        /* Player may have gained or spent credits during the turn, or 
         * performed actions which will block or allow powerups. Go through
         * the powerups and update their visuals to reflect if they can be used. */
        MasterManager.PowerupManager.CheckPowerupConditions(false);
    }

    /// <summary>
    /// Saves changes after a turn. Only to be called when the game has not yet ended.
    /// </summary>
    private void TurnEndedGameContinues()
    {
        //Save grid changes and latest tile spawned.
        MasterManager.BoardManager.SaveGrid(SaveTypes.WriteOnly);
        MasterManager.TileManager.SaveTile(SaveTypes.WriteOnly);
        //Process save.
        MasterManager.SaveManager.HandleSaveType(SaveTypes.NormalSave);
    }

    /// <summary>
    /// Called when the player loses and a game has ended.
    /// </summary>
    private void TurnEndedGameEnded()
    {
        Debug.Log("Game has ended.");
        //Save grid and tile as an empty by passing in game over as true.
        MasterManager.BoardManager.SaveGrid(SaveTypes.WriteOnly, true);
        MasterManager.TileManager.SaveTile(SaveTypes.WriteOnly, true);
        //Process a force save.
        MasterManager.SaveManager.HandleSaveType(SaveTypes.ForceSave);
        //Show lose canvas.
        MasterManager.CanvasManager.LoseCanvas.gameObject.SetActive(true);
        //Tell ad manager a game was lost.
        MasterManager.AdManager.GameLost();
    }
}
