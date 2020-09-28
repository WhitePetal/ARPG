/*********************************************************
	文件：HPItem
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/9/13 10:21:38
	功能：血条物体
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPItem : WindowRoot
{
    private Image imgHpGray;
    private Image imgHpRed;
    private Text txtDamage;
    private Text txtCritical;

    private Animation criticalAnim;
    private Animation dodgeAnim;
    private Animation damageAnim;

    private int hp;

    private Transform transRoot;
    private RectTransform rectTrans;
    private float scaleRate = 1.0f * Constans.ScreenStandardHeight / Screen.height;

    private void Awake()
    {
        imgHpGray = FindComponent<Image>("fgGray");
        imgHpRed = FindComponent<Image>("fgRed");
        txtDamage = FindComponent<Text>("txtDamage");
        txtCritical = FindComponent<Text>("txtCritical");

        criticalAnim = txtCritical.GetComponent<Animation>();
        damageAnim = txtDamage.GetComponent<Animation>();
        dodgeAnim = FindComponent<Animation>("txtDodge");

        rectTrans = transform.GetComponent<RectTransform>();
    }

    public void SetItemInfo(int hp, Transform root)
    {
        this.hp = hp;
        transRoot = root;

        timerSev.AddFrameTask((tid) =>
        {
            rectTrans.anchoredPosition = Camera.main.WorldToScreenPoint(transRoot.position) * scaleRate;
        }, 0, 0);
    }

    public void SetCritical(int critical)
    {
        criticalAnim.Stop();
        txtCritical.text = string.Format("暴击：{0}", critical);
        criticalAnim.Play();
    }

    public void SetDamage(int damage)
    {
        damageAnim.Stop();
        txtDamage.text = string.Format("-{0}", damage);
        damageAnim.Play();
    }

    public void SetDodge()
    {
        dodgeAnim.Stop();
        dodgeAnim.Play();
    }

    private float curHpPrg;
    private float targetHpPrg;
    public void SetHp(int oldVal, int newVal)
    {
        curHpPrg = 1f * oldVal / hp;
        targetHpPrg = 1f * newVal / hp;
        imgHpRed.fillAmount = targetHpPrg;
        timerSev.AddFrameTask((tid) =>
        {
            if (curHpPrg != targetHpPrg)
            {
                curHpPrg = Mathf.Lerp(curHpPrg, targetHpPrg, Constans.HpAcceleSpeed * TimerSev.Instance.deltaTime);
                imgHpGray.fillAmount = curHpPrg;
            }
            else TimerSev.Instance.DeleteFrameTask(tid);
        }, 0, 0);
    }

}
