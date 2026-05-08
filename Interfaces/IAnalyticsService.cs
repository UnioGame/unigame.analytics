namespace UniGame.Runtime.Analytics.Interfaces
{
    using System;
    using Runtime;
    using UniGame.GameFlow.Runtime;

    public interface IAnalyticsService : IGameService,IAnalyticsHandlers
    {
        void TrackEvent(IAnalyticsMessage message);
        IDisposable RegisterAdapter(IAnalyticsAdapter adapter);
    }
}
