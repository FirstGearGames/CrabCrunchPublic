using UnityEngine;

public class RollingNumber : TextEffect
{
    #region Private.
    /// <summary>
    /// Rate in which the text will travel.
    /// </summary>
    private float m_speed;
    /// <summary>
    /// Becomes true when text should be floating.
    /// </summary>
    private bool m_enabled = false;
    /// <summary>
    /// Number in which the roll started on.
    /// </summary>
    private int m_startNumber;
    /// <summary>
    /// Number to end rolling on.
    /// </summary>
    private int m_endNumber;
    /// <summary>
    /// Time in which the rolling text has begun. Used to lerp between numbers.
    /// </summary>
    private float m_startTime;
    /// <summary>
    /// Time in which the rolling text should end. Used to lerp between numbers.
    /// </summary>
    private float m_endTime;
    #endregion

    private void Update()
    {
        //If not enabled 
        if (!m_enabled)
            return;

        //Get percent of duration passed.
        float percentPassed = Mathf.InverseLerp(m_startTime, m_endTime, Time.time);
        //Set text according to that percent.
        base.TextMeshPro.text = Mathf.FloorToInt(Mathf.Lerp(m_startNumber, m_endNumber, percentPassed)).ToString();

        //If percent is 1f or higher destroy object.
        if (percentPassed >= 1f)
            m_enabled = false;
    }


    /// <summary>
    /// Instantly sets the text value, rather than rolling it.
    /// </summary>
    /// <param name="value">Value to set to.</param>
    public override void SetValue(int value)
    {
        base.TextMeshPro.text = value.ToString();
        //Stop rolling update if active.
        m_enabled = false;
    }

    /// <summary>
    /// Rolls text to a value over time.
    /// </summary>
    /// <param name="value">Value to roll to.</param>
    /// <param name="duration">Time it takes to roll to value.</param>
    public override void RollValue(int value, float duration)
    {
        int result;
        //If able to succesfully parse the text into an int.
        if (int.TryParse(base.TextMeshPro.text, out result))
        {
            m_startNumber = result;
            m_endNumber = value;
            m_startTime = Time.time;
            m_endTime = Time.time + duration;
            m_enabled = true;
        }
        //Couldn't parse into int.
        else
        {
            Debug.LogError(Debugs.ReturnErrorString(transform, " -> RollText -> Unable to parse into int."));
        }
    }


}
