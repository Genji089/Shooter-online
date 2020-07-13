using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BaseObject
{
    public AudioClip audioShoot; //射击音效
    public Transform fire; //枪焰
    public Transform bullet; //子弹
    public Transform myCamera = null;
    
    
    Fix64 jumpSpeed = (Fix64)10; //跳跃x轴速度
    float floatHeight = 0.68f; //人物位置到脚底距离
    float distFootToGround = 0.01f; //射线检测碰撞体，distance临界值
    float attackSpeed = 8; //攻击速度 
    float attackOffsetX = 0.5f; //子弹生成位置偏移量x
    float attackOffsetY = 0.2f; //子弹生成位置偏移量y
    int direction = 0; //移动方向，0不动，1右，-1左
    bool isShoot = false; //是否射击
    bool isInSky = false; //是否在空中
    bool isJump = false; //是否跳跃
    float cd = 0.0f; //射击cd
    float fireTime = 0.0f; //枪焰存在时间
    bool isFire = false; //射击，枪焰出现
    Fix64 logicDeltaTime = GameData.GD_logicFrameLenth; //逻辑帧运行间隔
    Animator animator; //动画组件
    SpriteRenderer fireSR; //枪焰渲染组件

    public void init()
    {
        moveSpeed = (Fix64)5; //移动速度
        speedY = Fix64.Zero; //速度，方向向下
        gravity = (Fix64)12.5f; //重力
        beginPosition = new FixVector3((Fix64)BaseTransform.position.x,(Fix64)BaseTransform.position.y,(Fix64)BaseTransform.position.z); //当前位置
        endPosition = new FixVector3((Fix64)BaseTransform.position.x, (Fix64)BaseTransform.position.y, (Fix64)BaseTransform.position.z); //计算出的下一帧位置
        animator = BaseTransform.GetComponent<Animator>();
        fireSR = fire.gameObject.GetComponent<SpriteRenderer>();
    }

    public override void updateLogic(GameCtrlMsg gameCtrlMsg)
    {
        //存储当前位置
        beginPosition = endPosition;

        //解析操作数据
        int[] playerCtrl = GameData.getPlayerCtrlArr(gameCtrlMsg.playerCtrl);

        //射击
        if (1 == playerCtrl[0])
        {
            isShoot = true;
        }
        if(0 == playerCtrl[0])
        {
            isShoot = false;
        }

        //移动,2是右，3是左
        if (1 == playerCtrl[2])
        {
            direction = 1;
        }
        if (1 == playerCtrl[3])
        {
            direction = -1;
        }
        if (1 == playerCtrl[2] && 1 == playerCtrl[3])
        {
            direction = 0;
        }
        if (0 == playerCtrl[2] && 0 == playerCtrl[3])
        {
            direction = 0;
        }

        //跳跃
        if (1 == playerCtrl[1] && !isInSky)
        {
            isJump = true;
        }
        if (isJump)
        {
            endPosition.y += jumpSpeed * logicDeltaTime;
        }

        //人物转向
        if (-1 == direction)
        {
            BaseTransform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }
        if(1 == direction)
        {
            BaseTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }

        //判断是否在地面或空中,在空中才有向下的速度
        RaycastHit2D hit = Physics2D.Raycast(new Vector2((float)beginPosition.x, (float)beginPosition.y - floatHeight), -Vector2.up);
        if (hit.collider != null) 
        {
            float distance = Mathf.Abs(hit.point.y - (float)beginPosition.y + floatHeight);
            if (distance <= distFootToGround)
            {
                isInSky = false;
                isJump = false;
                speedY = Fix64.Zero; //不在空中没有重力
            }
            else
            {
                isInSky = true;
                speedY += gravity * logicDeltaTime;
                if (isJump) //跳跃之后，计算向下位移量
                {
                    if (((speedY - jumpSpeed) * logicDeltaTime) > distance) //防止穿过地面
                    {
                        endPosition.y = endPosition.y - distance - (jumpSpeed * logicDeltaTime);
                        isJump = false;
                        isInSky = false;
                    }
                    else 
                    {
                        endPosition.y -= speedY * logicDeltaTime;
                    }
                }
                else //直接从高处落下，计算向下位移量
                {
                    if ((speedY * logicDeltaTime) > distance)
                    {
                        endPosition.y = endPosition.y - distance;
                        isJump = false;
                        isInSky = false;
                    }
                    else
                    {
                        endPosition.y -= speedY * logicDeltaTime;
                    }
                }
            }
        }
        else
        {
            isInSky = true;
            speedY += gravity * logicDeltaTime;
            endPosition.y -= speedY * logicDeltaTime;
        }

        //动画切换
        animator.SetBool("isJump", isInSky);
        animator.SetBool("isShoot", isShoot);
        animator.SetInteger("run", direction);

        //射击，生成子弹
        if(cd <= 1 / attackSpeed)
        {
            cd += (float)logicDeltaTime;
        }
        if (isShoot)
        {
            if(cd >= 1 / attackSpeed)
            {
                int isRight = 1;
                bool hasEmpty = false;
                if (BaseTransform.eulerAngles.y == 180.0f)
                {
                    isRight = -1;
                }
                for (int i = 0; i < GameData.GD_bulletManager.bullets.Count; i++)
                {
                    if (GameData.GD_bulletManager.bullets[i].isExist == false)
                    {
                        GameData.GD_bulletManager.bullets[i].bulletTransform = Object.Instantiate(bullet, new Vector3((float)beginPosition.x + (attackOffsetX * isRight), (float)beginPosition.y + attackOffsetY, 0.0f), BaseTransform.rotation);
                        GameData.GD_bulletManager.bullets[i].isExist = true;
                        hasEmpty = true;
                        break;
                    }
                }
                if (!hasEmpty)
                {
                    GameData.GD_bulletManager.bullets.Add(new Bullet());
                    GameData.GD_bulletManager.bullets[GameData.GD_bulletManager.bullets.Count - 1].bulletTransform = Object.Instantiate(bullet, new Vector3((float)beginPosition.x + (attackOffsetX * isRight), (float)beginPosition.y + attackOffsetY, 0.0f), BaseTransform.rotation);
                }
                fire.position = new Vector3((float)beginPosition.x + (attackOffsetX + 0.6f) * isRight, (float)beginPosition.y + attackOffsetY, 0.0f);
                fireSR.enabled = true;
                isFire = true;

                //音效播放
                AudioSource.PlayClipAtPoint(audioShoot, BaseTransform.localPosition);

                //镜头抖动
                if(myCamera != null)
                {
                    myCamera.SendMessage("cameraShake");
                }
                cd = 0.0f;
            }
        }

        //计算移动，x轴下一帧数据
        endPosition.x += direction * moveSpeed * logicDeltaTime;
    }

    public override void updateRender(float fracJourney)
    {
        //枪焰计时
        if (isFire)
        {
            fireTime += Time.deltaTime;
            if (fireTime >= 0.06f)
            {
                fireSR.enabled = false;
                fireTime = 0.0f;
                isFire = false;
            }
        }
        base.updateRender(fracJourney);
    }
}
