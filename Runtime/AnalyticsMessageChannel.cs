namespace Game.Runtime.Services.Analytics.Runtime
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Interfaces;
    using R3;
    using UniGame.Runtime.Common;
    using UniGame.Runtime.DataFlow;
    using UniGame.Runtime.Rx;
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

        private LifeTime _lifeTime;
        private MessageBroker _broker;
        private Subject<IAnalyticsMessage> _messageSubject;
        private Observable<IAnalyticsMessage> _shareConnection;

        public AnalyticsMessageChannel()
        {
            _lifeTime = new LifeTime();
            _broker = LifetimeExtension.AddTo(new MessageBroker(), _lifeTime);
            _messageSubject = new Subject<IAnalyticsMessage>();
            RxLifetimeExtension.AddTo(_messageSubject, _lifeTime);
            _shareConnection = _messageSubject.Share();
        }

        public void Dispose() => _lifeTime.Terminate();

        public IDisposable RegisterMessageHandler(IAnalyticsMessageHandler handler)
        {
            if (_handlers.Contains(handler))
                return Disposable.Empty;

            _handlers.Add(handler);
            
            return new DisposableAction()
                .Initialize(() => _handlers.Remove(handler));
        }

        public IAnalyticsMessageChannel UpdateHandlers(IEnumerable<IAnalyticsMessageHandler> messageHandlers)
        {
            _handlers.Clear();
            _handlers.AddRange(messageHandlers);
            return this;
        }

        public void Publish(IAnalyticsMessage message)
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

        public Observable<T> Receive<T>() => _broker.Receive<T>();

        public IDisposable Subscribe(Observer<IAnalyticsMessage> observer) => 
            _shareConnection.Subscribe(observer);

        public async UniTask PublishMessageAsync(IAnalyticsMessage message)
        {
            if (_lifeTime.IsTerminated) return;

            foreach (var handler in _handlers)
                message = await handler.UpdateMessageAsync(message);

            _messageSubject.OnNext(message);
        }

        public IDisposable Subscribe(IObserver<IAnalyticsMessage> observer)
        {
            return Subscribe(observer.ToObserver());
        }
    }
}