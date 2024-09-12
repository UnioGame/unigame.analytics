namespace Game.Runtime.Services.Analytics.Messages
{
    using Runtime;

    public class LoginMessage : AnalyticsEventMessage
    {
        public int PlayerCoins
        {
            set
            {
                this[AnalyticsEventsNames.player_coins] = value.ToString();
                SetParameter(AnalyticsEventsNames.player_coins, value);
            } 
        }

        public LoginMessage(string name) : base(name, name)
        {
        }
    }
}