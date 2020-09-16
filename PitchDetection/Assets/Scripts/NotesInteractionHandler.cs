using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotesInteractionHandler : MonoBehaviour
{
    public static int noteID;
    public float beatTempo;
    public bool hasStarted;
    public bool reset;
    GameObject Do1, Doh1, Re1, Reh1, Mi1, Fa1, Fah1, Sol1, Solh1, La1, Lah1, Si1, Do2, Doh2, Re2, Reh2, Mi2, Fa2, Fah2, Sol2, Solh2, La2, Lah2, Si2;
    int saveNoteID = -1;
    Button displayPlay;
    Button displayReplay;
    Transform[] childrens;

    // Start is called before the first frame update
    void Start()
    {
        noteID = -1;
        Do1 = GameObject.Find("1 Do/C");
        Doh1 = GameObject.Find("1 Do#/C#");
        Re1 = GameObject.Find("1 Re/D");
        Reh1 = GameObject.Find("1 Re#/D#");
        Mi1 = GameObject.Find("1 Mi/E");
        Fa1 = GameObject.Find("1 Fa/F");
        Fah1 = GameObject.Find("1 Fa#/F#");
        Sol1 = GameObject.Find("1 Sol/G");
        Solh1 = GameObject.Find("1 Sol#/G#");
        La1 = GameObject.Find("1 La/A");
        Lah1 = GameObject.Find("1 La#/A#");
        Si1 = GameObject.Find("1 Si/B");
        Do2 = GameObject.Find("2 Do/C");
        Doh2 = GameObject.Find("2 Do#/C#");
        Re2 = GameObject.Find("2 Re/D");
        Reh2 = GameObject.Find("2 Re#/D#");
        Mi2 = GameObject.Find("2 Mi/E");
        Fa2 = GameObject.Find("2 Fa/F");
        Fah2 = GameObject.Find("2 Fa#/F#");
        Sol2 = GameObject.Find("2 Sol/G");
        Solh2 = GameObject.Find("2 Sol#/G#");
        La2 = GameObject.Find("2 La/A");
        Lah2 = GameObject.Find("2 La#/A#");
        Si2 = GameObject.Find("2 Si/B");
        displayPlay = GameObject.Find("Play").GetComponent<Button>();
        displayReplay = GameObject.Find("Replay").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVirtualPiano();
        SongHandler();        

    }

    public void PlayButton()
    {
        hasStarted = true;
        displayPlay.gameObject.SetActive(false);
    }

    public void ReplayButton()
    {        
        reset = true;

    }

    void UpdateVirtualPiano()
    {
        //Debug.Log(noteID);        
        if (saveNoteID != noteID)
        {
            Do1.GetComponent<Image>().color = Color.white;
            Doh1.GetComponent<Image>().color = Color.black;
            Re1.GetComponent<Image>().color = Color.white;
            Reh1.GetComponent<Image>().color = Color.black;
            Mi1.GetComponent<Image>().color = Color.white;
            Fa1.GetComponent<Image>().color = Color.white;
            Fah1.GetComponent<Image>().color = Color.black;
            Sol1.GetComponent<Image>().color = Color.white;
            Solh1.GetComponent<Image>().color = Color.black;
            La1.GetComponent<Image>().color = Color.white;
            Lah1.GetComponent<Image>().color = Color.black;
            Si1.GetComponent<Image>().color = Color.white;
            Do2.GetComponent<Image>().color = Color.white;
            Doh2.GetComponent<Image>().color = Color.black;
            Re2.GetComponent<Image>().color = Color.white;
            Reh2.GetComponent<Image>().color = Color.black;
            Mi2.GetComponent<Image>().color = Color.white;
            Fa2.GetComponent<Image>().color = Color.white;
            Fah2.GetComponent<Image>().color = Color.black;
            Sol2.GetComponent<Image>().color = Color.white;
            Solh2.GetComponent<Image>().color = Color.black;
            La2.GetComponent<Image>().color = Color.white;
            Lah2.GetComponent<Image>().color = Color.black;
            Si2.GetComponent<Image>().color = Color.white;
        }
        saveNoteID = noteID;
        switch (noteID)
        {
            case 0:
                Do1.GetComponent<Image>().color = Color.gray;
                break;
            case 1:
                Doh1.GetComponent<Image>().color = Color.gray;
                break;
            case 2:
                Re1.GetComponent<Image>().color = Color.gray;
                break;
            case 3:
                Reh1.GetComponent<Image>().color = Color.gray;
                break;
            case 4:
                Mi1.GetComponent<Image>().color = Color.gray;
                break;
            case 5:
                Fa1.GetComponent<Image>().color = Color.gray;
                break;
            case 6:
                Fah1.GetComponent<Image>().color = Color.gray;
                break;
            case 7:
                Sol1.GetComponent<Image>().color = Color.gray;
                break;
            case 8:
                Solh1.GetComponent<Image>().color = Color.gray;
                break;
            case 9:
                La1.GetComponent<Image>().color = Color.gray;
                break;
            case 10:
                Lah1.GetComponent<Image>().color = Color.gray;
                break;
            case 11:
                Si1.GetComponent<Image>().color = Color.gray;
                break;
            case 12:
                Do2.GetComponent<Image>().color = Color.gray;
                break;
            case 13:
                Doh2.GetComponent<Image>().color = Color.gray;
                break;
            case 14:
                Re2.GetComponent<Image>().color = Color.gray;
                break;
            case 15:
                Reh2.GetComponent<Image>().color = Color.gray;
                break;
            case 16:
                Mi2.GetComponent<Image>().color = Color.gray;
                break;
            case 17:
                Fa2.GetComponent<Image>().color = Color.gray;
                break;
            case 18:
                Fah2.GetComponent<Image>().color = Color.gray;
                break;
            case 19:
                Sol2.GetComponent<Image>().color = Color.gray;
                break;
            case 20:
                Solh2.GetComponent<Image>().color = Color.gray;
                break;
            case 21:
                La2.GetComponent<Image>().color = Color.gray;
                break;
            case 22:
                Lah2.GetComponent<Image>().color = Color.gray;
                break;
            case 23:
                Si2.GetComponent<Image>().color = Color.gray;
                break;
        }

    }

    public void SongHandler()
    {

        if (hasStarted == true)
        {
            displayReplay.gameObject.SetActive(true);
            transform.localPosition -= new Vector3(0.0f, beatTempo * Time.deltaTime, 0.0f);
        }
        else
        {
            displayReplay.gameObject.SetActive(false);
        }

        if (reset == true)
        {
            transform.localPosition = new Vector3(-74.0f, -490.0f, -43.0f);
            childrens = this.GetComponentsInChildren<Transform>();

            foreach (Transform child in childrens)
            {
                if (child.gameObject.GetComponent<SpriteRenderer>() != null)
                {
                    child.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                }
                
            }            
            
            reset = false;
            hasStarted = false;
            displayPlay.gameObject.SetActive(true);
        }
    }
}
