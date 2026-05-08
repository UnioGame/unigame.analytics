namespace UniGame.Runtime.Analytics.Runtime
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [Serializable]
    public class AnalyticsConfiguration
    {
        public bool isEnabled = true;
        public string platformIdOverride = string.Empty;
        public List<string> enabledPlatforms = new();
        public List<string> disabledPlatforms = new();
        
        [Header("Analytics Adapters")]
        [ListDrawerSettings(ListElementLabelName = "@name")]
        [SerializeReference]
        public List<AnalyticsAdapterData> analytics = new();
        
        [Header("Analytics Handlers")]
        [SerializeReference]
        public List<IAnalyticsMessageHandler> messageHandlers = new();

        public bool IsPlatformAllowed(string platformId)
        {
            return AnalyticsPlatformPolicy.IsPlatformAllowed(platformId, enabledPlatforms, disabledPlatforms);
        }
    }

    public static class AnalyticsPlatformPolicy
    {
        public static bool IsPlatformAllowed(
            string platformId,
            IReadOnlyList<string> enabledPlatforms,
            IReadOnlyList<string> disabledPlatforms)
        {
            if (Contains(disabledPlatforms, platformId))
                return false;

            return enabledPlatforms == null ||
                   enabledPlatforms.Count == 0 ||
                   Contains(enabledPlatforms, platformId);
        }

        private static bool Contains(IReadOnlyList<string> platforms, string platformId)
        {
            if (platforms == null || string.IsNullOrWhiteSpace(platformId))
                return false;

            for (var i = 0; i < platforms.Count; i++)
            {
                if (string.Equals(platforms[i], platformId, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}