using UnityEngine;

[System.Serializable]
public class FloatRange
{
    public FloatRange(float start, float end)
    {
        Start = start;
        End = end;
    }

    public float Start = 0f;
    public float End = 1f;
}