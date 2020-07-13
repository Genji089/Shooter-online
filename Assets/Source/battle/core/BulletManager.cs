using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager
{
    public List<Bullet> bullets = new List<Bullet>();

    public void updateLogic()
    {
        for(int i = 0; i < bullets.Count; i++)
        {
            if (bullets[i].isExist)
            {
                bullets[i].updateLogic();
            }
        }
    }

    public void updateRender(float fracJourney)
    {
        for(int i = 0; i < bullets.Count; i++)
        {
            if (bullets[i].isExist)
            {
                bullets[i].updateRender(fracJourney);
            }
        }
    }
}
