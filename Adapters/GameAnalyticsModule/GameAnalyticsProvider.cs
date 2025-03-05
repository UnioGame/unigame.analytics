namespace Game.Modules.Analytics
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Game.Runtime.Services.Analytics.Interfaces;
    using GameAnalyticsSDK;
    using Runtime.Services.Analytics;
    using Runtime.Services.Analytics.Messages;
    using Runtime.Services.Analytics.Runtime;
    using Sirenix.OdinInspector;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.AddressableTools.Runtime;
    using UniModules.UniCore.Runtime.DataFlow;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Object = UnityEngine.Object;

    [Serializable]
    public sealed class GameAnalyticsProvider: IAnalyticsAdapter,IGameAnalyticsATTListener
    {
        public AssetReferenceGameObject gameAnalyticsPrefab;

        public bool enableUnderEditor = false;
        
        [ReadOnly]
        public string UserIdInfo;
        
        private GameAnalytics _gameAnalytics;
        private LifeTime _lifeTime;
        private bool _isUserIdInitialized;
        
        public async UniTask InitializeAsync()
        {
            if(Application.isEditor && !enableUnderEditor)
                return;
            
            _lifeTime?.Release();
            _lifeTime = new LifeTime();
            
            _gameAnalytics = await gameAnalyticsPrefab
                .InstantiateTaskAsync<GameAnalytics>(_lifeTime,true);
            _gameAnalytics.gameObject.SetActive(true);
            
            Object.DontDestroyOnLoad(_gameAnalytics.gameObject);
            
            if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                GameAnalytics.RequestTrackingAuthorization(this);
            }
            else
            {
                GameAnalytics.Initialize();
            }
        }

        public void TrackEvent(IAnalyticsMessage message)
        {
            if(Application.isEditor && !enableUnderEditor)
                return;
            
            SetupUserId(message);
            

            
            if(message is PaymentEventMessage { Name: AnalyticsEventsNames.payment_complete } purchaseMessage)
            {
#if UNITY_EDITOR
                LogGameAnalytics($"{nameof(PaymentEventMessage)} | {purchaseMessage}");
#endif
                TrackPurchase(purchaseMessage);
                return;
            }

            if (message is GameResourceFlowEventMessage gameResourceEventMessage)
            {
#if UNITY_EDITOR
                LogGameAnalytics($"{nameof(GameResourceFlowEventMessage)} | {gameResourceEventMessage}");
#endif
                TrackGameResource(gameResourceEventMessage);
                return;
            }
            
            if(message is AdsEventMessage adsEventMessage)
            {
#if UNITY_EDITOR
                LogGameAnalytics($"{nameof(AdsEventMessage)} | {adsEventMessage}");
#endif
                TrackAdsEvent(adsEventMessage);
                return;
            }

            if (message is GAProgressionEventMessage progressionEventMessage)
            {
#if UNITY_EDITOR
                LogGameAnalytics($"{nameof(GAProgressionEventMessage)} | {progressionEventMessage}");
#endif
                TrackProgression(progressionEventMessage);
                return;
            }
            
            var parametersValue = new Dictionary<string, object>();
            foreach (var keyValue in message.Parameters)
                parametersValue[keyValue.Key] = keyValue.Value;
            
            // send ad event
            GameAnalytics.NewDesignEvent(message.Name, parametersValue);

#if UNITY_EDITOR
            LogGameAnalytics($"NewDesignEvent | {message}");
#endif
        }
        
        public void TrackAdsEvent(AdsEventMessage message)
        {
            var adAction = GAAdAction.Undefined;
            var adType = GAAdType.Undefined;

            switch (message.AdsType)
            {
                case AnalyticsEventsNames.rewarded_video:
                    adType = GAAdType.RewardedVideo;
                    break;
                case AnalyticsEventsNames.video:
                    adType = GAAdType.Video;
                    break;
                case AnalyticsEventsNames.interstitial:
                    adType = GAAdType.Interstitial;
                    break;
                case AnalyticsEventsNames.offer_wall:
                    adType = GAAdType.OfferWall;
                    break;
                case AnalyticsEventsNames.banner:
                    adType = GAAdType.Banner;
                    break;
                case AnalyticsEventsNames.playable:
                    adType = GAAdType.Playable;
                    break;
                default:
                    adType = GAAdType.RewardedVideo;
                    break;
            }

            switch (message.ActionType)
            {
                case AnalyticsEventsNames.failed:
                    adAction = GAAdAction.FailedShow;
                    break;
                case AnalyticsEventsNames.clicked:
                    adAction = GAAdAction.Clicked;
                    break;
                case AnalyticsEventsNames.opened:
                    adAction = GAAdAction.Show;
                    break;
                case AnalyticsEventsNames.rewarded:
                    adAction = GAAdAction.RewardReceived;
                    break;
                case AnalyticsEventsNames.requested:
                    adAction = GAAdAction.Request;
                    break;
                case AnalyticsEventsNames.closed:
                    adAction = GAAdAction.Undefined;
                    break;   
                case AnalyticsEventsNames.unavailable:
                    adAction = GAAdAction.Undefined;
                    break;              
                case AnalyticsEventsNames.available:
                    adAction = GAAdAction.Loaded;
                    break;               
            }
            
            GameAnalytics.NewAdEvent(
                adAction,
                adType,
                message.SdkName,
                message.Placement,
                message.AdsDuration);
        }

        public void TrackProgression(GAProgressionEventMessage message)
        {
            var progressionStatus = GAProgressionStatus.Undefined;
            switch (message.ProgressionStatus)
            {
                case GameAnalyticsConstants.complete:
                    progressionStatus = GAProgressionStatus.Complete;
                    break;
                case GameAnalyticsConstants.fail:
                    progressionStatus = GAProgressionStatus.Fail;
                    break;
                case GameAnalyticsConstants.start:
                    progressionStatus = GAProgressionStatus.Start;
                    break;
            }
            
            GameAnalytics.NewProgressionEvent(
                progressionStatus,message.Progression01,message.Progression02,message.Progression03,message.Score);
        }

        public void TrackGameResource(GameResourceFlowEventMessage message)
        {
            var flowType = message.FlowType;
            var gaFlowType = GAResourceFlowType.Undefined;
            
            switch (flowType)
            {
                case AnalyticsEventsNames.resource_flow_added:
                    gaFlowType = GAResourceFlowType.Source;
                    break;
                case AnalyticsEventsNames.resource_flow_spend:
                    gaFlowType = GAResourceFlowType.Sink;
                    break;
                default:
                    gaFlowType = GAResourceFlowType.Undefined;
                    break;
            }
            
            GameAnalytics.NewResourceEvent(gaFlowType,message.ResourceType,message.ResourceValue,message.ItemType,message.ItemId);
        }
        
        public void TrackPurchase(PaymentEventMessage message)
        {
            var receipt = message.Receipt;
            var isReceiptEmpty = string.IsNullOrEmpty(receipt);
            var isSignatureEmpty = string.IsNullOrEmpty(message.Signature);
            
#if UNITY_IOS || UNITY_TVOS
            if (!isReceiptEmpty)
            {
                GameAnalytics.NewBusinessEventIOS(message.Currency,
                    message.Price,message.
                    ItemType,message.ItemId,message.CartType,message.Receipt);
                return;
            }
#endif

#if UNITY_ANDROID

            if (!isReceiptEmpty && isSignatureEmpty)
            {
                GameAnalytics.NewBusinessEventGooglePlay(
                    message.Currency,
                    (int)message.Price,
                    message.ItemType,
                    message.ItemId,
                    message.CartType,
                    message.Receipt,
                    message.Signature);
                return;
            }

#endif
            
            GameAnalytics.NewBusinessEvent(message.Currency, (int)message.Price, message.ItemType,message.ItemId, message.CartType);
        }

        public void SetupUserId(IAnalyticsMessage message)
        {
            if(_isUserIdInitialized) return;

            UserIdInfo = message[AnalyticsEventsNames.user_id];
            
            _isUserIdInitialized = string.IsNullOrEmpty(UserIdInfo) == false;
            
            if(_isUserIdInitialized)
                GameAnalytics.SetCustomId(UserIdInfo);
        }

        public void LogGameAnalytics(string message)
        {
            GameLog.Log($"GA ANALYTICS: {message}",Color.yellow);
        }
        
        public void Dispose() => _lifeTime?.Release();

        #region Game Analytics ATT Listener

        public void GameAnalyticsATTListenerNotDetermined()
        {
            GameAnalytics.Initialize();
        }
        public void GameAnalyticsATTListenerRestricted()
        {
            GameAnalytics.Initialize();
        }
        public void GameAnalyticsATTListenerDenied()
        {
            GameAnalytics.Initialize();
        }
        public void GameAnalyticsATTListenerAuthorized()
        {
            GameAnalytics.Initialize();
        }

        #endregion
        
        
    }
}
