using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DestroyOnClick : MonoBehaviour
{

    void Start()
    {
        AudioManager.instance.Play("Phone");
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
