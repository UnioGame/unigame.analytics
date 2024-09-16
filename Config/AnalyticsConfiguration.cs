namespace Game.Runtime.Services.Analytics.Runtime
{
    using System;
    using System.Collections.Generic;
    using Config;
    using Interfaces;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class AnalyticsConfiguration
    {
        public bool isEnabled = true;
        
        [InlineProperty]
        public List<AssetReferenceT<AnalyticsAdapter>> analytics = new();
        
        [SerializeReference]
        public List<IAnalyticsMessageHandler> messageHandlers = new();
        
        [TitleGroup("Analytics Model")]
        [InlineProperty]
        [HideLabel]
        public AnalyticsModel analyticsModel = new();
    }
}