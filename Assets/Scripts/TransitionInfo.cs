using System;

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
    }
}