using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using System.Collections;

public abstract class Runner : MonoBehaviour {
    public float gameOverY;
    public RunnerMotor motor;
    public RunnerInput inputController;
    public RunnerFSM runnerStateMachine;
    public RunnerAnimationEngine runnerAnim;
    public RunnerEffectEngine runnerEffectEngine;    
    public bool UseFixedStep = false;
    public DetectorCollection Detectors = new DetectorCollection();

    protected Transform Transform;
    protected Rigidbody RigidBody;
    protected tk2dSpriteAnimator SpriteAnimator;
    protected tk2dSprite Sprite;

    private Vector3 _startPosition;
    private static int _boosts;


    public static int Boosts
    {
        get { return _boosts; }
        set { 
            _boosts = value;
            Messenger.Default.Send(new BoostChangedMessage(_boosts));
        }
    }

    protected virtual Action Setup
    {
        get
        {
            return () =>
                   SetupDependencies(
                       new RunnerMotor(),
                       new KeyboardRunnerInput(),
                       new RunnerFSM(),
                       new RunnerAnimationEngine(),
                       new RunnerEffectEngine());
        }
    }

    void Awake()
    {
        Transform = transform;
        RigidBody = rigidbody;
        Setup();
        SetupMessages();
    }

    protected void SetupDependencies(RunnerMotor runnerMotor, RunnerInput controller, RunnerFSM runnerFsm, RunnerAnimationEngine animationEngine, RunnerEffectEngine effectEngine)
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
        RigidBody.isKinematic = true;
        enabled = false;
    }

    protected virtual void OnGameStart(GameStartMessage obj)
    {
        Boosts = 0;
        Transform.localPosition = _startPosition;
        RigidBody.isKinematic = false;
        enabled = true;
    }

    protected virtual void Start()
    {
        Boosts = 0;
        _startPosition = Transform.localPosition;
        RigidBody.isKinematic = true;
        Sprite = GetComponentInChildren<tk2dSprite>();
        SpriteAnimator = GetComponentInChildren<tk2dSpriteAnimator>();
        Detectors.Initialize(Transform);
        StartCoroutine(runnerStateMachine.Initialize());
        StartCoroutine(runnerEffectEngine.Initialize());
        runnerAnim.Initialize(Sprite, SpriteAnimator);
        motor.Initialize();
        OnGameStart(new GameStartMessage());
        Debug.Log("Using Fixed Step " + UseFixedStep);
    }

    protected virtual void ProcessState()
    {
        var collisionInfo = UpdateCollisionInfo();
        runnerStateMachine.Transition(inputController, collisionInfo, RigidBody);
        runnerAnim.Animate(runnerStateMachine.currentState);
    }

    private CollisionInfo UpdateCollisionInfo()
    {
        var bounds = Sprite.GetBounds();
        Detectors.Resize(bounds, Transform);
        var collisionInfo = new CollisionInfo
                                {
                                    Above = Detectors.Top.Colliding,
                                    Below = Detectors.Bottom.Colliding,
                                    Left = Detectors.Back.Colliding,
                                    Right = Detectors.Front.Colliding
                                };
        return collisionInfo;
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
                motor.Move(queuedState, RigidBody);
            lastState = queuedState;
        }
    }

    public static void AddBoost()
    {
        Boosts++;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
    }

    protected virtual void OnTriggerExit(Collider other)
    {
    }
}