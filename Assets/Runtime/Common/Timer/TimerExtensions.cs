using System;
using UnityEngine;

public static class TimerExtensions
{
    public static Timer SpawnTimer(this MonoBehaviour self, float waitTime, Action onTimeout = null, bool oneShot = true)
    {
        var holder = new GameObject("Timer");
        holder.hideFlags = HideFlags.HideInHierarchy;
        holder.transform.parent = self.transform;
        var timer = holder.AddComponent<Timer>();
        timer.WaitTime = waitTime;

        if (onTimeout != null)
            timer.Timeout += onTimeout;

        if (oneShot)
        {
            timer.OneShot = true;
            timer.DestroyOnTimeout = true;
            timer.Run();
        }

        return timer;
    }
}
