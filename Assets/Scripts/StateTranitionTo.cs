using System;

namespace Assets.Scripts
{
    [Serializable]
    public class StateTranitionTo
    {
        public StateTransition currentState;
        public RunnerState nextState;
    }
}