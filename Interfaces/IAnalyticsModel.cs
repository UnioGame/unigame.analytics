namespace Game.Runtime.Services.Analytics.Interfaces
{
    using UniGame.Core.Runtime;
    using UniRx;

    public interface IAnalyticsModel : ILifeTimeContext
    {
        IReactiveProperty<bool> IsDebug { get; }

        IAnalyticsMessageChannel MessageChannel { get; }
    }
}