using System;
using Assets.Scripts;

public class RunnerAnimation
{
    private IAnimatingSprite _sprite;

    public void Initialize(IAnimatingSprite sprite)
    {
        _sprite = sprite;
    }

    public void Animate(RunnerState runnerState)
    {
        if (_sprite == null)
            throw new InvalidOperationException("Animation must be initialized");
        switch (runnerState)
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

    private void ChangeAnimation(string anim)
    {
        if (_sprite.CurrentAnimation != anim)
            _sprite.Play(anim);
    }
}