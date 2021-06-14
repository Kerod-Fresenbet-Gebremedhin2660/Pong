using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    // Start is called before the first frame update

    public void PlayGame () {
        SceneManager.LoadScene ("Level 1");
    }

    public void Quit () {
        Application.Quit (0);
    }

    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }
}