namespace Game.Runtime.Services.Analytics.Adapters
{
    using System;
    using System.Collections.Generic;
    using Config;
    using Interfaces;
    using UniGame.Core.Runtime;
    using UniGame.UniNodes.GameFlow.Runtime;
    using UniRx;

    [Serializable]
    public class GameAnalyticsService : GameService, IAnalyticsService
    {
        private List<IAnalyticsAdapter> _adapters = new();

        private AnalyticsModel _model;
        private readonly IContext _context;
        private IAnalyticsMessageChannel _channel;

        public IAnalyticsModel Model => _model;

        public GameAnalyticsService(AnalyticsModel model, IContext context)
        {
            _model = model;
            _context = context;
            _channel = model.MessageChannel;

            InitializeMessageChannel(_channel);
        }

        public void Publish<T>(T message) => _channel.Publish(message);

        private void CleanUp() => _adapters.Clear();

        private void InitializeMessageChannel(IAnalyticsMessageChannel channel)
        {
            channel.Subscribe(PublishToAdapters).AddTo(LifeTime);
        }

        public void RegisterAdapter(IAnalyticsAdapter adapter)
        {
            if (_adapters.Contains(adapter))
                return;

            _adapters.Add(adapter);

            adapter.BindToModel(_model);
        }

        private void PublishToAdapters(IAnalyticsMessage message)
        {
            foreach (var adapter in _adapters)
                adapter.TrackEvent(message);
        }
    }
}