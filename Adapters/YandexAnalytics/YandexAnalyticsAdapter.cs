#if YANDEX_ANALYTICS_ENABLED
namespace UniGame.Runtime.Analytics.Adapters
{
    using System;
    using System.Reflection;
    using Cysharp.Threading.Tasks;
    using Interfaces;
    using UnityEngine;

    [Serializable]
    public sealed class YandexAnalyticsAdapter : IAnalyticsAdapter
    {
        private MethodInfo _sendEventMethod;

        public UniTask InitializeAsync()
        {
            var yg2Type = Type.GetType("YG.YG2, Assembly-CSharp");
            _sendEventMethod = yg2Type?.GetMethod(
                "MetricaSend",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(string) },
                null);

            if (_sendEventMethod == null)
                Debug.LogWarning("Yandex analytics adapter could not find YG2.MetricaSend(string).");

            return UniTask.CompletedTask;
        }

        public void TrackEvent(IAnalyticsMessage message)
        {
            _sendEventMethod?.Invoke(null, new object[] { message.Name });
        }

        public void Dispose()
        {
        }
    }
}
#endif
