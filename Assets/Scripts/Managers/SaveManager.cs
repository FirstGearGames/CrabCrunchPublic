using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{

    #region Private.
    /// <summary>
    /// Next time when a file save is allowed.
    /// </summary>
    private float m_nextFileSaveTime = 0f;
    /// <summary>
    /// Is no longer null when a file save is in progress.
    /// </summary>
    private Coroutine m_playerPrefsSave = null;
    #endregion

    #region Const and readonly.
    /// <summary>
    /// Minimum time between saving files when not using force save.
    /// </summary>
    private const float SAVE_INTERVAL = 5f;
    #endregion



    /// <summary>
    /// Issues a save to player prefs.
    /// </summary>
    /// <param name="forceSave">True to save immediately rather than use save intervals.</param>
    public void SavePlayerPrefs(bool forceSave = false)
    {
        //If to skip coroutine and save immediately.
        if (forceSave)
        {
            //Save immediately.
            PlayerPrefs.Save();
        }
        //Save normally.
        else
        {
            //Only try to save if a save currently isn't in progress.
            if (m_playerPrefsSave == null)
            {
                m_playerPrefsSave = StartCoroutine(C_SavePlayerPrefs());
            }
        }
    }

    /// <summary>
    /// Saves the player preferences if enough time has passed to do so. If not will wait the remaining daily. This throttles file writes.
    /// </summary>
    /// <returns></returns>
    private IEnumerator C_SavePlayerPrefs()
    {
        float timeLeft = m_nextFileSaveTime - Time.time;

        //If there is time left to wait then wait the time before saving.
        if (timeLeft > 0f)
            yield return new WaitForSeconds(timeLeft);

        //Save player prefs.
        PlayerPrefs.Save();
        //Set next allowed save time.
        m_nextFileSaveTime = Time.time + SAVE_INTERVAL;
        //Nullify save coroutine.
        m_playerPrefsSave = null;
    }

    /// <summary>
    /// Determines how to handle saving player prefs based on a specified save type.
    /// </summary>
    /// <param name="saveType">Type of save to handle.</param>
    public void HandleSaveType(SaveTypes saveType)
    {
        switch (saveType)
        {
            case SaveTypes.ForceSave:
                MasterManager.SaveManager.SavePlayerPrefs(true);
                break;
            case SaveTypes.NormalSave:
                MasterManager.SaveManager.SavePlayerPrefs(false);
                break;
            case SaveTypes.WriteOnly:
                break;
        }
    }

}
