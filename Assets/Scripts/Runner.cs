using Assets.Scripts;
using UnityEngine;
using System.Collections;

public abstract class Runner : MonoBehaviour {
    private Transform _transform;
    public float acceleration;
    public float maxSpeed;
    public Vector3 jumpVelocity;
    public Vector3 boostVelocity;
    public float gameOverY;
    public float currentSpeed = 0;
    public float runSpeed = 1.8f;
    public float time = 1;

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
        //renderer.enabled = true;
        _rigidBody.isKinematic = false;
        enabled = true;
    }

    // Use this for initialization
    private void Start()
    {
        DistanceTraveled = 0f;
        Boosts = 0;
        _startPosition = _transform.localPosition;
        _rigidBody.isKinematic = true;
        _sprite = GetSprite();
        OnGameStart(new GameStartMessage());
        //renderer.enabled = false;
    }

    protected abstract IAnimatingSprite GetSprite();

    // Update is called once per frame
	void Update ()
	{
	    Time.timeScale = time;

	    if (Input.GetButtonDown("Jump") || touchingPlatform == false)
	    {
            if (_sprite.CurrentAnimation != "Jump")
                _sprite.Play("Jump");
	    }

	    if (Input.GetButtonDown("Jump"))
	    {
	        if (touchingPlatform)
	        {
	            _rigidBody.AddForce(jumpVelocity, ForceMode.VelocityChange);
	        }
            else if (Boosts > 0)
            {
                _rigidBody.AddForce(boostVelocity, ForceMode.VelocityChange);
                Boosts--;
            }
            
	    }
	    else
	    {
	        if (touchingPlatform)
	        {
	            if (currentSpeed >= runSpeed && _sprite.CurrentAnimation != "Run")
	            {
	                _sprite.Play("Run");
	            }
	            else if (currentSpeed < runSpeed && _sprite.CurrentAnimation != "Walk")
	            {
	                _sprite.Play("Walk");
	            }
	        }
	    }
	    
        DistanceTraveled = transform.localPosition.x;
        //Debug.Log("Distance Traveled " + distanceTraveled);

        if (transform.localPosition.y < gameOverY)
        {
            Messenger.Default.Send(new GameOverMessage());
        }
	}

    void FixedUpdate()
    {
        currentSpeed = _rigidBody.velocity.x;
        if (touchingPlatform && currentSpeed < maxSpeed)
        {
            _rigidBody.AddForce(acceleration, 0f, 0f, ForceMode.Force);
        }
        if (touchingPlatform && currentSpeed > maxSpeed)
        {
            _rigidBody.AddForce(-acceleration, 0f, 0f, ForceMode.Force);
        }
    }

    void OnCollisionEnter()
    {
        touchingPlatform = true;
    }

    void OnCollisionExit()
    {
        touchingPlatform = false;
    }

    public static void AddBoost()
    {
        Boosts++;
    }
}