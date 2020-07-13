using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet
{
    public Transform bulletTransform = null;
    public float changeSpeed = 10;
    public float moveSpeed = 25;
    public float maxScaleX = 13;
    bool nothing = true;
    int direction = 1;
    public bool isExist = true;
    float existTime = 0.0f;
    float logicDeltaTime = (float)GameData.GD_logicFrameLenth;

    public Vector3 beginPosition = new Vector3(0, 0, 0);
    public Vector3 endPosition = new Vector3(0, 0, 0);

    public void updateLogic()
    {
        beginPosition = bulletTransform.position;
        endPosition = beginPosition;
        if(bulletTransform.eulerAngles.y == 180)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }
        if (nothing)
        {
            
            nothing = false;
        }

        endPosition.x += moveSpeed * logicDeltaTime * direction;

        //销毁
        if (existTime > 1.0f)
        {
            isExist = false;
            existTime = 0.0f;
            destoryBullet(); 
        }
        else
        {
            existTime += logicDeltaTime;
        }
    }

    public void updateRender(float fracJourney)
    {
        bulletTransform.position = Vector3.Lerp(beginPosition, endPosition, fracJourney);
    }

    public void destoryBullet()
    {
        Object.Destroy(bulletTransform.gameObject);
    }
}
