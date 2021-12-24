using FinTOKMAK.EventSystem.Runtime;
using Hextant;
#if UNITY_EDITOR
using Hextant.Editor;
using UnityEditor;
#endif

namespace FinTOKMAK.TimelineSystem.Runtime
{
    [Settings( SettingsUsage.RuntimeProject, "Timeline Event" )]
    public class TimelineEventSettings: Settings<TimelineEventSettings>
    {
        public UniversalEventConfig universalEventConfig;
        
#if UNITY_EDITOR
        [SettingsProvider]
        static SettingsProvider GetSettingsProvider() =>
            instance.GetSettingsProvider();
#endif
    }
}