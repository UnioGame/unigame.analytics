namespace Game.Runtime.Services.Analytics.Interfaces
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
