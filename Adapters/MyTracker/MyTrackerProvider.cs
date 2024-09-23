#if ANALYTICS_MYTRACKER

namespace VN.Runtime.Services
{
    
    using Game.Runtime.Services.Analytics.Interfaces;
    using Game.Runtime.Services.Analytics.Runtime;
    using Mycom.Tracker.Unity;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Services/Analytics/My Tracker Analytics")]
    public sealed class MyTrackerProvider: AnalyticsAdapter
    {
        public string iOSKey;
        public string AndroidKey;
        public bool IsDebug;

        protected override void OnInitialize(IAnalyticsModel config)
        {
            var myTrackerConfig = MyTracker.MyTrackerConfig;
            MyTracker.IsDebugMode = IsDebug;

#if UNITY_IOS
            if (string.IsNullOrEmpty(iOSKey))
            {
                Debug.LogError("MyTrackerAnalytics iOSKey is empty!");
                return;
            }
            MyTracker.Init(iOSKey);
            
#elif UNITY_ANDROID
            if (string.IsNullOrEmpty(AndroidKey))
            {
                Debug.LogError("MyTrackerAnalytics AndroidKey is empty!");
                return;
            }
            MyTracker.Init(AndroidKey);
#endif
        }

        public override void OnTrackEvent(IAnalyticsMessage message)
        {
            MyTracker.TrackEvent(message.Name, message.Parameters);
        }
    }
    
}

#endif
