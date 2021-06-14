using UnityEngine;

public class Player : MonoBehaviour {
    public float moveSpeed = 8.0f;

    public GameObject particle;
    public ParticleSystem ps;

    public float topBound = 8.7f;
    public float bottomBound = -8.7f;

    public Vector2 startingPostition = new Vector2 (-12.0f, 0.0f);

    // Start is called before the first frame update
    void Start () {
        ps = particle.GetComponent<ParticleSystem> ();
        transform.localPosition = new Vector3 (startingPostition.x, startingPostition.y, -2);
    }

    // Update is called once per frame
    void Update () {
        CheckUserInput ();

    }

    public void triggerBurst (float y) {
        ps.transform.position = new Vector3 (transform.position.x, y, transform.position.z);
        ps.Play ();

        // ParticleSystem.EmissionModule em = ps.emission;
        // em.enabled = true;
    }

    void CheckUserInput () {
        if (Input.GetAxis ("Vertical") > 0) {
            if (transform.localPosition.y >= topBound) {
                transform.localPosition = new Vector3 (transform.localPosition.x, topBound, transform.localPosition.z);
            } else {
                transform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;
            }
        } else if (Input.GetAxis ("Vertical") < 0) {
            if (transform.localPosition.y <= bottomBound) {
                transform.localPosition = new Vector3 (transform.localPosition.x, bottomBound, transform.localPosition.z);
            } else {
                transform.localPosition += Vector3.down * moveSpeed * Time.deltaTime;
            }
        } else { }
    }
}