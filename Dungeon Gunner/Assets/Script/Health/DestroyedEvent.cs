using System;
using UnityEngine;

public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent, DestroyedEventArgs> OnDestroyed;
    
    public void CallDestroyedEvent(bool playerDied, int point)
    {
        OnDestroyed?.Invoke(this, new DestroyedEventArgs() { playerDied = playerDied, point = point});
    }
}

public class DestroyedEventArgs : EventArgs
{
    public bool playerDied;
    public int point;
}
