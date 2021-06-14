using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gameover : MonoBehaviour {

    public Game game;
    private GameObject gameManager;
    public Text Score;

    // Start is called before the first frame update
    void Start () {
        gameManager = GameObject.FindGameObjectWithTag ("GameController");
        if (gameManager == null) {

            Debug.LogWarning ("Cant find gameobject");

        } else {

            game = gameManager.GetComponent<Game> ();
            Score.text = "Score: " + game.getGeneralScore ();
        }

    }

    void Update () {
        if (game.gameOverChange) {
            Score.text = "Score: " + game.getGeneralScore ();
            game.gameOverChange = false;
        }
    }

    public void retry () {
        if (gameManager != null) {
            Destroy (gameManager);
        }

        SceneManager.LoadScene ("Level 1");
    }

    public void menu () {
        if (gameManager != null) {
            Destroy (gameManager);
        }
        SceneManager.LoadScene ("Menu");
    }

}