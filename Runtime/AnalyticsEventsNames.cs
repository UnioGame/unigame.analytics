namespace Game.Runtime.Services.Analytics.Runtime
{
    public class AnalyticsEventsNames
    {
        /// <summary>
        /// Groups
        /// </summary>
        public const string debug = nameof(debug);
        public const string general = nameof(general);
        public const string ui_group = nameof(ui_group);
        
        /// <summary>
        /// events
        /// </summary>
        public const string exception = nameof(exception);
        
        /// <summary>
        /// General
        /// </summary>
        public const string event_name = nameof(event_name);
        public const string group_id = nameof(group_id);
        public const string device_model = nameof(device_model);
        public const string feature_use = nameof(feature_use);
        public const string other = nameof(other);
        
        /// <summary>
        /// Game State
        /// </summary>
        public const string first_open = nameof(first_open);
        public const string daily_reward_shown = nameof(daily_reward_shown);
        public const string daily_reward_gain = nameof(daily_reward_shown);
        public const string login = nameof(login);
        public const string load_bootstrap = nameof(load_bootstrap);
        public const string session_start = nameof(session_start);
        public const string scene_idle_start = nameof(scene_idle_start);
        
        /// <summary>
        /// Feature
        /// </summary>
        public const string feature_use_idle_start_button = nameof(feature_use_idle_start_button);
        public const string feature_use_idle_start_level = nameof(feature_use_idle_start_level);
        public const string feature_use_idle_moneybox = nameof(feature_use_idle_moneybox);
        
        /// <summary>
        /// Game params
        /// </summary>
        public const string gold_collected = nameof(gold_collected);
        public const string gold_rewarded = nameof(gold_rewarded);
        public const string level_number = nameof(level_number);
        
        /// <summary>
        /// ui data
        /// </summary>
        public const string ui_element = nameof(ui_element);
        
        /// <summary>
        /// Player info
        /// </summary>
        public const string player_coins = nameof(player_coins);
        public const string action_complete_idle_claim_reward_level = nameof(action_complete_idle_claim_reward_level);
        
        /// <summary>
        /// Debug
        /// </summary>
        public const string other_idle_boss_pass = nameof(other_idle_boss_pass);
        public const string other_idle_debug_stats = nameof(other_idle_debug_stats);
        public const string fps = nameof(fps);
    }
}