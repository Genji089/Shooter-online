using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float gravity = 12.0f;
    public float jumpSpeed = 10.0f;
    public float logicTime = 0.06f;
    public float speedY = 0.0f;

    public float floatHeight = 0.68f;
    public float distFootToGround = 0.5f;

    public float lastSpeedY = 0.0f;
    public float rayOffset = 0.0f;

    public bool isJump = false;
    public bool isGround = false;
    public int direction = 0;
    public bool isStartJump = false;
    public bool isNeedRay = false;
    public float rayCd = 0.0f;
    public bool isInSky = false;

    public Vector3 lastPosition = new Vector3(0, 0, 0);
    public Vector3 beginPosition = new Vector3(0, 0, 0);
    public Vector3 endPosition = new Vector3(0, 0, 0);

    public Rigidbody2D rb2D = null;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("testMan speedY:" + speedY);
        beginPosition = transform.position;
        endPosition = beginPosition;
        if (Input.GetKey(KeyCode.D))
        {
            endPosition.x += moveSpeed * logicTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            endPosition.x -= moveSpeed * logicTime;
        }

        if (Input.GetKeyDown(KeyCode.W) && !isInSky)
        {
            isJump = true;
            isStartJump = true;
        }

        if (isJump)
        {
            endPosition.y += jumpSpeed * logicTime;
            isInSky = true;
        }

        float distance = 0.0f;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(beginPosition.x, beginPosition.y - floatHeight), -Vector2.up);
        if (hit.collider != null)
        {
            distance = Mathf.Abs(hit.point.y - beginPosition.y + floatHeight);
            //Debug.Log(distance);
            if (distance <= distFootToGround)
            {
                //在地面
                isInSky = false;

                isJump = false;
                speedY = 0.0f;
            }
            else
            {
                isInSky = true;
                speedY += gravity * logicTime;
                if ((speedY * logicTime) > distance)
                {
                    endPosition.y -= distance;
                }
                else
                {
                    endPosition.y -= speedY * logicTime;
                }
            }
        }
        else
        {
            isInSky = true;
            speedY += gravity * logicTime;
            endPosition.y -= speedY * logicTime;
        }
        rayCd = 0.0f;
        if (!isInSky)
        {
        }
        if (isInSky)
        {
            //重力持续影响
            
            
            /*if (endPosition.y < hit.point.y)
            {
                //endPosition.y += Mathf.Abs(hit.point.y - endPosition.y) + rayOffset;
                speedY = 0.0f;
            }*/
        }

        transform.position = endPosition;
        lastPosition = beginPosition;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "ground")
        {
            /*isJump = false;
            isGround = true;
            endPosition.y += lastSpeedY * logicTime;

            lastSpeedY = 0.0f;*/
            Debug.Log("ground");
        }
    }
}
