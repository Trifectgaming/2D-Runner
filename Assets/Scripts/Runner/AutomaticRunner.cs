using Assets.Scripts;

public class AutomaticRunner : Runner
{
    protected override System.Action Setup
    {
        get
        {
            return () => SetupDependencies(new RunnerMotor(), new AutomaticRunnerInput(), new RunnerFSM(), new RunnerAnimationEngine(), new RunnerEffectEngine());
        }
    }
}