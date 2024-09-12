namespace Game.Runtime.Services.Analytics.Adapters.UnityAnalytics
{
    using Interfaces;
    using Runtime;
    using Unity.Services.Core;
    using UnityEngine;
    
#if ANALYTICS_UNITY
    using Event = Unity.Services.Analytics.Event;
    using Unity.Services.Analytics;

    [CreateAssetMenu(menuName = "Game/Services/Analytics/Unity Analytics Adapter")]
    public class UnityAnalyticsHandler : AnalyticsAdapter
    {
        protected override async void OnInitialize(IAnalyticsModel config)
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
        }

        public sealed override void OnTrackEvent(IAnalyticsMessage message)
        {
            AnalyticsService.Instance.RecordEvent(message as Event);
            AnalyticsService.Instance.Flush();
        }
    }

#else

    [CreateAssetMenu(menuName = "Game/Services/Analytics/Unity Analytics")]
    public class UnityAnalyticsHandler : AnalyticsAdapter
    {
        
    }
#endif    
}