using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JSONDialogue
{


    public Wrapper<DialogueLine> Lines;


    public JSONDialogue(Wrapper<DialogueLine> lines)
    {
        this.Lines = lines;
    }

    //public static T ImportJson<T>(string path)
    //{
    //    TextAsset textAsset = Resources.Load<TextAsset>(path);
    //    return JsonUtility.FromJson<T>(textAsset.text);
    //}
    //
    //public static T[] getJsonArray<T>(string json)
    //{
    //    string newJson = "{ \"array\": " + json + "}";
    //    Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
    //    return wrapper.array;
    //}

    public static DialogueLine[] ImportJSONDialogue(string path)
    {
        TextAsset textAsset = (TextAsset)Resources.Load(path);
        Debug.Log(textAsset);
        Wrapper<DialogueLine> wrapper = JsonUtility.FromJson<Wrapper<DialogueLine>>(textAsset.text);
        return wrapper.array;
    }

}


[System.Serializable]
public class Wrapper<T>
{
    public T[] array;
}



[System.Serializable]
public class DialogueLine
{
    public string name;
    public string sentence;

    public DialogueLine(string name, string sentence)
    {
        this.name = name;
        this.sentence = sentence;
    }
}


