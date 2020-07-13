using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMonoBehaviour : MonoBehaviour
{
    public bool isHit = false;
    public int bulletDirection = 1;
    float hitCounter = 0;
    public float moveDistance = 10;
    public float hp = 8;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(isHit == true)
        {
            if (hitCounter < 0.1)
            {
                hitCounter += Time.deltaTime;
            }
            else
            {
                isHit = false;
                hitCounter = 0;
            }
        }
        
    }

    void Hit(int bulletDirection)
    {
        //改变状态量。传递给Enemy
        isHit = true;
        this.bulletDirection = bulletDirection;
        hp -= 1;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
