using UnityEngine;

public class LoseCanvas : MonoBehaviour
{
 
    /// <summary>
    /// Called when the Retry buton is clicked.
    /// </summary>
    public void OnClick_Retry()
    {
        //Reload a save, which should be an empty game if on the lose screen.
        MasterManager.GameManager.LoadLastSaved();
        //Hide this canvas.
        MasterManager.CanvasManager.LoseCanvas.gameObject.SetActive(false);
    }
	
}
