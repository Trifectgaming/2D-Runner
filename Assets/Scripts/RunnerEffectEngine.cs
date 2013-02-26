using System;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using System.Collections;

public class RunnerEffectEngine
{
    private readonly Dictionary<string, EffectInfo> _effects;
    private bool _initialized;

    public RunnerEffectEngine()
    {
        _effects = new Dictionary<string, EffectInfo>();
        foreach (var effectInfo in StateMaster.AllRunnerStates)
        {
            _effects.Add(effectInfo.ToString(), new EffectInfo());
            _effects.Add(effectInfo.ToString()+"Recharge", new EffectInfo());
        }
    }

    public IEnumerator Initialize()
    {
        _initialized = true;
        foreach (var effectDesc in _effects)
        {
            var effect = GameObject.Find(effectDesc.Key + "Effect");
            if (effect == null)
            {
                Debug.LogWarning("Missing Effect for " + effectDesc.Key);
            }
            else
            {
                effectDesc.Value.effect = effect.GetComponent<ParticleSystem>();
                if (!effectDesc.Value.effect.loop)
                {
                    effectDesc.Value.duration = effectDesc.Value.effect.duration;
                }
            }
        }
        while (true)
        {
            foreach (var effectInfo in _effects)
            {
                effectInfo.Value.Expire();
            }
            yield return new WaitForSeconds(.5f);
        }
    }
    
    public void PlayEffect(string effectName)
    {
        if (!_initialized) throw new InvalidOperationException("Effect Engine must be initialized before use.");
        EffectInfo effect;
        if (_effects.TryGetValue(effectName, out effect))
            effect.Play();
    }
}

public enum RunnerEffect
{
    None,
    DashRecharge,
    Dash,
    GroundDashRecharge,
    GroundDash,
}

public class EffectInfo
{
    public ParticleSystem effect;
    public float duration;
    public float expirationTime;

    public void Expire()
    {
        if (expirationTime >= Time.time && effect != null && effect.isPlaying)
        {
            //Debug.Log("Stoping Effect: " + effect.name);
            //effect.Stop();
        }
    }

    public void Play()
    {
        if (effect != null)
        {
            expirationTime = Time.time + duration;
            Debug.Log("Playing Effect: " + effect.name);
            effect.Play(true);
        }
    }
}