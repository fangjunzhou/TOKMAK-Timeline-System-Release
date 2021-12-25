using FinTOKMAK.EventSystem.Editor;
using FinTOKMAK.EventSystem.Runtime;
using FinTOKMAK.EventSystem.Runtime.GlobalEvent;
using FinTOKMAK.TimelineSystem.Runtime;
using Hextant;
using UnityEditor;

namespace Package.Editor
{
    [CustomPropertyDrawer(typeof(TimelineEventAttribute))]
    public class TimelineEventDrawer: UniversalEventDrawer
    {
        public override UniversalEventConfig GetEventConfig()
        {
            return Settings<TimelineEventSettings>.instance.universalEventConfig;
        }
    }
}