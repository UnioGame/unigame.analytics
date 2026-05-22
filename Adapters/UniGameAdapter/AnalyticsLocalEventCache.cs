#if UNIGAME_ANALYTICS_ENABLED

namespace UniGame.Runtime.Analytics.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public sealed class AnalyticsLocalEventCache
    {
        private const string DefaultFileName = "mtt_analytics_cache.jsonl";

        private readonly string _filePath;
        private readonly string _tempFilePath;
        private readonly int _capacity;
        private readonly List<string> _events = new();

        public AnalyticsLocalEventCache(int capacity = 100, string fileName = DefaultFileName)
        {
            _capacity = capacity > 0 ? capacity : 100;
            _filePath = Path.Combine(Application.persistentDataPath, fileName);
            _tempFilePath = _filePath + ".tmp";
        }

        public int Count => _events.Count;

        public void Load()
        {
            _events.Clear();

            try
            {
                if (!File.Exists(_filePath))
                    return;

                var lines = File.ReadAllLines(_filePath);
                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    _events.Add(line);
                }

                if (_events.Count > _capacity)
                {
                    _events.RemoveRange(0, _events.Count - _capacity);
                    Persist();
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"MTT analytics local cache load failed: {exception.Message}");
                _events.Clear();
            }
        }

        public void Append(string payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
                return;

            var normalized = NormalizeLine(payload);
            if (string.IsNullOrEmpty(normalized))
                return;

            _events.Add(normalized);

            while (_events.Count > _capacity)
                _events.RemoveAt(0);

            Persist();
        }

        public IReadOnlyList<string> Peek(int max)
        {
            if (max <= 0 || _events.Count == 0)
                return Array.Empty<string>();

            var size = Math.Min(max, _events.Count);
            var result = new string[size];
            for (var i = 0; i < size; i++)
                result[i] = _events[i];
            return result;
        }

        public void RemoveProcessed(int count)
        {
            if (count <= 0)
                return;

            var toRemove = Math.Min(count, _events.Count);
            if (toRemove <= 0)
                return;

            _events.RemoveRange(0, toRemove);
            Persist();
        }

        private void Persist()
        {
            try
            {
                using (var writer = new StreamWriter(_tempFilePath, append: false))
                {
                    for (var i = 0; i < _events.Count; i++)
                        writer.WriteLine(_events[i]);
                }

                if (File.Exists(_filePath))
                    File.Delete(_filePath);
                File.Move(_tempFilePath, _filePath);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"MTT analytics local cache persist failed: {exception.Message}");
            }
        }

        private static string NormalizeLine(string payload)
        {
            // Single-event payload must remain on one line for JSONL format.
            if (payload.IndexOf('\n') < 0 && payload.IndexOf('\r') < 0)
                return payload;

            return payload.Replace("\r", string.Empty).Replace("\n", string.Empty);
        }
    }
}

#endif
