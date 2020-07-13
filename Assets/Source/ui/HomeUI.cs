using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeUI : MonoBehaviour
{
    public Button btnStart;
    public Text textLoading;
    public Text textConnect;
    public InputField hostInput;
    public InputField portInput;

    // Start is called before the first frame update
    void Start()
    {
        //初始化
        btnStart = btnStart.GetComponent<Button>();
        //绑定回调函数
        btnStart.onClick.AddListener(StartButton);

        textLoading.gameObject.SetActive(false);
        textConnect.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartButton()
    {
        if (hostInput.text != "" && portInput.text != "")
        {
            btnStart.interactable = false;
            StartCoroutine(ConnectServer(hostInput.text, int.Parse(portInput.text)));
        }
        else
        {
            textConnect.text = "Please Input Host and Port";
            StartCoroutine(LoadMainAsyncScene()); //单机模式
            return;
        }
    }

    IEnumerator LoadMainAsyncScene()
    {
        yield return null;

        //显示加载进度
        textLoading.gameObject.SetActive(true);
        //异步加载场景
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainScene");
        asyncOperation.allowSceneActivation = false;
        //Debug.Log("Pro:" + asyncOperation.progress);
        while (!asyncOperation.isDone)
        {
            textLoading.text = "Loading " + (asyncOperation.progress * 100) + "%";
            //Debug.Log("Pro:" + asyncOperation.progress * 100 + "%");
            if (asyncOperation.progress >= 0.9f)
            {
                textLoading.text = "Press P to continue";
                if (Input.GetKeyDown(KeyCode.P))
                {
                    asyncOperation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }

    IEnumerator ConnectServer(string host, int port)
    {
        yield return null;
        if (!GameData.GD_socket.isConnected)
        {
            GameData.GD_socket.ConnectServerByHost(host, port);
        }
        while (!GameData.GD_socket.isConnected && !GameData.GD_socket.failConnected)
        {
            textConnect.text = "Connecting...";
            Debug.Log("Connecting");
            yield return null;
        }
        if (GameData.GD_socket.failConnected)
        {
            textConnect.text = "Connect Server Fail!";
            btnStart.interactable = true;
        }
        else
        {
            textConnect.text = "Connect Server Success!";
            StartCoroutine(LoadMainAsyncScene());
        }
    }
}
