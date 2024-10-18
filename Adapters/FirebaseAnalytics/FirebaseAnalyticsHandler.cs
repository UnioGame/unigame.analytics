#if ANALYTICS_GOOGLE

namespace VN.Runtime.Services
{
    using System;
    using Cysharp.Threading.Tasks;
    using Firebase.Analytics;
    using Game.Runtime.Services.Analytics.Interfaces;
    using UnityEngine;

    [Serializable]
    public sealed class FirebaseAnalyticsHandler: IAnalyticsAdapter
    {
        public UniTask InitializeAsync()
        {
            return UniTask.CompletedTask;
        }

        public void TrackEvent(IAnalyticsMessage message)
        {
            int i = 0;
            Parameter[] Parameters = new Parameter[message.Parameters.Keys.Count];
            foreach (var pair in message.Parameters)
            {
                Parameters[i] = new Parameter(pair.Key, pair.Value);
                i++;
            }
            FirebaseAnalytics.LogEvent(message.Name, Parameters);
        }

        public void Dispose()
        {
            
        }
    }
    
}

#endif