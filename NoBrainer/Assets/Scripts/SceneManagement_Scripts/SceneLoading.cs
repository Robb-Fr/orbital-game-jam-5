using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{
    // Start is called before the first frame update

    public static SceneLoading instance;
void Awake()
    {

        if(instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
            return;
        }

           DontDestroyOnLoad(gameObject);

    }

     


    // Update is called once per frame
    void Update()
    {
        
    }


    public void LoadNextScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }

}
