using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    public List<Player> players;

    public void init()
    {
        for (int i = 0; i < GameData.GD_playerTotalNum; i++)
        {
            players[i].init();
        }
    }

    public void updateLogic()
    {
        //根据数据包顺序匹配相应的player对象
        for (int i = 0; i < GameData.GD_gameCtrlMsgListUse.Count; i++)
        {
            players[i].updateLogic(GameData.GD_gameCtrlMsgListUse[i]);
        }
    }

    public void updateRender(float fracJourney)
    {
        for (int i = 0; i < GameData.GD_playerTotalNum; i++)
        {
            players[i].updateRender(fracJourney);
        }
    }
}
