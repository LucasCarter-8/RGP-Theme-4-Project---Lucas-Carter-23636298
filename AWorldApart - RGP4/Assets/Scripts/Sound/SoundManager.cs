using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class SoundEffect
{
    public AudioClip Clip;
    public SoundEffectType Type;
}

[Serializable]
public class BGM
{
    public AudioClip Clip;
    public BGMType Type;
}
public class SoundManager : Singleton<SoundManager>
{
    public AudioMixer mainMixer;
    public AudioMixerGroup SFX;
    public AudioMixerGroup BGM;

    protected override void Awake()
    {
        base.Awake();
        SoundManager.StartSingleton();
    }

}