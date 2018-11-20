public class GameStates
{
    /// <summary>
    /// True if there is animating which should block input.
    /// </summary>
    public bool Animating = false;
    /// <summary>
    /// True if any menu canvas is visible.
    /// </summary>
    public bool MenuCanvasVisible = false;
    /// <summary>
    /// True if the title screen is up. Always starts as enabled by default.
    /// </summary>
    public bool TitleCanvasVisible = true;
    /// <summary>
    /// Number of credits player has.
    /// </summary>
    public int Credits;
    /// <summary>
    /// Highscore for the current game type.
    /// </summary>
    public int Highscore = 0;
    /// <summary>
    /// Becomes true when attempting to match tiles.
    /// </summary>
    public bool Matching = false;
    /// <summary>
    /// Returns if non-ui elements may receive input.
    /// </summary>
    public bool GameInputBlocked
    {
        get
        {
            if (MenuCanvasVisible)
                return true;
            if (Animating)
                return true;
            if (TitleCanvasVisible)
                return true;
            if (Matching)
                return true;

            return false;
        }
    }

}
