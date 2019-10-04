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
            Destroy(this);
        instance = this;
    }

    public void NextSceneLoad()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }


    public void PreviousSceneLoad()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 0) return;
        SceneManager.LoadScene(currentSceneIndex - 1);
        GameManager.instance.StartScene(currentSceneIndex - 1);
    }

}
