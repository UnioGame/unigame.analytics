namespace Game.Runtime.Services.Analytics.Messages
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using Runtime;
    using Newtonsoft.Json;
    using UniModules.UniCore.Runtime.Utils;
    using Unity.Services.Analytics;
    using UnityEngine.Device;

    [Serializable]
    public class AnalyticsEventMessage : IAnalyticsMessage
    {
        public Dictionary<string, string> parameters = new();

        public Dictionary<string, string> Parameters => parameters;
        
        public AnalyticsEventMessage(string name, string groupId) 
        {
            Name = name;
            GroupId = groupId;
            DeviceModel = SystemInfo.deviceModel;
        }
        
        public string Name
        {
            get => this[AnalyticsEventsNames.event_name];
            protected set => this[AnalyticsEventsNames.event_name] = value;
        }

        public string GroupId
        {
            get => parameters.TryGetValue(AnalyticsEventsNames.group_id, out var parameter)
                ? parameter
                : string.Empty;
            
            set => this[AnalyticsEventsNames.group_id] = value;
        }
        
        public string DeviceModel
        {
            set => this[AnalyticsEventsNames.device_model] = value;
        }

        public string this[string key] 
        {
            get => parameters.TryGetValue(key, out var value) ? value : string.Empty;
            set => parameters[key] = value;
        }

        public AnalyticsEventMessage Add(string key, string value)
        {
            parameters[key] = value;
            return this;
        }
        
        public override string ToString()
        {
            return $"ANALYTICS EVENT: {JsonConvert.SerializeObject(this)}";
        }
    }
}
