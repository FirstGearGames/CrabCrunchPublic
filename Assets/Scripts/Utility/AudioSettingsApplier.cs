using UnityEngine;

/// <summary>
/// Applies audio settings to audio sources for this object and potentially child objects based on settings.
/// </summary>
public class AudioSettingsApplier : MonoBehaviour
{
    /// <summary>
    /// Type of audio state from settings to use.
    /// </summary>
    [System.Serializable]
    private enum AudioTypes
    {
        Music,
        SoundEffects
    }

    /// <summary>
    /// Type of audio to set the states for.
    /// </summary>
    [Tooltip("Type of audio to set the states for.")]
    [SerializeField]
    private AudioTypes m_audioType = AudioTypes.SoundEffects;
    /// <summary>
    /// True to also set audio settings for children.
    /// </summary>
    [Tooltip("True to also set audio settings for children.")]
    [SerializeField]
    private bool m_includeChildren = true;

    /// <summary>
    /// True to play on wake if enabled within settings. Applies to all audio sources found by this script.
    /// </summary>
    [Tooltip("True to play on wake if enabled within settings. Applies to all audio sources found by this script.")]
    [SerializeField]
    private bool m_playOnWake = false;

    private void Start()
    {
        ApplyAudioSettings();
    }

    /// <summary>
    /// Apply audio settings to audio sources.
    /// </summary>
    public void ApplyAudioSettings()
    {
        bool audioEnabled = true;
        if (m_audioType == AudioTypes.SoundEffects)
            audioEnabled = MasterManager.GameManager.Settings.SoundEffectsEnabled;
        else if (m_audioType == AudioTypes.Music)
            audioEnabled = MasterManager.GameManager.Settings.MusicEnabled;

        //Will be used to store audio sources to change.
        AudioSource[] audioSources = new AudioSource[1];

        //If to include children.
        if (m_includeChildren)
            audioSources = GetComponentsInChildren<AudioSource>();
        else
            audioSources[0] = GetComponent<AudioSource>();

        //Go through each audio source.
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i] != null)
            { 

                audioSources[i].enabled = audioEnabled;

                //If enabled.
                if (audioEnabled)
                {
                    //Set volume.
                    float volume = 0f;
                    if (m_audioType == AudioTypes.Music)
                        volume = Settings.MUSIC_VOLUME;
                    else if (m_audioType == AudioTypes.SoundEffects)
                        volume = Settings.SOUND_EFFECTS_VOLUME;
                    audioSources[i].volume = volume;

                    //If play on wake and not already playing.
                    if (m_playOnWake && !audioSources[i].isPlaying)
                        audioSources[i].Play();
                }
            }
        }
    }
}
