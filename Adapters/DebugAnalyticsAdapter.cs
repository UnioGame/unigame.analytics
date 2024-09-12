namespace Game.Runtime.Services.Analytics.Debug
{
    using Game.Runtime.Services.Analytics.Interfaces;
    using Runtime;
    using UniCore.Runtime.ProfilerTools;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Services/Analytics/Debug Adapter")]
    public class DebugAnalyticsAdapter : AnalyticsAdapter
    {
        public override void OnTrackEvent(IAnalyticsMessage message)
        {
            if (!Model.IsDebug.Value) return;
            
            GameLog.LogRuntime(message.ToString(),Color.yellow);
        }
    }
}
