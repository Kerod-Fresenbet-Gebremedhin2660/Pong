using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    GameObject GameManager;
    Game Game;

    // Start is called before the first frame update
    void Start () {
        GameManager = GameObject.FindGameObjectWithTag ("GameController");
        Game = GameManager.GetComponent<Game> ();

    }

    public void ResumeButtom () {
        Game.Resume ();
    }

    public void MainMenuButton () {
        Game.mainMenu ();
    }

}