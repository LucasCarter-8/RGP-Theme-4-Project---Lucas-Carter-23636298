using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffectType { None, PlayerWalk, PlayerJump, UIHover, UIClick, Place, Collect, Slide, Death }
public class SFXEmitter : MonoBehaviour
{
    [SerializeField] private Dictionary<SoundEffectType, AudioSource> audioSources;
    [SerializeField] private SoundEffect[] effects;
    // Start is called before the first frame update
    private void Start()
    {
        audioSources = new Dictionary<SoundEffectType, AudioSource>();
        foreach (SoundEffect soundEffect in effects)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSources.Add(soundEffect.Type, audioSource);
            audioSource.outputAudioMixerGroup = SoundManager.Instance.SFX;
            audioSource.clip = soundEffect.Clip;
        }
    }

    public void Play(SoundEffectType soundEffect)
    {
        if (soundEffect == SoundEffectType.None)
        {
            return;
        }
        if (audioSources.TryGetValue(soundEffect, out AudioSource source))
        {
            if (source.isPlaying)
            {
                return;
            }
            source.Play();
        }
    }

    public void PlayOverlap(SoundEffectType soundEffect)
    {
        if (soundEffect == SoundEffectType.None)
        {
            return;
        }
        if (audioSources.TryGetValue(soundEffect, out AudioSource source))
        {
            source.Play();
        }
    }
    public void Stop(SoundEffectType soundEffect)
    {
        if (audioSources.TryGetValue(soundEffect, out AudioSource source) && source.isPlaying)
        {
            source.Stop();
        }
    }
}