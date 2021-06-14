using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHighscore : MonoBehaviour {
    public Text[] highscoreText;
    public Text rank;
    Highscores highscoreManager;

    public GameObject NetScore;

    // Start is called before the first frame update
    void Start () {
        for (int i = 0; i < highscoreText.Length; i++) {
            highscoreText[i].text = i + 1 + "." + "Fetching...";
        }
        highscoreManager = NetScore.GetComponent<Highscores> ();
        StartCoroutine ("RefreshHighscore");
        //OnHighscoreDownload ();

    }
    IEnumerator RefreshHighscore () {
        while (true) {
            yield return new WaitForSeconds (30);
            OnHighscoreDownload ();
        }
    }
    public void OnHighscoreDownload () {
        Highscore[] highscoreList = highscoreManager.GetHighscores ();
        if (highscoreList == null) {
            Debug.Log ("Fetching...");
        } else {
            for (int i = 0; i < highscoreText.Length; i++) {
                highscoreText[i].text = i + 1 + ". ";
                if (highscoreList.Length > i) {
                    highscoreText[i].text += highscoreList[i].username + " - " + highscoreList[i].score;
                }
            }
            rank.text = "YOUR RANK IS " + highscoreManager.playerRank;

        }
    }

    public void SetHighscore (Highscore[] _highscoreList) {
        if (_highscoreList == null) {
            Debug.Log ("Fetching...");
        } else {
            for (int i = 0; i < highscoreText.Length; i++) {
                highscoreText[i].text = i + 1 + ". ";
                if (_highscoreList.Length > i) {
                    highscoreText[i].text += _highscoreList[i].username + " - " + _highscoreList[i].score;
                }
            }
            rank.text = "YOUR RANK IS " + highscoreManager.playerRank;

        }
    }

}