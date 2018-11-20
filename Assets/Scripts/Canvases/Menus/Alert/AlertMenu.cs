using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertMenu : MonoBehaviour
{
    /// <summary>
    /// The text component used to display the alert text, found within the children of this object.
    /// </summary>
    [Tooltip("The text component used to display the alert text, found within the children of this object.")]
    [SerializeField]
    private TextMeshProUGUI m_textMeshPro;

    /// <summary>
    /// Sets the alert message for this menu.
    /// </summary>
    /// <param name="message"></param>
    public void SetAlertText(string message)
    {
        m_textMeshPro.text = message;
    }

    /// <summary>
    /// Called when the exit button is clicked for this menu.
    /// </summary>
    public void OnClick_CloseMenu()
    {
        MasterManager.CanvasManager.MenuCanvas.HideAlert();
    }

}
