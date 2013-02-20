using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class RunnerStateMachine
    {
        public class ToState
        {
            public RunnerState nextState;
            public float minSpeedX;
            public float minSpeedY;
            public float maxSpeedX;
            public float maxSpeedY;
        }

        public RunnerState currentState = RunnerState.Running;
        public bool touchingPlatform;
        public Vector3 velocity;
        public StateTranitionTo[] transitions;
        private Dictionary<StateTransition, ToState> _transitionCache;
        public Queue<RunnerState> StateProcessQueue = new Queue<RunnerState>();

        public void Initialize()
        {
            if (transitions != null)
            {
                _transitionCache = new Dictionary<StateTransition, ToState>(transitions.Length);
                Debug.Log("Initalized " + transitions.Length + " transitions");
                foreach (var stateTranitionTo in transitions)
                {
                    _transitionCache.Add(stateTranitionTo.currentState, new ToState
                                                                            {
                                                                                nextState = stateTranitionTo.nextState,
                                                                                maxSpeedX = stateTranitionTo.currentState.maxSpeedX,
                                                                                maxSpeedY = stateTranitionTo.currentState.maxSpeedY,
                                                                                minSpeedX = stateTranitionTo.currentState.minSpeedX,
                                                                                minSpeedY = stateTranitionTo.currentState.minSpeedY,
                                                                            });
                }
            }
        }

        public RunnerState Transition(InputState action, bool touching, Vector3 currentVelocity)
        {
            touchingPlatform = touching;
            velocity = currentVelocity;
            if (_transitionCache != null)
            {
                var state = new StateTransition
                                {
                                    currentState = currentState,
                                    command = action,
                                    below = touching,
                                    above = false,
                                    behind = false,
                                    front = false,
                                };

                ToState transition;
                var foundTransition = _transitionCache.TryGetValue(state, out transition);
                var allowedTransition =  foundTransition && (currentVelocity.x > transition.minSpeedX) && (transition.maxSpeedX > currentVelocity.x);
                if (allowedTransition)
                {
                    Debug.Log("Transitioning from " + currentState + " to " + transition.nextState);
                    currentState = transition.nextState;
                }
                else
                {
                    if (!foundTransition)
                        Debug.Log("No transition found for - CurrentState: " + currentState + ", Action: " + action + ", Touching: " + touching);
                }
            }
            else
            {
                Debug.LogError("Runner State is not initialized!");
            }
            StateProcessQueue.Enqueue(currentState);
            return currentState;
        }
    }
}