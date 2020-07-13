using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text.RegularExpressions;

public class textFix64 : MonoBehaviour
{
    ClientSocket simpleSocket;
    GameCtrlMsg gameCtrlMsg;
    float sendCd;

    void Awake()
    {
        gameCtrlMsg = new GameCtrlMsg();
    }

    // Start is called before the first frame update
    void Start()
    {
        /*if (!GameData.GD_socket.isConnected)
        {
            Debug.Log("开始连接服务器");
            GameData.GD_socket.ConnectServerByHost("server.natappfree.cc", 39715);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        /*if(sendCd < GameData.GD_logicFrameLenth)
        {
            sendCd += Time.deltaTime;
        }
        else
        {
            if (GameData.GD_socket.isConnected)
            {
                //simpleSocket.sendMessage(gameData.jsonPacking(gameCtrlMsg));
                GameData.GD_socket.sendMessage(GameData.jsonPacking(GameData.GD_gameCtrlMsg));
                //Debug.Log("发送成功");
            }
            else
            {
                Debug.Log("没有连接到服务器");
            }

            sendCd = 0.0f;
        }*/

        if (Input.GetKeyDown(KeyCode.Q))
        {
            string s = "{0:{}}{1:{}}";

            string pattern2 = @"\{([^\{\}]*)\}"; 
             string pattern3 = @"\{\}";
            MatchCollection m = Regex.Matches(s, pattern2);
            Debug.Log("m.count:" + m.Count);
            for (int i = 0; i < m.Count; i++)
            {
                Debug.Log(m[i].Value);
                if(m[i].Value == "{}")
                {
                    Debug.Log("yes");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            gameCtrlMsg.room = 1;
            gameCtrlMsg.playerCtrl = 2;
            string s = GameData.jsonPacking(gameCtrlMsg);
            string s1 = GameData.jsonPacking(gameCtrlMsg);
            s = s + s1;
            Debug.Log(s);


            /*int end = s.IndexOf("\n");
            Debug.Log(end);
            string ssub2 = s.Substring(0, end);
            Debug.Log(ssub2);*/


            string pattern = @"([^\n]+)\n";
            string pattern2 = @"\{([^\{\}]+)\}";
            GameCtrlMsg[] gameCtrl= new GameCtrlMsg[2];

            MatchCollection m = Regex.Matches(s, pattern2);
            for (int i = 0; i < 2; i++)
            {
                //gameCtrl[i] = GameData.jsonUnpacking(m[i].Value);
                //Debug.Log(m.Count);
                Debug.Log(m[i].Value);
            }

            /*string t2 = s.Substring(5, s.Length - 6);
            Debug.Log(t2);*/

            string t3 = "{hahha:{heiheihei}}{hahah:{biebiebie}}";
            MatchCollection m1 = Regex.Matches(t3, pattern2);
            for (int i = 0; i < m1.Count; i++)
            {
                Debug.Log(m1[i].Value);
            }

        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            //shoot:0; jump:1; left:2; right:3
            GameData.setPlayerCtrlArr(0);
            GameData.setPlayerCtrlArr(2);

            int[] a = GameData.getPlayerCtrlArr(GameData.GD_gameCtrlMsg.playerCtrl);
            foreach(int i in a)
            {
                Debug.Log(i);
            }
            Debug.Log(GameData.GD_gameCtrlMsg.playerCtrl);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            string s = GameData.jsonPacking(gameCtrlMsg);
            s = s + s;
            Debug.Log(s);
            string t1 = s.Substring(1,s.Length -2);
            Debug.Log(t1);
            //GameData.jsonUnpacking(t1);
            GameData.jsonUnpacking(s);

            
        }
    }
}
