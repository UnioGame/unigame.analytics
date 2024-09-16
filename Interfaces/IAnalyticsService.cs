namespace Game.Runtime.Services.Analytics.Interfaces
{
    using Runtime;
    using UniGame.GameFlow.Runtime.Interfaces;
    using UniRx;

    public interface IAnalyticsService : IGameService,IMessagePublisher,IAnalyticsHandlers
    {
        IAnalyticsModel Model { get; }
    }
}
