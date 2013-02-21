using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class RunnerFSM
    {
        public class RunnerFSMContext
        {
            private readonly RunnerFSM _fsm;
            private readonly List<TransitionInfo> _registeredTransitions = new List<TransitionInfo>();
 
            public RunnerFSMContext(RunnerFSM fsm)
            {
                _fsm = fsm;
            }

            public RunnerFSMContext AddTransition(RunnerState forState, InputState inputState, TransitionInfo transitionInfo)
            {
                _registeredTransitions.Add(transitionInfo);
                _fsm._availableTransitions[forState][inputState].Add(transitionInfo);
                return this;
            }

            public RunnerFSMContext AddTransitions(RunnerState forState, InputState inputState, IEnumerable<TransitionInfo> transitionInfos)
            {
                foreach (var transitionInfo in transitionInfos)
                {
                    AddTransition(forState, inputState, transitionInfo);
                }
                return this;
            }

            public RunnerFSMContext AddTransition(IEnumerable<RunnerState> forStates, InputState inputState, TransitionInfo transitionInfo)
            {
                foreach (var forState in forStates)
                {
                    AddTransition(forState, inputState, transitionInfo);
                }
                return this;
            }

            public void Complete()
            {
                _fsm.allTransitions = _registeredTransitions.ToArray();
            }
        }

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

        private RunnerFSMContext _internalContext;

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

        public RunnerFSMContext Initialize()
        {
            _internalContext = new RunnerFSMContext(this);
            return _internalContext;
        }

        public RunnerState Transition(InputState input, CollisionInfo collisionInfo, Rigidbody rigidbody)
        {
            if (_internalContext != null)
            {
                _internalContext.Complete();
                _internalContext = null;
            }
            Dictionary<InputState, List<TransitionInfo>> availableInputs;
            if (_availableTransitions.TryGetValue(currentState, out availableInputs))
            {
                List<TransitionInfo> inputTransitions;
                if (availableInputs.TryGetValue(input, out inputTransitions))
                {
                    var firstMatchingTransition = inputTransitions
                        .FirstOrDefault(t => t.CollisionRequirements == collisionInfo &&
                                             (t.LastUseTime - Time.time) <= t.ReuseTime // &&
                        );
                }
            }
            return currentState;
        }
    }
}