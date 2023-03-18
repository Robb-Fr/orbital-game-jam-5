using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string dialoguePath;

    public void TriggerDialogue(bool LoadNextScene)
    {
        DialogueLine[] dialogue = JSONDialogue.ImportJSONDialogue(dialoguePath);
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, LoadNextScene);
    }
}
