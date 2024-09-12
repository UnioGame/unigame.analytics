namespace Game.Runtime.Services.Analytics.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniRx;

    public interface IAnalyticsMessageChannel :
        IObservable<IAnalyticsMessage>,
        IMessageBroker,
        IDisposable
    {
        UniTask PublishMessageAsync(IAnalyticsMessage message);

        void PublishMessage(IAnalyticsMessage message);
        
        IAnalyticsMessageChannel UpdateHandlers(IEnumerable<IAnalyticsMessageHandler> messageHandlers);
    }
}