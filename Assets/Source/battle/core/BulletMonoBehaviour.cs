using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMonoBehaviour : MonoBehaviour
{
    public int direction = 1;
    public SpriteRenderer spriteRenderer = null;
    public BoxCollider2D box = null;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        if (transform.eulerAngles.y == 180)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.tag)
        {
            case "man":
                break;
            case "enemy":
                collider.SendMessage("Hit",direction);
                spriteRenderer.enabled = false;
                box.enabled = false;
                break;
            case "wall":
                spriteRenderer.enabled = false;
                box.enabled = false;
                break;
            case "platform":
                spriteRenderer.enabled = false;
                box.enabled = false;
                break;
            default:
                Debug.Log("unknown thing");
                break;
        }
    }
}
