using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using System.Collections;

public abstract class Runner : MonoBehaviour {
    private Transform _transform;
    public float gameOverY;
    public RunnerMotor motor = new RunnerMotor();
    public RunnerInput inputController = new KeyboardRunnerInput();
    public RunnerFSM runnerStateMachine = new RunnerFSM();
    public RunnerAnimationEngine runnerAnim = new RunnerAnimationEngine();
    public RunnerEffectEngine runnerEffectEngine = new RunnerEffectEngine();
    public bool UseFixedStep = false;

    private bool touchingPlatform;
    private Rigidbody _rigidBody;
    private Vector3 _startPosition;
    private static float _distanceTraveled;
    private static int _boosts;
    private tk2dAnimatedSprite _sprite;
    public CollisionInfo collisionInfo = CollisionInfo.Empty;


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
        _sprite = GetComponentInChildren<tk2dAnimatedSprite>();
        StartCoroutine(runnerStateMachine.Initialize());
        StartCoroutine(runnerEffectEngine.Initialize());
        runnerAnim.Initialize(_sprite);
        motor.Initialize();
        OnGameStart(new GameStartMessage());
        Debug.Log("Using Fixed Step " + UseFixedStep);
    }
    
    private void ProcessState()
    {
        while (inputController.QueuedStates.Count > 0)
        {
            DistanceTraveled = _transform.localPosition.x;
            UpdateCollisionInfo();
            runnerStateMachine.Transition(inputController.QueuedStates.Dequeue(), collisionInfo, _rigidBody);
            runnerAnim.Animate(runnerStateMachine.currentState);
        }
    }

    private void UpdateCollisionInfo()
    {
        var bounds = _sprite.GetBounds();
        var position = _transform.position;
        collisionInfo.Above = CollidedWith(Vector3.up, position, bounds);
        collisionInfo.Below = CollidedWith(Vector3.down, position, bounds);
        collisionInfo.Left = CollidedWith(Vector3.left, position, bounds);
        collisionInfo.Right = CollidedWith(Vector3.right, position, bounds);
        Debug.Log(collisionInfo);
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

    //private void OnDrawGizmos()
    //{
    //    var bounds = (GetComponentInChildren<tk2dAnimatedSprite>()).GetBounds();
    //    var position = (transform).position;
    //    CollidedWith(Vector3.up, position, bounds);
    //    CollidedWith(Vector3.down, position, bounds);
    //    CollidedWith(Vector3.left, position, bounds);
    //    CollidedWith(Vector3.right, position, bounds);
    //}

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

    //void OnCollisionEnter(Collision collision)
    //{
    //    touchingPlatform = true;
    //}

    //void OnCollisionStay(Collision collision)
    //{
    //    touchingPlatform = true;
    //}

    //void OnCollisionExit(Collision collision)
    //{
    //    touchingPlatform = false;
    //}

    public static void AddBoost()
    {
        Boosts++;
    }
}

public class RunnerEventMessage
{
    public string Effect;
    public string Audio;
}