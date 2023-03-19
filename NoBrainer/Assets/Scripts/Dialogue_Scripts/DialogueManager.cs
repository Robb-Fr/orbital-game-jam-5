using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{

    // public TextMeshProUGUI nameText;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    public float typingSpeed = 20f;
    public Animator animator;

    public Animator buttonAnimator;

    private Queue<string> sentences;
    private Queue<string> names;

    public bool LoadNextScene = false;

    public bool LinkToButton = false;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        names = new Queue<string>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            DisplayNextSentence();
        }

    }

    public void StartDialogue(DialogueLine[] dialogue, bool LoadNextScene)
    {


        sentences.Clear();
        names.Clear();
        // TODO nameText.text = dialogue.name;

        animator.SetBool("IsOpen", true);
        Debug.Log("after animator");
        this.LoadNextScene = LoadNextScene;

        // filling the queue
        foreach (DialogueLine line in dialogue)
        {
            Debug.Log("inside dialogue line loop");
            sentences.Enqueue(line.sentence);
            names.Enqueue(line.name);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        string name = names.Dequeue();
        Debug.Log(nameText);
        nameText.text = name;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        int i = 0;
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            if (i % 2 == 0)
            {
                AudioManager.instance.Play("DialogueBlip");
            }
            i += 1;
            yield return new WaitForSecondsRealtime(1 / typingSpeed);
        }
    }

    void EndDialogue()
    {

        animator.SetBool("IsOpen", false);

        if (LinkToButton)
        {
            buttonAnimator.SetBool("ButtonOpen", true);
        }


        if (LoadNextScene)
        {
            FindObjectOfType<SceneLoading>().LoadNextScene();
        }
    }


}
