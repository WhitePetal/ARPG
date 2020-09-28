/*********************************************************
	文件：TestPlayer
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/31 8:46:19
	功能：待定
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    private Animator animator;
    private Transform camTrasn;
    private Vector3 camOffset;
    private CharacterController ctrl;



    protected bool isMove = false;
    protected Vector2 dir = Vector2.zero;
    public Vector2 Dir
    {
        get { return dir; }
        set
        {
            if (value == Vector2.zero) isMove = false;
            else
            {
                isMove = true;
            }
            dir = value;
        }
    }

    private float targetBlend;
    private float curBlend;
    // Start is called before the first frame update
    void Start()
    {
        camTrasn = Camera.main.transform;
        camOffset = transform.position - camTrasn.position;
        ctrl = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        #region Input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector2 _dir = new Vector2(h, v).normalized;

        if (_dir != Vector2.zero)
        {
            Dir = _dir;
            SetBlend(1f);
        }
        else
        {
            Dir = Vector2.zero;
            SetBlend(0f);
        }
        #endregion

        if (curBlend != targetBlend) UpdateMixBlend();

        if (isMove)
        {
            // 设置方向
            SetDir();
            // 产生移动
            SetMove();
        }
        // 相机跟随
        SetCamera();
    }

    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, Vector2.up);
        Vector3 eulerAngle = new Vector3(0, angle + camTrasn.eulerAngles.y, 0);
        transform.localEulerAngles = eulerAngle;
    }

    private void SetMove()
    {
        ctrl.Move(transform.forward * Time.deltaTime * Constans.PlayerMoveSpeed);
    }

    private void SetCamera()
    {
        if (camTrasn != null)
        {
            camTrasn.position = transform.position - camOffset;
        }
    }


    public void SetBlend(float blend)
    {
        targetBlend = blend;
    }
    private void UpdateMixBlend()
    {
        curBlend = Mathf.Lerp(curBlend, targetBlend, Constans.AcceleSpeed * Time.deltaTime);
        animator.SetFloat("Blend", curBlend);
    }
}
