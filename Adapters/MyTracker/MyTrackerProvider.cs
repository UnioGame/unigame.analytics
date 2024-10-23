#if ANALYTICS_MYTRACKER

namespace VN.Runtime.Services
{
    using System;
    using Cysharp.Threading.Tasks;
    using Game.Runtime.Services.Analytics.Interfaces;
    using Mycom.Tracker.Unity;
    using UnityEngine;

    [Serializable]
    public sealed class MyTrackerProvider: IAnalyticsAdapter
    {
        public string iOSKey;
        public string AndroidKey;
        public bool IsDebug;

        public UniTask InitializeAsync()
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
                return UniTask.CompletedTask;
            }
            MyTracker.Init(AndroidKey);
#endif
            return UniTask.CompletedTask;
        }

        public void TrackEvent(IAnalyticsMessage message)
        {
            if(Application.isEditor) return;
            MyTracker.TrackEvent(message.Name, message.Parameters);
        }

        public void Dispose()
        {
            
        }
    }
    
}

#endif
