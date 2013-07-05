using Assets.Scripts;

public class AutomaticRunner : Runner
{
    protected override System.Action Setup
    {
        get
        {
            return () => SetupDependencies(new RunnerMotor(), new AutomaticRunnerInput(), new RunnerFSM(this), new RunnerAnimationEngine(), new RunnerEffectEngine());
        }
    }
}