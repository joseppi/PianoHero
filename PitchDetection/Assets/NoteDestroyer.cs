using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDestroyer : MonoBehaviour
{
    public bool canBeDestroyed = false;    

    // Start is called before the first frame update
    void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {
        string songNoteID = name.Remove(2);
        if (canBeDestroyed)        
        {
            if (NotesInteractionHandler.noteID == int.Parse(songNoteID))
            {
                this.GetComponent<SpriteRenderer>().color = Color.clear;                

            }
        }                      
    }

    private void OnTriggerEnter(Collider other)
    {
        canBeDestroyed = true;
        gameObject.GetComponent<SpriteRenderer>().color = Color.green;
    }

    private void OnTriggerExit(Collider other)
    {
        canBeDestroyed = false;
        this.GetComponent<SpriteRenderer>().color = Color.clear;
    }

}
