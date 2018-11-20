/// <summary>
/// How to handle saves after writing to player prefs.
/// </summary>
public enum SaveTypes
{
    /// <summary>
    /// Writes to player prefs and nothing more.
    /// </summary>
    WriteOnly,
    /// <summary>
    /// Writes to player prefs and saves if enough time has elapsed. If not, saves after a short duration.
    /// </summary>
    NormalSave,
    /// <summary>
    /// Writes to player prefs and saves immediately.
    /// </summary>
    ForceSave    
}
