using UnityEngine;

public class AI : MonoBehaviour {

    public float moveSpeed = 8.0f;
    public float topBound = 8.7f;
    public float bottomBound = -8.7f;

    public Vector2 startingPostition = new Vector2 (12.0f, 0.0f);

    private GameObject ball;
    private Vector2 ballPos;

    private GameObject player;

    float hitDirection = 0;

    bool centreRand = false;
    int centreNo = 0;
    public ParticleSystem ps;
    public GameObject particle;

    public AI_TYPE AITYPE;
    public bool returnToCentre;
    public hitDirectionStrength ballHitDirectionStrength;

    public enum hitDirectionStrength {
        NORMAL,
        EXTREME
    }
    public enum AI_TYPE {
        BASIC,
        AIM,
        PREDICT,
        PREDICTCURVE,
        PREDICTSIN
    }

    // Start is called before the first frame update
    void Start () {
        ps = particle.GetComponent<ParticleSystem> ();
        transform.localPosition = new Vector3 (startingPostition.x, startingPostition.y, -2);
        player = GameObject.FindGameObjectWithTag ("Player");

    }

    // Update is called once per frame
    void Update () {

        Move ();

    }

    public void triggerBurst (float y) {
        ps.transform.position = new Vector3 (transform.position.x, y, transform.position.z);
        ps.Play ();

        // ParticleSystem.EmissionModule em = ps.emission;
        // em.enabled = true;
    }

    void basicAI () {
        if (transform.localPosition.y > bottomBound && transform.localPosition.y > ballPos.y) {
            transform.localPosition += new Vector3 (0, -moveSpeed * Time.deltaTime, 0);
        }

        if (transform.localPosition.y < topBound && transform.localPosition.y < ballPos.y) {
            transform.localPosition += new Vector3 (0, moveSpeed * Time.deltaTime, 0);
        }

    }

