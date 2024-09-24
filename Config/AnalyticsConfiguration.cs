namespace Game.Runtime.Services.Analytics.Runtime
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
        
        [Header("Analytics Adapters")]
        [ListDrawerSettings(ListElementLabelName = "@name")]
        [SerializeReference]
        public List<AnalyticsAdapterData> analytics = new();
        
        [Header("Analytics Handlers")]
        [SerializeReference]
        public List<IAnalyticsMessageHandler> messageHandlers = new();
        
    }
}