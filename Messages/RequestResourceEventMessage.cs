namespace Game.Runtime.Services.Analytics
{
    using System;
    using Messages;
    using Runtime;
    using UniModules.UniCore.Runtime.Utils;

    [Serializable]
    public class RequestResourceEventMessage : GameResourceEventMessage
    {
        public RequestResourceEventMessage() 
            : base( AnalyticsEventsNames.game_resource_request) { }
        
    }
    
    [Serializable]
    public class GameResourceChangeEventMessage : GameResourceEventMessage
    {
        public GameResourceChangeEventMessage() 
            : base( AnalyticsEventsNames.game_resource_changed) { }
    }

    
    [Serializable]
    public class GameResourceFlowEventMessage : GameResourceEventMessage
    {
        public GameResourceFlowEventMessage() 
            : base( AnalyticsEventsNames.game_resource_flow) { }
        
        public string FlowType
        {
            set => this[AnalyticsEventsNames.resource_flow_type] = value;
            get => this[AnalyticsEventsNames.resource_flow_type];
        }
        
        public string ItemType
        {
            set => this[AnalyticsEventsNames.resource_receive] = value;
            get => this[AnalyticsEventsNames.resource_receive];
        }
        
        public string ItemId
        {
            set => this[AnalyticsEventsNames.resource_receive] = value;
            get => this[AnalyticsEventsNames.resource_receive];
        }

    }
    
    [Serializable]
    public class GameResourceEventMessage : AnalyticsEventMessage
    {
        private int _resourceValue;
        
        public GameResourceEventMessage(string eventName) 
            : base( eventName, AnalyticsEventsNames.game_resource_group) 
        {
            ResourceCurrency = string.Empty;
            ResourceSource = string.Empty;
        }

        public string ResourceCurrency
        {
            set => this[AnalyticsEventsNames.resource_currency] = value;
            get => this[AnalyticsEventsNames.resource_currency];
        }
        
        public string ResourceType
        {
            set => this[AnalyticsEventsNames.resource] = value;
            get => this[AnalyticsEventsNames.resource];
        }
        
        public int ResourceValue
        {
            set
            {
                _resourceValue = value;
                this[AnalyticsEventsNames.resource_value] = _resourceValue.ToStringFromCache();
            }
            get => _resourceValue;
        }
        
        public string ResourceSource
        {
            set => this[AnalyticsEventsNames.resource_source] = value;
            get => this[AnalyticsEventsNames.resource_source];
        }

    }
}
