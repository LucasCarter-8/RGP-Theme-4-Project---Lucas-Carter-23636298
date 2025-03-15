using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
    }
    public void SetMasterVolume()
    {
        SoundManager.Instance.mainMixer.SetFloat("MasterVolume", 20f * Mathf.Log10(masterSlider.value));
    }

    public void SetBGMVolume()
    {
        SoundManager.Instance.mainMixer.SetFloat("BGMVolume", 20f * Mathf.Log10(bgmSlider.value));
    }

    public void SetSFXVolume()
    {
        SoundManager.Instance.mainMixer.SetFloat("SFXVolume", 20f * Mathf.Log10(sfxSlider.value));
    }
}
