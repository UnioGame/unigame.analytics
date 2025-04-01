namespace Game.Runtime.Services.Analytics.Messages
{
    using System;
    using Runtime;

    [Serializable]
    public class PromoCodeAnalyticsMessage : AnalyticsEventMessage
    {
        public PromoCodeAnalyticsMessage() 
            : base(AnalyticsEventsNames.promocode_result, AnalyticsEventsNames.feature)
        {
        }
        public PromoCodeAnalyticsMessage(string messageName) 
            : base(messageName, AnalyticsEventsNames.feature)
        {
        }
        
        
        public string PromoCode
        {
            set
            {
                this[AnalyticsEventsNames.promocode] = value;
            } 
        }
        
        public string Reward
        {
            set
            {
                this[AnalyticsEventsNames.promocode_reward] = value;
            } 
        }
    }
    
    [Serializable]
    public class PromoCodeErrorAnalyticsMessage : PromoCodeAnalyticsMessage
    {
        public PromoCodeErrorAnalyticsMessage() 
            : base(AnalyticsEventsNames.promocode_error)
        {
        }
        
        public string Error
        {
            set
            {
                this[AnalyticsEventsNames.promocode_error] = value;
            } 
        }
    }
}