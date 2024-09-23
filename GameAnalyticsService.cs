namespace Game.Runtime.Services.Analytics.Adapters
{
    using System;
    using System.Collections.Generic;
    using Config;
    using Interfaces;
    using Runtime;
    using UniGame.Core.Runtime;
    using UniGame.UniNodes.GameFlow.Runtime;
    using UniRx;

    [Serializable]
    public class GameAnalyticsService : GameService, IAnalyticsService
    {
        private List<IAnalyticsAdapter> _adapters = new();

        private AnalyticsModel _model;
        private IAnalyticsMessageChannel _channel;

        public IAnalyticsModel Model => _model;

        public GameAnalyticsService(AnalyticsModel model)
        {
            _model = model;
            _channel = model.MessageChannel;

            InitializeMessageChannel(_channel);
        }

        public IDisposable RegisterMessageHandler(IAnalyticsMessageHandler handler)
        {
            return _channel.RegisterMessageHandler(handler);
        }

        public IAnalyticsMessageChannel UpdateHandlers(IEnumerable<IAnalyticsMessageHandler> messageHandlers)
        {
            return _channel.UpdateHandlers(messageHandlers);
        }

        public void Publish<T>(T message) => _channel.Publish(message);
        
        public void RegisterAdapter(IAnalyticsAdapter adapter)
        {
            if (_adapters.Contains(adapter))
                return;

            _adapters.Add(adapter);

            adapter.BindToModel(_model);
        }
        
        private void InitializeMessageChannel(IAnalyticsMessageChannel channel)
        {
            channel.Subscribe(PublishToAdapters).AddTo(LifeTime);
        }


        private void PublishToAdapters(IAnalyticsMessage message)
        {
            foreach (var adapter in _adapters)
                adapter.TrackEvent(message);
        }

        private void CleanUp() => _adapters.Clear();

    }
}