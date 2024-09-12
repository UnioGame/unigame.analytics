namespace Game.Runtime.Services.Analytics.Runtime
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Interfaces;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniRx;
    using UnityEngine;

    public class AnalyticsMessageChannel : IAnalyticsMessageChannel
    {
        #region static data

        public static IAnalyticsMessageChannel DefaultChannel = new AnalyticsMessageChannel();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void InitializeStatic()
        {
            DefaultChannel?.Dispose();
            DefaultChannel = new AnalyticsMessageChannel();
        }

        #endregion
        
        private List<IAnalyticsMessageHandler> _handlers = new();

        private LifeTimeDefinition _lifeTime;
        private MessageBroker _broker;
        private Subject<IAnalyticsMessage> _messageSubject;
        private IObservable<IAnalyticsMessage> _shareConnection;

        public AnalyticsMessageChannel()
        {
            _lifeTime = new LifeTimeDefinition();
            _broker = new MessageBroker().AddTo(_lifeTime);
            _messageSubject = new Subject<IAnalyticsMessage>().AddTo(_lifeTime);
            _shareConnection = _messageSubject.Share();
        }

        public void Dispose() => _lifeTime.Terminate();


        public IAnalyticsMessageChannel UpdateHandlers(IEnumerable<IAnalyticsMessageHandler> messageHandlers)
        {
            _handlers.Clear();
            _handlers.AddRange(messageHandlers);

            return this;
        }

        public void PublishMessage(IAnalyticsMessage message)
        {
            PublishMessageAsync(message).Forget();
        }

        public void Publish<T>(T message)
        {
            if (_lifeTime.IsTerminated)
                return;

            if (message is IAnalyticsMessage analyticsEventMessage)
            {
                PublishMessageAsync(analyticsEventMessage)
                    .AttachExternalCancellation(_lifeTime.Token)
                    .Forget();
            }

            _broker.Publish(message);
        }

        public IObservable<T> Receive<T>() => _broker.Receive<T>();

        public IDisposable Subscribe(IObserver<IAnalyticsMessage> observer) => _shareConnection.Subscribe(observer);

        public async UniTask PublishMessageAsync(IAnalyticsMessage message)
        {
            if (_lifeTime.IsTerminated) return;

            foreach (var handler in _handlers)
                message = await handler.UpdateEventAsync(message);

            _messageSubject.OnNext(message);
        }
    }
}