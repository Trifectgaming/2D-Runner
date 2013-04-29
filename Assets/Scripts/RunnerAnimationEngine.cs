using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

public class RunnerAnimationEngine
{
    private tk2dAnimatedSprite _sprite;
    private HashSet<string> _existingAnimations; 

    public void Initialize(tk2dAnimatedSprite sprite)
    {
        _sprite = sprite;
        _existingAnimations = new HashSet<string>(_sprite.anim.clips.Select(c => c.name).ToArray());
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
        if ((_sprite.CurrentClip != null ? _sprite.CurrentClip.name : null) != anim)
            _sprite.Play(anim);
    }
}