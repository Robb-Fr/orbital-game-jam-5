using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    // Start is called before the first frame update

    public string dialoguePath;

    public string skipDialoguePath;
    public float waitingTime = 1f;

    public bool loadNextScene = false;
    public void TriggerDialogue()
    {
        DialogueLine[] dialogue = JSONDialogue.ImportJSONDialogue(dialoguePath);
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        dialogueManager.StartDialogue(dialogue, loadNextScene);
    }

    void Start()
    {
        Invoke("TriggerDialogue", waitingTime);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.K))
        {
            DialogueLine[] dialogue = JSONDialogue.ImportJSONDialogue(skipDialoguePath);

            FindObjectOfType<DialogueManager>().StartDialogue(dialogue, true);

        }
    }
}
