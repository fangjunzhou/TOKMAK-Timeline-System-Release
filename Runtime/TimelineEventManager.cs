using FinTOKMAK.EventSystem.Runtime;
using Hextant;

namespace FinTOKMAK.TimelineSystem.Runtime
{
    public class TimelineEventManager: UniversalEventManager
    {
        public override UniversalEventConfig GetEventConfig()
        {
            return Settings<TimelineEventSettings>.instance.universalEventConfig;
        }
    }
}