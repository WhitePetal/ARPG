/*********************************************************
	文件：MonsterController
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/9/2 15:40:42
	功能：怪物表现实体控制器类
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : Controller
{
    private void Update()
    {
        if (isMove)
        {
            SetDir();

            SetMove();
        }

        if (skillMove)
        {
            SetSkillMove();
        }
    }

    protected override void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, Vector2.up);
        Vector3 eulerAngle = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngle;
    }

    protected override void SetMove()
    {
        ctrl.Move(transform.forward * timerSev.deltaTime * Constans.MonsterMoveSpeed);
        ctrl.Move(-transform.up * timerSev.deltaTime * 9.8f);
    }

}
