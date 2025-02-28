namespace Game.Runtime.Services.Analytics.Messages
{
    public class SessionStartMessage : AnalyticsEventMessage
    {
        public SessionStartMessage(string name) : base(name, name)
        {
        }
    }
}