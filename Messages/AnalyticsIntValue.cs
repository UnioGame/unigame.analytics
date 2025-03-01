namespace Game.Runtime.Services.Analytics.Messages
{
    using System;
    using System.Globalization;
    using UniModules.UniCore.Runtime.Utils;

    [Serializable]
    public class AnalyticsIntValue
    {
        private int _value;
        private AnalyticsEventMessage _message;
        private string _name;
        
        public AnalyticsIntValue(AnalyticsEventMessage message,string name, int value = 0)
        {
            _message = message;
            _name = name;
            _value = value;
        }
        
        public string Name => _name;

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
        
        public AnalyticsFloatValue(AnalyticsEventMessage message,string name, float value = 0)
        {
            _message = message;
            _name = name;
            _value = value;
        }
        
        public string Name => _name;

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
        
        public AnalyticsBoolValue(AnalyticsEventMessage message,string name, bool value = false)
        {
            _message = message;
            _name = name;
            _value = value;
        }
        
        public string Name => _name;

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
}