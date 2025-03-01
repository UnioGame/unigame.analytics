namespace Game.Modules.Analytics
{
    using System;
    using Runtime.Services.Analytics.Messages;

    [Serializable]
    public class GAProgressionEventMessage : AnalyticsEventMessage
    {
        private AnalyticsIntValue _score;
        
        public GAProgressionEventMessage() : base(GameAnalyticsConstants.ga_progression, GameAnalyticsConstants.ga_group)
        {
            _score = new AnalyticsIntValue(this, GameAnalyticsConstants.score);
            Progression01 = string.Empty;
            Progression02 = string.Empty;
            Progression03 = string.Empty;
        }
        
        public string ProgressionStatus
        {
            set => this[GameAnalyticsConstants.ga_progression_status] = value;
            get => this[GameAnalyticsConstants.ga_progression_status];
        }
        
        public string Progression01
        {
            set => this[GameAnalyticsConstants.ga_progression01] = value;
            get => this[GameAnalyticsConstants.ga_progression01];
        }
        
        public string Progression02
        {
            set => this[GameAnalyticsConstants.ga_progression02] = value;
            get => this[GameAnalyticsConstants.ga_progression02];
        }
        
        public string Progression03
        {
            set => this[GameAnalyticsConstants.ga_progression03] = value;
            get => this[GameAnalyticsConstants.ga_progression03];
        }
        
        public int Score
        {
            set => _score.Value = value;
            get => _score.Value;
        }
        
    }
}