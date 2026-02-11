
using System;
using System.Collections.Generic;
using UnityEngine;
public class EventBus : MonoBehaviour
{
    private static EventBus _instance;

    public static EventBus Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EventBus>();

                if (_instance == null)
                {
                    GameObject eventBusObject = new GameObject("EventBus");
                    _instance = eventBusObject.AddComponent<EventBus>();
                    DontDestroyOnLoad(eventBusObject);
                }
            }
            return _instance;
        }
    }

    private Dictionary<Type, List<Delegate>> _eventListeners = new Dictionary<Type, List<Delegate>>();
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void Subscribe<T>(Action<T> listener) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (!_eventListeners.ContainsKey(eventType))
        {
            _eventListeners[eventType] = new List<Delegate>();
        }

        if (!_eventListeners[eventType].Contains(listener))
        {
            _eventListeners[eventType].Add(listener);
        }
        else
        {
            Debug.LogWarning($"Listener already subscribed to event {eventType.Name}");
        }
    }

    public void Unsubscribe<T>(Action<T> listener) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (_eventListeners.ContainsKey(eventType))
        {
            _eventListeners[eventType].Remove(listener);

            // Clean up empty lists
            if (_eventListeners[eventType].Count == 0)
            {
                _eventListeners.Remove(eventType);
            }
        }
    }

    public void ClearSubscriptions<T>() where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (_eventListeners.ContainsKey(eventType))
        {
            _eventListeners[eventType].Clear();
            _eventListeners.Remove(eventType);
        }
    }
    public void ClearAllSubscriptions()
    {
        _eventListeners.Clear();
        Debug.Log("EventBus: All subscriptions cleared");
    }

    public int GetListenerCount<T>() where T : IGameEvent
    {
        Type eventType = typeof(T);
        return _eventListeners.ContainsKey(eventType) ? _eventListeners[eventType].Count : 0;
    }

    public bool HasListeners<T>() where T : IGameEvent
    {
        Type eventType = typeof(T);
        return _eventListeners.ContainsKey(eventType) && _eventListeners[eventType].Count > 0;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            ClearAllSubscriptions();
            _instance = null;
        }
    }
    public void PrintDebugInfo()
    {
        Debug.Log("=== EventBus Debug Info ===");
        Debug.Log($"Total Event Types: {_eventListeners.Count}");

        foreach (var kvp in _eventListeners)
        {
            Debug.Log($"  {kvp.Key.Name}: {kvp.Value.Count} listeners");
        }

        Debug.Log("=========================");
    }
}