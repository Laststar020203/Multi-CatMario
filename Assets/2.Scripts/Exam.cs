using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Exam : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("PLAYER"))
        {

            if (ClientGameSystem.instance.end)
            {
                SceneManager.UnloadSceneAsync(3);
                SceneManager.UnloadSceneAsync(4);

                ClientGameSystem.instance.end = false;
            }
        }
            

    }
}
