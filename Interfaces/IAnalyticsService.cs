namespace Game.Runtime.Services.Analytics.Interfaces
{
    using UniGame.GameFlow.Runtime.Interfaces;
    using UniRx;

    public interface IAnalyticsService : IGameService,IMessagePublisher
    {
        IAnalyticsModel Model { get; }
    }
}
