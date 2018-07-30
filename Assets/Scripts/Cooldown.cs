using UnityEngine;

public class Cooldown
{
    private readonly float Duration;
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
}