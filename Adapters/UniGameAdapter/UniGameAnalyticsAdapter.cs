#if UNIGAME_ANALYTICS_ENABLED

namespace UniGame.Runtime.Analytics.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Cysharp.Threading.Tasks;
    using DataFlow;
    using Interfaces;
    using Messages;
    using Newtonsoft.Json;
    using R3;
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
            SendWorker().Forget();
            return UniTask.CompletedTask;
        }

        public void TrackEvent(IAnalyticsMessage message)
        {
            _queue.Enqueue(message);
            var task = SendWorker();
            
            task.Forget();
        }

        public void Dispose()
        {
            _lifetime.Terminate();
            _queue.Clear();
        }

        private async UniTask SendWorker()
        {
            while (!_lifetime.IsTerminated)
            {
                if (_queue.Count == 0)
                {
                    await UniTask.Yield(_lifetime.Token);
                    continue;
                }
                try
                {
                    var message = _queue.Dequeue();
                    SendMessage(message).Forget();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"MTT analytics send worker error: {ex}");
                    continue;
                }
            }
        }

        private async UniTask<bool> SendMessage(IAnalyticsMessage message)
        {
            var status = await SendAsync(message);
            if (status) return true;
            
            await UniTask.Delay(retryDelayMilliseconds);
            
            //return message back to queue
            _queue.Enqueue(message);
            
            return false;
        }

        private async UniTask<bool> SendAsync(IAnalyticsMessage message)
        {
            var transportMessage = CreateTransportMessage(message);
            var payload = JsonConvert.SerializeObject(transportMessage);
            var body = Encoding.UTF8.GetBytes(payload);

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

        private static GameAnalyticsEventMessage CreateTransportMessage(IAnalyticsMessage message)
        {
            if (message is GameAnalyticsEventMessage transportMessage)
            {
                ApplyFallbacks(transportMessage);
                return transportMessage;
            }

            var transport = new GameAnalyticsEventMessage(message.Name, message.GroupId)
            {
                EventId = GetValue(message, "event_id", Guid.NewGuid().ToString("D")),
                GroupId = GetValue(message, AnalyticsEventsNames.group_id, AnalyticsEventsNames.feature),
                UserId = GetValue(message, AnalyticsEventsNames.user_id, "unknown"),
                SessionId = GetValue(message, "session_id", "unknown"),
                Timestamp = ResolveTimestamp(GetValue(message, "timestamp", string.Empty)),
                Platform = GetValue(message, "platform", "unknown"),
                BackendType = GetValue(message, "backend_type", "unknown"),
                Build = GetValue(message, "build", "unknown"),
                AppVersion = GetValue(message, "app_version", Application.version),
                DeviceModel = GetValue(message, AnalyticsEventsNames.device_model, SystemInfo.deviceModel)
            };

            foreach (var parameter in message.Parameters)
            {
                if (IsCommonParameter(parameter.Key))
                    continue;

                transport.SetProperty(parameter.Key, ToJsonValue(parameter.Value));
            }

            ApplyFallbacks(transport);
            return transport;
        }

        private static bool IsCommonParameter(string key)
        {
            return GameAnalyticsEventMessage.IsTransportParameter(key);
        }

        private static string GetValue(IAnalyticsMessage message, string key, string fallback)
        {
            var value = message[key];
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        private static long ResolveTimestamp(string value)
        {
            return long.TryParse(value, out var timestamp)
                ? timestamp
                : DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        private static void ApplyFallbacks(GameAnalyticsEventMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.EventName))
                message.EventName = AnalyticsEventsNames.other;

            if (string.IsNullOrWhiteSpace(message.EventId))
                message.EventId = Guid.NewGuid().ToString("D");

            if (string.IsNullOrWhiteSpace(message.UserId))
                message.UserId = "unknown";

            if (string.IsNullOrWhiteSpace(message.SessionId))
                message.SessionId = "unknown";

            if (string.IsNullOrWhiteSpace(message.GroupId))
                message.GroupId = AnalyticsEventsNames.feature;

            if (message.Timestamp <= 0)
                message.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (string.IsNullOrWhiteSpace(message.Platform))
                message.Platform = "unknown";

            if (string.IsNullOrWhiteSpace(message.BackendType))
                message.BackendType = "unknown";

            if (string.IsNullOrWhiteSpace(message.Build))
                message.Build = "unknown";

            if (string.IsNullOrWhiteSpace(message.AppVersion))
                message.AppVersion = Application.version;

            if (string.IsNullOrWhiteSpace(message.DeviceModel))
                message.DeviceModel = SystemInfo.deviceModel;
        }

        private static object ToJsonValue(string value)
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
