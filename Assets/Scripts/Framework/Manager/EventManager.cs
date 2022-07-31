using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void EventHandler(object args);

    private Dictionary<int, EventHandler> m_Events = new Dictionary<int, EventHandler>();

    public void Subscribe(int id, EventHandler e)
    {
        if (m_Events.ContainsKey(id))
            m_Events[id] += e;
        else
            m_Events.Add(id, e);
    }

    public void UnSubscribe(int id, EventHandler e)
    {
        if (m_Events.ContainsKey(id))
        {
            m_Events[id] -= e;
        }
        else
        {
            Debug.LogError("Event " + id + " is not exist, Fail UnSubscribe Event");
            return;
        }

        if (m_Events[id] == null)
            m_Events.Remove(id);
    }

    public void Fire(int id, object args = null)
    {
        if (!m_Events.TryGetValue(id, out EventHandler handler))
        {
            Debug.LogError("Event " + id + " is not exist, Fail Fire Event");
            return;
        }
        handler(args);
    }
}
