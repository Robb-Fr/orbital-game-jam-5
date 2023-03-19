using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioStarter : MonoBehaviour
{

    public string songName;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.Play(songName);
    }


}
