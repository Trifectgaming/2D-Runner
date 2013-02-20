using System;

namespace Assets.Scripts
{
    [Serializable]
    public class TransitionInfo
    {
        public string TransitionName;
        public CollisionInfo CollisionRequirements;
        public SpeedInfo VelocityRequirements;
        public float MinTransitionTime;
        public RunnerState NextState;
    }
}