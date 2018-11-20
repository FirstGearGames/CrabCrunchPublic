using UnityEngine;

public class TitleCanvas : MonoBehaviour
{


    #region Private.
    /// <summary>
    /// The last game mode loaded. Is only set after pressing play.
    /// </summary>
    private GameModes m_lastLoadedGameMode = GameModes.Unset;
    #endregion

    #region Const and readonly.
    private const string GAMEMODE_SAVE = "GameMode";
    #endregion
    
    private void Start()
    {
        HighlightCurrentGameModeButton();
    }
    
    /// <summary>
    /// Shows the title canvas and initializes it.
    /// </summary>
    public void Show()
    {
        //Set that the title canvas is visible.
        MasterManager.GameManager.States.TitleCanvasVisible = true;
        //Show.
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the title canvas and deinitializes it.
    /// </summary>
    private void Hide()
    {
        //Hide all potentially open menus. None should be open, but just in case.
        MasterManager.CanvasManager.MenuCanvas.HideAllMenus();
        //Unset that the title canvas is visible.
        MasterManager.GameManager.States.TitleCanvasVisible = false;
        //Hide object.
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when play game is clicked.
    /// </summary>
    public void PlayGameClicked()
    {
        //If the game mode has been changed.
        if (MasterManager.GameManager.Settings.GameMode != m_lastLoadedGameMode)
        {
            m_lastLoadedGameMode = MasterManager.GameManager.Settings.GameMode;
            //Load last saved data for current game mode.
            MasterManager.GameManager.LoadLastSaved();
        }

        Hide();
    }

    /// <summary>
    /// Highlights the game mode button for the currently selected game mode.
    /// </summary>
    private void HighlightCurrentGameModeButton()
    {
        //Get the current gamemode.
        int gameModeNumeric = PlayerPrefs.GetInt(GAMEMODE_SAVE, 0);
        GameModes gameMode = (GameModes)gameModeNumeric;
        //Select the loaded game mode.
        SelectGameMode(gameMode);
    }

    /// <summary>
    /// Selects a specified game mode, applies it to settings, and highlights the proper gamemode button.
    /// </summary>
    /// <param name="gameMode">GameMode to set.</param>
    public void SelectGameMode(GameModes gameMode, bool saveGameMode = false)
    {
        //If to save to file.
        if (saveGameMode)
            PlayerPrefs.SetInt(GAMEMODE_SAVE, (int)gameMode);

        //Set the value to the game states.
        MasterManager.GameManager.Settings.GameMode = gameMode;

        //Find all buttons.
        GameModeButton[] buttons = GetComponentsInChildren<GameModeButton>();
        //If no buttons found.
        if (buttons.Length == 0)
        {
            Debug.Log("TileCanvas -> HighlightGameModeButton -> Couldn't find any GameModeButton scripts.");
            return;
        }
        /* Go through each button and highlight the proper one if the right
         * one highlight it, otherwise remove highlight. */
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].GameMode == gameMode)
                buttons[i].Highlight();
            else
                buttons[i].RemoveHighlight();
        }
    }

    /// <summary>
    /// Called when the settings button is clicked.
    /// </summary>
    public void OnClick_Settings()
    {
        MasterManager.CanvasManager.MenuCanvas.ShowSettings();
    }
}
