using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    public static AudioManager instance;

    private Queue<AudioSource> activeSongs;

    public bool hasTriggered = false;

    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);


        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

        }

        activeSongs = new Queue<AudioSource>();
    }

    void Start()
    {

        // Play("MainTheme");
        // Debug.Log("START");
    }


    public void Play(string name)
    {

        if (name != "DialogueBlip")
        {
            Debug.Log("name: " + name);

        }

        if (name != "DialogueBlip")
        {
            while (activeSongs.Count != 0)
            {
                AudioSource song = activeSongs.Dequeue();
                Debug.Log("STOP");
                song.Stop();
            }

            activeSongs.Clear();

        }



        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound:" + name + " not found!");
            return;
        }

        s.source.Play();

        if (name != "DialogueBlip")
        {
            activeSongs.Enqueue(s.source);
        }

    }
}
