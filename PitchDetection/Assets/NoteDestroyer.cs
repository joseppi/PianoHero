using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDestroyer : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        string songNoteID = name.Remove(2);
        if (NotesInteractionHandler.noteID == 5)
        {
            if (NotesInteractionHandler.noteID == int.Parse(songNoteID))
            {
                Destroy(this.gameObject);
            }
        }
        
    }
}
