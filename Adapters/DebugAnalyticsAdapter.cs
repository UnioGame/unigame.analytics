namespace UniGame.Runtime.Analytics.Adapters
{
    using System;
    using Cysharp.Threading.Tasks;
    using Interfaces;
#if GAME_DEBUG
    using System.Text;
    using Newtonsoft.Json;
    using UniCore.Runtime.ProfilerTools;
    using UnityEngine;
#if UNIGAME_ANALYTICS_ENABLED
    using Messages;
#endif
#endif

    [Serializable]
    public class DebugAnalyticsAdapter : IAnalyticsAdapter
    {
#if GAME_DEBUG
        private static readonly StringBuilder _builder = new(256);
#endif

        public void Dispose()
        {
        }

        public UniTask InitializeAsync()
        {
            return UniTask.CompletedTask;
        }

        public void TrackEvent(IAnalyticsMessage message)
        {
#if GAME_DEBUG
            var builder = _builder;
            builder.Clear();
            builder.Append("[Analytics] ").Append(message.Name).Append(" | ");

            var first = true;
            foreach (var pair in message.Parameters)
            {
                if (!first)
                    builder.Append(", ");
                builder.Append(pair.Key).Append('=').Append(pair.Value);
                first = false;
            }

#if UNIGAME_ANALYTICS_ENABLED
            if (message is GameAnalyticsEventMessage transportMessage)
            {
                foreach (var pair in transportMessage.Properties)
                {
                    if (!first)
                        builder.Append(", ");
                    builder.Append(pair.Key).Append('=').Append(FormatValue(pair.Value));
                    first = false;
                }
            }
#endif

            GameLog.Log(builder.ToString(), Color.chocolate);
#endif
        }

#if GAME_DEBUG && UNIGAME_ANALYTICS_ENABLED
        private static string FormatValue(object value)
        {
            return value is string stringValue
                ? stringValue
                : JsonConvert.SerializeObject(value);
        }
#endif
    }
}
