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
        public float ReuseTime;
        public float LastUseTime;
        public RunnerState NextState;
        public RunnerEffect RechargeEffect;
        public RunnerEffect TransitionEffect;
        //public bool Locking;
        public void Use()
        {
            LastUseTime = Time.time + ReuseTime;
        }
    }
}