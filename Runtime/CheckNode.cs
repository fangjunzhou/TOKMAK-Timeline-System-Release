namespace FinTOKMAK.TimelineSystem.Runtime
{
    /// <summary>
    /// The check node type
    /// CheckAnim: check the present of anim clip
    /// CheckEvent: check the present of certain event
    /// </summary>
    public enum CheckNodeType
    {
        CheckAnim,
        CheckEvent
    }
    
    [System.Serializable]
    public class CheckNode
    {
        #region Public Field

        /// <summary>
        /// The node type of the CheckNode
        /// </summary>
        public CheckNodeType nodeType;

        /// <summary>
        /// The animation clip to play
        /// Or the event name to check
        /// </summary>
        public string field;

        #endregion
    }
}