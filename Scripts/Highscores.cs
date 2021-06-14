using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Highscores : MonoBehaviour {

    const string privateCode = "r3IWYXo0-kizRcK70006HADoadL27bYE-lQt-SYg7YGw";
    const string publicCode = "5fa7f751eb371a09c4be039b";
    const string webURL = "http://dreamlo.com/lb/";

    public Highscore[] highscoreList;
    public DisplayHighscore highscoreDisplay;
    public GameObject HighscoreScreen;

    static Highscores instance;

    public int playerRank;

    void Awake () {
        instance = this;
        highscoreDisplay = HighscoreScreen.GetComponent<DisplayHighscore> ();
        GetHighscores ();

    }
    public static void AddNewHighscore (string username, int score) {
        instance.StartCoroutine (instance.UploadNewHighscore (username, score));
    }

    public static bool AddLimitHighscore (string username, int score) {

        Highscore[] testHighscoreList = instance.GetHighscores ();
        if (testHighscoreList == null) {
            return false;
        }
        if (testHighscoreList.Length >= 1000) {

            bool lessThan1000 = false;

            for (int i = 0; i < testHighscoreList.Length; i++) {
                if (testHighscoreList[i].score < score) {
                    instance.playerRank = i;
                    lessThan1000 = true;
                }
            }
            if (lessThan1000) {
                instance.StartCoroutine (instance.UploadNewHighscore (username, score));
            } else {
                instance.playerRank = 1001;
            }

        } else {
            instance.playerRank = testHighscoreList.Length;
            instance.StartCoroutine (instance.UploadNewHighscore (username, score));
        }
        return true;

    }
    IEnumerator UploadNewHighscore (string username, int score) {
        UnityWebRequest www = new UnityWebRequest (webURL + privateCode + "/add/" + UnityWebRequest.EscapeURL (username) + "/" + score);
        yield return www.SendWebRequest ();

        if (string.IsNullOrEmpty (www.error)) {
            print ("upload successfull");
            getHighscores ();
        } else {
            print ("error" + www.error);
        }
    }

    public Highscore[] GetHighscores () {

        if (highscoreList == null) {
            StartCoroutine ("DownloadHighscores");
            return null;
        } else {
            return highscoreList;
        }
    }

    public void getHighscores () {
        StartCoroutine ("DownloadHighscores");
    }

    IEnumerator DownloadHighscores () {
        UnityWebRequest www = new UnityWebRequest (webURL + publicCode + "/pipe/");
        www.downloadHandler = new DownloadHandlerBuffer ();
        yield return www.SendWebRequest ();

        if (string.IsNullOrEmpty (www.error)) {
            FormatHighscores (www.downloadHandler.text);

            if (HighscoreScreen.activeSelf) {
                highscoreDisplay.SetHighscore (highscoreList);
            }

        } else {
            print ("error download" + www.error);
        }
    }

    void FormatHighscores (string text) {
        string[] values = text.Split (new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        highscoreList = new Highscore[values.Length];

        for (int i = 0; i < values.Length; i++) {
            string[] valuesInfo = values[i].Split (new char[] { '|' });
            string username = valuesInfo[0];
            int score = int.Parse (valuesInfo[1]);
            highscoreList[i] = new Highscore (username, score);
            print (highscoreList[i].username + ":" + highscoreList[i].score);
        }
    }

}

public struct Highscore {
    public string username;
    public int score;

    public Highscore (string _username, int _score) {
        username = _username;
        score = _score;
    }
}