namespace Game.Runtime.Services.Analytics.Debug
{
    using System;
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
#if GAME_DEBUG || UNITY_EDITOR
            GameLog.LogRuntime($"ANALYTICS EVENT: {message}",Color.yellow);
#endif
        }
    }
}
