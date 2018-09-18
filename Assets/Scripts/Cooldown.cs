using UnityEngine;
using ProtoBuf;

[ProtoContract]
public class Cooldown
{
    [ProtoMember(1)] private float Duration;
    [ProtoMember(2)] private float LastUseTime;

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