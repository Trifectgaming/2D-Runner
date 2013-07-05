public class TK2DRunner : Runner
{
    private static float _distanceTraveled;

    protected override System.Action Setup
    {
        get { return base.Setup; }
    }

    public static float DistanceTraveled
    {
        get { return _distanceTraveled; }
        set
        {
            _distanceTraveled = value;
            Messenger.Default.Send(new DistanceChangedMessage(_distanceTraveled));
        }
    }

    protected override void OnGameStart(GameStartMessage obj)
    {
        base.OnGameStart(obj);
        DistanceTraveled = 0f;
    }

    protected override void Start()
    {
        base.Start();
        DistanceTraveled = 0f;
        runnerStateMachine.reportState = true;
    }

    protected override void ProcessState()
    {
        DistanceTraveled = Transform.position.x;
        if (inputController.state.attempts == InputWrapper.AttempCounter)
            LevelRecording.Enqueue(new InputRecord(Transform.position, inputController.state.input));
        base.ProcessState();
    }
}