namespace Game.Runtime.Services.Analytics.Messages
{
    public class FirstOpenMessage : AnalyticsEventMessage
    {
        public FirstOpenMessage(string name) : base(name, name)
        {
        }
    }
}