using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputHighscore : MonoBehaviour {
    public GameObject inputfieldObject;

    public GameObject highscoreScreen;
    Game gameController;

    TMP_InputField inputHighScore;

    void Start () {

        gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<Game> ();

    }

    public void submit () {
        inputHighScore = inputfieldObject.GetComponent<TMP_InputField> ();

        if (!string.IsNullOrEmpty (inputHighScore.text)) {
            string formatText;
            if (inputHighScore.text.Contains ("*")) {
                formatText = inputHighScore.text.Replace ('*', '@');
            } else {
                formatText = inputHighScore.text;
            }

            if (Highscores.AddLimitHighscore (formatText, gameController.getGeneralScore ()) == false) {
                Debug.Log ("Error");
            } else {
                gameObject.SetActive (false);
                highscoreScreen.SetActive (true);
            }

        }
    }

}