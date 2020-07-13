using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameLogicMonoBehaviour : MonoBehaviour
{
    public Transform player;
    public Transform bullet;
    public Transform fire;
    public Transform myCamera;
    public AudioClip audioShoot;

    CameraMove cameraMove;
    bool isCameraBind = false;

    //游戏主逻辑对象
    GameLogic gameLogic = new GameLogic();

    void Awake()
    {
        GameData.GD_playerManager.players = new List<Player>();
        for(int i = 0; i < GameData.GD_playerTotalNum; i++)
        {
            GameData.GD_playerManager.players.Add(new Player());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //生成player，为player对象绑定组件
        for (int i = 0; i < GameData.GD_playerTotalNum; i++) 
        {
            Vector3 initPosition = new Vector3((transform.position.x) + (i * 2.5f), transform.position.y, transform.position.z);
            GameData.GD_playerManager.players[i].BaseTransform =Instantiate(player, initPosition, transform.rotation);
            GameData.GD_playerManager.players[i].fire = Instantiate(fire);
            GameData.GD_playerManager.players[i].bullet = bullet;
            GameData.GD_playerManager.players[i].audioShoot = audioShoot;
        }

        //根据enemy数量生成enemy对象
        GameObject enemies = GameObject.FindGameObjectWithTag("enemies");
        Debug.Log("enemy number:" + enemies.transform.childCount);
        GameData.GD_enemyManager.enemies = new List<Enemy>();
        for (int i = 0; i < enemies.transform.childCount; i++)
        {
            GameData.GD_enemyManager.enemies.Add(new Enemy());
            GameData.GD_enemyManager.enemies[i].enemyTransform = enemies.transform.GetChild(i);
        }
        
        cameraMove = myCamera.gameObject.GetComponent<CameraMove>();
        //init()，在生成player后
        gameLogic.init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameData.GD_isGameReady = true;
            //发送ready
            GameData.GD_socket.sendReadySign();
        }

        //绑定相机
        if (GameData.GD_isGameCanGo && !isCameraBind)
        {
            cameraMove.followTarget = GameData.GD_playerManager.players[GameData.GD_playerId].BaseTransform;
            GameData.GD_playerManager.players[GameData.GD_playerId].BaseTransform.GetChild(0).gameObject.SetActive(true);
            GameData.GD_playerManager.players[GameData.GD_playerId].myCamera = myCamera;
        }

        //游戏主逻辑开始
        if (GameData.GD_isGameCanGo)
        {
            gameLogic.updateFrameSyncLogic();
        }
    }


    void OnDestroy()
    {
        GameData.GD_socket.closeSocket();
    }
}
