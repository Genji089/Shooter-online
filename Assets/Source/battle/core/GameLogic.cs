using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//游戏逻辑
public class GameLogic
{
    FrameSyncLogic frameSyncLogic = null;

    public void init()
    {
        frameSyncLogic = new FrameSyncLogic();
        frameSyncLogic.setGameLogic(this);
        GameData.GD_playerManager.init();
        GameData.GD_enemyManager.init();
    }

    public void gameStart()
    {

    }

    public void getCtrlInput()
    {
        GameData.GD_gameCtrlMsg.playerCtrl = 0;
        if (Input.GetKey(KeyCode.A))
        {
            GameData.setPlayerCtrlArr(3);
        }
        if (Input.GetKey(KeyCode.D))
        {
            GameData.setPlayerCtrlArr(2);
        }
        if (Input.GetKey(KeyCode.W))
        {
            GameData.setPlayerCtrlArr(1);
        }
        if (Input.GetKey(KeyCode.J))
        {
            GameData.setPlayerCtrlArr(0);
        }
    }

    public void updateFrameSyncLogic()
    {
        //调用帧同步核心逻辑
        frameSyncLogic.updateLogic();
    }

    public void updateLogic()
    {
        //运行逻辑帧
        GameData.GD_playerManager.updateLogic();
        GameData.GD_bulletManager.updateLogic();
        GameData.GD_enemyManager.updateLogic();
    }

    public void updateRender(float fracJourney)
    {
        GameData.GD_playerManager.updateRender(fracJourney);
        GameData.GD_bulletManager.updateRender(fracJourney);
        GameData.GD_enemyManager.updateRender(fracJourney);
    }
}
