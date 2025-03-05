namespace Game.Runtime.Services.Analytics.Adapters
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Interfaces;
    using UniGame.UniNodes.GameFlow.Runtime;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class GameAnalyticsService : GameService, IAnalyticsService
    {
        private List<IAnalyticsAdapter> _adapters = new();
        private List<IAnalyticsMessageHandler> _handlers = new();
        
        public void TrackEvent(IAnalyticsMessage message)
        {
            TrackEventAsync(message).Forget();
        }

        public IDisposable RegisterMessageHandler(IAnalyticsMessageHandler handler)
        {
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
            foreach (var handler in _handlers)
                 data = await handler.UpdateMessageAsync(data);
            PublishToAdapters(data);
        }
        
        private void InitializeMessageChannel(IAnalyticsMessageChannel channel)
        {
            channel.Subscribe(PublishToAdapters).AddTo(LifeTime);
        }
        
        private void PublishToAdapters(IAnalyticsMessage message)
        {
            foreach (var adapter in _adapters)
            {
                try
                {
                    adapter.TrackEvent(message);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    continue;
                }
            }
        }

        private void CleanUp() => _adapters.Clear();

    }
}