namespace Game.Runtime.Services.Analytics.Messages
{
    using Runtime;
    
    public class SessionStartMessage : AnalyticsEventMessage
    {
        public int PlayerCoins
        {
            set => this[AnalyticsEventsNames.player_coins] = value.ToString();
        }

        public SessionStartMessage(string name) : base(name, name)
        {
        }
    }
}