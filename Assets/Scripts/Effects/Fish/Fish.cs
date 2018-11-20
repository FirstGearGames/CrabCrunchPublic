using FirstGearGames.Global.Structures;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    /// <summary>
    /// Types of actions the fish may perform.
    /// </summary>
    private enum ActionTypes
    {
        Unset,
        Float,
        Move
    }

    /// <summary>
    /// Holds data about how the fish should move, and where to move to.
    /// </summary>
    private class MoveData
    {
        #region Public.
        /// <summary>
        /// The type of action for this MoveData.
        /// </summary>
        public ActionTypes ActionType = ActionTypes.Unset;
        /// <summary>
        /// True if floating up, false if floating down.
        /// </summary>
        public bool FloatingUp;
        /// <summary>
        /// True if moving right, false if moving left.
        /// </summary>
        public bool MovingRight;
        /// <summary>
        /// Position the fish is moving to.
        /// </summary>
        public Vector3 TargetPosition;
        /// <summary>
        /// If not -1f this value overrides the defualt fish movement speed.
        /// </summary>
        public float SpeedOverride = -1f;
        /// <summary>
        /// Rotation the fish is rotating towards.
        /// </summary>
        public Quaternion TargetLookRotation;
        /// <summary>
        /// Total distance to move on a new move action. Used to calculate move completion to ease in and out of movements.
        /// </summary>
        public float MoveDistance;
        #endregion
    }

    #region Public.
    /// <summary>
    /// Use Depth.
    /// </summary>
    private float m_depth = 0f;
    /// <summary>
    /// Current depth of this fish. Used to set Z value and scale fish.
    /// </summary>
    public float Depth
    {
        get { return m_depth; }
    }
    #endregion

    #region Run-time component references.
    /// <summary>
    /// Reference to the sprite renderer component on this object.
    /// </summary>
    private SpriteRenderer m_spriteRenderer;
    #endregion

    #region Private.
    /// <summary>
    /// Current move data for fish.
    /// </summary>
    private MoveData m_moveData;
    /// <summary>
    /// Fish data for this fish.
    /// </summary>
    private FishData m_fishData;
    /// <summary>
    /// Moving direction for this fish. Use -1f or 1f.
    /// </summary>
    private float m_moveDirection;
    /// <summary>
    /// Position of the view center. The object which houses the camera that displays background activity.
    /// </summary>
    private Vector3 m_viewCenter = Vector3.zero;
    #endregion


    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //If a menu is blocking no need to make fish animate.
        if (MasterManager.GameManager.States.TitleCanvasVisible)
            return;

        //Fish can only operate if there is move data.
        if (m_moveData != null)
        {
            ProcessMoveData();
            CheckBecameInvisible();
        }
    }

    /// <summary>
    /// Processes the current set MoveData.
    /// </summary>
    private void ProcessMoveData()
    {
        //No move data.
        if (m_moveData == null)
            return;

        //Perform an action.
        switch (m_moveData.ActionType)
        {
            case ActionTypes.Float:
                Float();
                break;
            case ActionTypes.Move:
                Move();
                break;
        }
    }

    /// <summary>
    /// Moves the fish in a direction.
    /// </summary>
    private void Move()
    {
        float speed;
        if (m_moveData.SpeedOverride != -1f)
            speed = m_moveData.SpeedOverride;
        else
            speed = m_fishData.AverageSwimSpeed;

        //Get distance from current position to target.
        float distanceFromTarget = Vector3.Distance(transform.position, m_moveData.TargetPosition);
        //Get percentage of distance currently traveled. Compares against starting distance.
        float distanceTraveledPercent = 1f - (distanceFromTarget / m_moveData.MoveDistance);

        //Beginning and end percent to ease movement.
        float easePercent = 0.1f;
        /* If at beginning or end of distance traveled slow speed down. Could also use
         * smooth damp. */
        if (distanceTraveledPercent >= (1f - easePercent) || distanceTraveledPercent <= easePercent)
        {
            //Set remainder which holds either distance left or distance in.
            float remainder;
            if (distanceTraveledPercent > easePercent)
                remainder = 1f - distanceTraveledPercent;
            else
                remainder = distanceTraveledPercent;

            float lowestEaseValue = 0.25f;
            //Modify speed anywhere between lowestEaseValue to 100%(normal) of it's speed.
            float speedModifier = Mathf.Lerp(lowestEaseValue, 1f, remainder * (1f / easePercent));

            //Modify speed.
            speed *= speedModifier;
        }

        //Lerp torwards position.
        transform.position = Vector3.MoveTowards(transform.position, m_moveData.TargetPosition, speed * Time.deltaTime);

        Quaternion lookRotation;
        /* Rotate to movement. */
        //If near the end of the travel begin to smooth out a level rotation.
        if (distanceTraveledPercent >= (1f - easePercent))
            lookRotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f));
        //Not near end of the travel, rotate to look rotation.
        else
            lookRotation = m_moveData.TargetLookRotation;

        //Rotate to look rotation.
        transform.rotation = ReturnNextRotation(transform.rotation, lookRotation, m_fishData.RotateSpeed, true);


        //If at destination.
        if (transform.position == m_moveData.TargetPosition)
            NewRandomMoveData();
    }

    /// <summary>
    /// Floats the fish.
    /// </summary>
    private void Float()
    {
        float speed;
        //Set speed.
        if (m_moveData.SpeedOverride != -1f)
            speed = m_moveData.SpeedOverride;
        else
            speed = m_fishData.AverageSwimSpeed;

        //Set float direction to speed so that floating direction occuries at float speed.
        float floatdirection = speed * Time.deltaTime;
        //If not floating up inverse float direction.
        if (!m_moveData.FloatingUp)
            floatdirection *= -1f;

        //Set next position.
        transform.position += new Vector3(0f, floatdirection, 0f);
        //Lerp to rotation.
        transform.rotation = ReturnNextRotation(transform.rotation, m_moveData.TargetLookRotation, m_fishData.RotateSpeed, true);

        /* Compare position to see if action is over. */

        //If moving up and transforms Y position is at or above target position Y.
        if (m_moveData.FloatingUp && transform.position.y >= m_moveData.TargetPosition.y)
            NewRandomMoveData();
        //If moving down and transforms Y position is at or below target position Y.
        else if (!m_moveData.FloatingUp && transform.position.y <= m_moveData.TargetPosition.y)
            NewRandomMoveData();
    }

    /// <summary>
    /// Selects a new random move data type and sets up it's values using current fish data.
    /// </summary>
    private void NewRandomMoveData()
    {
        /* Hard coded values for likeliness of certain actions.
         * Action totals must add up to 1f otherwise an action won't be selected. */

        List<KeyValuePair<ActionTypes, float>> actions = new List<KeyValuePair<ActionTypes, float>>();
        //Add items with lowest probability first.
        actions.Add(new KeyValuePair<ActionTypes, float>(ActionTypes.Move, 0.31f)); //Change to 0.15f once moving is coded, and change
        actions.Add(new KeyValuePair<ActionTypes, float>(ActionTypes.Float, 0.7f)); //float chance to 0.85f.

        //Roll is what the total probability must exceed for the item to be selected.
        float roll = Random.Range(0f, 1f);
        //roll total
        float total = 0f;
        //Set default of result to unset.
        ActionTypes result = ActionTypes.Unset;

        for (int i = 0; i < actions.Count; i++)
        {
            //Add this actions value (aka probability) to total.
            total += actions[i].Value;
            //If roll is less than or equal to total then select as result.
            if (roll <= total)
            {
                result = actions[i].Key;
                break;
            }
        }

        //Handle result.
        switch (result)
        {
            case ActionTypes.Unset:
                Debug.LogError(Debugs.ReturnErrorString(transform, " -> Fish -> NewRandomMoveData -> Action type couldn't be determined."));
                break;
            case ActionTypes.Float:
                NewFloatAction();
                break;
            case ActionTypes.Move:
                NewMoveAction();
                break;
        }
    }

    /// <summary>
    /// Generates a new move action. Fish will move forward as well sometimes up or down.
    /// </summary>
    private void NewMoveAction()
    {
        m_moveData = new MoveData();
        m_moveData.ActionType = ActionTypes.Move;

        //Set if moving right.
        m_moveData.MovingRight = (m_moveDirection == 1f);

        //Limit vertical movement to 45 degrees.
        float moveY = Random.Range(-0.45f, 0.45f);
        float moveX = 1f;
        //If not moving right inverst x direction.
        if (!m_moveData.MovingRight)
            moveX *= -1f;
        //Set move direction.
        Vector3 direction = new Vector3(moveX, moveY, 0f);

        //Random distance multiplier based on average swim distance.
        float distanceMultiplier = m_fishData.AverageSwimDistance * Random.Range(0.5f, 1.5f);
        Vector3 targetPosition = transform.position + (direction * distanceMultiplier);
        /* Clamp target position with fish's boundaries. */
        float bottomOfCenterView = m_viewCenter.y - MasterManager.CameraManager.HalfOrthographicHeight;
        float lowestYPosition = bottomOfCenterView + (MasterManager.CameraManager.OrthographicHeight * (m_fishData.SwimHeightRange.Minimum * 0.9f));
        float highestYPosition = bottomOfCenterView + (MasterManager.CameraManager.OrthographicHeight * (m_fishData.SwimHeightRange.Maximum * 1.1f));
        targetPosition.y = Mathf.Clamp(targetPosition.y, lowestYPosition, highestYPosition);

        m_moveData.TargetPosition = targetPosition;

        //Set move speed.
        m_moveData.SpeedOverride = m_fishData.AverageSwimSpeed * (Random.Range(0.75f, 1.25f));

        Quaternion targetRotation;

        Vector3 angleDirection = new Vector3(m_moveData.TargetPosition.x, m_moveData.TargetPosition.y, transform.position.z) - transform.position;
        float angle = Vector3.Angle(transform.up, angleDirection);
        //If moving down then flip angle.
        if (m_moveData.TargetPosition.y < transform.position.y)
            angle *= -1f;

        //Reduce angle by 15% so fish don't look exactly towards movig direction. 
        angle *= 0.15f;

        //If move speed is normal or higher, or angle is larger than 20 degrees.
        if (m_moveData.SpeedOverride >= m_fishData.AverageSwimSpeed || Mathf.Abs(angle) >= 20f)
            targetRotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angle));
        //Below average move speed don't bother changing angle.
        else
            targetRotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f));

        m_moveData.TargetLookRotation = targetRotation;

        //Calculate distance from start to end. Used for lerping.
        m_moveData.MoveDistance = Vector3.Distance(transform.position, m_moveData.TargetPosition);
    }

    /// <summary>
    /// Generates a new float action. Floats fish up and down.
    /// </summary>
    private void NewFloatAction()
    {
        //True if floating up.
        bool floatUp;

        //If was previously floating.
        if (m_moveData != null)
        {
            //If previous action was a float then set float up opposite of last floating up.
            if (m_moveData.ActionType == ActionTypes.Float)
                floatUp = !m_moveData.FloatingUp;
            //Different previous action type.
            else
                floatUp = Bools.RandomBool();

        }
        //No current move data.
        else
        {
            floatUp = Bools.RandomBool();
        }

        /* Floating should be fairly consistent. 
         * MovingRight isn't used for floating. */

        //Make a new move data to erase old values.
        m_moveData = new MoveData();
        m_moveData.ActionType = ActionTypes.Float;

        //Hard coded float distance and speed.
        float floatDistance = 15f;
        float floatSpeed = 10f;

        m_moveData.FloatingUp = floatUp;
        m_moveData.SpeedOverride = floatSpeed;
        //If not floating up inverse float distance.
        if (!floatUp)
            floatDistance *= -1f;
        //Set new target position.
        m_moveData.TargetPosition = transform.position + new Vector3(0f, floatDistance, 0f);

        //Set target rotation.
        Quaternion targetRotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f));
        m_moveData.TargetLookRotation = targetRotation;
    }

    /// <summary>
    /// Returns a rotation value using rotate towards.
    /// </summary>
    /// <param name="currentRotation">Current rotation.</param>
    /// <param name="targetRotation">Rotation goal.</param>
    /// <param name="speed">How quickly to rotate. Do not include Time.deltaTime.</param>
    /// <param name="distanceBased">True to adjust rotate speed based on remaining rotation.</param>
    private Quaternion ReturnNextRotation(Quaternion currentRotation, Quaternion targetRotation, float speed, bool distanceBased)
    {
        float distanceMultiplier;
        if (distanceBased)
            distanceMultiplier = 1f;
        else
            distanceMultiplier = Mathf.Max(0.25f, Quaternion.Angle(currentRotation, targetRotation));

        return Quaternion.RotateTowards(currentRotation, targetRotation, speed * distanceMultiplier * Time.deltaTime);
    }


    /// <summary>
    /// Checks if the fish has moved off the screen on the X axis.
    /// </summary>
    private void CheckBecameInvisible()
    {
        float outOfViewX;

        if (m_moveDirection == -1f)
        {
            //Set value which is considered out of view.
            outOfViewX = m_viewCenter.x - (MasterManager.CameraManager.OrthographicWidth / 2f) - (m_spriteRenderer.bounds.size.x / 2f);
            //If this fish transofrm passes the value in the left direction.
            if (transform.position.x < outOfViewX)
                MasterManager.FishManager.FishBecameInvisible(this);
        }
        else
        {
            //Set value which is considered out of view.
            outOfViewX = m_viewCenter.x + (MasterManager.CameraManager.OrthographicWidth / 2f) + (m_spriteRenderer.bounds.size.x / 2f);
            //If this fish transform passes the value in the right direction.
            if (transform.position.x > outOfViewX)
                MasterManager.FishManager.FishBecameInvisible(this);
        }
    }

    /// <summary>
    /// Sets the Fish Data for this fish.
    /// </summary>
    /// <param name="fishData"></param>
    public void SetFishData(FishData fishData)
    {
        m_fishData = fishData;
    }

    /// <summary>
    /// Sets the sprite renderers sprite based on fish data.
    /// </summary>
    public void SetSprite()
    {
        if (m_fishData != null)
            m_spriteRenderer.sprite = m_fishData.Image;
    }

    /// <summary>
    /// Sets the fish facing as well it's moving direction.
    /// </summary>
    /// <param name="right">True if facing right.</param>
    /// <param name="direction">Direction to move.</param>
    public void SetFacing(bool right, float direction)
    {
        //Face right on screen.
        if (right)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0f, transform.eulerAngles.z);
        //If to face left on screen.
        else
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180f, transform.eulerAngles.z);

        m_moveDirection = direction;
    }

    /// <summary>
    /// Sets fish depth on the z axis. Also calls ScaleToDepth.
    /// </summary>
    /// <param name="depth">Depth to set.</param>
    public void SetDepth(float depth)
    {
        m_depth = depth;
    }

    /// <summary>
    /// Scales fish in accordance to a depth value. Scaling based on the percentage between min and max possible depth.
    /// </summary>
    /// <param name="depth">Depth of the fish. Typically it's Z value.</param>
    /// <param name="minPossibleDepth">Minimum possible depth.</param>
    /// <param name="maxPossibleDepth">Maximum possible depth.</param>
    public void ScaleToDepth(float depth, float minPossibleDepth, float maxPossibleDepth)
    {
        //Percentage of depth deep out of min/max depth using depth value.
        float depthPercent = Mathf.InverseLerp(minPossibleDepth, maxPossibleDepth, depth);
        //Get scale based on depth percent.
        float newScale = 1f - Mathf.Lerp(m_fishData.ScaleRange.Start, m_fishData.ScaleRange.End, depthPercent);
        //Scale fish.
        transform.localScale = new Vector3(newScale, newScale, 1f);
    }

    /// <summary>
    /// Sets a new position for the fish, as though it just spawned or cleared an edge.
    /// </summary>
    /// <param name="viewCenter">Position of the camera viewing the fish. Typically the background activity camera.</param>
    /// <param name="anyXPosition">True to start in any X position rather than off the screen. Typically true to initially spawn fish.</param>
    public void SetNewPosition(Vector3 viewCenter, bool anyXPosition)
    {
        m_viewCenter = viewCenter;

        //Get a percentage of the screen which fish can start within.
        float heightPercent = Random.Range(m_fishData.SwimHeightRange.Minimum, m_fishData.SwimHeightRange.Maximum);

        //Set Y position to that of screen height using the base of the camera position as a starting point.
        float positionY = viewCenter.y + (MasterManager.CameraManager.OrthographicHeight * heightPercent) - MasterManager.CameraManager.HalfOrthographicHeight;

        //Get bound size of the sprite so that it may be placed just outside screen view.
        Vector2 spriteHalfBounds = new Vector2(m_spriteRenderer.sprite.bounds.size.x / 2f, m_spriteRenderer.sprite.bounds.size.y / 2f);

        float positionX = 0f;
        //If using any position. Typically used to immediately load fish into the scene.
        if (anyXPosition)
        {
            /* If moving left allow fish to spawn anywhere with 10% of the left,
             * to 25% past the right. To an edge from the center would be 50% in either
             * direction; so 0.40f outward from the center to the left is actually 10% from left edge. */
            if (m_moveDirection == -1f)
                positionX = viewCenter.x + Random.Range(-MasterManager.CameraManager.OrthographicWidth * 0.40f, MasterManager.CameraManager.OrthographicWidth * 0.75f);
            else
                positionX = viewCenter.x + Random.Range(-MasterManager.CameraManager.OrthographicWidth * 0.75f, MasterManager.CameraManager.OrthographicWidth * 0.40f);
        }
        //If not use any position start on an edge.
        else
        {
            if (m_moveDirection == -1f)
                positionX = viewCenter.x + (MasterManager.CameraManager.OrthographicWidth / 2f) + spriteHalfBounds.x;
            else
                positionX = viewCenter.x - (MasterManager.CameraManager.OrthographicWidth / 2f) - spriteHalfBounds.x;
        }

        //If moving left then flip positionX.
        transform.position = new Vector3(positionX, positionY, m_depth);

        //With a new position, new move data should be generated.
        NewRandomMoveData();
    }

}
