namespace Game.Runtime.Services.Analytics.Config
{
    using System;
    using Interfaces;
    using Runtime;
    using UniGame.Core.Runtime;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniRx;

    [Serializable]
    public class AnalyticsModel : IAnalyticsModel, IDisposable
    {
        public BoolReactiveProperty isDebug = new();
        public IReadOnlyReactiveProperty<string> customerUserId;

        private LifeTimeDefinition _lifeTime;
        private IAnalyticsMessageChannel _messageChannel;

        public IReadOnlyReactiveProperty<string> CustomerUserId => customerUserId;
        public IAnalyticsMessageChannel DefaultChannel => AnalyticsMessageChannel.DefaultChannel;
        
        public IReactiveProperty<bool> IsDebug => isDebug;
        public ILifeTime LifeTime => _lifeTime ??= new LifeTimeDefinition();
        public IAnalyticsMessageChannel MessageChannel =>
            _messageChannel ??= new AnalyticsMessageChannel().AddTo(LifeTime);

        public AnalyticsModel()
        {
            _messageChannel = new AnalyticsMessageChannel()
                .AddTo(LifeTime);

            AnalyticsMessageChannel.DefaultChannel
                .Do(x => _messageChannel.Publish(x))
                .Subscribe()
                .AddTo(LifeTime);
        }

        public void Dispose()
        {
            _lifeTime?.Terminate();
            GC.SuppressFinalize(this);
        }

        ~AnalyticsModel() => _lifeTime?.Terminate();
    }
}