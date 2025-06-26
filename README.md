# UniGame Game Analytics

Comprehensive analytics system for Unity games with support for multiple analytics providers and flexible event tracking.

- [UniGame Game Analytics](#unigame-game-analytics)
  - [Overview](#overview)
  - [Installation](#installation)
    - [Dependencies](#dependencies)
  - [Core Architecture](#core-architecture)
    - [System Components](#system-components)
    - [Analytics Flow](#analytics-flow)
  - [Quick Start](#quick-start)
    - [Basic Setup](#basic-setup)
    - [Configuration](#configuration)
  - [Analytics Service](#analytics-service)
    - [Service Interface](#service-interface)
    - [Service Registration](#service-registration)
    - [Event Tracking](#event-tracking)
  - [Analytics Adapters](#analytics-adapters)
    - [Supported Providers](#supported-providers)
    - [GameAnalytics Integration](#gameanalytics-integration)
    - [Firebase Analytics](#firebase-analytics)
    - [Unity Analytics](#unity-analytics)
    - [MyTracker](#mytracker)
    - [Debug Adapter](#debug-adapter)
  - [Event System](#event-system)
    - [Analytics Messages](#analytics-messages)
    - [Built-in Events](#built-in-events)
      - [Session Events](#session-events)
      - [Purchase Events](#purchase-events)
      - [Ads Events](#ads-events)
      - [Game Resource Events](#game-resource-events)
      - [Progression Events](#progression-events)
    - [Custom Events](#custom-events)
    - [Event Parameters](#event-parameters)
  - [Message Handlers](#message-handlers)
    - [Handler Interface](#handler-interface)
    - [Custom Handlers](#custom-handlers)
    - [Handler Registration](#handler-registration)
  - [Configuration System](#configuration-system)
    - [Analytics Configuration](#analytics-configuration)
    - [Adapter Configuration](#adapter-configuration)
    - [Runtime Configuration](#runtime-configuration)
  - [FPS Service](#fps-service)
    - [FPS Monitoring](#fps-monitoring)
    - [Performance Tracking](#performance-tracking)
  - [Examples](#examples)
    - [Basic Event Tracking](#basic-event-tracking)
    - [Custom Event Messages](#custom-event-messages)
    - [Purchase Events](#purchase-events-1)
    - [Ads Events](#ads-events-1)
    - [Game Resource Events](#game-resource-events-1)
    - [Progression Events](#progression-events-1)
  - [Best Practices](#best-practices)
    - [Event Design](#event-design)
    - [Performance](#performance)
    - [Testing](#testing)
    - [Configuration](#configuration-1)
  - [Troubleshooting](#troubleshooting)
    - [Common Issues](#common-issues)
      - [Events Not Sending](#events-not-sending)
      - [Adapter Initialization Failures](#adapter-initialization-failures)
      - [Missing Parameters](#missing-parameters)
    - [Debug Tips](#debug-tips)
      - [Enable Debug Logging](#enable-debug-logging)
      - [Event Validation](#event-validation)
      - [Performance Monitoring](#performance-monitoring)

## Overview

The UniGame Game Analytics module provides a comprehensive analytics solution for Unity games with the following features:

- **Multi-Provider Support**: Integrate with GameAnalytics, Firebase Analytics, Unity Analytics, MyTracker, and more
- **Event-Driven Architecture**: Flexible event system with custom message types
- **Async Operations**: Full async/await support for all analytics operations
- **Message Pipeline**: Configurable message handlers for data preprocessing
- **Performance Monitoring**: Built-in FPS tracking and performance analytics
- **Debug Support**: Comprehensive logging and debug analytics adapter
- **Type Safety**: Strongly typed event messages with parameter validation
- **Lifetime Management**: Automatic resource cleanup and memory management

## Installation

Add the package to your project via Package Manager:

```json
{
  "dependencies": {
    "com.unigame.analytics": "https://github.com/UnioGame/game.analytics.git"
  }
}
```

### Dependencies

The module requires these packages:

- `com.unity.addressables`: Asset management
- `com.cysharp.unitask`: Async operations
- `com.cysharp.r3`: Reactive programming
- `com.unigame.unicore`: Core utilities
- `com.unigame.addressablestools`: Addressable extensions

## Core Architecture

### System Components

```csharp
// Core service interface
public interface IAnalyticsService : IGameService
{
    void TrackEvent(IAnalyticsMessage message);
    IDisposable RegisterAdapter(IAnalyticsAdapter adapter);
    IDisposable RegisterMessageHandler(IAnalyticsMessageHandler handler);
}

// Analytics adapter interface
public interface IAnalyticsAdapter : IDisposable
{
    UniTask InitializeAsync();
    void TrackEvent(IAnalyticsMessage message);
}

// Message interface
public interface IAnalyticsMessage
{
    string GroupId { get; }
    string Name { get; }
    Dictionary<string, string> Parameters { get; }
    string this[string key] { get; set; }
}
```

### Analytics Flow

```
Event Creation → Message Handlers → Analytics Service → Adapters → External Services
```

1. **Event Creation**: Create analytics messages using built-in or custom types
2. **Message Handlers**: Process and enrich messages with additional data
3. **Analytics Service**: Distribute messages to registered adapters
4. **Adapters**: Send events to external analytics providers
5. **External Services**: GameAnalytics, Firebase, Unity Analytics, etc.

## Quick Start

### Basic Setup

1. **Create Analytics Configuration**:

```csharp
// Create configuration asset
[CreateAssetMenu(menuName = "Game/Services/Analytics/Analytics Configuration")]
public class AnalyticsConfigurationAsset : ScriptableObject
{
    public AnalyticsConfiguration configuration;
}
```

2. **Register Service**:

```csharp
// In your game context
public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private AnalyticsServiceSource analyticsSource;
    
    private async void Start()
    {
        var context = new GameContext();
        var analyticsService = await analyticsSource.CreateAsync(context);
        
        // Service is now available
        context.Get<IAnalyticsService>();
    }
}
```

### Configuration

```csharp
// Configure analytics in inspector
[Serializable]
public class AnalyticsConfiguration
{
    public bool isEnabled = true;
    public List<AnalyticsAdapterData> analytics = new();
    public List<IAnalyticsMessageHandler> messageHandlers = new();
}
```

## Analytics Service

### Service Interface

```csharp
public interface IAnalyticsService : IGameService, IAnalyticsHandlers
{
    // Track analytics event
    void TrackEvent(IAnalyticsMessage message);
    
    // Register analytics adapter
    IDisposable RegisterAdapter(IAnalyticsAdapter adapter);
}

public interface IAnalyticsHandlers
{
    // Register message handler
    IDisposable RegisterMessageHandler(IAnalyticsMessageHandler handler);
    
    // Update handlers collection
    void UpdateHandlers(IEnumerable<IAnalyticsMessageHandler> messageHandlers);
}
```

### Service Registration

```csharp
// Register service in context
public class AnalyticsServiceSource : DataSourceAsset<IAnalyticsService>
{
    public AddressableValue<AnalyticsConfigurationAsset> configurationReference;

    protected override async UniTask<IAnalyticsService> CreateInternalAsync(IContext context)
    {
        var configuration = await configurationReference.LoadAssetInstanceTaskAsync(context.LifeTime);
        var service = new GameAnalyticsService();
        
        // Register adapters
        foreach (var adapterData in configuration.analytics)
        {
            if (adapterData.isEnabled)
            {
                await adapterData.adapter.InitializeAsync();
                service.RegisterAdapter(adapterData.adapter);
            }
        }
        
        return service;
    }
}
```

### Event Tracking

```csharp
// Basic event tracking
public class GameController : MonoBehaviour
{
    private IAnalyticsService _analyticsService;
    
    private void Start()
    {
        _analyticsService = Context.Get<IAnalyticsService>();
    }
    
    public void TrackLevelComplete(int level, int score)
    {
        var message = new AnalyticsEventMessage("level_complete", "gameplay")
        {
            ["level"] = level.ToString(),
            ["score"] = score.ToString(),
            ["time"] = Time.time.ToString()
        };
        
        _analyticsService.TrackEvent(message);
    }
}
```

## Analytics Adapters

### Supported Providers

The module includes adapters for:

- **GameAnalytics**: Full GameAnalytics SDK integration
- **Firebase Analytics**: Google Firebase Analytics
- **Unity Analytics**: Unity's built-in analytics
- **MyTracker**: VK MyTracker analytics
- **Debug Adapter**: Console logging for development

### GameAnalytics Integration

```csharp
[Serializable]
public class GameAnalyticsProvider : IAnalyticsAdapter, IGameAnalyticsATTListener
{
    public AssetReferenceGameObject gameAnalyticsPrefab;
    public bool enableUnderEditor = false;
    
    public async UniTask InitializeAsync()
    {
        if (Application.isEditor && !enableUnderEditor)
            return;
            
        // Initialize GameAnalytics
        var gameAnalytics = await gameAnalyticsPrefab.InstantiateTaskAsync();
        
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            GameAnalytics.RequestTrackingAuthorization(this);
        else
            GameAnalytics.Initialize();
    }
    
    public void TrackEvent(IAnalyticsMessage message)
    {
        // Handle different message types
        if (message is PaymentEventMessage purchaseMessage)
        {
            TrackPurchase(purchaseMessage);
            return;
        }
        
        if (message is AdsEventMessage adsMessage)
        {
            TrackAdsEvent(adsMessage);
            return;
        }
        
        // Default design event
        GameAnalytics.NewDesignEvent(message.Name, message.Parameters);
    }
}
```

### Firebase Analytics

```csharp
[Serializable]
public class FirebaseAnalyticsHandler : IAnalyticsAdapter
{
    public async UniTask InitializeAsync()
    {
        var status = await FirebaseApp.CheckDependenciesAsync();
        if (status == DependencyStatus.Available)
        {
            // Firebase is ready
        }
    }
    
    public void TrackEvent(IAnalyticsMessage message)
    {
        var parameters = message.Parameters
            .Select(p => new Parameter(p.Key, p.Value))
            .ToArray();
            
        FirebaseAnalytics.LogEvent(message.Name, parameters);
    }
}
```

### Unity Analytics

```csharp
[Serializable]
public class UnityAnalyticsHandler : IAnalyticsAdapter
{
    public async UniTask InitializeAsync()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
    }
    
    public void TrackEvent(IAnalyticsMessage message)
    {
        var unityEvent = new UnityEventMessage(message.Name);
        foreach (var parameter in message.Parameters)
            unityEvent[parameter.Key] = parameter.Value;
            
        AnalyticsService.Instance.RecordEvent(unityEvent);
        AnalyticsService.Instance.Flush();
    }
}
```

### MyTracker

```csharp
[Serializable]
public class MyTrackerProvider : IAnalyticsAdapter
{
    public string iOSKey;
    public string AndroidKey;
    public bool IsDebug;
    
    public UniTask InitializeAsync()
    {
        MyTracker.IsDebugMode = IsDebug;
        
#if UNITY_IOS
        MyTracker.Init(iOSKey);
#elif UNITY_ANDROID
        MyTracker.Init(AndroidKey);
#endif
        
        return UniTask.CompletedTask;
    }
    
    public void TrackEvent(IAnalyticsMessage message)
    {
        MyTracker.TrackEvent(message.Name, message.Parameters);
    }
}
```

### Debug Adapter

```csharp
[Serializable]
public class DebugAnalyticsAdapter : IAnalyticsAdapter
{
    public UniTask InitializeAsync() => UniTask.CompletedTask;
    
    public void TrackEvent(IAnalyticsMessage message)
    {
#if GAME_DEBUG || UNITY_EDITOR
        GameLog.LogRuntime($"ANALYTICS EVENT: {message}", Color.yellow);
#endif
    }
}
```

## Event System

### Analytics Messages

```csharp
// Base analytics message
[Serializable]
public class AnalyticsEventMessage : IAnalyticsMessage
{
    public Dictionary<string, string> parameters = new();
    
    public AnalyticsEventMessage(string name, string groupId)
    {
        Name = name;
        GroupId = groupId;
        DeviceModel = SystemInfo.deviceModel;
    }
    
    public string Name { get; protected set; }
    public string GroupId { get; set; }
    public string UserId { get; set; }
    public string EventSource { get; set; }
    public string SceneName { get; set; }
    
    public string this[string key]
    {
        get => parameters.TryGetValue(key, out var value) ? value : string.Empty;
        set => parameters[key] = value;
    }
}
```

### Built-in Events

#### Session Events

```csharp
// Login event
public class LoginMessage : AnalyticsEventMessage
{
    public LoginMessage() : base("login", "session_group") { }
}

// Session start
public class SessionStartMessage : AnalyticsEventMessage
{
    public SessionStartMessage(string name) : base(name, name) { }
}

// First app open
public class FirstOpenMessage : AnalyticsEventMessage
{
    public FirstOpenMessage(string name) : base(name, name) { }
}
```

#### Purchase Events

```csharp
[Serializable]
public class PaymentEventMessage : AnalyticsEventMessage
{
    public PaymentEventMessage(string eventName) : base(eventName, "shop_group") { }
    
    public float Price { get; set; }
    public string Currency { get; set; }
    public string ItemType { get; set; }
    public string ItemId { get; set; }
    public string Receipt { get; set; }
    public string Signature { get; set; }
    public string EntryPoint { get; set; }
}

// Specific purchase events
public class PaymentAttemptingEventMessage : PaymentEventMessage
{
    public PaymentAttemptingEventMessage() : base("payment_attempting") { }
}

public class PurchaseCompleteEventMessage : PaymentEventMessage
{
    public PurchaseCompleteEventMessage() : base("payment_complete") { }
}

public class PurchaseErrorEventMessage : PaymentEventMessage
{
    public PurchaseErrorEventMessage() : base("payment_error") { }
}
```

#### Ads Events

```csharp
[Serializable]
public class AdsEventMessage : AnalyticsEventMessage
{
    public AdsEventMessage(string name) : base(name, "shop_group") { }
    
    public string Placement { get; set; }
    public string SdkName { get; set; }
    public string AdsType { get; set; }
    public string ActionType { get; set; }
    public bool FirstTimeAds { get; set; }
    public float AdsPrice { get; set; }
    public int AdsDuration { get; set; }
    public string Message { get; set; }
    public string ErrorCode { get; set; }
}
```

#### Game Resource Events

```csharp
[Serializable]
public class GameResourceEventMessage : AnalyticsEventMessage
{
    public GameResourceEventMessage(string eventName) : base(eventName, "game_resource_group") { }
    
    public string ResourceCurrency { get; set; }
    public string ResourceType { get; set; }
    public int ResourceValue { get; set; }
    public string ResourceSource { get; set; }
}

public class GameResourceFlowEventMessage : GameResourceEventMessage
{
    public GameResourceFlowEventMessage() : base("game_resource_flow") { }
    
    public string FlowType { get; set; } // "resource_flow_added" or "resource_flow_spend"
    public string ItemType { get; set; }
    public string ItemId { get; set; }
}
```

#### Progression Events

```csharp
[Serializable]
public class GAProgressionEventMessage : AnalyticsEventMessage
{
    public GAProgressionEventMessage() : base("ga_progression", "ga_group") { }
    
    public string ProgressionStatus { get; set; } // "start", "complete", "fail"
    public string Progression01 { get; set; }
    public string Progression02 { get; set; }
    public string Progression03 { get; set; }
    public int Score { get; set; }
}
```

### Custom Events

```csharp
// Create custom event message
[Serializable]
public class CustomGameEvent : AnalyticsEventMessage
{
    public CustomGameEvent() : base("custom_event", "custom_group") { }
    
    public string CustomParameter
    {
        get => this["custom_param"];
        set => this["custom_param"] = value;
    }
    
    public int CustomValue
    {
        get => int.TryParse(this["custom_value"], out var value) ? value : 0;
        set => this["custom_value"] = value.ToString();
    }
}

// Usage
var customEvent = new CustomGameEvent
{
    CustomParameter = "test_value",
    CustomValue = 42,
    UserId = "user123",
    SceneName = "MainMenu"
};

analyticsService.TrackEvent(customEvent);
```

### Event Parameters

```csharp
// Typed parameter values
[Serializable]
public class AnalyticsIntValue
{
    private int _value;
    private AnalyticsEventMessage _message;
    private string _name;
    
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            _message[_name] = value.ToStringFromCache();
        }
    }
}

[Serializable]
public class AnalyticsFloatValue
{
    private float _value;
    private AnalyticsEventMessage _message;
    private string _name;
    
    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            _message[_name] = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}

[Serializable]
public class AnalyticsBoolValue
{
    public const string TrueValue = "true";
    public const string FalseValue = "false";
    
    private bool _value;
    private AnalyticsEventMessage _message;
    private string _name;
    
    public bool Value
    {
        get => _value;
        set
        {
            _value = value;
            _message[_name] = value ? TrueValue : FalseValue;
        }
    }
}
```

## Message Handlers

### Handler Interface

```csharp
public interface IAnalyticsMessageHandler
{
    UniTask<IAnalyticsMessage> UpdateMessageAsync(IAnalyticsMessage message);
}
```

### Custom Handlers

```csharp
// Example: Add timestamp to all events
[Serializable]
public class TimestampHandler : IAnalyticsMessageHandler
{
    public async UniTask<IAnalyticsMessage> UpdateMessageAsync(IAnalyticsMessage message)
    {
        message["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        message["time_from_start"] = Time.time.ToString("F2");
        return message;
    }
}

// Example: Add user data
[Serializable]
public class UserDataHandler : IAnalyticsMessageHandler
{
    private readonly IUserService _userService;
    
    public async UniTask<IAnalyticsMessage> UpdateMessageAsync(IAnalyticsMessage message)
    {
        var user = await _userService.GetCurrentUserAsync();
        if (user != null)
        {
            message["user_id"] = user.Id;
            message["user_level"] = user.Level.ToString();
            message["user_coins"] = user.Coins.ToString();
        }
        
        return message;
    }
}
```

### Handler Registration

```csharp
// Register handlers in configuration
public class AnalyticsConfiguration
{
    [SerializeReference]
    public List<IAnalyticsMessageHandler> messageHandlers = new()
    {
        new TimestampHandler(),
        new UserDataHandler(),
        new FpsHandler()
    };
}

// Or register at runtime
analyticsService.RegisterMessageHandler(new CustomHandler());
```

## Configuration System

### Analytics Configuration

```csharp
[Serializable]
public class AnalyticsConfiguration
{
    public bool isEnabled = true;
    
    [Header("Analytics Adapters")]
    [ListDrawerSettings(ListElementLabelName = "@name")]
    public List<AnalyticsAdapterData> analytics = new();
    
    [Header("Analytics Handlers")]
    public List<IAnalyticsMessageHandler> messageHandlers = new();
}
```

### Adapter Configuration

```csharp
[Serializable]
public class AnalyticsAdapterData
{
    public string name;
    public bool isEnabled = true;
    
    [SerializeReference]
    public IAnalyticsAdapter adapter;
}
```

### Runtime Configuration

```csharp
// Configure adapters at runtime
public void ConfigureAnalytics()
{
    var configuration = new AnalyticsConfiguration
    {
        isEnabled = true,
        analytics = new List<AnalyticsAdapterData>
        {
            new()
            {
                name = "GameAnalytics",
                isEnabled = true,
                adapter = new GameAnalyticsProvider()
            },
            new()
            {
                name = "Firebase",
                isEnabled = true,
                adapter = new FirebaseAnalyticsHandler()
            }
        }
    };
}
```

## FPS Service

### FPS Monitoring

```csharp
public interface IFpsService : IGameService
{
    ReadOnlyReactiveProperty<float> CurrentFps { get; }
}

[Serializable]
public class FpsService : GameService, IFpsService
{
    public ReadOnlyReactiveProperty<float> CurrentFps { get; }
    
    public FpsService()
    {
        CurrentFps = Observable.EveryUpdate()
            .Select(_ => Time.unscaledDeltaTime)
            .Chunk(bufferSize, skipFrame)
            .Select(x => 1 / x.Average())
            .ToReadOnlyReactiveProperty();
    }
}
```

### Performance Tracking

```csharp
// Track FPS in analytics
public class FpsHandler : IAnalyticsMessageHandler
{
    public async UniTask<IAnalyticsMessage> UpdateMessageAsync(IAnalyticsMessage message)
    {
        message["fps"] = FpsService.Fps.ToString("F1");
        return message;
    }
}

// Monitor performance
public class PerformanceMonitor : MonoBehaviour
{
    private IFpsService _fpsService;
    private IAnalyticsService _analyticsService;
    
    private void Start()
    {
        _fpsService = Context.Get<IFpsService>();
        _analyticsService = Context.Get<IAnalyticsService>();
        
        // Track low FPS events
        _fpsService.CurrentFps
            .Where(fps => fps < 30f)
            .Throttle(TimeSpan.FromSeconds(5))
            .Subscribe(fps => TrackLowFps(fps))
            .AddTo(this);
    }
    
    private void TrackLowFps(float fps)
    {
        var message = new AnalyticsEventMessage("low_fps", "performance")
        {
            ["fps"] = fps.ToString("F1"),
            ["scene"] = SceneManager.GetActiveScene().name
        };
        
        _analyticsService.TrackEvent(message);
    }
}
```

## Examples

### Basic Event Tracking

```csharp
public class BasicAnalyticsExample : MonoBehaviour
{
    private IAnalyticsService _analyticsService;
    
    private void Start()
    {
        _analyticsService = Context.Get<IAnalyticsService>();
        
        // Track simple event
        TrackSimpleEvent();
        
        // Track event with parameters
        TrackEventWithParameters();
    }
    
    private void TrackSimpleEvent()
    {
        var message = new AnalyticsEventMessage("game_start", "session");
        _analyticsService.TrackEvent(message);
    }
    
    private void TrackEventWithParameters()
    {
        var message = new AnalyticsEventMessage("level_start", "gameplay")
        {
            ["level"] = "1",
            ["difficulty"] = "normal",
            ["character"] = "warrior"
        };
        
        _analyticsService.TrackEvent(message);
    }
}
```

### Custom Event Messages

```csharp
// Define custom event
[Serializable]
public class PlayerDeathEvent : AnalyticsEventMessage
{
    public PlayerDeathEvent() : base("player_death", "gameplay") { }
    
    public string DeathCause
    {
        get => this["death_cause"];
        set => this["death_cause"] = value;
    }
    
    public int Level
    {
        get => int.TryParse(this["level"], out var level) ? level : 0;
        set => this["level"] = value.ToString();
    }
    
    public float SurvivalTime
    {
        get => float.TryParse(this["survival_time"], out var time) ? time : 0f;
        set => this["survival_time"] = value.ToString("F2");
    }
}

// Usage
public class GameplayAnalytics : MonoBehaviour
{
    private IAnalyticsService _analyticsService;
    
    public void OnPlayerDeath(string cause, int level, float survivalTime)
    {
        var deathEvent = new PlayerDeathEvent
        {
            DeathCause = cause,
            Level = level,
            SurvivalTime = survivalTime,
            UserId = PlayerData.UserId,
            SceneName = SceneManager.GetActiveScene().name
        };
        
        _analyticsService.TrackEvent(deathEvent);
    }
}
```

### Purchase Events

```csharp
public class PurchaseAnalytics : MonoBehaviour
{
    private IAnalyticsService _analyticsService;
    
    public void OnPurchaseAttempt(string productId, float price, string currency)
    {
        var attemptEvent = new PaymentAttemptingEventMessage
        {
            ItemId = productId,
            Price = price,
            Currency = currency,
            EntryPoint = "shop_main"
        };
        
        _analyticsService.TrackEvent(attemptEvent);
    }
    
    public void OnPurchaseComplete(string productId, float price, string currency, string receipt)
    {
        var completeEvent = new PurchaseCompleteEventMessage
        {
            ItemId = productId,
            ItemType = "consumable",
            Price = price,
            Currency = currency,
            Receipt = receipt,
            EntryPoint = "shop_main"
        };
        
        _analyticsService.TrackEvent(completeEvent);
    }
    
    public void OnPurchaseError(string productId, string error)
    {
        var errorEvent = new PurchaseErrorEventMessage
        {
            ItemId = productId,
            EntryPoint = "shop_main"
        };
        errorEvent["error_message"] = error;
        
        _analyticsService.TrackEvent(errorEvent);
    }
}
```

### Ads Events

```csharp
public class AdsAnalytics : MonoBehaviour
{
    private IAnalyticsService _analyticsService;
    
    public void OnRewardedAdShown(string placement, string sdkName)
    {
        var adsEvent = new AdsEventMessage("ads_rewarded_shown")
        {
            Placement = placement,
            SdkName = sdkName,
            AdsType = "rewarded_video",
            ActionType = "opened",
            FirstTimeAds = IsFirstTimeAd(placement)
        };
        
        _analyticsService.TrackEvent(adsEvent);
    }
    
    public void OnRewardedAdCompleted(string placement, string reward, int rewardAmount)
    {
        var adsEvent = new AdsEventMessage("ads_rewarded_completed")
        {
            Placement = placement,
            AdsType = "rewarded_video",
            ActionType = "rewarded"
        };
        adsEvent["reward_type"] = reward;
        adsEvent["reward_amount"] = rewardAmount.ToString();
        
        _analyticsService.TrackEvent(adsEvent);
    }
    
    public void OnAdsFailed(string placement, string error)
    {
        var adsEvent = new AdsEventMessage("ads_failed")
        {
            Placement = placement,
            ActionType = "failed",
            Message = error
        };
        
        _analyticsService.TrackEvent(adsEvent);
    }
    
    private bool IsFirstTimeAd(string placement)
    {
        // Check if this is the first time showing ad for this placement
        return !PlayerPrefs.HasKey($"ads_shown_{placement}");
    }
}
```

### Game Resource Events

```csharp
public class ResourceAnalytics : MonoBehaviour
{
    private IAnalyticsService _analyticsService;
    
    public void OnResourceEarned(string resourceType, int amount, string source)
    {
        var resourceEvent = new GameResourceFlowEventMessage
        {
            ResourceCurrency = resourceType,
            ResourceValue = amount,
            ResourceSource = source,
            FlowType = "resource_flow_added",
            ItemType = "currency",
            ItemId = resourceType
        };
        
        _analyticsService.TrackEvent(resourceEvent);
    }
    
    public void OnResourceSpent(string resourceType, int amount, string itemType, string itemId)
    {
        var resourceEvent = new GameResourceFlowEventMessage
        {
            ResourceCurrency = resourceType,
            ResourceValue = amount,
            FlowType = "resource_flow_spend",
            ItemType = itemType,
            ItemId = itemId
        };
        
        _analyticsService.TrackEvent(resourceEvent);
    }
    
    public void OnResourceChanged(string resourceType, int newAmount, int change)
    {
        var changeEvent = new GameResourceChangeEventMessage
        {
            ResourceCurrency = resourceType,
            ResourceValue = newAmount
        };
        changeEvent["change_amount"] = change.ToString();
        changeEvent["change_type"] = change > 0 ? "increase" : "decrease";
        
        _analyticsService.TrackEvent(changeEvent);
    }
}
```

### Progression Events

```csharp
public class ProgressionAnalytics : MonoBehaviour
{
    private IAnalyticsService _analyticsService;
    
    public void OnLevelStart(string world, string level)
    {
        var progressionEvent = new GAProgressionEventMessage
        {
            ProgressionStatus = "start",
            Progression01 = world,
            Progression02 = level,
            Score = 0
        };
        
        _analyticsService.TrackEvent(progressionEvent);
    }
    
    public void OnLevelComplete(string world, string level, int score)
    {
        var progressionEvent = new GAProgressionEventMessage
        {
            ProgressionStatus = "complete",
            Progression01 = world,
            Progression02 = level,
            Score = score
        };
        
        _analyticsService.TrackEvent(progressionEvent);
    }
    
    public void OnLevelFailed(string world, string level, int score, string failReason)
    {
        var progressionEvent = new GAProgressionEventMessage
        {
            ProgressionStatus = "fail",
            Progression01 = world,
            Progression02 = level,
            Score = score
        };
        progressionEvent["fail_reason"] = failReason;
        
        _analyticsService.TrackEvent(progressionEvent);
    }
}
```

## Best Practices

### Event Design

1. **Use Consistent Naming**: Follow consistent naming conventions for events and parameters
2. **Group Related Events**: Use meaningful group IDs to organize related events
3. **Limit Parameter Count**: Keep parameter count reasonable (< 25 parameters per event)
4. **Validate Data**: Always validate parameter values before sending events

### Performance

1. **Async Operations**: Use async/await for all analytics operations
2. **Batch Events**: Consider batching events for better performance
3. **Error Handling**: Implement proper error handling in adapters
4. **Memory Management**: Use lifetime management for proper cleanup

### Testing

1. **Debug Adapter**: Use debug adapter during development
2. **Conditional Compilation**: Use conditional compilation for debug code
3. **Test Events**: Create test events to verify integration
4. **Monitor Logs**: Monitor analytics logs for errors

### Configuration

1. **Environment-Specific**: Use different configurations for dev/staging/production
2. **Feature Flags**: Use feature flags to enable/disable analytics
3. **Adapter Selection**: Choose appropriate adapters for your needs
4. **Handler Pipeline**: Design message handler pipeline carefully

## Troubleshooting

### Common Issues

#### Events Not Sending

```csharp
// Check if service is registered
var analyticsService = Context.Get<IAnalyticsService>();
if (analyticsService == null)
{
    Debug.LogError("Analytics service not registered!");
    return;
}

// Check if adapters are initialized
foreach (var adapter in configuration.analytics)
{
    if (!adapter.isEnabled)
    {
        Debug.LogWarning($"Adapter {adapter.name} is disabled");
    }
}
```

#### Adapter Initialization Failures

```csharp
// Add error handling in adapter initialization
public async UniTask InitializeAsync()
{
    try
    {
        await SomeAsyncInitialization();
    }
    catch (Exception ex)
    {
        Debug.LogError($"Analytics adapter initialization failed: {ex.Message}");
        throw;
    }
}
```

#### Missing Parameters

```csharp
// Validate required parameters
public class ValidationHandler : IAnalyticsMessageHandler
{
    public async UniTask<IAnalyticsMessage> UpdateMessageAsync(IAnalyticsMessage message)
    {
        // Ensure required parameters are present
        if (string.IsNullOrEmpty(message["user_id"]))
        {
            message["user_id"] = "unknown";
        }
        
        if (string.IsNullOrEmpty(message["session_id"]))
        {
            message["session_id"] = SystemInfo.deviceUniqueIdentifier;
        }
        
        return message;
    }
}
```

### Debug Tips

#### Enable Debug Logging

```csharp
// Use debug adapter for development
var debugAdapter = new DebugAnalyticsAdapter();
analyticsService.RegisterAdapter(debugAdapter);

// Enable debug mode in GameAnalytics
var gameAnalyticsProvider = new GameAnalyticsProvider
{
    enableUnderEditor = true
};
```

#### Event Validation

```csharp
// Create validation utility
public static class AnalyticsValidator
{
    public static bool ValidateEvent(IAnalyticsMessage message)
    {
        if (string.IsNullOrEmpty(message.Name))
        {
            Debug.LogError("Event name is required");
            return false;
        }
        
        if (message.Parameters.Count > 25)
        {
            Debug.LogWarning($"Event {message.Name} has {message.Parameters.Count} parameters (recommended: < 25)");
        }
        
        return true;
    }
}
```

#### Performance Monitoring

```csharp
// Monitor analytics performance
public class AnalyticsPerformanceMonitor : IAnalyticsMessageHandler
{
    public async UniTask<IAnalyticsMessage> UpdateMessageAsync(IAnalyticsMessage message)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // Process message
        var result = await ProcessMessage(message);
        
        stopwatch.Stop();
        
        if (stopwatch.ElapsedMilliseconds > 100)
        {
            Debug.LogWarning($"Analytics message processing took {stopwatch.ElapsedMilliseconds}ms");
        }
        
        return result;
    }
}
```

The UniGame Game Analytics module provides a comprehensive, flexible, and performant solution for analytics integration in Unity games. With its multi-provider support, event-driven architecture, and extensive customization options, it can handle analytics needs for games of any scale and complexity.