    void AimAI () {
        getHitDirection ();
        if (transform.localPosition.y > bottomBound) {
            if (hitDirection + (ball.GetComponent<Ball> ().getBallHeight () / 2) > ballPos.y) {
                transform.localPosition += new Vector3 (0, -moveSpeed * Time.deltaTime, 0);
            }
        }
        if (transform.localPosition.y < topBound) {
            if (hitDirection - (ball.GetComponent<Ball> ().getBallHeight () / 2) < ballPos.y) {
                transform.localPosition += new Vector3 (0, moveSpeed * Time.deltaTime, 0);
            }
        }

    }
    public delegate float predict_vy (float bounceAngle, Vector2 ballPos);
    void PredictAI (predict_vy predict) {
        getHitDirection ();
        //First find where the ball is going to be (BounceTracing :))
        Vector2 ballDirectionVector = ball.GetComponent<Ball> ().GetDirectionVector ();
        float ballBounceAngle = ball.GetComponent<Ball> ().getBounceAngle ();
        float distance; //distance from ball to wall or paddle
        bool wallBounce = true; //if the ball will bounce 

        //Where the ball is going to be after bouncing n times
        Vector2 presumedBallPos = new Vector2 (ballPos.x, ballPos.y);
        int count = 0;
        while (wallBounce) {

            if (ballDirectionVector.y > 0) {
                //if the ball is moving up

                distance = (topBound - presumedBallPos.y) / ballDirectionVector.y;
                //distance to the wall = (wallY - ballY) / directionVectorY
                float intersectX = (distance * ballDirectionVector.x) + presumedBallPos.x;
                // intersection of wall x = (distance to wall * directionVectorX) + ballX
                if (intersectX < transform.localPosition.x) {
                    ballBounceAngle *= -1;
                    float vx = Mathf.Cos (ballBounceAngle);
                    float vy = predict (ballBounceAngle, presumedBallPos);

                    ballDirectionVector = new Vector2 (vx, vy);
                    presumedBallPos = new Vector2 (intersectX, topBound);

                    //Debug.Log ("Bouncing on top " + count + " at " + intersectX);
                } else {
                    distance = (startingPostition.x - intersectX) / ballDirectionVector.x;
                    float aiPaddleY = (distance * ballDirectionVector.y) + topBound;
                    presumedBallPos = new Vector2 (startingPostition.x, aiPaddleY);
                    wallBounce = false;

                    //Debug.Log ("Coming up " + count + " at " + aiPaddleY);
                }
            } else if (ballDirectionVector.y < 0) {
                //Bottom bounce
                distance = (bottomBound - presumedBallPos.y) / ballDirectionVector.y;
                float intersectX = (distance * ballDirectionVector.x) + presumedBallPos.x;

                if (intersectX < transform.localPosition.x) {
                    ballBounceAngle *= -1;
                    float vx = Mathf.Cos (ballBounceAngle);
                    float vy = predict (ballBounceAngle, presumedBallPos);

                    ballDirectionVector = new Vector2 (vx, vy);
                    presumedBallPos = new Vector2 (intersectX, bottomBound);

                    //Debug.Log ("Bouncing on bottom " + count + " at " + intersectX);
                } else {
                    distance = (startingPostition.x - intersectX) / ballDirectionVector.x;
                    float aiPaddleY = (distance * ballDirectionVector.y) + bottomBound;
                    presumedBallPos = new Vector2 (startingPostition.x, aiPaddleY);

                    wallBounce = false;
                    //Debug.Log ("Coming down " + count + " at " + aiPaddleY);
                }
            } else {
                //Going straight
                presumedBallPos = new Vector2 (startingPostition.x, ballPos.y);
                wallBounce = false;
                //Debug.Log ("Going straight " + count);
            }
            count++;
            //infiniteloop breaker
            if (count > 10) {
                wallBounce = false;
                Debug.Log ("Too many bounces");

            }
        }

        if (transform.localPosition.y > bottomBound) {
            if (hitDirection + (ball.GetComponent<Ball> ().getBallHeight () / 2) > presumedBallPos.y) {
                transform.localPosition += new Vector3 (0, -moveSpeed * Time.deltaTime, 0);
            }
        }
        if (transform.localPosition.y < topBound) {
            if (hitDirection - (ball.GetComponent<Ball> ().getBallHeight () / 2) < presumedBallPos.y) {
                transform.localPosition += new Vector3 (0, moveSpeed * Time.deltaTime, 0);
            }
        }

    }

    void centreReturn () {
        if (transform.localPosition.y < -1) {
            transform.localPosition += new Vector3 (0, moveSpeed * Time.deltaTime, 0);
        }

        if (transform.localPosition.y > 1) {
            transform.localPosition += new Vector3 (0, -moveSpeed * Time.deltaTime, 0);
        }
    }
    float linearBallY (float bounceAngle, Vector2 pBallPos) {
        return -Mathf.Sin (bounceAngle);
    }

    float curveBallY (float bounceAngle, Vector2 pBallPos) {
        return -Mathf.Sin (bounceAngle) - (pBallPos.y / 17);
    }

    float sinBallY (float bounceAngle, Vector2 pBallPos) {
        return (-Mathf.Sin (bounceAngle) - (Mathf.Sin (pBallPos.x))) - (ballPos.y / 17);
    }

    void Move () {

        //IF ball is null
        if (!ball) {
            ball = GameObject.FindGameObjectWithTag ("ball");
        }

        if (!ball) {
            //If there are no gameobjects with tag "ball"

        } else {

            if (ball.GetComponent<Ball> ().ballDirection == Vector2.right) {

                ballPos = ball.transform.localPosition;
                if (AITYPE == AI_TYPE.BASIC) {
                    basicAI ();
                } else if (AITYPE == AI_TYPE.PREDICT) {
                    PredictAI (linearBallY);
                } else if (AITYPE == AI_TYPE.PREDICTCURVE) {
                    PredictAI (curveBallY);

                } else if (AITYPE == AI_TYPE.PREDICTSIN) {
                    PredictAI (sinBallY);
                } else {
                    AimAI ();
                }

            } else {
                centreRand = false;
                if (returnToCentre) {
                    centreReturn ();
                }

            }
        }

    }

