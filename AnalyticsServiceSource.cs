namespace Game.Runtime.Services.Analytics
{
    using Adapters;
    using Cysharp.Threading.Tasks;
    using FpsService;
    using Interfaces;
    using Runtime;
    using UniGame.AddressableTools.Runtime;
    using UniGame.AddressableTools.Runtime.AssetReferencies;
    using UniGame.Core.Runtime;
    using UniGame.GameFlow.Runtime.Services;
    using UniRx;
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
            var configurationAsset = await configurationReference
                .reference
                .LoadAssetInstanceTaskAsync(LifeTime, true);
            
            var configuration = configurationAsset.configuration;
            var channel = AnalyticsMessageChannel.DefaultChannel;
            var service = CreateService(LifeTime);
            
            channel.Subscribe(service.TrackEvent).AddTo(LifeTime);
            
            RegisterAdapters(service, configuration);
            
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

        private void RegisterAdapters(IAnalyticsService service,AnalyticsConfiguration configuration)
        {
            foreach (var analyticsItem in configuration.analytics)
            {
                if(analyticsItem.isEnabled == false)
                    continue;
                InitializeAdapter(analyticsItem.adapter, service).Forget();
            }
        }

        private async UniTask InitializeAdapter(IAnalyticsAdapter adapter, IAnalyticsService service)
        {
            adapter.AddTo(LifeTime);
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