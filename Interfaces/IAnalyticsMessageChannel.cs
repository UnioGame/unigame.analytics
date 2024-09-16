namespace Game.Runtime.Services.Analytics.Interfaces
{
    using System;
    using Cysharp.Threading.Tasks;
    using Runtime;
    using UniRx;

    public interface IAnalyticsMessageChannel :
        IObservable<IAnalyticsMessage>,
        IAnalyticsHandlers,
        IMessageBroker,
        IDisposable
    {
        UniTask PublishMessageAsync(IAnalyticsMessage message);

        void PublishMessage(IAnalyticsMessage message);
    }
}