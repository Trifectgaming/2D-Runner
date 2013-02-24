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
                if (transitionInfo.TransitionName == null)
                    transitionInfo.TransitionName = forState + "To" + transitionInfo.NextState;
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
                if (transitionInfo.TransitionName == null)
                {
                    transitionInfo.TransitionName = GetStartName(forStates) + "To" + transitionInfo.NextState;
                }
                foreach (var forState in forStates)
                {
                    AddTransition(forState, inputState, transitionInfo);
                }
                return this;
            }

            private string GetStartName(IEnumerable<RunnerState> forStates)
            {
                return "[" + string.Join(",", forStates.Select(s => s.ToString()).ToArray()) + "]";
            }

            public void Complete()
            {
                _fsm.allTransitions = _registeredTransitions.ToArray();
            }
        }

        public RunnerState currentState = RunnerState.None;
        public Vector3 velocity;
        public CollisionInfo contacts;
        public string chosenTransition;
        public string lastTransition;
        public float lastTransitionChange;
        public float transitionExpirationTime;
        [NonSerialized]
        public TransitionInfo[] allTransitions;
        public Queue<RunnerState> StateProcessQueue = new Queue<RunnerState>();

        private readonly Dictionary<RunnerState, Dictionary<InputState, List<TransitionInfo>>> _availableTransitions =
            new Dictionary<RunnerState, Dictionary<InputState, List<TransitionInfo>>>();

        private RunnerFSMContext _internalContext;

        public RunnerFSM()
        {
            foreach (var runnerState in StateMaster.AllRunnerStates)
            {
                var runnersActions = new Dictionary<InputState, List<TransitionInfo>>();
                _availableTransitions.Add(runnerState, runnersActions);
                foreach (var inputState in StateMaster.AllInputStates)
                {
                    var actionTransitions = new List<TransitionInfo>();
                    runnersActions.Add(inputState, actionTransitions);
                }
            }
            Initialize()
                .AddTransition(new[]
                                   {
                                       RunnerState.Jumped,
                                       RunnerState.Walking,
                                       RunnerState.Falling,
                                       RunnerState.None,
                                       RunnerState.Running,
                                       RunnerState.Dropping,
                                   }, InputState.None,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = true,
                                                                   },
                                       VelocityRequirements = new SpeedInfo
                                                                  {
                                                                      minX = 7.0f,
                                                                  },
                                       NextState = RunnerState.Walking
                                   })
                .AddTransition(new[]
                                   {
                                       RunnerState.Jumped,
                                       RunnerState.Walking,
                                       RunnerState.Falling,
                                       RunnerState.None,
                                       RunnerState.Dropping,
                                       RunnerState.Running,
                                   }, InputState.None,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = true,
                                                                   },
                                       VelocityRequirements = new SpeedInfo
                                                                  {
                                                                      maxX = 8.0f,
                                                                  },
                                       NextState = RunnerState.Running,
                                   })
                .AddTransition(RunnerState.Running, InputState.SwipeUp,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = true,
                                                                   },
                                       NextState = RunnerState.Jumping,
                                   })
                .AddTransition(RunnerState.Walking, InputState.SwipeUp,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = true,
                                                                   },
                                       NextState = RunnerState.Jumping,
                                   })
                .AddTransition(RunnerState.Jumping, InputState.None,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = false,
                                                                   },
                                       NextState = RunnerState.Jumped,
                                   })
                .AddTransition(new[]
                                   {
                                       RunnerState.Jumped,
                                       RunnerState.Falling
                                   }, InputState.SwipeDown,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = false,
                                                                   },
                                       NextState = RunnerState.Dropping
                                   })
                .AddTransition(new[]
                                   {
                                       RunnerState.Jumped,
                                       RunnerState.Falling
                                   }, InputState.SwipeRight,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = false,
                                                                   },
                                       NextState = RunnerState.Dash
                                   })
                .AddTransition(StateMaster.AllRunnerStates, InputState.None,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = false,
                                                                   },
                                       VelocityRequirements = new SpeedInfo
                                                                  {
                                                                      minY = -1,
                                                                  },
                                       NextState = RunnerState.Falling,
                                   })
                .Complete();
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
            velocity = rigidbody.velocity;
            contacts = collisionInfo;
            Dictionary<InputState, List<TransitionInfo>> availableInputs;
            if (_availableTransitions.TryGetValue(currentState, out availableInputs))
            {
                List<TransitionInfo> inputTransitions;
                if (availableInputs.TryGetValue(input, out inputTransitions))
                {
                    var firstMatchingTransition = inputTransitions
                        .FirstOrDefault(t => (t.CollisionRequirements ?? CollisionInfo.Empty).Equals(collisionInfo) &&
                                             (t.LastUseTime - Time.time) <= t.ReuseTime &&
                                             t.VelocityRequirements.Equals(rigidbody.velocity));
                    if (firstMatchingTransition != null)
                    {
                        lastTransition = currentState + "To" + firstMatchingTransition.NextState;
                        currentState = firstMatchingTransition.NextState;
                        chosenTransition = firstMatchingTransition.TransitionName;
                        lastTransitionChange = Time.time;
                        firstMatchingTransition.LastUseTime = transitionExpirationTime = Time.time + firstMatchingTransition.ReuseTime;
                    }
                }
            }
            StateProcessQueue.Enqueue(currentState);
            return currentState;
        }
    }

    public class StateMaster
    {
        public static RunnerState[] AllRunnerStates;
        public static InputState[] AllInputStates;

        static StateMaster()
        {
            AllRunnerStates = Enum.GetValues(typeof(RunnerState)).Cast<RunnerState>().ToArray();
            AllInputStates = Enum.GetValues(typeof(InputState)).Cast<InputState>().ToArray();
        }
    }
}