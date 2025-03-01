namespace Game.Runtime.Services.Analytics
{
    using Messages;
    using System;
    using Runtime;

    [Serializable]
    public class ExceptionEventMessage : AnalyticsEventMessage
    {
        public ExceptionEventMessage()
            : base(AnalyticsEventsNames.exception, AnalyticsEventsNames.exception_group)
        {

        }

        public string Exception
        {
            set => this[AnalyticsEventsNames.exception_message] = value;
        }
    }
}
