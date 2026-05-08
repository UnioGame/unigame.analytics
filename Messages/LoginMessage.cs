namespace UniGame.Runtime.Analytics.Messages
{
    using System;
    using Runtime;

    [Serializable]
    public class LoginMessage : AnalyticsEventMessage
    {
        public LoginMessage() : base(AnalyticsEventsNames.login, AnalyticsEventsNames.session_group)
        {
            
        }
    }
}