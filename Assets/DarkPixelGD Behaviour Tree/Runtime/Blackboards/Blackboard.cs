using System.Collections.Generic;
using System;

namespace DarkPixelGD.BehaviourTree
{


    public class Blackboard
    {
        private Dictionary<string, object> data = new();
        private Dictionary<string, Action> observers = new();
        private Dictionary<string, System.Type> types = new();


        public void Initialize(BlackboardAsset asset)
        {
            data.Clear();

            foreach (var key in asset.keys)
            {
                switch (key.type)
                {
                    case BlackboardValueType.Bool:
                        Set(key.name, key.boolValue);
                        break;

                    case BlackboardValueType.Float:
                        Set(key.name, key.floatValue);
                        break;

                    case BlackboardValueType.Int:
                        Set(key.name, key.intValue);
                        break;
                }
            }
        }

        // SET VALUE
        public void Set<T>(string key, T value)
        {
            bool changed = !data.ContainsKey(key) || !data[key].Equals(value);

            data[key] = value;

            //Track type
            types[key] = typeof(T);

            if (changed && observers.TryGetValue(key, out var callback))
            {
                callback?.Invoke();
            }
        }
        public void Subscribe(string key, Action callback)
        {
            if (!observers.ContainsKey(key))
                observers[key] = null;

            observers[key] += callback;
        }

        // UNSUBSCRIBE
        public void Unsubscribe(string key, Action callback)
        {
            if (observers.ContainsKey(key))
                observers[key] -= callback;
        }

        // SAFE GET (typed)
        public bool TryGet<T>(string key, out T value)
        {
            if (data.TryGetValue(key, out var obj) && obj is T casted)
            {
                value = casted;
                return true;
            }

            value = default;
            return false;
        }

        // GENERIC GET (fallback)
        public T Get<T>(string key)
        {
            if (TryGet<T>(key, out var value))
                return value;

            return default;
        }

        // OBJECT GET (needed for generic systems like Condition)
        public bool TryGetValue(string key, out object value)
        {
            return data.TryGetValue(key, out value);
        }

        // CHECK EXISTS
        public bool Has(string key)
        {
            return data.ContainsKey(key);
        }

        // REMOVE (useful later)
        public void Remove(string key)
        {
            data.Remove(key);
        }

        // CLEAR (debug/testing)
        public void Clear()
        {
            data.Clear();
        }
        public Dictionary<string, object> GetAll()
        {
            return data;
        }

        public List<string> GetKeys()
        {
            return new List<string>(data.Keys);
        }

        public System.Type GetKeyType(string key)
        {
            if (types.TryGetValue(key, out var type))
                return type;

            return null;
        }
        public Dictionary<string, System.Type> GetMissingKeysWithType(BlackboardAsset asset)
        {
            Dictionary<string, System.Type> missing = new();

            foreach (var kvp in data)
            {
                if (!asset.keys.Exists(k => k.name == kvp.Key))
                {
                    missing[kvp.Key] = kvp.Value?.GetType();
                }
            }

            return missing;
        }
        public System.Type GetValueType(string key)
        {
            if (data.TryGetValue(key, out var value))
                return value?.GetType();

            return null;
        }
    }
}