using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameCtrlMsg
{
    public int room = 0;

    /// <summary>
    /// int后四位 0001射击 0010跳跃 0000不动 1000往左走 0100往右走
    /// </summary>
    public int playerCtrl = 0; //操作数据
}

public class GameData
{
    //GameCrtlMsg队列 互斥锁
    public static readonly object GD_gameCtrlMsgQueueLock = new object();

    public static ClientSocket GD_socket = new ClientSocket();

    //本客户端的输入存放，发送给服务器
    public static GameCtrlMsg GD_gameCtrlMsg = new GameCtrlMsg();

    //游戏总人数
    public static int GD_playerTotalNum = 2;

    //驱动逻辑帧的GameCtrlMsg
    public static List<GameCtrlMsg> GD_gameCtrlMsgListUse = new List<GameCtrlMsg>();

    //本客户端的玩家id
    public static int GD_playerId = 0;

    //存放接收到的GameCtrlMsg List
    public static Queue<List<GameCtrlMsg>> GD_gameCtrlMsgQueue = new Queue<List<GameCtrlMsg>>();

    public static PlayerManager GD_playerManager = new PlayerManager();

    public static BulletManager GD_bulletManager = new BulletManager();

    public static EnemyManager GD_enemyManager = new EnemyManager();

    //逻辑帧长度
    public static Fix64 GD_logicFrameLenth = (Fix64)0.06f;

    //发送间隔
    public static Fix64 GD_sendGameCtrlMsgCd = (Fix64)0.06f;

    public static int GD_logicFrameNum = 0;

    public static bool GD_isGameReady = false;

    public static bool GD_isGameCanGo = false;

    public static string jsonPacking(GameCtrlMsg gameCtrlMsg)
    {
        string s = JsonUtility.ToJson(gameCtrlMsg);
        return s;
    }

    public static GameCtrlMsg jsonUnpacking(string gameCtrlMsg_str)
    {
        GameCtrlMsg gameCtrlMsg = JsonUtility.FromJson<GameCtrlMsg>(gameCtrlMsg_str);
        return gameCtrlMsg;
    }

    /// <summary>
    /// returns:{0:shoot; 1:jump; 2:right; 3:left}
    /// </summary>
    /// <param name="playerCtrl"></param>
    /// <returns></returns>
    public static int[] getPlayerCtrlArr(int playerCtrl)
    {
        int[] playerCtrlArr = new int[4];
        for(int i = 0; i < 4; i++)
        {
            playerCtrlArr[i] = (playerCtrl & (1 << i)) >> i;
        }
        return playerCtrlArr;
    }

    /// <summary>
    /// 记得先清零！offset:{shoot:0; jump:1; right:2; left:3}
    /// </summary>
    /// <param name="offset"></param>
    public static void setPlayerCtrlArr(int offset)
    {
        GD_gameCtrlMsg.playerCtrl = GD_gameCtrlMsg.playerCtrl | (1 << offset);
    }
}


