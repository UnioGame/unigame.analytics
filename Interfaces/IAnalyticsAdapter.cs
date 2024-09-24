namespace Game.Runtime.Services.Analytics.Interfaces
{
    using System;
    using Cysharp.Threading.Tasks;

    public interface IAnalyticsAdapter : IDisposable
    {
        UniTask InitializeAsync();
        
        void TrackEvent(IAnalyticsMessage message);
    }
}