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
        public string target;

        /// <summary>
        /// The name of animation trigger to be trigger
        /// Or the name of the event to be invoke
        /// No need when the PlayableNodeType is EndMark
        /// </summary>
        public string field;

        #endregion
    }
}