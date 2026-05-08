#if UNIGAME_ANALYTICS_ENABLED

namespace UniGame.Runtime.Analytics.Adapters
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using DataFlow;
    using Interfaces;
    using Newtonsoft.Json.Linq;
    using Runtime;
    using UnityEngine;
    using UnityEngine.Networking;

    [Serializable]
    public sealed class UniGameAnalyticsAdapter : IAnalyticsAdapter
    {
        public string endpoint = "http://localhost:8080/v1/events";
        public int retryDelayMilliseconds = 1000;

        private bool _isSending;
        private readonly Queue<IAnalyticsMessage> _queue = new();
        private LifeTime  _lifetime = new();

        public UniTask InitializeAsync()
        {
            return UniTask.CompletedTask;
        }

        public void TrackEvent(IAnalyticsMessage message)
        {
            _queue.Enqueue(message);
            var task = FlushAsync();
            task.Forget();
        }

        public void Dispose()
        {
            _lifetime.Terminate();
            _queue.Clear();
        }

        private async UniTask FlushAsync()
        {
            if (_isSending) return;

            _isSending = true;
            
            try
            {
                while (_queue.Count > 0 && !_lifetime.IsTerminated)
                {
                    var message = _queue.Dequeue();
                    SendMessage(message).Forget();
                }
            }
            finally
            {
                _isSending = false;
            }
        }

        private async UniTask<bool> SendMessage(IAnalyticsMessage message)
        {
            while (!_lifetime.IsTerminated)
            {
                var status = await SendAsync(message);
                if (status) return true;
                await UniTask.Delay(retryDelayMilliseconds);
            }
            
            return false;
        }

        private async UniTask<bool> SendAsync(IAnalyticsMessage message)
        {
            var body = System.Text.Encoding.UTF8.GetBytes(CreatePayload(message).ToString());

            using var request = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(body),
                downloadHandler = new DownloadHandlerBuffer()
            };

            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                return true;

            Debug.LogWarning($"MTT analytics event send failed: {request.responseCode} {request.error}");
            return false;
        }

        private static JObject CreatePayload(IAnalyticsMessage message)
        {
            var properties = new JObject();
            foreach (var parameter in message.Parameters)
            {
                if (IsCommonParameter(parameter.Key))
                    continue;

                properties[parameter.Key] = ToJsonToken(parameter.Value);
            }

            return new JObject
            {
                ["event_name"] = message.Name,
                ["event_id"] = GetValue(message, "event_id", Guid.NewGuid().ToString("D")),
                ["user_id"] = GetValue(message, AnalyticsEventsNames.user_id, "unknown"),
                ["session_id"] = GetValue(message, "session_id", "unknown"),
                ["timestamp"] = long.TryParse(GetValue(message, "timestamp", "0"), out var timestamp) ? timestamp : DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                ["platform"] = GetValue(message, "platform", Application.platform.ToString().ToLowerInvariant()),
                ["app_version"] = GetValue(message, "app_version", Application.version),
                ["properties"] = properties
            };
        }

        private static bool IsCommonParameter(string key)
        {
            return key is "event_id" or AnalyticsEventsNames.user_id or "session_id" or "timestamp" or "platform" or "app_version" or AnalyticsEventsNames.event_name;
        }

        private static string GetValue(IAnalyticsMessage message, string key, string fallback)
        {
            var value = message[key];
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        private static JToken ToJsonToken(string value)
        {
            if (long.TryParse(value, out var longValue))
                return longValue;

            if (double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var doubleValue))
                return doubleValue;

            if (bool.TryParse(value, out var boolValue))
                return boolValue;

            return value ?? string.Empty;
        }

    }
}
#endif
