namespace UniGame.Runtime.Analytics.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using global::UniGame.Runtime.Analytics.Interfaces;
    using global::UniGame.Runtime.Analytics.Messages;

    public static class GameAnalyticsMessageFactory
    {
        public static IAnalyticsMessage Build(
            string eventName,
            IReadOnlyDictionary<string, object> properties = null,
            string groupId = AnalyticsEventsNames.feature)
        {
#if UNIGAME_ANALYTICS_ENABLED
            var message = new GameAnalyticsEventMessage(eventName, groupId);
            if (properties != null)
            {
                foreach (var property in properties)
                    message.SetProperty(property.Key, property.Value);
            }
            return message;
#else
            var message = new AnalyticsEventMessage(eventName, groupId);
            if (properties != null)
            {
                foreach (var property in properties)
                    message[property.Key] = FormatValue(property.Value);
            }
            return message;
#endif
        }

        public static string FormatValue(object value)
        {
            return value switch
            {
                null => string.Empty,
                bool boolValue => boolValue ? "true" : "false",
                float floatValue => floatValue.ToString(CultureInfo.InvariantCulture),
                double doubleValue => doubleValue.ToString(CultureInfo.InvariantCulture),
                decimal decimalValue => decimalValue.ToString(CultureInfo.InvariantCulture),
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString()
            };
        }
    }
}
