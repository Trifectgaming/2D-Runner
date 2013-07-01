using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using System.Collections;

public abstract class Runner : MonoBehaviour {
    private Transform _transform;
    public float gameOverY;
    public RunnerMotor motor;
    public RunnerInput inputController;
    public RunnerFSM runnerStateMachine;
    public RunnerAnimationEngine runnerAnim;
    public RunnerEffectEngine runnerEffectEngine;    
    public bool UseFixedStep = false;
    public DetectorCollection Detectors = new DetectorCollection();

    private bool touchingPlatform;
    private Rigidbody _rigidBody;
    private Vector3 _startPosition;
    private static float _distanceTraveled;
    private static int _boosts;
    private tk2dSprite _sprite;
    //private CollisionInfo collisionInfo = CollisionInfo.Empty;
    private tk2dSpriteAnimator _spriteAnimator;


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
        SetupDependencies(new RunnerMotor(), new AutomaticRunnerInput(), );
        SetupMessages();
    }

    protected virtual void SetupDependencies(RunnerMotor runnerMotor, RunnerInput controller, RunnerFSM runnerFsm, RunnerAnimationEngine animationEngine, RunnerEffectEngine effectEngine)
    {
        motor = runnerMotor;
        inputController = controller;
        runnerStateMachine = runnerFsm;
        runnerAnim = animationEngine;
        runnerEffectEngine = effectEngine;
    }

    protected virtual void SetupMessages()
    {
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
        _sprite = GetComponentInChildren<tk2dSprite>();
        _spriteAnimator = GetComponentInChildren<tk2dSpriteAnimator>();
        Detectors.Initialize(_transform);
        StartCoroutine(runnerStateMachine.Initialize());
        StartCoroutine(runnerEffectEngine.Initialize());
        runnerAnim.Initialize(_sprite, _spriteAnimator);
        motor.Initialize();
        OnGameStart(new GameStartMessage());
        Debug.Log("Using Fixed Step " + UseFixedStep);
    }
    
    private void ProcessState()
    {
        //while (inputController.QueuedStates.Count > 0)
        {
            DistanceTraveled = _transform.localPosition.x;
            var collisionInfo = UpdateCollisionInfo();
            runnerStateMachine.Transition(inputController, collisionInfo, _rigidBody);
            runnerAnim.Animate(runnerStateMachine.currentState);
        }
    }

    private CollisionInfo UpdateCollisionInfo()
    {
        var bounds = _sprite.GetBounds();
        Detectors.Resize(bounds, _transform);
        var collisionInfo = new CollisionInfo
                                {
                                    Above = Detectors.Top.Colliding,
                                    Below = Detectors.Bottom.Colliding,
                                    Left = Detectors.Back.Colliding,
                                    Right = Detectors.Front.Colliding
                                };
        return collisionInfo;
        //Debug.Log(collisionInfo);
    }

    private bool CollidedWith(Vector3 direction, Vector3 p, Bounds bounds)
    {
        var xMath = Mathf.Abs(direction.y)*bounds.size.x;
        var yMath = Mathf.Abs(direction.x)*bounds.size.y;
        return TestCast(direction, p) ||
            TestCast(direction, new Vector3(p.x + xMath / 3f, p.y + (yMath / 3f), p.z)) ||
            TestCast(direction, new Vector3(p.x - xMath / 3f, p.y - (yMath / 3f), p.z)) ||
            TestCast(direction, new Vector3(p.x + xMath/ 5, p.y + (yMath / 5), p.z)) ||
            TestCast(direction, new Vector3(p.x - xMath/ 5, p.y - (yMath / 5), p.z));
    }

    private static bool TestCast(Vector3 direction, Vector3 position)
    {
        Debug.DrawRay(position, direction, Color.blue);
        return Physics.Raycast(position, direction, .8f);
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
        var lastState = RunnerState.None;
        while (runnerStateMachine.StateProcessQueue.Count > 0)
        {
            var queuedState = runnerStateMachine.StateProcessQueue.Dequeue();
            if (queuedState != lastState)
                motor.Move(queuedState, _rigidBody);
            lastState = queuedState;
        }
    }

    public static void AddBoost()
    {
        Boosts++;
    }
}