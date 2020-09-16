using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    GameObject songDisplay;
    Button startGameButton;
    Button mainMenuButton;
        
    // Start is called before the first frame update
    void Start()
    {
        songDisplay = GameObject.FindGameObjectWithTag("SongDisplay");
        //songDisplay.SetActive(false);

        startGameButton = GameObject.FindGameObjectWithTag("StartGameButton").GetComponent<Button>();
        startGameButton.onClick.AddListener(StartGameButton);
    }

    public void StartGameButton()
    {
        songDisplay.SetActive(true);        
        startGameButton.gameObject.SetActive(false);
    }
}
