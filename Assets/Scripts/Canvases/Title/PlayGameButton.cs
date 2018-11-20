using UnityEngine;

public class PlayGameButton : MonoBehaviour
{

    /// <summary>
    /// Called when the play game button is clicked.
    /// </summary>
    public void OnClick_PlayGame()
    {
        MasterManager.CanvasManager.TitleCanvas.PlayGameClicked();
    }
	
}
