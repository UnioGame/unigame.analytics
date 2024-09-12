namespace Game.Runtime.Services.Analytics.Interfaces
{
    using UniGame.Core.Runtime;

    public interface IAnalyticsAdapter
    {
        ILifeTime AnalyticsLifetime { get; }

        void TrackEvent(IAnalyticsMessage message);

        void BindToModel(IAnalyticsModel analyticsModel);
    }
}