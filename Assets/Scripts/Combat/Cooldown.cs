﻿using UnityEngine;
using MessagePack;

namespace ADL.Combat
{
    [MessagePackObject]
    public class Cooldown
    {
        [Key(0)] private float Duration;
        [Key(1)] private float LastUseTime;

        public Cooldown(float duration)
        {
            Duration = duration;
            LastUseTime = Time.time - Duration;
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
}