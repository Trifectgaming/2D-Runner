using System;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class TransitionInfo
    {
        public string TransitionName;
        public CollisionInfo CollisionRequirements;
        public SpeedInfo VelocityRequirements;
        public float? DelayTime;
        public float ReuseTime;
        public float LastUseTime;
        public RunnerState NextState;
        public bool HasRechargeEffect;
        public bool HasTransitionEffect;
        
        public void Use()
        {
            LastUseTime = Time.time + ReuseTime;
        }
    }
}