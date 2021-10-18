using System.Collections.Generic;
using UnityEngine;

namespace FinTOKMAK.TimelineSystem.Runtime
{
    [CreateAssetMenu(fileName = "Timeline", menuName = "FinTOKMAK/Timeline System/Timeline", order = 0)]
    public class Timeline : ScriptableObject
    {
        #region Public Field

        /// <summary>
        /// All the playable nodes
        /// </summary>
        public List<PlayableNode> playableNodes;

        /// <summary>
        /// All the check nodes
        /// </summary>
        public List<CheckNode> checkNodes;

        #endregion
    }
}