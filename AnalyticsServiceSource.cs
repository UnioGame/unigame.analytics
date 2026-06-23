namespace UniGame.Runtime.Analytics
{
    using System.Collections.Generic;
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
    public class AnalyticsServiceSource : DataSourceAsset
    {
        public AddressableValue<AnalyticsConfigurationAsset> configurationReference;

        protected override async UniTask<IContext> OnRegisterAsync(IContext context)
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

            var fpsService = new FpsService.FpsService();

            context.Publish(fpsService);
            context.Publish<IFpsService>(fpsService);
            context.Publish(channel);
            context.Publish(service);
            
            if (!configuration.isEnabled || !configuration.IsPlatformAllowed(platformId))
                return context;
            
            await RegisterAdapters(service, configuration, lifeTime, platformId);
            
            foreach (var messageHandler in configuration.messageHandlers)
                service.RegisterMessageHandler(messageHandler);
            
            return context;
        }

        private async UniTask RegisterAdapters(
            IAnalyticsService service,
            AnalyticsConfiguration configuration,
            ILifeTime lifeTime,
            string platformId)
        {
            var initializeTasks = new List<UniTask>();
            foreach (var analyticsItem in configuration.analytics)
            {
                if (!analyticsItem.isEnabled || analyticsItem.adapter == null)
                    continue;

                if (!analyticsItem.IsPlatformAllowed(platformId))
                    continue;

                var adapterTask = InitializeAdapter(analyticsItem.adapter, service, lifeTime);
                initializeTasks.Add(adapterTask);
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
                Debug.LogException(exception);
            }
        }

        private GameAnalyticsService CreateService(
            ILifeTime lifeTime)
        {
            var service = new GameAnalyticsService().AddTo(lifeTime);
            return service;
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