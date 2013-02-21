using Assets.Scripts;
using UnityEngine;
using System.Collections;

public abstract class Runner : MonoBehaviour {
    private Transform _transform;
    public Vector3 jumpVelocity;
    public Vector3 boostVelocity;
    public float gameOverY;
    public float currentSpeed = 0;
    public float time = 1;
    public RunnerMotor motor = new RunnerMotor();
    public RunnerInput inputController = new KeyboardRunnerInput();
    public RunnerFSM runnerStateMachine = new RunnerFSM();
    public float maxSpeed = 20;
    public float acceleration = 5;
    
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
        runnerStateMachine.Initialize();
        motor.Initialize();
        Messenger.Default.Register<GameStartMessage>(this, OnGameStart);
        Messenger.Default.Register<GameOverMessage>(this, OnGameOver);
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
        OnGameStart(new GameStartMessage());
        //StartCoroutine(DoProcessState());
    }

    protected abstract IAnimatingSprite GetSprite();
    private IEnumerator DoProcessState()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            ProcessState();
        }
    }

    private void ProcessState()
    {
        Time.timeScale = time;
        DistanceTraveled = _transform.localPosition.x;
        var collisionInfo = new CollisionInfo
                                {
                                    Below = touchingPlatform,
                                };
        runnerStateMachine.Transition(inputController.state, collisionInfo, _rigidBody);
        switch (runnerStateMachine.currentState)
        {
            case RunnerState.Walking:
                {
                    ChangeAnimation("Walk");
                }
                break;
            case RunnerState.Running:
                {
                    ChangeAnimation("Run");
                }
                break;
            case RunnerState.Jumping:
                {
                    ChangeAnimation("Jump");
                }
                break;
        }
    }

    void Update()
    {
        inputController.Update();
        ProcessState();
    }

    private void ChangeAnimation(string anim)
    {
        if (_sprite.CurrentAnimation != anim)
            _sprite.Play(anim);
    }

    void FixedUpdate()
    {
        currentSpeed = _rigidBody.velocity.x;
        Debug.Log("Process Queue Contains " + runnerStateMachine.StateProcessQueue.Count);
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