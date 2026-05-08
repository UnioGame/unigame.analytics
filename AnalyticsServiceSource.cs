namespace UniGame.Runtime.Analytics
{
    using Adapters;
    using Cysharp.Threading.Tasks;
    using FpsService;
    using Interfaces;
    using R3;
    using Runtime;
    using UniGame.AddressableTools.Runtime;
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
                .LoadAssetTaskAsync(lifeTime, true);
            
            var configuration = configurationAsset.configuration;
            var channel = AnalyticsMessageChannel.DefaultChannel;
            var service = CreateService(lifeTime);
            var platformId = ResolvePlatformId(configuration);
            
            channel.ToObservable()
                .Subscribe(service.TrackEvent)
                .AddTo(lifeTime);

            if (!configuration.isEnabled || !configuration.IsPlatformAllowed(platformId))
            {
                PublishCoreServices(context, channel);
                return service;
            }
            
            await RegisterAdapters(service, configuration, lifeTime, platformId);
            
            foreach (var messageHandler in configuration.messageHandlers)
            {
                service.RegisterMessageHandler(messageHandler);
            }
            
            PublishCoreServices(context, channel);

            return service;
        }

        private async UniTask RegisterAdapters(
            IAnalyticsService service,
            AnalyticsConfiguration configuration,
            ILifeTime lifeTime,
            string platformId)
        {
            var initializeTasks = new System.Collections.Generic.List<UniTask>();
            foreach (var analyticsItem in configuration.analytics)
            {
                if (analyticsItem.isEnabled == false || analyticsItem.adapter == null)
                    continue;

                if (!analyticsItem.IsPlatformAllowed(platformId))
                    continue;

                initializeTasks.Add(InitializeAdapter(analyticsItem.adapter, service, lifeTime));
            }

            await UniTask.WhenAll(initializeTasks);
        }

        private async UniTask InitializeAdapter(IAnalyticsAdapter adapter, IAnalyticsService service,ILifeTime lifeTime)
        {
            try
            {
                adapter.AddTo(lifeTime);
                await adapter.InitializeAsync();
                service.RegisterAdapter(adapter);
            }
            catch (System.Exception exception)
            {
                UnityEngine.Debug.LogException(exception);
            }
        }

        private GameAnalyticsService CreateService(
            ILifeTime lifeTime)
        {
            var service = new GameAnalyticsService().AddTo(lifeTime);
            return service;
        }

        private static void PublishCoreServices(IContext context, IAnalyticsMessageChannel channel)
        {
            var fpsService = new FpsService.FpsService();

            context.Publish(fpsService);
            context.Publish<IFpsService>(fpsService);
            context.Publish(channel);
        }

        private static string ResolvePlatformId(AnalyticsConfiguration configuration)
        {
            if (!string.IsNullOrWhiteSpace(configuration.platformIdOverride))
                return configuration.platformIdOverride.ToLowerInvariant();

            return Application.platform switch
            {
                RuntimePlatform.Android => "android",
                RuntimePlatform.IPhonePlayer => "ios",
                RuntimePlatform.WebGLPlayer => "webgl",
                _ => Application.platform.ToString().ToLowerInvariant()
            };
        }
    }
}