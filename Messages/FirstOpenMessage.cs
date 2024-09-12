namespace Game.Runtime.Services.Analytics.Messages
{
    using Runtime;

    public class FirstOpenMessage : AnalyticsEventMessage
    {
        public int PlayerCoins
        {
            set
            {
                this[AnalyticsEventsNames.player_coins] = value.ToString();
                SetParameter(AnalyticsEventsNames.player_coins, value);
            } 
        }

        public FirstOpenMessage(string name) : base(name, name)
        {
        }
    }
}