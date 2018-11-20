using TMPro;
using UnityEngine;

public abstract class TextEffect : MonoBehaviour
{
    #region Serialize.
    /// <summary>
    /// Reference to the text mesh pro component on this objects children.
    /// </summary>
    [Tooltip("Reference to the text mesh pro component on this objects children.")]
    [SerializeField]
    protected TextMeshProUGUI TextMeshPro;
    #endregion

    #region Virtual and abstract.
    /// <summary>
    /// Override to float text upward.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="speed"></param>
    /// <param name="startPosition"></param>
    /// <param name="direction"></param>
    /// <param name="distance"></param>
    /// <param name="fontSize"></param>
    /// <param name="color"></param>
    public virtual void FloatText(string text, float speed, Vector3 startPosition, Vector3 direction, float distance, float fontSize, Color32 color) { }
    /// <summary>
    /// Override to roll text, in a tallying fashion.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="duration"></param>
    public virtual void RollText(string text, float duration) { }
    /// <summary>
    /// Override to roll text, in a tallying fashion.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="duration"></param>
    public virtual void RollValue(int value, float duration) { }
    public virtual void RollText(int startValue, int endValue, float duration) { }
    /// <summary>
    /// Override to set text directly.
    /// </summary>
    /// <param name="text"></param>
    public virtual void SetText(string text) { }
    /// <summary>
    /// Override to set text directly.
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetValue(int value) { }
    #endregion

}
