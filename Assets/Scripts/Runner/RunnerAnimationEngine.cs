using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

public class RunnerAnimationEngine
{
    private tk2dSpriteAnimator _animator;
    private HashSet<string> _existingAnimations;
    private tk2dSprite _sprite;

    public void Animate(RunnerState runnerState)
    {
        if (_animator == null)
            throw new InvalidOperationException("Animation must be initialized");
        if (_existingAnimations.Contains(runnerState.ToString()))
        {
            ChangeAnimation(runnerState.ToString());
        }
        else
        {
            Debug.LogWarning("No animation defined for " + runnerState);
        }
    }

    private void ChangeAnimation(string anim)
    {
        if ((_animator.CurrentClip != null ? _animator.CurrentClip.name : null) != anim)
        {
            Debug.Log("Playing animation " + anim);
            _animator.Play(anim);
        }
    }

    public void Initialize(tk2dSprite sprite, tk2dSpriteAnimator spriteAnimator)
    {
        _animator = spriteAnimator;
        _sprite = sprite;
        _existingAnimations = new HashSet<string>(_animator.Library.clips.Select(c => c.name).ToArray());
    }
}