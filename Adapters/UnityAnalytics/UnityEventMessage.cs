namespace Game.Runtime.Services.Analytics.Adapters.UnityAnalytics
{
    using Event = Unity.Services.Analytics.Event;

    public class UnityEventMessage : Event
    {
        public string this[string key]
        {
            set => SetParameter(key, value);
        }
        
        public UnityEventMessage(string name) : base(name)
        {
        }
    }
}