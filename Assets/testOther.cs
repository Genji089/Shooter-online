using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testOther : MonoBehaviour
{
    public long rawNum;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            float a = 0.06f;
            float b = 0.06f;
            Fix64 fa = (Fix64)0.06;
            Fix64 fb = (Fix64)0.06;
            float c = (float)fa;
            Fix64 fc = Fix64.FromRaw(rawNum);
            /*Debug.Log("float a * b = " + a * b);
            Debug.Log("Fix64 float a * b = " + fa * fb);*/

            Debug.Log("fa = " + fa);
            Debug.Log("fb = " + fb);
            Debug.Log("c = " + c);
            Debug.Log(fc);
        }
    }

    void OnDestroy()
    {
        GameData.GD_socket.closeSocket();
    }
}
