using FirstGearGames.Global.Structures;
using UnityEngine;

public class FloatingText : TextEffect
{
    #region Private.
    /// <summary>
    /// Position where text started. Used to lerp text between points.
    /// </summary>
    private Vector3 m_startPosition;
    /// <summary>
    /// Position where the text will end. Used to lerp text between points.
    /// </summary>
    private Vector3 m_endPosition;
    /// <summary>
    /// Rate in which the text will travel.
    /// </summary>
    private float m_speed;
    /// <summary>
    /// Becomes true when text should be floating.
    /// </summary>
    private bool m_enabled = false;
    /// <summary>
    /// Halfway point between start and end of floating positions. Used to determine when to begin fading text.
    /// </summary>
    private Vector3 m_halfDistance;
    #endregion

    private void Update()
    {
        PerformFloating();
    }

    /// <summary>
    /// Floats the text upward over time.
    /// </summary>
    private void PerformFloating()
    {
        //If not yet enabled.
        if (!m_enabled)
            return;

        //Move towards the end position.
        transform.position = Vector3.MoveTowards(transform.position, m_endPosition, m_speed * Time.deltaTime);

        //Determine progress between middle point and end. Returns 0f if not at least to half way.
        float progress = Vector3s.InverseLerp(m_halfDistance, m_endPosition, transform.position);
        //If enough progress has been made to begin fading.
        if (progress > 0f)
            base.TextMeshPro.color = new Color(base.TextMeshPro.color.r, base.TextMeshPro.color.g, base.TextMeshPro.color.b, (1f - progress));

        //If at the end position destroy object.
        if (transform.position == m_endPosition)
            Destroy(gameObject);
    }

    /// <summary>
    /// Floats text using custom options.
    /// </summary>
    /// <param name="text">Text to float.</param>
    /// <param name="speed">Speed in which to float.</param>
    /// <param name="startPosition">Position where text starts.</param>
    /// <param name="direction">Direction to float.</param>
    /// <param name="distance">Distance to float.</param>
    /// <param name="fontSize">Size of font for  text.</param>
    /// <param name="color">Color of text.</param>
    public override void FloatText(string text, float speed, Vector3 startPosition, Vector3 direction, float distance, float fontSize, Color32 color)
    {
        m_startPosition = startPosition;
        transform.position = m_startPosition;
        m_endPosition = m_startPosition + (direction * distance);
        m_halfDistance = m_startPosition + (direction * (distance * 0.5f));
        m_speed = speed;
        base.TextMeshPro.color = color;
        base.TextMeshPro.text = text;
        base.TextMeshPro.fontSize = fontSize;
        m_enabled = true;
    }


}


