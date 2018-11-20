using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Powerup : MonoBehaviour
{
    #region Public.
    /// <summary>
    /// Use InteractionState.
    /// </summary>
    private PowerupInteractionStates m_interactionState = PowerupInteractionStates.None;
    /// <summary>
    /// Current state of the powerup
    /// </summary>
    public PowerupInteractionStates InteractionState
    {
        get { return m_interactionState; }
        set { m_interactionState = value; }
    }
    #endregion

    #region Serialized.
    /// <summary>
    /// Use PowerupType.
    /// </summary>
    [Tooltip("Type of powerup for this script.")]
    [SerializeField]
    private PowerupTypes m_powerupType = PowerupTypes.Unset;
    /// <summary>
    /// Type of powerup for this script.
    /// </summary>
    public PowerupTypes PowerupType
    {
        get { return m_powerupType; }
    }
    /// <summary>
    /// Reference to the TextMeshProUGUI component within the children of this object.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI m_textMeshPro;
    #endregion

    #region Runtime component references.
    /// <summary>
    /// Reference to the Animator component on this object. Used to animate object during interactions.
    /// </summary>
    private Animator m_animator;
    /// <summary>
    /// Reference to the RawImage component on this object. Used to change appearances when powerup conditions are set.
    /// </summary>
    private RawImage m_rawImage;
    #endregion

    #region Private.
    private PowerupData m_powerupData = null;
    /// <summary>
    /// True if the player meets the conditions of this powerup such as available credits and if states are met.
    /// </summary>
    private bool m_conditionsMet = false;
    #endregion

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rawImage = GetComponent<RawImage>();
    }

    private void Start()
    {
        SetupPowerup();
    }

    /// <summary>
    /// Initializes the powerup with it's data and component properties.
    /// </summary>
    private void SetupPowerup()
    {
        //Set data reference.
        m_powerupData = MasterManager.PowerupManager.ReturnPowerupData(PowerupType);
        //Data couldn't be found. Shouldn't occur if everything is setup properly.
        if (m_powerupData == null)
        {
            Debug.LogError(Debugs.ReturnErrorString(transform, "-> SetupPowerup -> Returned data is null. Object has been destroyed."));
            Destroy(gameObject);
        }
        //Data found.
        else
        {
            m_textMeshPro.text = m_powerupData.Cost.ToString();
        }

        //Check current conditions to see if powerup can be used.
        CheckPowerupConditions(true);
    }

    /// <summary>
    /// Checks if the powerup conditions are met. Includes checking credits and data required to use the powerup. Does not consider other blocking elements such as menus and animations.
    /// </summary>
    public void CheckPowerupConditions(bool forceConditionUpdate)
    {
        //Not enough credits.
        if (MasterManager.GameManager.States.Credits < m_powerupData.Cost)
        {
            UnsetConditionsMet(forceConditionUpdate);
            return;
        }

        //If undo powerup.
        if (PowerupType == PowerupTypes.Undo)
        {
            //If nothing to undo.
            if (MasterManager.BoardManager.PreviousBoardSave == string.Empty && MasterManager.TileManager.PreviousTileSave == string.Empty)
            {
                UnsetConditionsMet(forceConditionUpdate);
                return;
            }
        }

        //If here all conditions are met.
        SetConditionsMet(forceConditionUpdate);
    }

    /// <summary>
    /// Called when CheckPowerupConditions finds the powerup does not meet conditions.
    /// </summary>
    private void UnsetConditionsMet(bool forceConditionUpdate)
    {
        //If not force condition update and condition hasn't changed exit method.
        if (!forceConditionUpdate && m_conditionsMet == false)
            return;

        m_conditionsMet = false;
        //Change color to slightly translucent.
        m_rawImage.color = new Color(1f, 1f, 1f, 0.68f);
    }

    /// <summary>
    /// Called when CheckPowerupConditions finds that the powerup conditions are met.
    /// </summary>
    /// <param name="forceConditionUpdate"></param>
    private void SetConditionsMet(bool forceConditionUpdate)
    {
        //If not force condition update and condition hasn't changed exit method.
        if (!forceConditionUpdate && m_conditionsMet == true)
            return;

        m_conditionsMet = true;
        //Change color to full opaque.
        m_rawImage.color = new Color(1f, 1f, 1f, 1f);
    }

    /// <summary>
    /// Powerup has been interacted with.
    /// </summary>
    private void OnMouseDown()
    {
        //If conditions are met.
        if (!m_conditionsMet)
            return;

        /* If coroutine is running, which means actions are blocked. Or if powerup manager says
        * interactions are blocked. */
        if (!MasterManager.PowerupManager.CanInteractWithPowerups(this))
            return;

        PowerupClicked();
    }

    /// <summary>
    /// Called when collider is clicked and interactions are allowed.
    /// </summary>
    private void PowerupClicked()
    {
        //If unable to use powerup.
        if (MasterManager.GameManager.States.Credits < m_powerupData.Cost)
        {
            AnimateBlockedUse();
        }
        //Can use powerup.
        else
        {
            //Perform actions based on interactoin type.
            switch (m_powerupData.InteractionType)
            {
                //Prompt.
                case PowerupInteractionsTypes.Prompt:
                    if (InteractionState == PowerupInteractionStates.None)
                    {
                        RaisePowerup();
                        MasterManager.PowerupManager.ShowPowerupSelectors(PowerupType);
                    }
                    //Drop the powerup.
                    else
                    {
                        DropPowerup();
                        MasterManager.PowerupManager.HidePowerupSelectors();
                    }
                    break;
                //Use immediately.
                case PowerupInteractionsTypes.UseImmediately:
                    AnimateImmediatelyUsed();
                    StartCoroutine(MasterManager.PowerupManager.C_PerformPowerup(m_powerupData.Type));
                    break;
            }
        }
    }

    /// <summary>
    /// Sets a trigger within the animator and locks out clickable time is specified.
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="blockedDuration">Duration in which to block all powerup interactions globally.</param>
    /// <param name="newState">The state in which to set this powerup after the blocked duration is exceeded.</param>
    private void SetAnimationTrigger(int hash, float blockedDuration, PowerupInteractionStates newState)
    {
        //Set the hash.
        m_animator.SetTrigger(hash);
        //Set the global cooldown.
        MasterManager.PowerupManager.SetGlobalBlockedTime(blockedDuration);
        //Set new state.
        InteractionState = newState;
    }

    /// <summary>
    /// Called to animate powerup dropped.
    /// </summary>
    public void DropPowerup()
    {
        //Interaction type doesn't support dropping.
        if (m_powerupData.InteractionType != PowerupInteractionsTypes.Prompt)
            return;
        //Not in raised state, no need to drop.
        if (InteractionState != PowerupInteractionStates.Raised)
            return;

        int animatorHash = MasterManager.PowerupManager.ReturnPowerupInteractionStateHash(PowerupInteractionStates.Dropped);
        SetAnimationTrigger(animatorHash, m_powerupData.InteractedBlockDuration, PowerupInteractionStates.None);
    }

    /// <summary>
    /// Called to animate powerup raised.
    /// </summary>
    private void RaisePowerup()
    {
        //Interaction type doesn't support dropping.
        if (m_powerupData.InteractionType != PowerupInteractionsTypes.Prompt)
            return;
        //Not in raised state, no need to drop.
        if (InteractionState != PowerupInteractionStates.None)
            return;

        int animatorHash = MasterManager.PowerupManager.ReturnPowerupInteractionStateHash(PowerupInteractionStates.Raised);
        SetAnimationTrigger(animatorHash, m_powerupData.InteractedBlockDuration, PowerupInteractionStates.Raised);
    }

    /// <summary>
    /// Called to animate an immedate use.
    /// </summary>
    private void AnimateImmediatelyUsed()
    {
        int animatorHash = MasterManager.PowerupManager.ReturnPowerupInteractionStateHash(PowerupInteractionStates.ImmediatelyUsed);
        SetAnimationTrigger(animatorHash, m_powerupData.InteractedBlockDuration, PowerupInteractionStates.None);
    }

    /// <summary>
    /// Called to animate a blocked use.
    /// </summary>
    private void AnimateBlockedUse()
    {
        int animatorHash = MasterManager.PowerupManager.ReturnPowerupInteractionStateHash(PowerupInteractionStates.Blocked);
        SetAnimationTrigger(animatorHash, m_powerupData.InteractedBlockDuration, PowerupInteractionStates.None);
    }
}
