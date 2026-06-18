#if UNIGAME_ANALYTICS_ENABLED

namespace UniGame.Runtime.Analytics.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using Cysharp.Threading.Tasks;
    using DataFlow;
    using Interfaces;
    using Messages;
    using Newtonsoft.Json;
    using Runtime;
    using UnityEngine;
    using UnityEngine.Networking;

    [Serializable]
    public sealed class UniGameAnalyticsAdapter : IAnalyticsAdapter
    {
        private const int DrainBatchSize = 10;
        private const int DefaultBatchDebounceMilliseconds = 5000;
        private const int DefaultMaxBatchSize = 100;

        public string endpoint = "http://localhost:8080/v1/events";
        public string batchEndpoint = "http://localhost:8080/v1/events/batch";
        public int retryDelayMilliseconds = 1000;
        public int retryCount = 5;
        public int batchDebounceMilliseconds = DefaultBatchDebounceMilliseconds;
        public int maxBatchSize = DefaultMaxBatchSize;
        public bool gzipEnabled = true;
        public Dictionary<string, string> requestHeaders = new();

        private bool _workerStarted;
        private bool _draining;
        private readonly Queue<IAnalyticsMessage> _queue = new();
        private LifeTime _lifetime = new();
        private readonly AnalyticsLocalEventCache _localCache = new();

        public UniTask InitializeAsync()
        {
            _localCache.Load();
            EnsureWorker();
            if (_localCache.Count > 0)
                DrainCacheAsync().Forget();
            return UniTask.CompletedTask;
        }

        public void TrackEvent(IAnalyticsMessage message)
        {
            _queue.Enqueue(message);
            EnsureWorker();
        }

        private void EnsureWorker()
        {
            if (_workerStarted)
                return;

            _workerStarted = true;
            SendWorker().Forget();
        }

        public void Dispose()
        {
            var batch = DequeueAvailableEvents(ResolveMaxBatchSize());
            if (batch.Count > 0)
                SendBatchWithRetry(batch).Forget();

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
                    var batch = DequeueAvailableEvents(ResolveMaxBatchSize());
                    await CollectDebouncedBatch(batch);
                    await SendBatchWithRetry(batch);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"MTT analytics send worker error: {ex}");
                    continue;
                }
            }
        }

        private async UniTask CollectDebouncedBatch(List<string> batch)
        {
            var maxSize = ResolveMaxBatchSize();
            var debounceMilliseconds = ResolveBatchDebounceMilliseconds();
            var elapsedMilliseconds = 0;

            while (!_lifetime.IsTerminated && batch.Count < maxSize && elapsedMilliseconds < debounceMilliseconds)
            {
                if (_queue.Count > 0)
                {
                    DrainQueuedMessages(batch, maxSize);
                    continue;
                }

                var delay = Math.Min(50, debounceMilliseconds - elapsedMilliseconds);
                if (delay <= 0)
                    break;

                await UniTask.Delay(delay);
                elapsedMilliseconds += delay;
            }
        }

        private async UniTask<bool> SendBatchWithRetry(IReadOnlyList<string> batch)
        {
            if (batch == null || batch.Count == 0)
                return true;
            if (string.IsNullOrWhiteSpace(batchEndpoint))
            {
                CacheBatch(batch);
                return false;
            }

            var payload = BuildBatchPayload(batch);
            var attempts = 0;
            while (!_lifetime.IsTerminated)
            {
                var status = await PostAsync(batchEndpoint, payload);
                if (status)
                {
                    if (_localCache.Count > 0)
                        DrainCacheAsync().Forget();
                    return true;
                }

                attempts++;
                if (retryCount > 0 && attempts > retryCount)
                {
                    CacheBatch(batch);
                    Debug.LogWarning(
                        $"MTT analytics batch dropped after {attempts} attempts, cached locally: {batch.Count} events");
                    return false;
                }

                if (_lifetime.IsTerminated)
                    break;

                try
                {
                    await UniTask.Delay(retryDelayMilliseconds, cancellationToken: _lifetime.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            CacheBatch(batch);
            return false;
        }

        private List<string> DequeueAvailableEvents(int maxSize)
        {
            var batch = new List<string>(Math.Min(maxSize, Math.Max(1, _queue.Count)));
            DrainQueuedMessages(batch, maxSize);
            return batch;
        }

        private void DrainQueuedMessages(List<string> batch, int maxSize)
        {
            while (_queue.Count > 0 && batch.Count < maxSize)
            {
                var message = _queue.Dequeue();
                var transportMessage = CreateTransportMessage(message);
                var payload = JsonConvert.SerializeObject(transportMessage);
                batch.Add(payload);
            }
        }

        private void CacheBatch(IReadOnlyList<string> batch)
        {
            for (var i = 0; i < batch.Count; i++)
                _localCache.Append(batch[i]);
        }

        private async UniTask<bool> PostAsync(string url, string payload)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            var body = BuildRequestBody(payload, out var compressed);

            using var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(body),
                downloadHandler = new DownloadHandlerBuffer()
            };

            request.SetRequestHeader("Content-Type", "application/json");
            if (compressed)
                request.SetRequestHeader("Content-Encoding", "gzip");
            ApplyRequestHeaders(request);

            try
            {
                await request.SendWebRequest().ToUniTask();
            }
            catch (Exception e)
            {
#if GAME_DEBUG
                Debug.LogWarning($"MTT analytics event send failed: {e.Message} {request.responseCode} {request.error}");
#endif
                return false;
            }

            if (request.result == UnityWebRequest.Result.Success)
                return true;

#if GAME_DEBUG
            Debug.LogWarning($"MTT analytics event send failed: {request.responseCode} {request.error}");
#endif

            return false;
        }

        private byte[] BuildRequestBody(string payload, out bool compressed)
        {
            var body = Encoding.UTF8.GetBytes(payload);
            if (!gzipEnabled)
            {
                compressed = false;
                return body;
            }

            compressed = true;
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionLevel.Fastest, leaveOpen: true))
                gzip.Write(body, 0, body.Length);

            return output.ToArray();
        }

        private int ResolveBatchDebounceMilliseconds()
        {
            return batchDebounceMilliseconds > 0
                ? batchDebounceMilliseconds
                : DefaultBatchDebounceMilliseconds;
        }

        private int ResolveMaxBatchSize()
        {
            return maxBatchSize > 0
                ? maxBatchSize
                : DefaultMaxBatchSize;
        }

        private async UniTask DrainCacheAsync()
        {
            if (_draining)
                return;
            if (_localCache.Count == 0)
                return;
            if (string.IsNullOrWhiteSpace(batchEndpoint))
                return;

            _draining = true;
            try
            {
                while (!_lifetime.IsTerminated && _localCache.Count > 0)
                {
                    var batch = _localCache.Peek(DrainBatchSize);
                    if (batch.Count == 0)
                        break;

                    var payload = BuildBatchPayload(batch);
                    var ok = await PostAsync(batchEndpoint, payload);
                    if (!ok)
                        break;

                    _localCache.RemoveProcessed(batch.Count);
                }
            }
            finally
            {
                _draining = false;
            }
        }

        private static string BuildBatchPayload(IReadOnlyList<string> events)
        {
            var builder = new StringBuilder(64 + events.Count * 64);
            builder.Append("{\"events\":[");
            for (var i = 0; i < events.Count; i++)
            {
                if (i > 0)
                    builder.Append(',');
                builder.Append(events[i]);
            }
            builder.Append("]}");
            return builder.ToString();
        }

        private void ApplyRequestHeaders(UnityWebRequest request)
        {
            foreach (var header in requestHeaders)
            {
                if (string.IsNullOrWhiteSpace(header.Key) || string.IsNullOrWhiteSpace(header.Value))
                    continue;

                request.SetRequestHeader(header.Key, header.Value);
            }
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
                AppId = GetValue(message, "app_id", Application.identifier),
                DeviceId = GetValue(message, "device_id", SystemInfo.deviceUniqueIdentifier),
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

            if (string.IsNullOrWhiteSpace(message.AppId))
                message.AppId = Application.identifier;

            if (string.IsNullOrWhiteSpace(message.DeviceId))
                message.DeviceId = SystemInfo.deviceUniqueIdentifier;

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
