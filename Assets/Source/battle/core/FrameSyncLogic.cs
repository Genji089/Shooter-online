using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameSyncLogic
{
    //游戏逻辑对象
    GameLogic gameLogic = null;

    //逻辑帧间隔
    float logicFrameLenth;

    //客户端逻辑帧时间线总时长
    float totalDeltaTime;

    //下一个逻辑帧开始时间
    float logicFrameBeginTime;

    //渲染插值参数
    float fracJourney;

    //发送间隔
    float sendCd;

    //下一发送时间
    float sendTime;

    //客户端发送操作时间线总时长
    float totalSendDeltaTime;

    //是否从队列中获取到数据
    bool isGetCtrlMsg = false;

    public FrameSyncLogic()
    {
        init();
    }

    public void init()
    {
        logicFrameLenth = (float)GameData.GD_logicFrameLenth;
        totalDeltaTime = 0;
        logicFrameBeginTime = 0;
        fracJourney = 0;
        sendCd = (float)GameData.GD_sendGameCtrlMsgCd;
        sendTime = 0;
        totalSendDeltaTime = 0;
    }

    public void setGameLogic(GameLogic gameLogic)
    {
        this.gameLogic = gameLogic;
    }

    public void updateLogic()
    {
        totalDeltaTime += Time.deltaTime;
        totalSendDeltaTime += Time.deltaTime;

        //收集并发送操作数据，按固定时间间隔发送
        if (totalSendDeltaTime > sendTime)
        {
            //收集操作
            gameLogic.getCtrlInput();
            //发送操作
            GameData.GD_socket.sendMessage(GameData.jsonPacking(GameData.GD_gameCtrlMsg));

            sendTime += sendCd;
        }

        //逻辑帧，按固定时长更新。当客户端跟不上逻辑帧时，循环执行逻辑帧，追上后才继续渲染
        while (totalDeltaTime > logicFrameBeginTime)
        {
            //操作数据队列中取出数据
            lock (GameData.GD_gameCtrlMsgQueueLock)
            {
                int num = GameData.GD_gameCtrlMsgQueue.Count;

                if (num == 0)
                {
                    if(logicFrameBeginTime == 0)
                    {
                        totalDeltaTime = 0;
                        Debug.Log("第一帧还没有数据！");
                        return;
                    }
                    Debug.Log("没有数据");
                    isGetCtrlMsg = false;
                }
                if (num != 0)
                {
                    isGetCtrlMsg = true;
                    GameData.GD_gameCtrlMsgListUse = GameData.GD_gameCtrlMsgQueue.Dequeue();
                    //游戏逻辑帧序号增加
                    GameData.GD_logicFrameNum++;
                    //Debug.Log("logicFrameNum now : " + GameData.GD_logicFrameNum);
                }
            }
            //更新客户端逻辑
            if (isGetCtrlMsg)
            {
                gameLogic.updateLogic();
            }

            logicFrameBeginTime += logicFrameLenth;
        }

        //渲染帧插值参数计算：logicFrameLenth-(logicFrameBeginTime-totalDeltaTime)
        fracJourney = (logicFrameLenth - logicFrameBeginTime + totalDeltaTime) / logicFrameLenth;

        //更新客户端渲染
        gameLogic.updateRender(fracJourney);
    }
}
