using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    #region Serialized.
    /// <summary>
    /// Prefab to show over a tile which is matched.
    /// </summary>
    [Tooltip("Prefab to show over a tile which is matched.")]
    [SerializeField]
    private GameObject m_matchTileFXPrefab;
    /// <summary>
    /// Prefab to show for floating text.
    /// </summary>
    [Tooltip("Prefab to show for floating text.")]
    [SerializeField]
    private GameObject m_floatingTextPrefab;

    [SerializeField]
    private AudioSource m_bubblingAudio;
    [SerializeField]
    private AudioSource m_tilePoppedAudio;
    #endregion

    /// <summary>
    /// Spawns a matched made prefab over a specified board tile.
    /// </summary>
    /// <param name="boardTile"></param>
    public void SpawnMatchEffect(BoardTile boardTile)
    {
        Vector3 matchedTilePosition = boardTile.transform.position;
        //Instantiate the prefab and scale it.
        GameObject obj = Instantiate(m_matchTileFXPrefab, matchedTilePosition - new Vector3(0f, 0f, 10f), Quaternion.identity, null);
        float scale = MasterManager.BoardManager.BoardTileScale;
        obj.transform.localScale = new Vector3(scale, scale, 1f);

        AudioSource audioSource = obj.GetComponent<AudioSource>();
        //If audio source is found, which it should be.
        if (audioSource != null)
        {
            //Change the pitch based on tile value.
            audioSource.pitch = 1f + (0.25f * (int)boardTile.TileValue);
        }
    }

    /// <summary>
    /// Spawns a floating text prefab with specified data.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="speed"></param>
    /// <param name="direction"></param>
    /// <param name="startPosition"></param>
    /// <param name="distance"></param>
    /// <param name="fontSize"></param>
    /// <param name="color"></param>
    public void SpawnFloatingText(string text, float speed, Vector3 direction, Vector3 startPosition, float distance, float fontSize, Color32 color)
    {
        GameObject obj = Instantiate(m_floatingTextPrefab, startPosition, Quaternion.identity, null);
        TextEffect textEffect = obj.GetComponent<FloatingText>();

        textEffect.FloatText(text, speed, startPosition, direction, distance, fontSize, color);
    }

    /// <summary>
    /// Called when a match starts to be made. Plays continual audio until PlayLastPopped is called.
    /// </summary>
    public void StartBubblingAudio()
    {
        if (m_bubblingAudio.enabled)
            m_bubblingAudio.Play();
    }

    /// <summary>
    /// Stops the bubbling audio.
    /// </summary>
    public void StopBubblingAudio()
    {
        if (m_bubblingAudio.enabled)
            m_bubblingAudio.Stop();
    }

    /// <summary>
    /// Call when the final tile is matched.
    /// </summary>
    /// <param name="tileValue">Value of the tile being popped. Used to alter pitch of audio.</param>
    public void PlayPopAudio(TileValues tileValue)
    {
        //Play last popped audio if enabled.
        if (m_tilePoppedAudio.enabled)
        {
            m_tilePoppedAudio.pitch = 1f + (0.25f * (int)tileValue);
            m_tilePoppedAudio.Play();
        }
    }

    public void PlayCrunchAudio()
    {
        print("CRUNCHY");
    }
}
