using UnityEngine;


[CreateAssetMenu(fileName = "NewFishData", menuName = "Fish/New Fish Data", order = 1)]
public class FishData : ScriptableObject
{
    #region Public.
    /// <summary>
    /// How quickly rotate torwards moving directions.
    /// </summary>
    [Tooltip("How quickly rotate torwards moving directions.")]
    public float RotateSpeed = 5f;
    /// <summary>
    /// Average swim speed of the fish. Can vary in either direction by 25%.
    /// </summary>
    [Tooltip("Average swim speed of the fish. Can vary in either direction by 25%.")]
    public float AverageSwimSpeed = 5f;
    /// <summary>
    /// Average swim distance of the fish. Can vary in either direction by 50%.
    /// </summary>
    [Tooltip("Average swim distance of the fish. Can vary in either direction by 50%.")]
    public float AverageSwimDistance = 100f;
    /// <summary>
    /// Image used for the fish.
    /// </summary>
    [Tooltip("Image used for the fish.")]
    public Sprite Image;
    /// <summary>
    /// How small or large the fish may become in scale based on it's depth.
    /// </summary>
    [Tooltip("How small or large the fish may become in scale based on it's depth.")]
    public FloatRange ScaleRange = new FloatRange(0.25f, 0.75f);
    /// <summary>
    /// Screen height percentage which fish may occupy.
    /// </summary>
    [Tooltip("Screen height percentage which fish may occupy.")]
    public SwimRange SwimHeightRange;
    #endregion
}
