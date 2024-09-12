namespace Game.Runtime.Services.Analytics.Runtime
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Services/Analytics/Analytics Configuration")]
    public class AnalyticsConfigurationAsset : ScriptableObject
    {
        [HideLabel] 
        [InlineProperty] 
        public AnalyticsConfiguration configuration = new();

        public bool IsEnabled
        {
            get => configuration.isEnabled;
            set => configuration.isEnabled = value;
        }
    }
}