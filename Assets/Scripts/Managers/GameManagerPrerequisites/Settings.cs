
public class Settings
{
    #region Public.
    /// <summary>
    /// True if music is enabled.
    /// </summary>
    public bool MusicEnabled = true;
    /// <summary>
    /// True if sound effects are enabled.
    /// </summary>
    public bool SoundEffectsEnabled = true;
    /// <summary>
    /// Volume of music.
    /// </summary>
    public float MusicVolume = 0.25f;
    /// <summary>
    /// Volume of sound effects.
    /// </summary>
    public float SoundEffectsVolume = 0.75f;
    /// <summary>
    /// Current game mode.
    /// </summary>
    public GameModes GameMode = GameModes.Normal;
    #endregion

    #region Const and readonly.
    /// <summary>
    /// Default volume for music.
    /// </summary>
    public const float MUSIC_VOLUME = 0.25f;
    /// <summary>
    /// Default volume for sound effects.
    /// </summary>
    public const float SOUND_EFFECTS_VOLUME = 0.75f;
    #endregion
}
