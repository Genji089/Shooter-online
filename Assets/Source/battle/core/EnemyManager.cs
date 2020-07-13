using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager
{
    public List<Enemy> enemies = new List<Enemy>();

    public void init()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].init();
        }
    }

    public void updateLogic()
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].isExist)
            {
                enemies[i].updateLogic();
            }
        }
    }

    public void updateRender(float fracJourney)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].isExist)
            {
                enemies[i].updateRender(fracJourney);
            }
        }
    }
}
