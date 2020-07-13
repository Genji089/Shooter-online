using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyFactory : MonoBehaviour
{
    public float cdTime = 4;
    //public float DestroyTime; //相当于die后尸体存留时间
    public Transform enemy;

    float timeCounter1;
    float timeCounter2;
    // Start is called before the first frame update

    void Awake() 
    {
        timeCounter1 = 0.0f;
        timeCounter2 = 0.0f;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timeCounter1 < cdTime)
        {
            timeCounter1 += Time.deltaTime;
        }
        else
        {
            Instantiate(enemy, transform.position, transform.rotation);
            timeCounter1 = 0.0f;
        }
    }
}
