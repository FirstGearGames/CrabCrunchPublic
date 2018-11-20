using FirstGearGames.Global.Structures;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    #region Serialized.
    /// <summary>
    /// Camera used to view fish. Typically background activity camera.
    /// </summary>
    [Tooltip("Camera used to view fish. Typically background activity camera.")]
    [SerializeField]
    private Camera m_fishCamera;
    /// <summary>
    /// How many fish to spawn.
    /// </summary>
    [Tooltip("How many fish to spawn.")]
    [SerializeField]
    private int m_fishCount;
    /// <summary>
    /// Prefab to spawn for fish.
    /// </summary>
    [Tooltip("Prefab to spawn for fish.")]
    [SerializeField]
    private GameObject m_fishPrefab;
    /// <summary>
    /// Depths which fish may occupy. Represents Z value of fish, which also handles scaling.
    /// </summary>
    [SerializeField]
    private IntRange m_depthRange;
    /// <summary>
    /// Depths within depth range which should be blocked. Typically reserved for props.
    /// </summary>
    [Tooltip("Depths within depth range which should be blocked. Typically reserved for props.")]
    [SerializeField]
    private IntRange[] m_blockedDepthRanges = new IntRange[0];
    /// <summary>
    /// FishData entries of fish that may spawn.
    /// </summary>
    [Tooltip("FishData entries of fish that may spawn.")]
    [SerializeField]
    private FishData[] m_possibleFish;
    #endregion

    #region Private.
    /// <summary>
    /// List of all possible fish depths which fish may occupy.
    /// </summary>
    private List<float> m_fishDepths = new List<float>();
    /// <summary>
    /// Houses the gameobjects for fish.
    /// </summary>
    private GameObject m_fishObjectsParent = null;
    #endregion


    private void Awake()
    {
        BuildFishDepths();
    }

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Clears all current fish.
    /// </summary>
    private void ClearFish()
    {
        if (m_fishObjectsParent != null)
            Destroy(m_fishObjectsParent);
    }

    /// <summary>
    /// Initializes the manager and sets up fish for the first time.
    /// </summary>
    private void Initialize()
    {
        ClearFish();
        //Make a parent to attach fish to.
        GameObject parent = new GameObject();
        parent.name = "Fish Parent";
        m_fishObjectsParent = parent;

        for (int i = 0; i < m_fishCount; i++)
        {
            GameObject obj = Instantiate(m_fishPrefab, m_fishObjectsParent.transform);
            Fish fish = obj.GetComponent<Fish>();
            //skipping null check for testing sake.
            //this should setup the fish.
            SetupNewFish(fish, true);
        }
    }

    /// <summary>
    /// Builds a list of depths fish may spawn at. Ensures no fish occupies the same depth value. Excludes blocked depth ranges.
    /// </summary>
    private void BuildFishDepths()
    {
        for (int i = m_depthRange.Start; i <= m_depthRange.End; i++)
        {
            bool skipDepth = false;

            for (int z = 0; z < m_blockedDepthRanges.Length; z++)
            {
                if (i >= m_blockedDepthRanges[z].Start && i <= m_blockedDepthRanges[z].End)
                {
                    skipDepth = true;
                    break;
                }
            }
            if (skipDepth)
                continue;

            m_fishDepths.Add((float)i);
        }
    }

    /// <summary>
    /// Returns a random entry from fish depths and removes it from the list of entries.
    /// </summary>
    /// <returns>A random entry from fish depths.</returns>
    private float ReturnRandomFishDepth()
    {
        //Shouldn't ever happen, unless there's a ridiculous amount of fish in the scene.
        if (m_fishDepths.Count == 0)
            return 0f;

        //Pick a random index.
        int randomIndex = Ints.RandomExclusiveRange(0, m_fishDepths.Count);

        //Store value then remove it from list.
        float result = m_fishDepths[randomIndex];
        m_fishDepths.RemoveAt(randomIndex);

        //Return value.
        return result;
    }

    /// <summary>
    /// Adds an entry back to fish depths.
    /// </summary>
    /// <param name="value">Entry to add back.</param>
    private void AddToFishDepths(float value)
    {
        m_fishDepths.Add(value);
    }

    /// <summary>
    /// Called from a Fish component when the Fish renderer loses visibility.
    /// </summary>
    /// <param name="fish"></param>
    public void FishBecameInvisible(Fish fish)
    {
        //Add depth back as an available depth.
        int depth = Mathf.RoundToInt(fish.Depth);
        AddToFishDepths(depth);
        //Setup a new fish.
        SetupNewFish(fish, false);
    }

    /// <summary>
    /// Assigns fish data to a specified Fish reference and performs setup on the fish.
    /// </summary>
    /// <param name="fish">Fish reference to setup.</param>
    /// <param name="anyXPosition">True to start in any X position rather than off the screen. Typically true to initially spawn fish.</param>
    private void SetupNewFish(Fish fish, bool anyXPosition)
    {
        //50% chance to spawn the next fish on the left side.
        bool spawnOnLeftSide = (Random.Range(0f, 1f) <= 0.5f);

        //If next fish is to appear on left side.
        if (spawnOnLeftSide)
        {
            //If spawning on left side, face right.
            fish.SetFacing(true, 1f);
        }
        else
        {
            //If spawning on right side, face left.
            fish.SetFacing(false, -1f);
        }

        //Set a new fish data index.
        int fishDataindex = Ints.RandomExclusiveRange(0, m_possibleFish.Length);
        fish.SetFishData(m_possibleFish[fishDataindex]);

        //Set sprite for fish.
        fish.SetSprite();
        //Set depth of fish and scale fish to depth.
        float newDepth = ReturnRandomFishDepth();
        fish.SetDepth(newDepth);
        //Scale to new depth.
        fish.ScaleToDepth(newDepth, m_depthRange.Start, m_depthRange.End);
        //Set a new position.
        fish.SetNewPosition(m_fishCamera.transform.position, anyXPosition);
    }

}
