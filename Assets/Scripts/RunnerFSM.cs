using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
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

        public RunnerFSM AddTransition(RunnerState forState, InputState inputState, TransitionInfo transitionInfo)
        {
            _availableTransitions[forState][inputState].Add(transitionInfo);
            return this;
        }
        
        public RunnerFSM AddTransitions(RunnerState forState, InputState inputState, IEnumerable<TransitionInfo> transitionInfos)
        {
            foreach (var transitionInfo in transitionInfos)
            {
                _availableTransitions[forState][inputState].Add(transitionInfo);
            }
            return this;
        }
        
        public RunnerFSM AddTransition(IEnumerable<RunnerState> forStates, InputState inputState, TransitionInfo transitionInfo)
        {
            foreach (var forState in forStates)
            {
                AddTransition(forState, inputState, transitionInfo);
            }
            return this;
        }

        public RunnerState Transition(InputState input, CollisionInfo collisionInfo, Rigidbody rigidbody)
        {
            return currentState;
        }
    }
}