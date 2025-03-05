namespace Game.Runtime.Services.Analytics.Runtime
{
    public static class AnalyticsEventsNames
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

        //===================ADS===================
        //ads
        public const string ads_reward_start = nameof(ads_reward_start);
        public const string ads_reward_granted = nameof(ads_reward_granted);
        public const string ads_reward_other = nameof(ads_reward_other);
        public const string ads_reward_skipped = nameof(ads_reward_skipped);
        
        //ads parameters
        public const string ads_reward_count = nameof(ads_reward_count);
        public const string ads_reward = nameof(ads_reward);
        public const string ads_placement_name = nameof(ads_placement_name);
        public const string ads_action_type = nameof(ads_action_type);
        public const string ads_type = nameof(ads_type);
        public const string ads_message = nameof(ads_message);
        public const string ads_error_code = nameof(ads_error_code);
        public const string ads_sdk_name = nameof(ads_sdk_name);
        public const string ads_duration = nameof(ads_duration);
        public const string ads_first_time = nameof(ads_first_time);
        public const string ads_price = nameof(ads_price);
        
        //ads types
        public const string video = nameof(video);
        public const string rewarded_video = nameof(rewarded_video);
        public const string playable = nameof(playable);
        public const string interstitial = nameof(interstitial);
        public const string offer_wall = nameof(offer_wall);
        public const string banner = nameof(banner);
        
        //ads actions
        public const string clicked     = nameof(clicked);
        public const string opened      = nameof(opened);
        public const string failed      = nameof(failed);
        public const string rewarded    = nameof(rewarded);
        public const string requested   = nameof(requested);
        public const string closed      = nameof(closed);
        public const string unavailable = nameof(unavailable);
        public const string available    = nameof(available);
        
        //===================COMMON===================
        /// <summary>
        /// events
        /// </summary>
        public const string exception = nameof(exception);
        public const string exception_message = nameof(exception_message);
        
        //parameters
        public const string fps = nameof(fps);
        public const string event_source = nameof(event_source);
        public const string event_name = nameof(event_name);
        public const string group_id = nameof(group_id);
        public const string device_model = nameof(device_model);
        public const string feature_use = nameof(feature_use);
        public const string other = nameof(other);
        public const string user_id = nameof(user_id);
        public const string scene_name = nameof(scene_name);
        public const string time = nameof(time);
        public const string time_from_start = nameof(time_from_start);
        
        //===================Monetization===================
        
        //monetization events
        public const string payment_attempting = nameof(payment_attempting);
        public const string payment_start = nameof(payment_start);
        public const string payment_complete = nameof(payment_complete);
        public const string payment_error = nameof(payment_error);
        
        
        //monetization parameters
        public const string entry_point = nameof(entry_point);
        public const string currency = nameof(currency);
        public const string price = nameof(price);
        public const string iap_type = nameof(iap_type);
        public const string iap_id = nameof(iap_id);
        public const string iap_receipt = nameof(iap_receipt);
        public const string iap_signature = nameof(iap_signature);
        public const string iap_cart_type = nameof(iap_cart_type);
        
        //===================Game Resources===================
        
        //game resources
        public const string game_resource_request = nameof(game_resource_request);
        public const string game_resource_changed = nameof(game_resource_changed);
        public const string game_resource_flow = nameof(game_resource_flow);
        
        // game resources parameters
        public const string resource = nameof(resource);
        public const string resource_currency = nameof(resource_currency);
        public const string resource_value = nameof(resource_value);
        public const string resource_source = nameof(resource_source);
        public const string resource_flow_type = nameof(resource_flow_type);
        public const string resource_action = nameof(resource_action);
        
        public const string resource_cost = nameof(resource_cost);
        public const string resource_id = nameof(resource_id);
        public const string resource_get = nameof(resource_get);
        public const string resource_item_type = nameof(resource_item_type);
        public const string resource_item_id = nameof(resource_item_id);
        public const string resource_receive_value = nameof(resource_receive_value);
        public const string resource_spend = nameof(resource_spend);
        public const string resource_spend_value = nameof(resource_spend_value);
        //Undefined progression
        public const string resource_flow_undefined = nameof(resource_flow_undefined);
        // Source: Used when adding resource to a user
        public const string resource_flow_added = nameof(resource_flow_added);
        // Sink: Used when removing a resource from a user
        public const string resource_flow_spend = nameof(resource_flow_spend);
        
        //resource types
        public const string gold = nameof(gold);
        public const string diamond = nameof(diamond);
        public const string energy = nameof(energy);
        public const string level = nameof(level);
        public const string experience = nameof(experience);
        public const string silver = nameof(silver);
        public const string coins = nameof(coins);
        public const string keys = nameof(keys);
        public const string life = nameof(life);
        
        //===================Session===================
        
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
        
        
        //===================UI===================
        /// <summary>
        /// ui data
        /// </summary>
        public const string ui_element = nameof(ui_element);
        
        
        //===================Methods===================
        
        public static string GetAdsType(this AnalyticsAdsType type)
        {
            switch (type)
            {
                case AnalyticsAdsType.RewardedVideo:
                    return rewarded_video;
                case AnalyticsAdsType.Video:
                    return video;
                case AnalyticsAdsType.Playable:
                    return playable;
                case AnalyticsAdsType.Interstitial:
                    return interstitial;
                case AnalyticsAdsType.OfferWall:
                    return offer_wall;
                case AnalyticsAdsType.Banner:
                    return banner;
                default:
                    return rewarded_video;
            }
        }
        
    }

    public enum AnalyticsAdsType
    {
        RewardedVideo = 0,
        Video = 1,
        Undefined = 2,
        Playable = 3,
        Interstitial = 4,
        OfferWall = 5,
        Banner = 6,
    }
}