using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager instance;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    public void NextSceneLoad()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        GameManager.instance.QuitScene(currentSceneIndex);
        SceneManager.LoadScene(currentSceneIndex + 1, LoadSceneMode.Single);
    }


    public void PreviousSceneLoad()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 0) return;
        GameManager.instance.QuitScene(currentSceneIndex);
        SceneManager.LoadScene(currentSceneIndex - 1, LoadSceneMode.Single);
    }

    public void SceneLoad(int index)
    {
        GameManager.instance.QuitScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(index, LoadSceneMode.Single);
    }

    public void GameSceneLoad(int mapIndex)
    {
        if (SceneManager.GetActiveScene().buildIndex != 2) return;
        GameManager.instance.QuitScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(mapIndex, LoadSceneMode.Additive);
    }

}
