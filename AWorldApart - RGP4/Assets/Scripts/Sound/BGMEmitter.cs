using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public enum BGMType { None, Game, MainMenu }
public class BGMEmitter : MonoBehaviour
{
    [SerializeField] private Dictionary<BGMType, AudioSource> audioSources;
    [SerializeField] private BGM[] BGMList;
    [SerializeField] private BGMType initialBGM;
    // Start is called before the first frame update
    private void Start()
    {
        audioSources = new Dictionary<BGMType, AudioSource>();
        foreach (BGM bgm in BGMList)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSources.Add(bgm.Type, audioSource);
            audioSource.outputAudioMixerGroup = SoundManager.Instance.BGM;
            audioSource.clip = bgm.Clip;
            audioSource.loop = true;
        }

        PlayBGM(initialBGM);
    }

    public void PlayBGM(BGMType bGMType)
    {
        if (bGMType == BGMType.None)
        {
            return;
        }
        if (audioSources.TryGetValue(bGMType, out AudioSource source))
        {
            if (source.isPlaying)
            {
                return;
            }
            source.Play();
        }
    }
    public void Stop(BGMType bGMType)
    {
        if (audioSources.TryGetValue(bGMType, out AudioSource source) && source.isPlaying)
        {
            source.Stop();
        }
    }
}