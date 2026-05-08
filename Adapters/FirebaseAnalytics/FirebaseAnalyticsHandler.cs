#if ANALYTICS_GOOGLE

namespace UniGame.Runtime.Analytics.Adapters
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Firebase;
    using Firebase.Analytics;
    using UniGame.Runtime.Analytics.Interfaces;
    using UniGame.Runtime.DataFlow;

    [Serializable]
    public sealed class FirebaseAnalyticsHandler: IAnalyticsAdapter
    {
        private List<FirebaseDataEvent> _events = new();
        private bool _isInitializing = false;
        private bool _initialized;
        private LifeTime _lifeTime = new();
        
        public async UniTask InitializeAsync()
        {
            await WaitForInitialization();
        }

        public void TrackEvent(IAnalyticsMessage message)
        {
            var i = 0;
            var parameters = new Parameter[message.Parameters.Keys.Count];
            foreach (var pair in message.Parameters)
            {
                parameters[i] = new Parameter(pair.Key, pair.Value);
                i++;
            }

            var eventData = new FirebaseDataEvent
            {
                Name = message.Name,
                Parameters = parameters,
            };
            
            SendEvent(eventData).Forget();
        }

        public async UniTask SendEvent(FirebaseDataEvent eventData)
        {
            _events.Add(eventData);

            var initialized = await WaitForInitialization();
            if(initialized == false) return;
            
            foreach (var dataEvent in _events)
            {
                FirebaseAnalytics.LogEvent(dataEvent.Name, dataEvent.Parameters);
            }
            
            _events.Clear();
        }

        public void Dispose()
        {
            _lifeTime.Terminate();
        }
        
        public async UniTask<bool> WaitForInitialization()
        {
            if (_initialized) return true;
            if (_isInitializing) return false;
            
            _isInitializing = true;
            
            while (_initialized == false && _lifeTime.IsTerminated == false)
            {
                var checkResult = await FirebaseApp.CheckDependenciesAsync()
                    .AsUniTask();
                await UniTask.SwitchToMainThread();
                if(checkResult != DependencyStatus.Available)
                {
                    await UniTask.DelayFrame(1, cancellationToken: _lifeTime.Token);
                    continue;
                }
                _initialized = true;
            }
            
            _isInitializing = false;
            return _initialized;
        }
    }

    [Serializable]
    public struct FirebaseDataEvent
    {
        public string Name;
        public Parameter[] Parameters;
    }
}

#endif