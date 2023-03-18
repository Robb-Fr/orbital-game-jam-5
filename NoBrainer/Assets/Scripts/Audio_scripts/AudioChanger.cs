using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChanger : MonoBehaviour
{
    public string theme;
    void Start()
    {
        AudioManager.instance.Play(theme);
    }

}
