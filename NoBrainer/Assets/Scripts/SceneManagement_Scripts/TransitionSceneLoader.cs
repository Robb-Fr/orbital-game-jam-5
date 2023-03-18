using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionSceneLoader : MonoBehaviour
{
    // Start is called before the first frame update

    public float waitingTime = 10f;
    public bool useSpace = false;

    void Start()
    {
        if (!useSpace)
        {
            Invoke("LoadNextScene", waitingTime);

        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadNextScene();
        }
    }


    void LoadNextScene()
    {
        FindObjectOfType<SceneLoading>().LoadNextScene();
    }
}
