namespace Game.Modules.Analytics
{
    using System;
    using Runtime.Services.Analytics.Messages;
    using Runtime.Services.Analytics.Runtime;
    using UniModules.UniCore.Runtime.Utils;

    [Serializable]
    public class PaymentEventMessage : AnalyticsEventMessage
    {
        private int _price;
        
        public PaymentEventMessage(string eventName) 
            : base(eventName, AnalyticsEventsNames.shop_group)
        {
            
        }

        public int Price
        {
            set
            {
                this[AnalyticsEventsNames.price] = value.ToStringFromCache();
                _price = value;
            }
            get => _price;
        }
        
        public string EntryPoint
        {
            set => this[AnalyticsEventsNames.entry_point] = value;
            get => this[AnalyticsEventsNames.entry_point];
        }
            
        public string Receipt
        {
            set => this[AnalyticsEventsNames.iap_receipt] = value;
            get => this[AnalyticsEventsNames.iap_receipt];
        }
        
        public string Signature
        {
            set => this[AnalyticsEventsNames.iap_signature] = value;
            get => this[AnalyticsEventsNames.iap_signature];
        }
        
        public string ItemType
        {
            set => this[AnalyticsEventsNames.iap_type] = value;
            get => this[AnalyticsEventsNames.iap_type];
        }
        
        public string ItemId
        {
            set => this[AnalyticsEventsNames.iap_id] = value;
            get => this[AnalyticsEventsNames.iap_id];
        }
        
        public string CartType
        {
            set => this[AnalyticsEventsNames.iap_cart_type] = value;
            get => this[AnalyticsEventsNames.iap_cart_type];
        }
        
        public string Currency
        {
            set => this[AnalyticsEventsNames.currency] = value;
            get => this[AnalyticsEventsNames.currency];
        }
    }


    [Serializable]
    public class PaymentAttemptingEventMessage : PaymentEventMessage
    {
        public PaymentAttemptingEventMessage() : base(AnalyticsEventsNames.payment_attempting)
        {
        }
    }
    
    [Serializable]
    public class PurchaseCompleteEventMessage : PaymentEventMessage
    {
        public PurchaseCompleteEventMessage() : base(AnalyticsEventsNames.payment_complete)
        {
        }
    }
    
    [Serializable]
    public class PurchaseErrorEventMessage : PaymentEventMessage
    {
        public PurchaseErrorEventMessage() : base(AnalyticsEventsNames.payment_error)
        {
        }
    }
    
    
}