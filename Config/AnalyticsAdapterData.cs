namespace Game.Runtime.Services.Analytics.Runtime
{
    using System;
    using Interfaces;
    using UnityEngine;

    [Serializable]
    public class AnalyticsAdapterData
    {
        public string name;
        public bool isEnabled = true;
        [SerializeReference]
        public IAnalyticsAdapter adapter;
    }
}