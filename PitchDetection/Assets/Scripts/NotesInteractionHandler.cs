using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotesInteractionHandler : MonoBehaviour
{
    public static int noteID;
    public float beatTempo;
    public bool hasStarted;  
    public GameObject Fa;
    // Start is called before the first frame update
    void Start()
    {
        noteID = -1;               
        Fa = GameObject.Find("Fa/F");        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVirtualPiano();
        SongHandler();
    }
    void UpdateVirtualPiano()
    {
        Debug.Log(noteID);
        switch (noteID)
        {
            case -1:
                Fa.GetComponent<Image>().color = Color.white;
                break;
            case 5:
                Fa.GetComponent<Image>().color = Color.gray;
                break;
        }
    }
    void SongHandler()
    {
        if (hasStarted == true)
        {
            transform.localPosition -= new Vector3(0f, beatTempo * Time.deltaTime, 0f);
        }        
    }
}
