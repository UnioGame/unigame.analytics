namespace Game.Runtime.Services.Analytics
{
    using System;
    using Adapters;
    using Config;
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
    using UnityEngine.AddressableAssets;

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

            var userId = Observable.Return(Application.identifier);
            var model = CreateModel(configuration, userId);
            var channel = AnalyticsMessageChannel.DefaultChannel;
            var service = await CreateService(context, model, configuration, context.LifeTime);

            foreach (var messageHandler in configuration.messageHandlers)
            {
                await messageHandler.Initialize(context,LifeTime);
                service.RegisterMessageHandler(messageHandler);
            }
            
            var fpsService = new FpsService.FpsService();

            context.Publish(fpsService);
            context.Publish<IFpsService>(fpsService);

            context.Publish(channel);
            context.Publish<IAnalyticsModel>(model);

            return service;
        }

        private async UniTask<GameAnalyticsService> CreateService(IContext context,
            AnalyticsModel model,
            AnalyticsConfiguration configuration,
            ILifeTime lifeTime)
        {
            if (!configuration.isEnabled)
                return new GameAnalyticsService(model);

            var analytics = await UniTask
                .WhenAll(configuration.analytics
                    .Select(x => x.LoadAssetInstanceTaskAsync(lifeTime, true)));

            foreach (var analyticsAdapter in analytics)
                analyticsAdapter.AddTo(lifeTime);

            var service = new GameAnalyticsService(model).AddTo(lifeTime);

            foreach (var analytic in analytics)
                service.RegisterAdapter(analytic);

            return service;
        }

        private AnalyticsModel CreateModel(AnalyticsConfiguration configuration, IObservable<string> userId)
        {
            if (configuration.isEnabled == false)
                return new AnalyticsModel();

            var model = configuration.analyticsModel;
            var isDebugMode = false;

#if DEBUG
            isDebugMode = true;
#endif

            model = new AnalyticsModel()
            {
                isDebug = new BoolReactiveProperty(isDebugMode),
                customerUserId = userId.ToReadOnlyReactiveProperty()
            }
                .AddTo(LifeTime);

            return model;
        }
    }
}