namespace Game.Runtime.Services.Analytics.Interfaces
{
    using R3;
    using UniGame.Core.Runtime;

    public interface IAnalyticsModel : ILifeTimeContext
    {
        ReactiveProperty<bool> IsDebug { get; }

        IAnalyticsMessageChannel MessageChannel { get; }
    }
}