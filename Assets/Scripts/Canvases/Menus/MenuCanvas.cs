using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCanvas : MonoBehaviour
{
    /// <summary>
    /// Reference to the SettingsMenu object within this objects children.
    /// </summary>
    [Tooltip("Reference to the SettingsMenu object within this objects children.")]
    [SerializeField]
    private GameObject m_settingsMenu;
    /// <summary>
    /// Reference to the AlertMenu script within this objects children.
    /// </summary>
    [Tooltip("Reference to the AlertMenu script within this objects children.")]
    [SerializeField]
    private AlertMenu m_alertMenu;


    /// <summary>
    /// Hides all menus beneath this object.
    /// </summary>
    public void HideAllMenus()
    {
        HideSettings();
    }

    /// <summary>
    /// Shows the settings menu.
    /// </summary>
    public void ShowSettings()
    {
        MasterManager.GameManager.States.MenuCanvasVisible = true;
        m_settingsMenu.SetActive(true);
    }
    /// <summary>
    /// Hides the settings menu.
    /// </summary>
    public void HideSettings()
    {
        MasterManager.GameManager.States.MenuCanvasVisible = false;
        m_settingsMenu.SetActive(false);
    }

    /// <summary>
    /// Shows the alert menu after setting it's text.
    /// </summary>
    /// <param name="message"></param>
    public void ShowAlert(string message)
    {
        MasterManager.GameManager.States.MenuCanvasVisible = true;
        m_alertMenu.SetAlertText(message);
        m_alertMenu.gameObject.SetActive(true);
    }
    /// <summary>
    /// Hides the alert menu.
    /// </summary>
    public void HideAlert()
    {
        MasterManager.GameManager.States.MenuCanvasVisible = false;
        m_alertMenu.gameObject.SetActive(false);
    }
}
