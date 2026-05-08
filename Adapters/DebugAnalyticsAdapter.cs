namespace UniGame.Runtime.Analytics.Adapters
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UnityEngine;

    [Serializable]
    public class DebugAnalyticsAdapter : IAnalyticsAdapter
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
#if GAME_DEBUG
            var payload = string.Join(", ", message.Parameters.Select(static x => $"{x.Key}={x.Value}"));
            GameLog.Log($"[Analytics] {message.Name} | {payload}",Color.chocolate);
#endif
        }
    }
}
