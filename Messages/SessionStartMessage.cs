namespace UniGame.Runtime.Analytics.Messages
{
    public class SessionStartMessage : AnalyticsEventMessage
    {
        public SessionStartMessage(string name) : base(name, name)
        {
        }
    }
}