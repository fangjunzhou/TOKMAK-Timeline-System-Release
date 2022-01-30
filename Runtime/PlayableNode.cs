namespace FinTOKMAK.TimelineSystem.Runtime
{
    /// <summary>
    /// The playable node type
    /// PlayAnim: trigger a certain animation
    /// InvokeEvent: invoke a certain event
    /// EndMark: end the entire timeline and trigger the timeline system check
    /// </summary>
    public enum PlayableNodeType
    {
        PlayAnim,
        InvokeEvent,
        EndMark
    }

    public enum AnimOperationType
    {
        SetTrigger,
        SetBool,
        SetFloat,
        SetInt,
        PlayDirectly
    }
    
    [System.Serializable]
    public class PlayableNode
    {
        #region Public Field

        /// <summary>
        /// The time stamp of the node on the timeline
        /// </summary>
        public float time;

        /// <summary>
        /// The node type of the playable node
        /// </summary>
        public PlayableNodeType nodeType;

        /// <summary>
        /// The target animator (only enabled when PlayableNodeType is PlayAnim)
        /// </summary>
        public string targetAnimator;

        /// <summary>
        /// The anim operation to be done (only enabled when PlayableNodeType is PlayAnim)
        /// </summary>
        public AnimOperationType animOperationType;

        /// <summary>
        /// The target variable of the animator (only enabled when PlayableNodeType is PlayAnim)
        /// </summary>
        public string targetVar;

        /// <summary>
        /// The bool value to be set (only enabled when PlayableNodeType is PlayAnim and animOperationType is SetBool)
        /// </summary>
        public bool boolValue;

        /// <summary>
        /// The float value to be set (only enabled when PlayableNodeType is PlayAnim and animOperationType is SetFloat)
        /// </summary>
        public float floatValue;

        /// <summary>
        /// The int value to be set (only enabled when PlayableNodeType is PlayAnim and animOperationType is SetInt)
        /// </summary>
        public int intValue;

        /// <summary>
        /// The animation to play directly (only enabled when PlayableNodeType is PlayAnim and animOperationType is PlayDirectly)
        /// </summary>
        public string targetAnimation;

        /// <summary>
        /// The target layer to play directly (only enabled when PlayableNodeType is PlayAnim and animOperationType is PlayDirectly)
        /// </summary>
        public int targetLayer;

        /// <summary>
        /// The name of the event to be invoke (only enabled when PlayableNodeType is InvokeEvent)
        /// </summary>
        [TimelineEvent]
        public string eventName;

        #endregion
    }
}