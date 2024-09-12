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
    public class AnalyticsEventMessage : Event, IAnalyticsMessage
    {
        public Dictionary<string, string> Parameters { get; } = new(8);
        
        public string Name
        {
            get => this[AnalyticsEventsNames.event_name];
            protected set => this[AnalyticsEventsNames.event_name] = value;
        }

        public string GroupId
        {
            get => Parameters.TryGetValue(AnalyticsEventsNames.group_id, out var parameter)
                ? parameter
                : string.Empty;
            
            set => Parameters[AnalyticsEventsNames.group_id] = value;
        }
        
        public string DeviceModel
        {
            set
            {
                Parameters[AnalyticsEventsNames.device_model] = value;
                SetParameter(AnalyticsEventsNames.device_model, value);
            } 
        }

        public AnalyticsEventMessage(string name, string groupId) : base(name)
        {
            Name = name;
            GroupId = groupId;
            DeviceModel = SystemInfo.deviceModel;
        }

        public string this[string key] 
        {
            get => Parameters.TryGetValue(key, out var value) ? value : string.Empty;
            set => Parameters[key] = value;
        }

        public AnalyticsEventMessage Add(string key, string value)
        {
            Parameters[key] = value;
            return this;
        }
        
        public override string ToString()
        {
            return $"ANALYTICS EVENT: {JsonConvert.SerializeObject(this)}";
        }
    }
}
