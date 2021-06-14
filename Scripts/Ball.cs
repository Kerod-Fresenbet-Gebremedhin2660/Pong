using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    private Game game;

    public float moveSpeed = 12.0f;
    public Vector2 ballDirection = Vector2.right;
    // Start is called before the first frame update

    //Paddle sizes
    private float playerPaddleHeight, playerPaddleWidth;
    private float computerPaddleHeight, computerPaddleWidth;

    //Ball size
    private float ballWidth, ballHeight;

    //Wall collider
    public float topBound = 9.5f;
    public float bottomBound = -9.5f;

    //X score collider

    public float xPlayerScore = 17;
    public float xComputerScore = -17;

    //Player paddle box collider
    private float playerPaddleMaxX, playerPaddleMinX;
    private float playerPaddleMaxY, playerPaddleMinY;

    //Computer paddle box collider
    private float computerPaddleMaxX, computerPaddleMinX;
    private float computerPaddleMaxY, computerPaddleMinY;

    private GameObject playerPaddle, computerPaddle;

    private ballManager ballManage;

    //Complex bouncing
    private float bounceAngle;
    private float vx, vy;
    private float maxAngle = 45.0f;

    void Start () {

        ballManage = GameObject.FindGameObjectWithTag ("ballManager").GetComponent<ballManager> ();
        game = GameObject.FindGameObjectWithTag ("GameController").GetComponent<Game> ();

        playerPaddle = GameObject.FindGameObjectWithTag ("Player");
        computerPaddle = GameObject.FindGameObjectWithTag ("Computer");

        playerPaddleHeight = playerPaddle.transform.GetComponent<SpriteRenderer> ().bounds.size.y;
        playerPaddleWidth = playerPaddle.transform.GetComponent<SpriteRenderer> ().bounds.size.x;

        computerPaddleHeight = computerPaddle.transform.GetComponent<SpriteRenderer> ().bounds.size.y;
        computerPaddleWidth = computerPaddle.transform.GetComponent<SpriteRenderer> ().bounds.size.x;

        ballHeight = transform.GetComponent<SpriteRenderer> ().bounds.size.y;
        ballWidth = transform.GetComponent<SpriteRenderer> ().bounds.size.x;

        //Get Player Collider in x
        playerPaddleMaxX = playerPaddle.transform.localPosition.x + playerPaddleWidth / 2;
        playerPaddleMinX = playerPaddle.transform.localPosition.x - playerPaddleWidth / 2;

        //Get Computer Collider in x
        computerPaddleMaxX = computerPaddle.transform.localPosition.x + computerPaddleWidth / 2;
        computerPaddleMinX = computerPaddle.transform.localPosition.x - computerPaddleWidth / 2;

        //Paddle Serve Angle
        if (game.serve <= 1) {
            bounceAngle = GetRandomBounceAngle (110, 200);

        } else {
            if (Random.Range (0, 2) == 0) {
                bounceAngle = GetRandomBounceAngle (0, 70);
            } else {
                bounceAngle = GetRandomBounceAngle (290, 360);
            }

        }

    }

    // Update is called once per frame
    void Update () {

        Move ();

    }

    bool checkCollision () {
        //Get Player Collider in y
        playerPaddleMaxY = playerPaddle.transform.localPosition.y + playerPaddleHeight / 2;
        playerPaddleMinY = playerPaddle.transform.localPosition.y - playerPaddleHeight / 2;

        //Get Computer Collider in y
        computerPaddleMaxY = computerPaddle.transform.localPosition.y + computerPaddleHeight / 2;
        computerPaddleMinY = computerPaddle.transform.localPosition.y - computerPaddleHeight / 2;

        //If the ball is less than the max player x 
        //and the ball is greater than the min player x
        if (transform.localPosition.x - ballWidth / 2 < playerPaddleMaxX && transform.localPosition.x + ballWidth / 2 > playerPaddleMinX) {

            //if the ball is less than the max player y
            //and the ball is greater than the min player y
            if (transform.localPosition.y - ballHeight / 2 < playerPaddleMaxY && transform.localPosition.y + ballHeight / 2 > playerPaddleMinY) {
                //COLLISION WITH PLAYER PADDLE
                game.hits += 1;
                ballDirection = Vector2.right;

                //Play paddle collsion sound
                AudioManager.instance.Play ("paddleHit");

                if (moveSpeed < 0) {
                    moveSpeed *= -1;
                }

                //Bounce Angle
                float relativeIntersectY = playerPaddle.transform.localPosition.y - transform.localPosition.y;
                float normalRIY = (relativeIntersectY / (playerPaddleHeight / 2));
                bounceAngle = normalRIY * (maxAngle * Mathf.Deg2Rad);

                //Shoot Particles
                playerPaddle.GetComponent<Player> ().triggerBurst (transform.localPosition.y);

                //transform.localPosition = new Vector3 (playerPaddleMaxX + ballWidth / 2, transform.localPosition.y, transform.localPosition.z);
                return true;

            }
        }

        //If the ball is less than the max computer x 
        //and the ball is greater than the min computer x
        if (transform.localPosition.x - ballWidth / 2 < computerPaddleMaxX && transform.localPosition.x + ballWidth / 2 > computerPaddleMinX) {

            //if the ball is less than the max computer y
            //and the ball is greater than the min computer y
            if (transform.localPosition.y - ballHeight / 2 < computerPaddleMaxY && transform.localPosition.y + ballHeight / 2 > computerPaddleMinY) {

                //COLLISION WITH COMPUTER PADDLE
                game.hits += 1;
                ballDirection = Vector2.left;

                //Play paddle collsion sound
                AudioManager.instance.Play ("paddleHit");

                //Bounce Angle
                float relativeIntersectY = computerPaddle.transform.localPosition.y - transform.localPosition.y;
                float normalRIY = (relativeIntersectY / (computerPaddleHeight / 2));
                bounceAngle = normalRIY * (maxAngle * Mathf.Deg2Rad);

                //Shoot Particles
                computerPaddle.GetComponent<AI> ().triggerBurst (transform.localPosition.y);

                //transform.localPosition = new Vector3 (computerPaddleMinX - ballWidth / 2, transform.localPosition.y, transform.localPosition.z);
                return true;

            }
        }

        //Wall Collider
        //If hit a wall, inverse bounce angle
        if (transform.localPosition.y > topBound) {
            transform.localPosition = new Vector3 (transform.localPosition.x, topBound, transform.localPosition.z);
            bounceAngle *= -1;
            //Play paddle collsion sound
            AudioManager.instance.Play ("wallHit");
            return true;
        } else if (transform.localPosition.y < bottomBound) {
            transform.localPosition = new Vector3 (transform.localPosition.x, bottomBound, transform.localPosition.z);
            bounceAngle *= -1;
            //Play paddle collsion sound
            AudioManager.instance.Play ("wallHit");
            return true;
        }

        return false;

    }

    //Scoring
    void checkScore () {

        if (transform.localPosition.x > xPlayerScore) {
            game.playerPoint ();
        } else if (transform.localPosition.x < xComputerScore) {

            game.computerPoint ();

        }

    }

    void normalMove () {
        vx = Mathf.Cos (bounceAngle);
        vy = -Mathf.Sin (bounceAngle);

    }

    void curveMove () {
        vx = Mathf.Cos (bounceAngle);

        // if (transform.localPosition.y > 1) {
        //     vy = -Mathf.Sin (bounceAngle) + (transform.localPosition.x / 17);
        // } else if (transform.localPosition.y < -1) {
        //     vy = -Mathf.Sin (bounceAngle) - (transform.localPosition.x / 17);
        // } else {
        //     vy = -Mathf.Sin (bounceAngle);
        // }
        vy = -Mathf.Sin (bounceAngle) - (transform.localPosition.y / 17);

    }
    void sinMove () {
        vx = Mathf.Cos (bounceAngle);
        if (transform.localPosition.y > 0) {
            vy = -Mathf.Sin (bounceAngle) - (Mathf.Sin (transform.localPosition.x));
            vy = vy - (transform.localPosition.y / 17);
        } else {
            vy = -Mathf.Sin (bounceAngle) - (Mathf.Sin (transform.localPosition.x));
            vy = vy - (transform.localPosition.y / 17);
        }

    }

    void twitchMove () {
        vx = Mathf.Cos (bounceAngle);
        vy = -Mathf.Sin (bounceAngle);

        if (transform.localPosition.x < 3 && transform.localPosition.x > -3) {
            vy = vy - (2 * transform.localPosition.y / 17);
        }
    }

    void Move () {
        checkCollision ();
        checkScore ();

        //Speed up the ball
        if ((game.hits % game.speeder) == 0) {
            if (moveSpeed < 35) {
                game.updateLocalScore ((int) 1);
                moveSpeed += game.speedMultipler;
            }
            game.hits += 1;
            TrailRenderer tr = GetComponent<TrailRenderer> ();
            tr.time += 0.1f;

        }

        //Direction Vector
        switch (ballManage.ballMoveType) {
            case ballManager.ballMovement.NORMAL:
                normalMove ();
                break;
            case ballManager.ballMovement.CURVE:
                curveMove ();
                break;
            case ballManager.ballMovement.SIN:
                sinMove ();
                break;
            case ballManager.ballMovement.TWITCH:
                twitchMove ();
                break;
            default:
                normalMove ();
                break;
        }

        // vy = -Mathf.Sin (bounceAngle) - (transform.localPosition.x / 17);
        // vy = -Mathf.Sin (bounceAngle) + (transform.localPosition.x / 17);

        // vy = -Mathf.Sin (bounceAngle) * ((Mathf.Cos (2 * Mathf.PI * (transform.localPosition.x / 17))) * 2);

        // if (transform.localPosition.x > 0) {
        //     vy = -Mathf.Sin (bounceAngle) * ((Mathf.Cos (2 * Mathf.PI * (transform.localPosition.x / 17))) * 2);
        // } else {
        //     vy = -Mathf.Sin (bounceAngle) * ((-Mathf.Cos (2 * Mathf.PI * (transform.localPosition.x / 17))) * 2);
        // }

        // if (ballDirection == Vector2.right) {
        //     vy = -Mathf.Sin (bounceAngle);
        // } else {
        //     vy = -Mathf.Sin (bounceAngle) - (transform.localPosition.x / 17);
        // }

        // if (ballDirection == Vector2.left) {

        //     if (transform.localPosition.y < -8 && transform.localPosition.x < 0 && transform.localPosition.x > -8) {
        //         transform.localPosition = new Vector3 (transform.localPosition.x, 8, transform.localPosition.z);
        //         bounceAngle *= -1;
        //     }

        // }

        float xMove = (vx * (moveSpeed));
        float yMove = (vy * (moveSpeed));

        transform.localPosition += new Vector3 (ballDirection.x * xMove * Time.deltaTime, yMove * Time.deltaTime, 0);
    }

    float GetRandomBounceAngle (float minDegrees = 0f, float maxDegrees = 360f) {
        float minRad = minDegrees * Mathf.PI / 180;
        float maxRad = maxDegrees * Mathf.PI / 180;

        return Random.Range (minRad, maxRad);
    }

    public Vector2 GetDirectionVector () {
        return new Vector2 (vx, vy);
    }

    public float getBounceAngle () {
        return bounceAngle;
    }

    public float getBallHeight () {
        return ballHeight;
    }

    //Why am i gettting the computer paddle height from here
    //Because im too lazy to rewrite this 

    public float getComputerPaddleHeight () {
        return computerPaddleHeight;
    }
}