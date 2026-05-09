#if UNIGAME_ANALYTICS_ENABLED

namespace UniGame.Runtime.Analytics.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Newtonsoft.Json;
    using Runtime;

    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GameAnalyticsEventMessage : AnalyticsEventMessage
    {
        [JsonIgnore]
        private readonly Dictionary<string, object> _properties = new();

        public GameAnalyticsEventMessage(string eventName, string groupId = AnalyticsEventsNames.feature)
            : base(eventName, groupId)
        {
        }

        [JsonProperty("event_name")]
        public string EventName
        {
            get => Name;
            set => Name = value;
        }

        [JsonProperty("event_id")]
        public string EventId
        {
            get => this["event_id"];
            set => this["event_id"] = value;
        }

        [JsonProperty("user_id")]
        public new string UserId
        {
            get => this[AnalyticsEventsNames.user_id];
            set => this[AnalyticsEventsNames.user_id] = value;
        }

        [JsonProperty("session_id")]
        public string SessionId
        {
            get => this["session_id"];
            set => this["session_id"] = value;
        }

        [JsonProperty("timestamp")]
        public long Timestamp
        {
            get => long.TryParse(this["timestamp"], out var value) ? value : 0L;
            set => this["timestamp"] = value.ToString(CultureInfo.InvariantCulture);
        }

        [JsonProperty("platform")]
        public string Platform
        {
            get => this["platform"];
            set => this["platform"] = value;
        }

        [JsonProperty("backend_type")]
        public string BackendType
        {
            get => this["backend_type"];
            set => this["backend_type"] = value;
        }

        [JsonProperty("build")]
        public string Build
        {
            get => this["build"];
            set => this["build"] = value;
        }

        [JsonProperty("app_version")]
        public string AppVersion
        {
            get => this["app_version"];
            set => this["app_version"] = value;
        }

        [JsonProperty("group_id")]
        public string GroupId
        {
            get => this[AnalyticsEventsNames.group_id];
            set => this[AnalyticsEventsNames.group_id] = value;
        }

        [JsonProperty("device_model")]
        public string DeviceModel
        {
            get => this[AnalyticsEventsNames.device_model];
            set => this[AnalyticsEventsNames.device_model] = value;
        }

        [JsonProperty("properties")]
        public Dictionary<string, object> Properties => CreateSerializedProperties();

        public void SetProperty(string key, object value)
        {
            _properties[key] = value;
        }

        public static bool IsTransportParameter(string key)
        {
            return key is
                "event_id" or
                AnalyticsEventsNames.user_id or
                "session_id" or
                "timestamp" or
                "platform" or
                "backend_type" or
                "build" or
                "app_version" or
                AnalyticsEventsNames.group_id or
                AnalyticsEventsNames.device_model or
                AnalyticsEventsNames.event_name;
        }

        public void SetProperties(IReadOnlyDictionary<string, object> properties)
        {
            foreach (var property in properties)
                _properties[property.Key] = property.Value;
        }

        private Dictionary<string, object> CreateSerializedProperties()
        {
            var properties = new Dictionary<string, object>(_properties);

            foreach (var parameter in Parameters)
            {
                if (IsTransportParameter(parameter.Key))
                    continue;

                if (properties.ContainsKey(parameter.Key))
                    continue;

                properties[parameter.Key] = ToPropertyValue(parameter.Value);
            }

            return properties;
        }

        private static object ToPropertyValue(string value)
        {
            if (long.TryParse(value, out var longValue))
                return longValue;

            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
                return doubleValue;

            if (bool.TryParse(value, out var boolValue))
                return boolValue;

            return value ?? string.Empty;
        }
    }
}
#endif
