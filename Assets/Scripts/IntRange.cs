using UnityEngine;

[System.Serializable]
public class IntRange
{
    public IntRange(int start, int end)
    {
        Start = start;
        End = end;
    }

    public int Start = 0;
    public int End = 1;
}