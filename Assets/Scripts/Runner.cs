using Assets.Scripts;
using UnityEngine;
using System.Collections;

public abstract class Runner : MonoBehaviour {
    private Transform _transform;
    public float gameOverY;
    public RunnerMotor motor = new RunnerMotor();
    public RunnerInput inputController = new KeyboardRunnerInput();
    public RunnerFSM runnerStateMachine = new RunnerFSM();
    public RunnerAnimation runnerAnim = new RunnerAnimation();
    public RunnerEffectEngine runnerEffectEngine = new RunnerEffectEngine();
    public bool UseFixedStep = false;

    private bool touchingPlatform;
    private Rigidbody _rigidBody;
    private Vector3 _startPosition;
    private static float _distanceTraveled;
    private static int _boosts;
    private IAnimatingSprite _sprite;
    

    public static float DistanceTraveled
    {
        get { return _distanceTraveled; }
        set
        {
            _distanceTraveled = value;
            Messenger.Default.Send(new DistanceChangedMessage(_distanceTraveled));
        }
    }

    public static int Boosts
    {
        get { return _boosts; }
        set { 
            _boosts = value;
            Messenger.Default.Send(new BoostChangedMessage(_boosts));
        }
    }

    void Awake()
    {
        _transform = transform;
        _rigidBody = rigidbody;
        Messenger.Default.Register<GameStartMessage>(this, OnGameStart);
        Messenger.Default.Register<GameOverMessage>(this, OnGameOver);
        Messenger.Default.Register<RunnerEventMessage>(this, OnRunnerEvent);
    }

    private void OnRunnerEvent(RunnerEventMessage obj)
    {
        Debug.Log("Runner Event Received: " + obj.Effect);
        runnerEffectEngine.PlayEffect(obj.Effect);
    }

    private void OnGameOver(GameOverMessage obj)
    {
        _rigidBody.isKinematic = true;
        enabled = false;
    }

    private void OnGameStart(GameStartMessage obj)
    {
        Boosts = 0;
        DistanceTraveled = 0f;
        _transform.localPosition = _startPosition;
        _rigidBody.isKinematic = false;
        enabled = true;
    }

    void Start()
    {
        DistanceTraveled = 0f;
        Boosts = 0;
        _startPosition = _transform.localPosition;
        _rigidBody.isKinematic = true;
        _sprite = GetSprite();
        StartCoroutine(runnerStateMachine.Initialize());
        StartCoroutine(runnerEffectEngine.Initialize());
        runnerAnim.Initialize(_sprite);
        OnGameStart(new GameStartMessage());
        Debug.Log("Using Fixed Step " + UseFixedStep);
    }

    protected abstract IAnimatingSprite GetSprite();

    private void ProcessState()
    {
        while (inputController.QueuedStates.Count > 0)
        {
            DistanceTraveled = _transform.localPosition.x;
            var collisionInfo = new CollisionInfo
                                    {
                                        Below = touchingPlatform,
                                    };
            runnerStateMachine.Transition(inputController.QueuedStates.Dequeue(), collisionInfo, _rigidBody);
            runnerAnim.Animate(runnerStateMachine.currentState);
        }
    }

    void Update()
    {
        inputController.Update();
        if (!UseFixedStep)
            ProcessState();
    }
    
    void FixedUpdate()
    {
        if (UseFixedStep)
            ProcessState();
        RunnerState lastState = RunnerState.None;
        while (runnerStateMachine.StateProcessQueue.Count > 0)
        {
            var queuedState = runnerStateMachine.StateProcessQueue.Dequeue();
            if (queuedState != lastState)
                motor.Move(queuedState, _rigidBody);
            lastState = queuedState;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        touchingPlatform = true;
    }

    void OnCollisionStay(Collision collision)
    {
        touchingPlatform = true;
    }

    void OnCollisionExit(Collision collision)
    {
        touchingPlatform = false;
    }

    public static void AddBoost()
    {
        Boosts++;
    }
}

public class RunnerEventMessage
{
    public RunnerEffect Effect;
}