namespace UniGame.Runtime.Analytics
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Interfaces;
    using R3;
    using UniGame.GameFlow.Runtime;
    using UnityEngine;

    [Serializable]
    public class GameAnalyticsService : GameService, IAnalyticsService
    {
        private List<IAnalyticsAdapter> _adapters = new();
        private List<IAnalyticsMessageHandler> _handlers = new();

        public GameAnalyticsService()
        {
            LifeTime.AddCleanUpAction(CleanUp);
        }
        
        public void TrackEvent(IAnalyticsMessage message)
        {
            if(LifeTime.IsTerminated) return;
            TrackEventAsync(message).Forget();
        }

        public IDisposable RegisterMessageHandler(IAnalyticsMessageHandler handler)
        {
            if (LifeTime.IsTerminated)
                return Disposable.Empty;

            if (_handlers.Contains(handler))
                return Disposable.Empty;

            _handlers.Add(handler);

            return Disposable.Create(() =>
            {
                if (_handlers.Contains(handler))
                    _handlers.Remove(handler);
            });
        }

        public void UpdateHandlers(IEnumerable<IAnalyticsMessageHandler> messageHandlers)
        {
            foreach (var messageHandler in messageHandlers)
            {
                RegisterMessageHandler(messageHandler);
            }
        }

        public IDisposable RegisterAdapter(IAnalyticsAdapter adapter)
        {
            if(LifeTime.IsTerminated) 
                return Disposable.Empty;
            
            if (_adapters.Contains(adapter))
                return Disposable.Empty;

            _adapters.Add(adapter);
            
            return Disposable.Create(() =>
            {
                if (_adapters.Contains(adapter))
                    _adapters.Remove(adapter);
            });
        }

        private async UniTask TrackEventAsync(IAnalyticsMessage message)
        {
            var data = message;
            for (var i = 0; i < _handlers.Count; i++)
            {
                try
                {
                    data = await _handlers[i].UpdateMessageAsync(data);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            PublishToAdapters(data);
        }
        
        private void InitializeMessageChannel(IAnalyticsMessageChannel channel)
        {
            channel.ToObservable()
                .Subscribe(PublishToAdapters)
                .AddTo(LifeTime);
        }
        
        private void PublishToAdapters(IAnalyticsMessage message)
        {
            for (var i = 0; i < _adapters.Count; i++)
            {
                try
                {
                    _adapters[i].TrackEvent(message);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private void CleanUp()
        {
            _adapters.Clear();
            _handlers.Clear();
        }

    }
}