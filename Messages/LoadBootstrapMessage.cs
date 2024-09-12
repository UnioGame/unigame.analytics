namespace Game.Runtime.Services.Analytics.Messages
{
    using Runtime;

    public class LoadBootstrapMessage : AnalyticsEventMessage
    {
        public int PlayerCoins
        {
            set
            {
                this[AnalyticsEventsNames.player_coins] = value.ToString();
                SetParameter(AnalyticsEventsNames.player_coins, value);
            } 
        }

        public LoadBootstrapMessage(string name) : base(name, name)
        {
        }
    }
}