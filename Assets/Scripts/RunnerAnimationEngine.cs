using System;
using System.Collections.Generic;
using Assets.Scripts;

public class RunnerAnimationEngine
{
    private IAnimatingSprite _sprite;
    private HashSet<string> _existingAnimations; 

    public void Initialize(IAnimatingSprite sprite)
    {
        _sprite = sprite;
        _existingAnimations = new HashSet<string>(_sprite.AvailableAnimations);
    }

    public void Animate(RunnerState runnerState)
    {
        if (_sprite == null)
            throw new InvalidOperationException("Animation must be initialized");
        if (_existingAnimations.Contains(runnerState.ToString()))
            ChangeAnimation(runnerState.ToString());
    }

    private void ChangeAnimation(string anim)
    {
        if (_sprite.CurrentAnimation != anim)
            _sprite.Play(anim);
    }
}