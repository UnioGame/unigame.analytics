namespace Game.Runtime.Services.Analytics.Runtime
{
    using Sirenix.OdinInspector;
    using UnityEngine;

#if UNITY_EDITOR
    using UniModules.Editor;
#endif
    
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

        [Button]
        public void Save()
        {
#if UNITY_EDITOR
            this.SaveAsset();
#endif
        }
    }
}