using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ITimer
{
    void Run(float time);
    void Stop();
    void Pause(bool status = true);
}

public class Timer : MonoBehaviour, ITimer
{
    public bool AutoStart = false;
    public bool OneShot = false;
    public bool DestroyOnTimeout = false;

    public float WaitTime;

    public event Action Timeout;

    public bool Paused { get; private set; } = true;
    public float TimeLeft => timeRemaining;

    public float Complete => 1 - (TimeLeft / WaitTime);

    [NaughtyAttributes.ShowNonSerializedField]
    private float timeRemaining = 0f;

    public void Run(float time = 0)
    {
        if (time > 0)
            WaitTime = time;
        Paused = false;
        timeRemaining = WaitTime;
    }

    public void Stop()
    {
        Paused = true;
        timeRemaining = 0f;
    }

    public void Pause(bool status = true)
    {
        Paused = status;
    }

    public bool Stopped => Paused;

    private void Awake()
    {
        if (AutoStart)
            Run();
    }

    private void Update()
    {
        if (Paused) return;
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            Stop();
            Timeout?.Invoke();
            if (DestroyOnTimeout)
                GameObject.Destroy(this);
            if (!OneShot)
                Run(WaitTime);
        }
    }
}
