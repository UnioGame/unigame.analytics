namespace Game.Modules.Analytics
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Game.Runtime.Services.Analytics.Interfaces;
    using GameAnalyticsSDK;
    using Runtime.Services.Analytics;
    using Runtime.Services.Analytics.Runtime;
    using Sirenix.OdinInspector;
    using UniGame.AddressableTools.Runtime;
    using UniModules.UniCore.Runtime.DataFlow;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Object = UnityEngine.Object;

    [Serializable]
    public sealed class GameAnalyticsProvider: IAnalyticsAdapter,IGameAnalyticsATTListener
    {
        public AssetReferenceGameObject gameAnalyticsPrefab;

        public string UserIdField = "UserId";
        public string AdName;

        [ReadOnly]
        public string UserIdInfo;
        
        private GameAnalytics _gameAnalytics;
        private LifeTime _lifeTime;
        private bool _isUserIdInitialized;
        
        public async UniTask InitializeAsync()
        {
            _lifeTime?.Release();
            _lifeTime = new LifeTime();
            
            _gameAnalytics = await gameAnalyticsPrefab
                .InstantiateTaskAsync<GameAnalytics>(_lifeTime,true);
            
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
            SetupUserId(message);

            if(message is PaymentEventMessage { Name: AnalyticsEventsNames.payment_complete } purchaseMessage)
            {
                TrackPurchase(purchaseMessage);
                return;
            }

            if (message is GameResourceFlowEventMessage gameResourceEventMessage)
            {
                TrackGameResource(gameResourceEventMessage);
                return;
            }
            
            var parametersValue = new Dictionary<string, object>();
            foreach (var keyValue in message.Parameters)
                parametersValue[keyValue.Key] = keyValue.Value;
            
            // send ad event
            GameAnalytics.NewDesignEvent(message.Name, parametersValue);
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
                    message.Price,message.
                        ItemType,message.ItemId,
                    message.CartType,
                    message.Receipt,
                    message.Signature);
                return;
            }

#endif
            
            GameAnalytics.NewBusinessEvent(message.Currency, message.Price, message.ItemType,message.ItemId, message.CartType);
        }

        public void SetupUserId(IAnalyticsMessage message)
        {
            if(_isUserIdInitialized) return;

            UserIdInfo = message[UserIdField];
            
            _isUserIdInitialized = string.IsNullOrEmpty(UserIdInfo) == false;
            
            if(_isUserIdInitialized)
                GameAnalytics.SetCustomId(UserIdInfo);
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
