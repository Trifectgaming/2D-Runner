using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class RunnerFSM
    {
        private readonly Runner _parent;

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
        public float lastUseTime;
        public TransitionInfo[] allTransitions;
        public bool reportState;
        public Queue<RunnerState> StateProcessQueue = new Queue<RunnerState>();

        private readonly Dictionary<RunnerState, Dictionary<InputState, List<TransitionInfo>>> _availableTransitions =
            new Dictionary<RunnerState, Dictionary<InputState, List<TransitionInfo>>>();
        private readonly Queue<TransitionInfo> _rechargingStates = new Queue<TransitionInfo>(); 
        public RunnerFSM(Runner parent)
        {
            _parent = parent;
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
        }

        public IEnumerator Initialize()
        {
            new RunnerFSMContext(this)
                .AddTransition(new[]
                                   {
                                       RunnerState.Walk, 
                                       RunnerState.Run,
                                       RunnerState.SlideStart,
                                       RunnerState.Sliding,
                                       RunnerState.SlideEnd,
                                       RunnerState.Dash,
                                       RunnerState.GroundDash,
                                   }, InputState.None,
                                   new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                       {
                                           Right = true,
                                       },
                                       NextState = RunnerState.None
                                   })
                .AddTransition(new[]
                                   {
                                       RunnerState.Falling,
                                   }, InputState.None,
                                   new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                       {
                                           Right = true,
                                           Below = true,
                                       },
                                       NextState = RunnerState.None
                                   })
                .AddTransition(new[]
                                   {
                                       RunnerState.Jumped,
                                       RunnerState.Walk,
                                       RunnerState.Falling,
                                       RunnerState.None,
                                       RunnerState.Run,
                                       RunnerState.Dropping,
                                   }, InputState.None,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = true,
                                                                       Right = false
                                                                   },
                                       VelocityRequirements = new SpeedInfo
                                                                  {
                                                                      minX = 3.0f,
                                                                  },
                                       NextState = RunnerState.Walk
                                   })
                
                .AddTransition(new[]
                                   {
                                       RunnerState.Jumped,
                                       RunnerState.Walk,
                                       RunnerState.Falling,
                                       RunnerState.None,
                                       RunnerState.Dropping,
                                       RunnerState.Run,
                                   }, InputState.None,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = true,
                                                                       Right = false
                                                                   },
                                       VelocityRequirements = new SpeedInfo
                                                                  {
                                                                      maxX = 3.0f,
                                                                  },
                                       NextState = RunnerState.Run,
                                   })
                .AddTransition(new[]
                                   {
                                       RunnerState.Run,
                                       RunnerState.Walk,
                                       RunnerState.None, 
                                   }, InputState.SwipeUp,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = true,
                                                                   },
                                       NextState = RunnerState.Jump,
                                   })
                .AddTransition(RunnerState.Jump, InputState.None,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = false,
                                                                       Above = false,
                                                                   },
                                       VelocityRequirements = new SpeedInfo
                                                                  {
                                                                      maxY = 0f
                                                                  },
                                       NextState = RunnerState.Jumped,
                                   })
                .AddTransition(RunnerState.Jumped, InputState.None,
                               new TransitionInfo
                               {
                                   CollisionRequirements = new CollisionInfo
                                   {
                                       Right = true,
                                   },
                                   NextState = RunnerState.Falling,
                               })
                .AddTransition(RunnerState.Jump, InputState.None,
                               new TransitionInfo
                               {
                                   CollisionRequirements = new CollisionInfo
                                   {
                                       Above = true,
                                   },
                                   NextState = RunnerState.Falling,
                               })
                .AddTransition(new[]
                                   {
                                       RunnerState.Jumped,
                                       RunnerState.Falling,
                                       RunnerState.Dash,
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
                                       RunnerState.Jump, 
                                       RunnerState.Falling
                                   }, InputState.SwipeRight,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = false,
                                                                   },
                                       ReuseTime = 3,
                                       NextState = RunnerState.Dash
                                   })
                .AddTransition(new[]
                                   {
                                       RunnerState.Run,
                                       RunnerState.Walk
                                   }, InputState.SwipeRight,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = true,
                                                                   },

                                       ReuseTime = 4,
                                       HasRechargeEffect = true,
                                       HasTransitionEffect = true,
                                       NextState = RunnerState.GroundDash
                                   })
                .AddTransition(new[]
                                   {
                                       RunnerState.Run,
                                       RunnerState.Walk,
                                       RunnerState.None, 
                                       RunnerState.SlideEnd
                                   }, InputState.SwipeDown,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = true,
                                                                   },
                                       NextState = RunnerState.SlideStart
                                   })
                .AddTransition(new[]
                                   {
                                       RunnerState.SlideStart,
                                   }, InputState.None,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = true,
                                                                   },
                                       DelayTime = .2f,
                                       NextState = RunnerState.Sliding
                                   })
                .AddTransition(new[]
                                   {
                                       RunnerState.Sliding,
                                       
                                   }, InputState.SwipeDown,
                                   new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                       {
                                           Below = true,
                                       },
                                       NextState = RunnerState.Sliding
                                   })
                .AddTransition(new[]
                                   {
                                       RunnerState.Sliding
                                   }, InputState.None,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = true,
                                                                       Above = false,
                                                                   },
                                       DelayTime = .3f,
                                       NextState = RunnerState.SlideEnd
                                   })
               .AddTransition(new[]
                                   {
                                       RunnerState.SlideEnd,
                                       RunnerState.SlideStart, 
                                       RunnerState.Sliding, 
                                   }, InputState.SwipeUp,
                               new TransitionInfo
                               {
                                   CollisionRequirements = new CollisionInfo
                                   {
                                       Below = true,
                                       Above = false,
                                   },
                                   NextState = RunnerState.Jump
                               })
                .AddTransition(new[]
                                   {
                                       RunnerState.SlideEnd
                                   }, InputState.None,
                               new TransitionInfo
                                   {
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = true,
                                                                       
                                                                   },
                                       DelayTime = .3f,
                                       NextState = RunnerState.Run
                                   })
                .AddTransition(new[]
                                   {
                                       RunnerState.GroundDash,
                                   }, InputState.None,
                               new TransitionInfo
                                   {
                                       DelayTime = .2f,
                                       CollisionRequirements = new CollisionInfo
                                                                   {
                                                                       Below = true,
                                                                   },
                                       NextState = RunnerState.Run
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

            while (true)
            {
                ScanForRecharge();
                yield return new WaitForSeconds(.5f);
            }
        }

        private void ScanForRecharge()
        {
            var notReady = new List<TransitionInfo>();
            while (_rechargingStates.Count > 0)
            {
                var toTest = _rechargingStates.Dequeue();
                if (!IsTransitionReady(toTest))
                {
                    notReady.Add(toTest);
                }
                else
                {
                    if (toTest.HasRechargeEffect)
                        TrySendEventMessage(toTest.NextState + "Recharge");
                }
            }
            foreach (var transitionInfo in notReady)
            {
                _rechargingStates.Enqueue(transitionInfo);
            }
        }

        private void TrySendEventMessage(string effect)
        {
            if (!string.IsNullOrEmpty(effect))
            {
                _parent.runnerEffectEngine.PlayEffect(effect);
            }
        }

        public RunnerState Transition(RunnerInput input, CollisionInfo collisionInfo, Rigidbody rigidbody)
        {
            velocity = rigidbody.velocity;
            contacts = collisionInfo;
            var inputToTry = input.Try();
            Dictionary<InputState, List<TransitionInfo>> availableInputs;
            if (_availableTransitions.TryGetValue(currentState, out availableInputs))
            {
                List<TransitionInfo> inputTransitions;
                if (availableInputs.TryGetValue(inputToTry, out inputTransitions))
                {
                    var firstMatchingTransition = inputTransitions
                        .FirstOrDefault(t => (t.CollisionRequirements ?? CollisionInfo.Empty).Valid(collisionInfo) &&
                                             IsTransitionReady(t) &&
                                             IsDelayUp(t) &&
                                             t.VelocityRequirements.Equals(rigidbody.velocity));
                    if (firstMatchingTransition != null)
                    {
                        lastTransition = currentState + "To" + firstMatchingTransition.NextState;
                        currentState = firstMatchingTransition.NextState;
                        chosenTransition = firstMatchingTransition.TransitionName;
                        lastUseTime = Time.time;
                        firstMatchingTransition.Use();
                        if (firstMatchingTransition.ReuseTime > 0)
                        {
                            Debug.Log("Queuing for effect: " + firstMatchingTransition.NextState);
                            _rechargingStates.Enqueue(firstMatchingTransition);
                        }
                        if (firstMatchingTransition.HasTransitionEffect)
                            TrySendEventMessage(firstMatchingTransition.NextState.ToString());
                        input.Complete();
                    }
                }
            }

            StateProcessQueue.Enqueue(currentState);
            if (reportState)
                Messenger.Default.Send(new RunnerStatsMessage(currentState, velocity, collisionInfo, inputToTry));
            return currentState;
        }

        private bool IsDelayUp(TransitionInfo transitionInfo)
        {
            if (transitionInfo.DelayTime.HasValue)
            {
                var delay = (lastUseTime + transitionInfo.DelayTime.Value);
                var delayUp =  delay <= Time.time;
                return delayUp;
            }
            return true;
        }

        private static bool IsTransitionReady(TransitionInfo t)
        {
            return (t.LastUseTime - Time.time) <= 0;
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