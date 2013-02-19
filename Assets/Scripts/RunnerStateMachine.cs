using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public struct CollisionInfo
    {
        public static readonly CollisionInfo Empty = new CollisionInfo();
        public bool Above;
        public bool Below;
        public bool Left;
        public bool Right;
    }

    [Serializable]
    public struct SpeedInfo
    {
        public static readonly SpeedInfo Empty = new SpeedInfo();
        public float? minX;
        public float? minY;
        public float? maxX;
        public float? maxY;
    }

    [Serializable]
    public class RunnerFSM
    {
        public RunnerState currentState = RunnerState.None;
        public Vector3 velocity;
        public CollisionInfo contacts;
        public string lastTransition;
        public float lastTransitionChange;
        public float transitionExpirationTime;
        public TransitionInfo[] allTransitions;
        public Queue<RunnerState> StateProcessQueue = new Queue<RunnerState>();

        private readonly Dictionary<RunnerState, Dictionary<InputState, List<TransitionInfo>>> _availableTransitions =
            new Dictionary<RunnerState, Dictionary<InputState, List<TransitionInfo>>>();

        public RunnerFSM()
        {
            var runnerStates = Enum.GetValues(typeof(RunnerState)).Cast<RunnerState>().ToArray();
            var inputStates = Enum.GetValues(typeof(InputState)).Cast<InputState>().ToArray();
            foreach (var runnerState in runnerStates)
            {
                var runnersActions = new Dictionary<InputState, List<TransitionInfo>>();
                _availableTransitions.Add(runnerState, runnersActions);
                foreach (var inputState in inputStates)
                {
                    var actionTransitions = new List<TransitionInfo>();
                    runnersActions.Add(inputState, actionTransitions);
                }
            }
        }

        public RunnerFSM AddTransition(RunnerState forState, InputState inputState)
        {

            return this;
        }

        public RunnerState Transition(InputState input, CollisionInfo collisionInfo, Rigidbody rigidbody)
        {
            return currentState;
        }
    }

    [Serializable]
    public class TransitionInfo
    {
        public string TransitionName;
        public CollisionInfo CollisionRequirements;
        public SpeedInfo VelocityRequirements;
        public float MinTransitionTime;
    }


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