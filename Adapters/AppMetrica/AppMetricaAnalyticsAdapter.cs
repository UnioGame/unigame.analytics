#if APP_METRICA_ENABLED
namespace UniGame.Runtime.Analytics.Adapters
{
    using System;
    using Cysharp.Threading.Tasks;
    using Interfaces;
    using Io.AppMetrica;
    using Newtonsoft.Json;

    [Serializable]
    public sealed class AppMetricaAnalyticsAdapter : IAnalyticsAdapter
    {
        public UniTask InitializeAsync()
        {
            return UniTask.CompletedTask;
        }

        public void TrackEvent(IAnalyticsMessage message)
        {
            AppMetrica.ReportEvent(message.Name, JsonConvert.SerializeObject(message.Parameters));
        }

        public void Dispose()
        {
        }
    }
}
#endif
