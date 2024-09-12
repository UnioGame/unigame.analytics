namespace Game.Runtime.Services.Analytics.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using UniGame.Core.Runtime;
    using UniGame.Core.Runtime.ScriptableObjects;
    using UniRx;
    using UnityEngine;

    public abstract class AnalyticsAdapter : LifetimeScriptableObject,
        IAnalyticsAdapter,
        IValidator<IAnalyticsMessage>
    {
        private HashSet<Type> _registeredTypes;
        private Subject<IAnalyticsMessage> _messageChannel;
        private IAnalyticsModel _model;

        public void BindToModel(IAnalyticsModel config)
        {
            _registeredTypes = new HashSet<Type>();
            _messageChannel = new Subject<IAnalyticsMessage>().AddTo(LifeTime);
            _model = config;

            _messageChannel
                .Do(HandleEvent)
                .DoOnError(x => Debug.LogError($"Handler {GetType().Name} Error {x}"))
                .CatchIgnore()
                .Subscribe()
                .AddTo(LifeTime);

            OnInitialize(_model);
        }

        public IAnalyticsModel Model => _model;

        public ILifeTime AnalyticsLifetime => _model.LifeTime;

        public virtual bool Validate(IAnalyticsMessage message) => true;

        public void TrackEvent(IAnalyticsMessage message)
        {
            _messageChannel.OnNext(message);
        }

        public virtual void OnTrackEvent(IAnalyticsMessage message)
        {
        }

        protected virtual void OnInitialize(IAnalyticsModel config)
        {
        }

        protected IDisposable Bind<T>(Action<T> handlerAction, bool catchMessage = true)
            where T : IAnalyticsMessage
        {
            return Receive<T>(catchMessage)
                .Where(x => Validate(x))
                .Do(handlerAction)
                .DoOnError(x => Debug.LogError($"Handler {GetType().Name} MessageType {typeof(T).Name} Error {x}"))
                .DoOnError(Debug.LogError)
                .CatchIgnore()
                .Subscribe()
                .AddTo(_model.LifeTime);
        }

        private IObservable<T> Receive<T>(bool catchMessage = true)
            where T : IAnalyticsMessage
        {
            var messageType = typeof(T);
            if (catchMessage) _registeredTypes.Add(messageType);

            return _messageChannel.OfType<IAnalyticsMessage, T>();
        }

        private void HandleEvent(IAnalyticsMessage eventMessage)
        {
            if (_registeredTypes.Any(x => x.IsInstanceOfType(eventMessage)) ||
                !Validate(eventMessage))
                return;

            //default handler for all non registered event types
            OnTrackEvent(eventMessage);
        }
    }
}