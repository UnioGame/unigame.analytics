namespace Game.Runtime.Services.Analytics
{
    using Adapters;
    using Cysharp.Threading.Tasks;
    using FpsService;
    using Interfaces;
    using R3;
    using Runtime;
    using UniGame.AddressableTools.Runtime;
    using UniGame.AddressableTools.Runtime.AssetReferencies;
    using UniGame.Context.Runtime;
    using UniGame.Core.Runtime;
    using UnityEngine;

    /// <summary>
    /// game analytics service
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Services/Analytics/Analytics Service Source")]
    public class AnalyticsServiceSource : DataSourceAsset<IAnalyticsService>
    {
        public AddressableValue<AnalyticsConfigurationAsset> configurationReference;

        protected override async UniTask<IAnalyticsService> CreateInternalAsync(IContext context)
        {
            var lifeTime = context.LifeTime;
            var configurationAsset = await configurationReference
                .reference
                .LoadAssetInstanceTaskAsync(lifeTime, true);
            
            var configuration = configurationAsset.configuration;
            var channel = AnalyticsMessageChannel.DefaultChannel;
            var service = CreateService(lifeTime);
            
            channel.ToObservable()
                .Subscribe(service.TrackEvent)
                .AddTo(lifeTime);
            
            RegisterAdapters(service, configuration,lifeTime);
            
            foreach (var messageHandler in configuration.messageHandlers)
            {
                service.RegisterMessageHandler(messageHandler);
            }
            
            var fpsService = new FpsService.FpsService();

            context.Publish(fpsService);
            context.Publish<IFpsService>(fpsService);
            context.Publish(channel);

            return service;
        }

        private void RegisterAdapters(IAnalyticsService service,AnalyticsConfiguration configuration, ILifeTime lifeTime)
        {
            foreach (var analyticsItem in configuration.analytics)
            {
                if(analyticsItem.isEnabled == false)
                    continue;
                InitializeAdapter(analyticsItem.adapter, service,lifeTime).Forget();
            }
        }

        private async UniTask InitializeAdapter(IAnalyticsAdapter adapter, IAnalyticsService service,ILifeTime lifeTime)
        {
            adapter.AddTo(lifeTime);
            await adapter.InitializeAsync();
            service.RegisterAdapter(adapter);
        }

        private GameAnalyticsService CreateService(
            ILifeTime lifeTime)
        {
            var service = new GameAnalyticsService().AddTo(lifeTime);
            return service;
        }
    }
}