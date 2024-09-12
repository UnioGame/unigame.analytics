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
            } 
        }

        public LoginMessage(string name) : base(name, name)
        {
        }
    }
}