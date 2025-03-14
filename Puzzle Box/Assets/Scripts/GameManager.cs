using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Sprite finishedLevel;
    [SerializeField] public SpriteRenderer finishPointRenderer;
    public void LoadNextLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        Debug.Log(SceneManager.sceneCountInBuildSettings);
        if (currentLevel < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(currentLevel + 1);
        }
    }

    public IEnumerator LoadNextLevel(float delay)
    {
        finishPointRenderer.sprite = finishedLevel;
        yield return new WaitForSeconds(delay);
        LoadNextLevel();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public IEnumerator ReloadLevel(float delay)
    {
        finishPointRenderer.sprite = finishedLevel;
        yield return new WaitForSeconds(delay);
        ReloadLevel();
    }

}
