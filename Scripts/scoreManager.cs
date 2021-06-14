using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreManager : MonoBehaviour {

    //LevelScore

    public int scoreMultipler = 1;
    public int score;

    public int getScore () {
        return score;
    }

    public void putScore (int newScore) {
        score += newScore * scoreMultipler;
    }

}