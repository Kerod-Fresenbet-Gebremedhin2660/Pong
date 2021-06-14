using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

    private GameObject ball;

    private int computerScore;
    private int playerScore;

    public int winnerScore = 5;

    public int speedMultipler = 1;
    public int hits = 0;
    public int speeder = 5;
    private HUD hUD;

    private bool gameOver = false;
    private bool nextLevel = true;

    private scoreManager sm;

    public int serve = 0;

    public static Scene level;

    public static Game instance;

    int generalScore;

    public int lives;

    bool totalUp = true;

    public bool gameOverChange = false;

    //Just using enumeration just to see how it works
    public enum WINNER {
        PLAYER,
        COMPUTER
    }
    public WINNER win;

    public enum GameState {
        PLAY,
        PAUSE
    }
    public GameState state;

    void Awake () {

        if (instance == null) {
            instance = this;
        } else {
            Destroy (gameObject);
            return;
        }

        DontDestroyOnLoad (gameObject);
    }

    void newLevel () {

    }

    // Start is called before the first frame update
    void Start () {
        lives = 5;
        state = GameState.PLAY;
    }

    public int getPlayerScore () {
        return playerScore;
    }

    public void updateLocalScore (int score) {
        sm.putScore (score);
        hUD.generalScore.text = "score: " + (generalScore + sm.getScore ());
    }

    public void updateGeneralScore (int score) {
        generalScore += score;
    }

    public int getGeneralScore () {
        return generalScore;
    }

    public void setGeneralScore (int score) {
        generalScore = score;
    }
    public int getComputerScore () {
        return computerScore;
    }

    // Update is called once per frame
    void Update () {
        if (nextLevel && SceneManager.GetActiveScene ().buildIndex != SceneManager.GetSceneByName ("Gameover").buildIndex &&
            SceneManager.GetActiveScene ().buildIndex != SceneManager.GetSceneByName ("WinScreen").buildIndex) {
            Debug.Log (state);
            if (!gameOver) {
                hUD = GameObject.FindGameObjectWithTag ("Canvas").GetComponent<HUD> ();
            }
            sm = GameObject.FindGameObjectWithTag ("scoreManager").GetComponent<scoreManager> ();

            level = SceneManager.GetActiveScene ();
            nextLevel = false;
            hUD.generalScore.text = "score: " + (generalScore + sm.getScore ());
            hUD.lives.text = "Lives: " + lives;
            startGame ();
            StartCoroutine (waiterSeconds (2));
        }
        if (gameOver) {
            if (Input.GetKeyUp (KeyCode.Space) && state == GameState.PLAY) {

                if (win == WINNER.PLAYER) {
                    gameOver = false;
                    nextLevel = true;
                    generalScore += sm.getScore ();
                    SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
                } else {
                    if (lives > 0) {

                        lives--;
                        hUD.lives.text = "Lives: " + lives;

                        startGame ();
                        gameOver = false;
                        StartCoroutine (waiterSeconds (2));
                        //SpawnBall ();
                    } else {
                        SceneManager.LoadScene ("Gameover");
                    }
                }

            }
        } else {

            //Pause Menu
            if (Input.GetKeyDown (KeyCode.P)) {
                if (state == GameState.PLAY) {
                    Pause ();
                } else if (state == GameState.PAUSE) {
                    Resume ();
                }
            }
        }

        if (SceneManager.GetActiveScene ().buildIndex == SceneManager.GetSceneByName ("WinScreen").buildIndex) {
            if (totalUp) {
                updateGeneralScore (lives * 1000);
                totalUp = false;
                gameOverChange = true;

            }
        }

    }

    public void Pause () {
        state = GameState.PAUSE;
        Time.timeScale = 0;
        hUD.PauseMenu.SetActive (true);
    }

    public void Resume () {
        state = GameState.PLAY;
        Time.timeScale = 1;
        hUD.PauseMenu.SetActive (false);
    }

    public void mainMenu () {
        Resume ();
        Destroy (GameObject.FindGameObjectWithTag ("GameController"));
        SceneManager.LoadScene (0);
    }

    void startGame () {
        playerScore = 0;
        computerScore = 0;
        hUD.playAgain.enabled = false;
        hUD.playerScore.text = "0";
        hUD.computerScore.text = "0";

    }

    void SpawnBall () {
        ball = GameObject.Instantiate ((GameObject) Resources.Load ("Prefabs/Circle", typeof (GameObject)));
        ball.transform.localPosition = new Vector3 (0, 0, -2);

    }

    public bool checkWin (WINNER winner) {

        switch (winner) {
            case WINNER.PLAYER:
                if (playerScore >= winnerScore) {
                    //Player wins
                    win = WINNER.PLAYER;
                    if (computerScore == 0) {
                        updateLocalScore (200);
                    }
                    //Play player win sound
                    AudioManager.instance.Play ("playerWin");

                    hUD.playerScore.text = "WIN";
                    hUD.playAgain.enabled = true;
                    gameOver = true;
                    return true;
                }

                break;

            case WINNER.COMPUTER:
                if (computerScore >= winnerScore) {
                    //Computer wins
                    win = WINNER.COMPUTER;

                    //Play computer win sound
                    AudioManager.instance.Play ("computerWin");

                    hUD.computerScore.text = "WIN";
                    hUD.playAgain.enabled = true;
                    gameOver = true;
                    return true;
                }
                break;

        }
        return false;

    }

    IEnumerator waiterSeconds (int seconds = 1) {

        yield return new WaitForSeconds (seconds);
        SpawnBall ();
        if (serve == 1) {
            hUD.playerServe.enabled = false;
        } else if (serve == 2) {
            hUD.computerServe.enabled = false;

        }
    }

    public void computerPoint () {
        computerScore++;

        updateLocalScore (-sm.getScore () / 10);

        //Play computer score sound
        AudioManager.instance.Play ("computerScore");

        hUD.computerScore.text = computerScore.ToString ();
        Destroy (ball);
        if (checkWin (WINNER.COMPUTER)) {

        } else {
            serve = 2;
            hUD.computerServe.enabled = true;
            StartCoroutine (waiterSeconds ());

        }

    }

    public void playerPoint () {
        playerScore++;

        updateLocalScore (50);

        //Play player score sound
        AudioManager.instance.Play ("playerScore");

        hUD.playerScore.text = playerScore.ToString ();
        Destroy (ball);
        if (checkWin (WINNER.PLAYER)) {

        } else {
            serve = 1;
            hUD.playerServe.enabled = true;
            StartCoroutine (waiterSeconds ());

        }

    }

}