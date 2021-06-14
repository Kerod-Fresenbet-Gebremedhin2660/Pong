using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class background : MonoBehaviour {

    public float scrollSpeed = 0.5f;
    public SpriteRenderer spriteRenderer;
    Vector2 size;

    // Start is called before the first frame update
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer> ();
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;
        size = spriteRenderer.size;

    }

    // Update is called once per frame
    void Update () {
        size.y += scrollSpeed * Time.deltaTime;
        spriteRenderer.size = size;

    }
}