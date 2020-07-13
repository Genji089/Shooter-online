using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject
{
    public Transform BaseTransform = null;
    public bool isExist = true;
    public Fix64 moveSpeed = Fix64.Zero;
    public Fix64 gravity = Fix64.Zero;
    public Fix64 speedY = Fix64.Zero;

    public FixVector3 beginPosition = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);
    public FixVector3 endPosition = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);

    /// <summary>
    /// 更新渲染
    /// </summary>
    /// <param name="fracJourney">插值参数</param>
    public virtual void updateRender(float fracJourney)
    {
        if (!isExist)
        {
            return;
        }
        
        BaseTransform.position = Vector3.Lerp(beginPosition.ToVector3(), endPosition.ToVector3(), fracJourney);
    }

    /// <summary>
    /// 更新逻辑
    /// </summary>
    public virtual void updateLogic(GameCtrlMsg gameCtrlMsg)
    {

    }
}
