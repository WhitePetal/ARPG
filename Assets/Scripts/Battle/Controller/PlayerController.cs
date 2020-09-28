/*********************************************************
	文件：PlayerController
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/16 8:43:03
	功能：表现实体角色控制器
***********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : Controller
{
    private Vector3 camOffset;
    
    private NavMeshAgent nav;
    private Action navDesCB;

    private bool isNaving = false;

    private float targetBlend;
    private float curBlend;

    public override void Init()
    {
        base.Init();

        nav = GetComponent<NavMeshAgent>();
        camOffset = transform.position - camTrasn.position;
    }
    public void BattleInit()
    {
        Init();
        InitFX();
    }
    private void InitFX()
    {
        fxDic.Add("dagger_skill1", transform.Find("Bip_master/dagger_skill1").gameObject);
        fxDic.Add("dagger_skill2", transform.Find("Bip_master/dagger_skill2").gameObject);
        fxDic.Add("dagger_skill3", transform.Find("Bip_master/dagger_skill3").gameObject);
        fxDic.Add("dagger_atk1", transform.Find("Bip_master/dagger_atk1").gameObject);
        fxDic.Add("dagger_atk2", transform.Find("Bip_master/dagger_atk2").gameObject);
        fxDic.Add("dagger_atk3", transform.Find("Bip_master/dagger_atk3").gameObject);
        fxDic.Add("dagger_atk4", transform.Find("Bip_master/dagger_atk4").gameObject);
        fxDic.Add("dagger_atk5", transform.Find("Bip_master/dagger_atk5").gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        #region Input
        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");

        //Vector2 _dir = new Vector2(h, v).normalized;

        //if (_dir != Vector2.zero)
        //{
        //    Dir = _dir;
        //    SetBlend(1f);
        //}
        //else
        //{
        //    Dir = Vector2.zero;
        //    SetBlend(0f);
        //}
        #endregion

        if(curBlend != targetBlend) UpdateMixBlend();

        if (isMove)
        {
            if (isNaving) StopNav();
            // 设置方向
            SetDir();
            // 产生移动
            SetMove();
            // 相机跟随
            SetCamera();
        }

        if (skillMove)
        {
            SetSkillMove();
            SetCamera();
        }

        // 寻路终点停止
        if (isNaving && Vector3.SqrMagnitude(transform.position - nav.destination) < 0.25f)
        {
            StopNav();
            if (navDesCB != null) navDesCB.Invoke();
        }
    }

    private void SetCamera()
    {
        if(camTrasn != null)
        {
            camTrasn.position = transform.position - camOffset;
        }
    }

    public void StartNav(Vector3 des, Action desCB = null)
    {
        isNaving = true;
        nav.enabled = true;
        float dis = Vector3.SqrMagnitude(transform.position - des);
        ctrl.enabled = false;
        nav.enabled = true;
        nav.speed = Constans.PlayerMoveSpeed;
        nav.SetDestination(des);
        SetBlend(1f);
        if (desCB != null) navDesCB = desCB;
    }
    public void StopNav()
    {
        if (!isNaving) return;
        isNaving = false;
        nav.isStopped = true;
        nav.enabled = false;
        ctrl.enabled = true;
        SetBlend(0f);
    }

    public override void SetBlend(float blend)
    {
        targetBlend = blend;
    }
    private void UpdateMixBlend()
    {
        curBlend = Mathf.Lerp(curBlend, targetBlend, Constans.AcceleSpeed * Time.deltaTime);
        animator.SetFloat("Blend", curBlend);
    }

    public override void SetFX(string name, float dur)
    {
        GameObject fx = null;
        if(fxDic.TryGetValue(name, out fx))
        {
            fx.SetActive(true);
            timerSev.AddTimerTask((tid) =>
            {
                fx.SetActive(false);
            }, dur);
        }
    }
}
