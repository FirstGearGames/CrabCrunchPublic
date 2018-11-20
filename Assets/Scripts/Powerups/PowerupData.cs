using UnityEngine;

/// <summary>
/// Holds data about each powerup cost.
/// </summary>
[System.Serializable]
public class PowerupData
{
    /// <summary>
    /// Type of powerup.
    /// </summary>
    public PowerupTypes Type = PowerupTypes.Unset;
    /// <summary>
    /// How the powerup reacts when interacted with.
    /// </summary>
    public PowerupInteractionsTypes InteractionType = PowerupInteractionsTypes.Unset;
    /// <summary>
    /// Cost of the specified powerup type.
    /// </summary>
    public int Cost = 100;
    /// <summary>
    /// Duration to block interaction with all powerups when this powerup is interacted with.
    /// </summary>
    public float InteractedBlockDuration = 0.5f;
}

