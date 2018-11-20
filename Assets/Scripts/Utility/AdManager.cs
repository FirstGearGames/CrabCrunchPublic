using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{

    #region Serialized.
    /// <summary>
    /// How frequently to show ads when the application is resumed from a pause. Use -1 to disable.
    /// </summary>
    [Tooltip("How frequently to show ads when the application is resumed from a pause. Use -1 to disable.")]
    [SerializeField]
    private int m_pauseAdInterval = 2;
    /// <summary>
    /// How frequently to show ads when a player loses or restarts. Use -1 to disable.
    /// </summary>
    [Tooltip("How frequently to show ads when a player loses or restarts. Use -1 to disable.")]
    [SerializeField]
    private int m_loseAdInterval = 1;
    /// <summary>
    /// If an ad hasn't been shown for at least this long then show an ad when the next opportunity arises.
    /// </summary>
    [Tooltip("If an ad hasn't been shown for at least this long then show an ad when the next opportunity arises.")]
    [SerializeField]
    private float m_forcedAdFrequency = 300f;
    /// <summary>
    /// Minimum time which must past to show an ad.
    /// </summary>
    [Tooltip("Minimum time which must past to show an ad.")]
    [SerializeField]
    private float m_minimumAdFrequency = 30;
    #endregion

    #region Private.
    /// <summary>
    /// Describes if the game is paused or not.
    /// </summary>
    private bool m_paused = false;
    /// <summary>
    /// Current count for times lost.
    /// </summary>
    private int m_loseAdCount = 0;
    /// <summary>
    /// Current count for how many times the game was paused (lost focus).
    /// </summary>
    private int m_pauseAdCount = 0;
    /// <summary>
    /// The last time an ad was shown.
    /// </summary>
    private float m_lastAdTime = -1f;
    /// <summary>
    /// True if awaiting for a shown ad to callback.
    /// </summary>
    private bool m_awaitingCallback = false;
    /// <summary>
    /// PlacementId string for the ad currently awaiting callback.
    /// </summary>
    private string m_callbackPlaementId;
    #endregion

    #region Const and readonly.
    /// <summary>
    /// String to use to save and read ad pause count.
    /// </summary>
    private const string AD_PAUSE_COUNT_SAVE = "AdPauseCount";
    /// <summary>
    /// String to use to save and read ad lose count.
    /// </summary>
    private const string AD_LOSE_COUNT_SAVE = "AdLoseCount";
    /// <summary>
    /// PlacementId to show for forced ads.
    /// </summary>
    public const string ID_FORCED_AD = "video";
    /// <summary>
    /// PlacementId to show for rewarded credits ads.
    /// </summary>
    public const string ID_REWARDED_CREDITS = "rewardedVideo";
    #endregion

    private void Awake()
    {
        LoadSavedCounts();
    }

    /// <summary>
    /// Checks to make sure settings are acceptable.
    /// </summary>
    private void VerifySettings()
    {
        if (m_forcedAdFrequency < m_minimumAdFrequency)
            m_forcedAdFrequency = m_minimumAdFrequency;
    }

    /// <summary>
    /// Loads the saved counts 
    /// </summary>
    private void LoadSavedCounts()
    {
        m_loseAdCount = PlayerPrefs.GetInt(AD_LOSE_COUNT_SAVE, 0);
        m_pauseAdCount = PlayerPrefs.GetInt(AD_PAUSE_COUNT_SAVE, 0);
    }

    /// <summary>
    /// Called when the application is paused.
    /// </summary>
    /// <param name="pauseStatus"></param>
    private void OnApplicationPause(bool pauseStatus)
    {
        //Pausing doesn't matter if pause ads are disabled.
        if (m_pauseAdInterval < 0)
            return;

        //If forced add frequency met then set count to interval to ensure it fires.
        if (ForcedAdFrequencyMet())
            m_pauseAdCount = m_pauseAdInterval;

        //If ad frequency hasn't been met don't even increase count.
        if (AdFrequencyMet())
        {
            //If was previously paused but now is not.
            if (m_paused && !pauseStatus)
            {
                //Increase pause count.
                m_pauseAdCount++;

                //If count is at interval.
                if (m_pauseAdCount >= m_pauseAdInterval)
                {
                    //Only reset pause ad count if showing an ad was successful.
                    if (ShowForcedAd())
                        m_pauseAdCount = 0;
                    //If not successful reset pause ad count to it's interval.
                    else
                        m_pauseAdCount = m_pauseAdInterval;
                }
            }

            //Set and save pause ad count.
            PlayerPrefs.SetInt(AD_PAUSE_COUNT_SAVE, m_pauseAdCount);
            MasterManager.SaveManager.SavePlayerPrefs();
        }

        //Set new paused state.
        m_paused = pauseStatus;
    }

    /// <summary>
    /// Called when a game is lost, and the retry menu is shown.
    /// </summary>
    public void GameLost()
    {
        //Don't care if game lost if game lost ads are disabled.
        if (m_loseAdInterval < 0)
            return;

        //If forced add frequency met then set count to interval to ensure it fires.
        if (ForcedAdFrequencyMet())
            m_loseAdCount = m_loseAdInterval;

        //If ad frequency hasn't been met don't even increase count.
        if (AdFrequencyMet())
        {
            //Increase lose ad count.
            m_loseAdCount++;

            //If count is at interval.
            if (m_loseAdCount >= m_loseAdInterval)
            {
                //Only reset lose ad count if showing an ad was successful.
                if (ShowForcedAd())
                    m_loseAdCount = 0;
                //If not successful reset lose ad count to it's interval.
                else
                    m_loseAdCount = m_loseAdInterval;
            }

            //set and save changes to lose ad count.
            PlayerPrefs.SetInt(AD_LOSE_COUNT_SAVE, m_loseAdCount);
            MasterManager.SaveManager.SavePlayerPrefs();
        }
    }

    /// <summary>
    /// Returns if enough time has passed to show an ad.
    /// </summary>
    /// <returns></returns>
    private bool AdFrequencyMet()
    {
        //Unset, meaning an ad hasn't been shown yet.
        if (m_lastAdTime == -1f)
            return true;

        //Return if enough time has passed since last time to exceed minimum ad frequency.
        return ((Time.time - m_lastAdTime) > m_minimumAdFrequency);
    }

    /// <summary>
    /// Returns if enough time has passed to force an add, regardless of counts.
    /// </summary>
    /// <returns></returns>
    private bool ForcedAdFrequencyMet()
    {
        float lastAdTime;
        //Set last ad time based on if an ad has been shown this app launch.
        if (m_lastAdTime == -1f)
            lastAdTime = 0f;
        else
            lastAdTime = m_lastAdTime;

        //Return if time since last ad is greater than forced ad frequency.
        return ((Time.time - lastAdTime) > m_forcedAdFrequency);
    }

    /// <summary>
    /// Shows a forced ad.
    /// </summary>
    private bool ShowForcedAd()
    {
        return ShowUnityAd(ID_FORCED_AD, false);
    }

    /// <summary>
    /// Shows a rewarded ad and grants credits upon a successful response.
    /// </summary>
    /// <param name="credits"></param>
    public bool ShowRewardedAd()
    {
        return ShowUnityAd(ID_REWARDED_CREDITS, true);
    }


    /// <summary>
    /// Show an ad with a specified placementId, with the option to enable callback.
    /// </summary>
    /// <param name="placementId"></param>
    /// <param name="enableCallback"></param>
    /// <returns></returns>
    public bool ShowUnityAd(string placementId, bool enableCallback)
    {
#if UNITY_ADS
        //If the ad is ready.
        if (!Advertisement.IsReady(placementId))
        {
            Debug.LogWarning("Ad is not ready for " + placementId);
            return false;
        }
        else
        {
            ShowOptions options = new ShowOptions();
            //If to enable callback.
            if (enableCallback)
            {
                //Return false as already in callback.
                if (m_awaitingCallback)
                    return false;

                //Setup callback information.
                m_callbackPlaementId = placementId;
                m_awaitingCallback = true;
                //Setup options.
                options.resultCallback = OnAd_Result;
            }
            //Show add then return true.
            Advertisement.Show(placementId, options);
            return true;
        }
#endif
        return false;
    }

#if UNITY_ADS
    /// <summary>
    /// Called when an ad callback is successful.
    /// </summary>
    /// <param name="result"></param>
    private void OnAd_Result(ShowResult result)
    {
        //Set no longer awaiting callback.
        m_awaitingCallback = false;
        //Process the result.
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("Ad was shown successfully and played throughout.");
                ProcessAdFinished(m_callbackPlaementId);
                break;
            case ShowResult.Skipped:
                Debug.Log("As was shown but skipped partially through.");
                break;
            case ShowResult.Failed:
                Debug.Log("Ad display failed.");
                break;
        }
    }

    /// <summary>
    /// Process results of a calledback ad based on placementId.
    /// </summary>
    /// <param name="placementId"></param>
    private void ProcessAdFinished(string placementId)
    {
        switch (placementId)
        {
            case ID_REWARDED_CREDITS:
                int creditsToAdd = 200;
                //Add to credits, saving immediately.
                MasterManager.GameManager.AddCredits(creditsToAdd);
                //Let the user know they received credits.
                MasterManager.CanvasManager.MenuCanvas.ShowAlert("Thank you! You have received " + creditsToAdd.ToString() + " credits.");
                break;
        }
    }
#endif


}
