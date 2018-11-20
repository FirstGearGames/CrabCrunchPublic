using UnityEngine;

public class GameModeButton : MonoBehaviour
{
    /// <summary>
    /// Use GameMode.
    /// </summary>
    [Tooltip("The gamemode this button represents.")]
    [SerializeField]
    private GameModes m_gameMode;
    /// <summary>
    /// The gamemode this button represents.
    /// </summary>
    public GameModes GameMode
    {
        get { return m_gameMode; }
    }

    /// <summary>
    /// Called when this button is clicked.
    /// </summary>
    /// <param name="gameMode"></param>
    public void OnClick_SelectGameMode(int gameMode)
    {
        MasterManager.CanvasManager.TitleCanvas.SelectGameMode(GameMode, true);
    }

    /// <summary>
    /// Highlight this game mode button to indicate that it's selected.
    /// </summary>
    public void Highlight()
    {
        transform.localScale = Vector3.one * 1.25f;
    }

    /// <summary>
    /// Removes the highlight on this game mode button to show that it's deselected.
    /// </summary>
    public void RemoveHighlight()
    {
        transform.localScale = Vector3.one;
    }

}
