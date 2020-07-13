using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 0.1f;
    public float beginTime = 2;
    public Transform followTarget = null;
    public float shakeNum;

    float beginCd = 0;
    bool isMove = false;
    bool isShake = false;
    Vector3 shakePos = Vector3.zero;
    float shakeTime = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(followTarget != null)
        {
            Vector3 nextPosition = new Vector3(followTarget.position.x, followTarget.position.y, -10);
            if(nextPosition.y < -24)
            {
                nextPosition.y = -24;
            }
            transform.position = nextPosition;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isMove)
            {
                isMove = false;
            }
            else
            {
                isMove = true;
            }
        }
        if (isMove)
        {
            transform.Translate(Vector3.right * moveSpeed, Space.World);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            isShake = true;
        }
       
        if (isShake)
        {
            shakeTime += Time.deltaTime;
            if (shakeTime < 0.1f)
            {
                transform.localPosition -= shakePos;
                shakePos = Random.insideUnitSphere / shakeNum;
                transform.localPosition += shakePos;
            }
            else
            {
                shakeTime = 0;
                isShake = false;
            }
        }
        
        if (Input.GetKey(KeyCode.K))
        {
            transform.Translate(Vector3.right * moveSpeed, Space.World);
        }
        if (Input.GetKey(KeyCode.L))
        {
            transform.Translate(Vector3.left * moveSpeed, Space.World);
        }
    }

    void cameraShake()
    {
        isShake = true;
    }
}