    void getHitDirection () {
        float[] top;
        float[] bottom;
        float[] centre;
        switch (ballHitDirectionStrength) {
            case hitDirectionStrength.NORMAL:
                // Player top {AI top, AI bottom, AI centre}
                top = new float[] {
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 4)),
                    transform.localPosition.y,
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2)),
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2))
                };
                // Player bottom {AI top, AI bottom, AI centre}
                bottom = new float[] {
                    transform.localPosition.y,
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 4)),
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2)),
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2))
                };
                // Player centre {AI top, AI bottom, AI centre random 1, AI centre random 2}
                centre = new float[] {
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 4)),
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 4)),
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 4)),
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 4))
                };
                break;
            case hitDirectionStrength.EXTREME:
                // player top {AI top, AI bottom, AI centre}
                top = new float[] {
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2)),
                    transform.localPosition.y,
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2)),
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2))
                };
                // player bottom {AI top, AI bottom, AI centre}
                bottom = new float[] {
                    transform.localPosition.y,
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2)),
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2)),
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2))
                };
                // player centre {AI top, AI bottom, AI centre random 1, AI centre random 2}
                centre = new float[] {
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2)),
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2)),
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2)),
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2))
                };
                break;
            default:
                // player top {AI top, AI bottom, AI centre}
                top = new float[] {
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 4)),
                    transform.localPosition.y,
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2)),
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2))
                };
                // player bottom {AI top, AI bottom, AI centre}
                bottom = new float[] {
                    transform.localPosition.y,
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 4)),
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2)),
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 2))
                };
                // player centre {AI top, AI bottom, AI centre random 1, AI centre random 2}
                centre = new float[] {
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 4)),
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 4)),
                    (transform.localPosition.y + (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 4)),
                    (transform.localPosition.y - (ball.GetComponent<Ball> ().getComputerPaddleHeight () / 4))
                };
                break;
        }

        if (player.transform.localPosition.y > (topBound / 2)) {
            //If the player is at the top

            if (transform.localPosition.y > (topBound / 2)) {
                //Debug.Log ("Player at the top, AI at the top");
                //Hit it towards the bottom
                hitDirection = top[0];
            } else if (transform.localPosition.y < (bottomBound / 2)) {
                //Debug.Log ("Player at the top, AI at the bottom");
                hitDirection = top[1];
            } else {
                //Debug.Log ("Player at the top, AI at the centre");
                hitDirection = top[2];
            }
        } else if (player.transform.localPosition.y < (bottomBound / 2)) {
            //If the player is at the bottom
            if (transform.localPosition.y > (topBound / 2)) {
                //Debug.Log ("Player at the bottom, AI at the top");
                //Hit it towards the top
                hitDirection = bottom[0];
            } else if (transform.localPosition.y < (bottomBound / 2)) {
                //Debug.Log ("Player at the bottom, AI at the bottom");
                hitDirection = bottom[1];
            } else {
                //Debug.Log ("Player at the bottom, AI at the centre");
                hitDirection = bottom[2];
            }
        } else {
            //If the player is at the centre 
            if (transform.localPosition.y > (topBound / 2)) {
                //Debug.Log ("Player at the centre, AI at the top");
                //Hit it towards the top
                hitDirection = centre[0];
            } else if (transform.localPosition.y < (bottomBound / 2)) {
                //Debug.Log ("Player at the centre, AI at the bottom");
                hitDirection = centre[1];
            } else {
                //Debug.Log ("Player at the centre, AI at the centre");

                if (!centreRand) {
                    centreNo = Random.Range (0, 2);
                    centreRand = true;
                }

                if (centreNo == 0) {
                    hitDirection = centre[2];
                } else {
                    hitDirection = centre[3];
                }

            }
        }
    }
}