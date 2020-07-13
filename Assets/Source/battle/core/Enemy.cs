using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public Transform enemyTransform = null;
    public bool isExist = true;

    Animator animator;
    //Rigidbody2D rb2D;
    BoxCollider2D bc2D;
    SpriteRenderer alertSign;
    EnemyMonoBehaviour enemyMonoBehaviour;

    public Fix64 moveSpeed = (Fix64)3;
    public float hp = 5;
    public float hitTime = 0.5f;
    public float hitSpeed = 2; //被击退的速度
    public Fix64 hitBackLength = (Fix64)5;
    public float DestroyTime = 5; //相当于die后尸体存留时间
    public Fix64 moveDistance = Fix64.Zero;
    public AudioClip audioHit;

    int direction = -1;
    int directionLast = 0; //存放上一次的direction
    float timeCounter = 0.0f;
    Fix64 lastPositionX;
    Fix64 initPositionX;
    Fix64 newPositionX;
    bool isHit = false;
    bool hitLock = false;
    Vector3 rayBeginPos = new Vector3(0.0f, 0.0f, 0.0f);
    FixVector3 targetPosition = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);
    Fix64 logicDeltaTime = GameData.GD_logicFrameLenth;

    public FixVector3 beginPosition = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);
    public FixVector3 endPosition = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);

    public void init()
    {
        lastPositionX = (Fix64)enemyTransform.position.x;
        initPositionX = lastPositionX;
        animator = enemyTransform.GetComponent<Animator>();
        //rb2D = enemyTransform.GetComponent<Rigidbody2D>();
        bc2D = enemyTransform.GetComponent<BoxCollider2D>();
        alertSign = enemyTransform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        enemyMonoBehaviour = enemyTransform.GetComponent<EnemyMonoBehaviour>();
        moveDistance = (Fix64)enemyMonoBehaviour.moveDistance;

        beginPosition = new FixVector3((Fix64)enemyTransform.position.x, (Fix64)enemyTransform.position.y, (Fix64)enemyTransform.position.z);
        endPosition = new FixVector3((Fix64)enemyTransform.position.x, (Fix64)enemyTransform.position.y, (Fix64)enemyTransform.position.z);
    }

    public void updateLogic()
    {
        if(hp == 0)
        {
            die();
        }
        else
        {
            //获取起始位置
            beginPosition = endPosition;

            //获取当前hp
            hp = enemyMonoBehaviour.hp;
            
            newPositionX = (Fix64)enemyTransform.position.x;
            if (Fix64.Abs(newPositionX - lastPositionX) > moveDistance)
            {
                direction = direction * -1;
                lastPositionX = newPositionX;
            }

            if (isHit)
            {
                if (!hitLock)
                {
                    Hit(enemyMonoBehaviour.bulletDirection);
                    hitLock = true;
                }
                animator.SetBool("isHit", true);
                alertSign.enabled = true;
                if (timeCounter < hitTime)//被打中，停下，进入isHit状态
                {
                    timeCounter += (float)logicDeltaTime;
                    if (directionLast == 0)
                    {
                        directionLast = direction;
                    }
                    direction = 0;
                    HitBack(beginPosition, targetPosition);
                }
                else
                {
                    timeCounter = 0.0f;
                    direction = directionLast;
                    directionLast = 0;
                    isHit = false; //解除状态
                    hitLock = false;
                    animator.SetBool("isHit", false);
                }
            }
            else
            {
                isHit = enemyMonoBehaviour.isHit;
            }
            //模型转向
            if (direction == -1 || directionLast == -1)
            {
                enemyTransform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            }
            if (direction == 1 || directionLast == 1)
            {
                enemyTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }

            if (direction == -1)
            {
                rayBeginPos = new Vector3((float)beginPosition.x - 1, (float)beginPosition.y, (float)beginPosition.z);
            }
            if (direction == 1)
            {
                rayBeginPos = new Vector3((float)beginPosition.x + 1, (float)beginPosition.y, (float)beginPosition.z);
            }
            RaycastHit2D raycast = Physics2D.Raycast(rayBeginPos, new Vector2(direction, 0));
            if (raycast.collider != null && raycast.collider.tag == "man")
            {
                alertSign.enabled = true;
                if (raycast.point.x < (float)initPositionX && raycast.point.x > (float)(initPositionX - moveDistance))
                {
                    shoot();
                }
            }
            else
            {
                alertSign.enabled = false;
            }

            //计算下一帧移动位置
            if (!isHit)
            {
                endPosition.x += moveSpeed * direction * logicDeltaTime;
            }
        }
    }

    public void die()
    {
        isExist = false;
        animator.SetBool("isDie", true);
        alertSign.enabled = false;
        //Object.Destroy(rb2D);
        Object.Destroy(bc2D);
        Object.Destroy(enemyTransform.gameObject, DestroyTime);
    }

    public void shoot()
    {

    }
    public void Hit(int bulletDirection)
    {
        if (direction == bulletDirection)
        {
            direction = direction * -1;
        }

        if (bulletDirection == 1)
        {
            targetPosition = new FixVector3(beginPosition.x + hitBackLength, beginPosition.y, beginPosition.z);
        }
        else
        {
            targetPosition = new FixVector3(beginPosition.x - hitBackLength, beginPosition.y, beginPosition.z);
        }
    }

    public void HitBack(FixVector3 startPosition, FixVector3 endPosition)
    {
        Fix64 JourneyLength = FixVector3.Distance(endPosition, startPosition);
        Fix64 distCover = logicDeltaTime * hitSpeed;
        Fix64 fracJourney = Fix64.Zero;
        if (JourneyLength == Fix64.Zero)
        {
            Debug.LogError("JourneyLength(hitBackLength) is 0.");
            return;
        }
        else
        {
            fracJourney = distCover / JourneyLength;
        }
        this.endPosition = FixVector3.Lerp(startPosition, endPosition, fracJourney);
    }

    public void updateRender(float fracJourney)
    {
        enemyTransform.position = Vector3.Lerp(beginPosition.ToVector3(), endPosition.ToVector3(), fracJourney);
    }
}
