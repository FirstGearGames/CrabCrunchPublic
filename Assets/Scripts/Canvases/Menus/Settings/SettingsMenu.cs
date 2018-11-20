using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject m_musicCheckmark;
    [SerializeField]
    private GameObject m_effectsCheckmark;

    private void Start()
    {
        LoadDisplay();
    }

    /// <summary>
    /// Sets the checkmarks on buttons based on their status.
    /// </summary>
    private void LoadDisplay()
    {
        //Effects checkmark s hould be set based on if sound effects are enabled.
        m_effectsCheckmark.SetActive(MasterManager.GameManager.Settings.SoundEffectsEnabled);
        //Music the same.
        m_musicCheckmark.SetActive(MasterManager.GameManager.Settings.MusicEnabled);
    }

    /// <summary>
    /// Called when the music button is clicked.
    /// </summary>
    public void OnClick_MusicState()
    {
        //Flip the boolean in settings.
        MasterManager.GameManager.Settings.MusicEnabled = !MasterManager.GameManager.Settings.MusicEnabled;
        //Write and save to player prefs.
        int musicEnabled = Convert.ToInt16(MasterManager.GameManager.Settings.MusicEnabled);
        PlayerPrefs.SetInt(GameManager.MUSIC_ENABLED_SAVE, musicEnabled);
        MasterManager.SaveManager.SavePlayerPrefs();
        //Update displays.
        LoadDisplay();
        //Update audio across the game.
        MasterManager.GameManager.AssignLoadedAudioSettings();
    }

    /// <summary>
    /// Called when the sound effects button is clicked
    /// </summary>
    public void OnClick_SoundEffectsState()
    {
        //Flip the boolean in settings.
        MasterManager.GameManager.Settings.SoundEffectsEnabled = !MasterManager.GameManager.Settings.SoundEffectsEnabled;
        //Write and save to player prefs.
        int effectsEnabled = Convert.ToInt16(MasterManager.GameManager.Settings.SoundEffectsEnabled);
        PlayerPrefs.SetInt(GameManager.SOUND_EFFECTS_ENABLED_SAVE, effectsEnabled);
        MasterManager.SaveManager.SavePlayerPrefs();
        //Update displays.
        LoadDisplay();
        //Update audio across the game.
        MasterManager.GameManager.AssignLoadedAudioSettings();
    }

    /// <summary>
    /// Called when the exit button is clicked for this menu.
    /// </summary>
    public void OnClick_CloseMenu()
    {
        MasterManager.CanvasManager.MenuCanvas.HideSettings();
    }

}
