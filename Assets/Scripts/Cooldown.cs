using UnityEngine;
using System;

[Serializable]
public class Cooldown
{
    private float Duration;
    private float LastUseTime;

    public Cooldown(float duration)
    {
        Duration = duration;
    }

    public bool Use()
    {
        float currentTime = Time.time;
        if (currentTime - LastUseTime > Duration)
        {
            LastUseTime = currentTime;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Modify(float duration)
    {
        Duration = duration;
    }
}