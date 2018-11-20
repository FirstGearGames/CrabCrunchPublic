/// <summary>
/// Details about the current game. Typically can be used to quickly reset a game by instantiating a new copy of this.
/// </summary>
public class CurrentGame
{
    /// <summary>
    /// Score of the current game.
    /// </summary>
    public int Score = 0;
    /// <summary>
    /// Highest placed tile for the current game.
    /// </summary>
    public int HighestPlacedTile = 2;
}