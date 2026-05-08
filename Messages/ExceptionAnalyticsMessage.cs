namespace UniGame.Runtime.Analytics.Messages
{
    using System;
    using Runtime;

    [Serializable]
    public class ExceptionAnalyticsMessage : AnalyticsEventMessage
    {
        public ExceptionAnalyticsMessage() 
            : base(AnalyticsEventsNames.exception, AnalyticsEventsNames.debug)
        {
        }
        
        public string Exception
        {
            set
            {
                this[AnalyticsEventsNames.exception] = value;
            } 
        }
        
    }
}