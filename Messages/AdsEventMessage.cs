namespace Game.Runtime.Services.Analytics.Messages
{
    using System;
    using Runtime;

    [Serializable]
    public class AdsEventMessage : AnalyticsEventMessage
    {
        private AnalyticsFloatValue _price;
        private AnalyticsIntValue _duration;
        private AnalyticsBoolValue _firstAds;
        
        public AdsEventMessage(string name)
            : base(name, AnalyticsEventsNames.shop_group)
        {
            _price = new AnalyticsFloatValue(this, AnalyticsEventsNames.ads_price);
            _duration = new AnalyticsIntValue(this, AnalyticsEventsNames.ads_duration);
            _firstAds = new AnalyticsBoolValue(this, AnalyticsEventsNames.ads_first_time);
        }

        
        public string Placement
        {
            get => this[AnalyticsEventsNames.ads_placement_name];
            set => this[AnalyticsEventsNames.ads_placement_name] = value;
        }
        
        public string SdkName
        {
            get => this[AnalyticsEventsNames.ads_sdk_name];
            set => this[AnalyticsEventsNames.ads_sdk_name] = value;
        }
        
        public string AdsType
        {
            set => this[AnalyticsEventsNames.ads_type] = value;
            get => this[AnalyticsEventsNames.ads_type];
        }
        
        public bool FirstTimeAds
        {
            set => _firstAds.Value = value;
            get => _firstAds.Value;
        }

        public float AdsPrice
        {
            set => _price.Value = value;
            get => _price.Value;
        }
        
        public int AdsDuration
        {
            set => _duration.Value = value;
            get => _duration.Value;
        }

        public string ActionType
        {
            set => this[AnalyticsEventsNames.ads_action_type] = value;
        }
        
        public string Message
        {
            set => this[AnalyticsEventsNames.ads_message] = value;
        }
        
        public string ErrorCode
        {
            set => this[AnalyticsEventsNames.ads_error_code] = value;
        }

        public string AdsCountAward
        {
            set => this[AnalyticsEventsNames.ads_reward_count] = value;
        }
    }
}
