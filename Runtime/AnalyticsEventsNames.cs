namespace Game.Runtime.Services.Analytics.Runtime
{
    public class AnalyticsEventsNames
    {
        /// <summary>
        /// Groups
        /// </summary>
        public const string debug = nameof(debug);
        public const string general = nameof(general);
        public const string game_resource_group = nameof(game_resource_group);
        public const string ui_group = nameof(ui_group);
        public const string load_group = nameof(load_group);
        public const string session_group = nameof(session_group);
        public const string shop_group = nameof(shop_group);
        public const string exception_group = nameof(exception_group);
        public const string daily_group = nameof(daily_group);

        //===================Events===================
        
        /// <summary>
        /// events
        /// </summary>
        public const string exception = nameof(exception);
        
        //monetization events
        public const string payment_attempting = nameof(payment_attempting);
        public const string payment_start = nameof(payment_start);
        public const string payment_complete = nameof(payment_complete);
        public const string payment_error = nameof(payment_error);
        
        
        //game resources
        public const string game_resource_request = nameof(game_resource_request);
        public const string game_resource_changed = nameof(game_resource_changed);
        public const string game_resource_flow = nameof(game_resource_flow);
        
        /// <summary>
        /// Game Session
        /// </summary>
        public const string first_open = nameof(first_open);
        public const string daily_reward_shown = nameof(daily_reward_shown);
        public const string daily_reward_gain = nameof(daily_reward_shown);
        public const string login = nameof(login);
        public const string load_bootstrap = nameof(load_bootstrap);
        public const string session_start = nameof(session_start);
        public const string scene_start = nameof(scene_start);
        
        /// <summary>
        /// ui data
        /// </summary>
        public const string ui_element = nameof(ui_element);
        
        /// <summary>
        /// Debug
        /// </summary>
        public const string fps = nameof(fps);
        
        //==================Parameters==================
        
        /// common parameters
        public const string event_source = nameof(event_source);
        public const string event_name = nameof(event_name);
        public const string group_id = nameof(group_id);
        public const string device_model = nameof(device_model);
        public const string feature_use = nameof(feature_use);
        public const string other = nameof(other);
        public const string user_id = nameof(user_id);
        public const string scene_name = nameof(scene_name);

        //monetization parameters
        public const string entry_point = nameof(entry_point);
        public const string currency = nameof(currency);
        public const string price = nameof(price);
        public const string iap_type = nameof(iap_type);
        public const string iap_id = nameof(iap_id);
        public const string iap_receipt = nameof(iap_receipt);
        public const string iap_signature = nameof(iap_signature);
        public const string iap_cart_type = nameof(iap_cart_type);
        
        // some game resources
        public const string resource = nameof(resource);
        public const string resource_value = nameof(resource_value);
        public const string resource_source = nameof(resource_source);
        public const string resource_flow_type = nameof(resource_flow_type);
        public const string resource_action = nameof(resource_action);
        
        public const string resource_cost = nameof(resource_cost);
        public const string resource_id = nameof(resource_id);
        public const string resource_get = nameof(resource_get);
        public const string resource_receive = nameof(resource_receive);
        public const string resource_receive_value = nameof(resource_receive_value);
        public const string resource_spend = nameof(resource_spend);
        public const string resource_spend_value = nameof(resource_spend_value);
        
        //Undefined progression
        public const string resource_flow_undefined = nameof(resource_flow_undefined);
        // Source: Used when adding resource to a user
        public const string resource_flow_added = nameof(resource_flow_added);
        // Sink: Used when removing a resource from a user
        public const string resource_flow_spend = nameof(resource_flow_spend);
        
        public const string gold = nameof(gold);
        public const string diamond = nameof(diamond);
        public const string energy = nameof(energy);
        public const string level = nameof(level);
        public const string experience = nameof(experience);
        public const string silver = nameof(silver);
        public const string coins = nameof(coins);
        public const string keys = nameof(keys);
        public const string life = nameof(life);
        
        /// <summary>
        /// Game params
        /// </summary>
        public const string gold_collected = nameof(gold_collected);
        public const string gold_rewarded = nameof(gold_rewarded);
        public const string level_number = nameof(level_number);
    }
}