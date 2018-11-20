using UnityEngine;

public class LevelCanvas : MonoBehaviour
{
    #region Serialized.
    /// <summary>
    /// Reference to the RollingNumber component to the current score text.
    /// </summary>
    [Tooltip("Reference to the rolling number component to the current score text.")]
    [SerializeField]
    private RollingNumber m_currentScoreTextEffect;
    /// <summary>
    /// Reference to the RollingNumber component to the highscore text.
    /// </summary>
    [Tooltip("Reference to the rolling number component to the highscore text.")]
    [SerializeField]
    private RollingNumber m_highScoreTextEffect;
    /// <summary>
    /// Reference to the RollingNumber component to the credits text.
    /// </summary>
    [Tooltip("Reference to the rolling number component to the credits text.")]
    [SerializeField]
    private RollingNumber m_creditsTextEffect;
    #endregion

    /// <summary>
    /// Sets the current score text.
    /// </summary>
    /// <param name="value">Value to set.</param>
    public void SetCurrentScore(int value, float duration)
    {
        m_currentScoreTextEffect.SetValue(value);
    }

    /// <summary>
    /// Sets the high score text.
    /// </summary>
    /// <param name="value">Value to set.</param>
    public void SetHighScore(int value)
    {
        m_highScoreTextEffect.SetValue(value);
    }

    /// <summary>
    /// Sets the credits text.
    /// </summary>
    /// <param name="value"></param>
    public void SetCredits(int value)
    {
        m_creditsTextEffect.SetValue(value);
    }

    /// <summary>
    /// Rolls the current score text.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="duration">Time to roll the new text.</param>
    public void RollCurrentScore(int value, float duration)
    {
        m_currentScoreTextEffect.RollValue(value, duration);
    }

    /// <summary>
    /// Rolls the high score text.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="duration">Time to roll the new text.</param>
    public void RollHighScore(int value, float duration)
    {
        m_highScoreTextEffect.RollValue(value, duration);
    }

    /// <summary>
    /// Rolls the credits text.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="duration">Time to roll the new text.</param>
    public void RollCredts(int value, float duration)
    {
        m_creditsTextEffect.RollValue(value, duration);
    }

    /// <summary>
    /// Called when pause is clicked.
    /// </summary>
    public void PauseClicked()
    {
        //Show the title canvas on pause.
        MasterManager.CanvasManager.TitleCanvas.Show();
    }
}
