using UnityEngine;

public static class Debugs
{
    
    /// <summary>
    /// Logs an error message using the gameobject name, it's parent name, and a message.
    /// </summary>
    /// <param name="transform">Gameobject name to log, and to find parent of.</param>
    /// <param name="message">Message to log.</param>
    public static string ReturnErrorString(Transform transform, string message)
    {
        return (transform.name + "(" + transform.root.name + ") " + message);
    }
}
