using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextSceneHitBox : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Invoke("LoadNextScene", 0.3f);

    }



    void LoadNextScene()
    {
        FindObjectOfType<SceneLoading>().LoadNextScene();
    }

}
