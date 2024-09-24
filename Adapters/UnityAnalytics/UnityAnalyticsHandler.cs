namespace Game.Runtime.Services.Analytics.Adapters.UnityAnalytics
{
    using System;
    using Cysharp.Threading.Tasks;
    using Interfaces;
    using Unity.Services.Core;

#if ANALYTICS_UNITY
    using Unity.Services.Analytics;

    [Serializable]
    public class UnityAnalyticsHandler : IAnalyticsAdapter
    {
        public async UniTask InitializeAsync()
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
        }

        public void TrackEvent(IAnalyticsMessage message)
        {
            var unityEvent = new UnityEventMessage(message.Name);
            foreach (var parameter in message.Parameters)
                unityEvent[parameter.Key] = parameter.Value;
            
            AnalyticsService.Instance.RecordEvent(unityEvent);
            AnalyticsService.Instance.Flush();
        }

        public void Dispose()
        {
            
        }
    }

#else

    [Serializable]
    public class UnityAnalyticsHandler : IAnalyticsAdapter
    {
        public void Dispose()
        {
            
        }

        public UniTask InitializeAsync()
        {
            return UniTask.CompletedTask;
        }

        public void TrackEvent(IAnalyticsMessage message)
        {
            
        }
    }
#endif    
}