using UnityEngine;

public class PauseButton : MonoBehaviour
{
    /// <summary>
    /// Called when the pause game button is pressed.
    /// </summary>
    public void OnClick_PauseGame()
    {
        MasterManager.CanvasManager.LevelCanvas.PauseClicked();
    }
}
