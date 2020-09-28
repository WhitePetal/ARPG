/*********************************************************
	文件：Controller
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/30 16:51:52
	功能：表现实体控制类
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    protected TimerSev timerSev;

    protected Transform camTrasn;
    protected AkEvent sound;
    protected GameObject soundObj;

    public Animator animator;
    protected CharacterController ctrl;

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

    protected bool skillMove = false;
    protected float skillMoveSpeed = 0f;

    protected Dictionary<string, GameObject> fxDic = new Dictionary<string, GameObject>();

    [HideInInspector] public Transform _transform;
    [HideInInspector] public GameObject _gameObject;

    [HideInInspector] public Transform hpRoot;


    public virtual void Init()
    {
        timerSev = TimerSev.Instance;
        camTrasn = Camera.main.transform;
        _transform = transform;
        _gameObject = gameObject;
        animator = GetComponent<Animator>();
        hpRoot = transform.Find("hpRoot");
        ctrl = GetComponent<CharacterController>();
        Transform st = transform.Find("Sound");
        soundObj = st == null ? null : st.gameObject;
        sound = soundObj == null ? null : soundObj.GetComponent<AkEvent>();
    }

    protected virtual void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, Vector2.up);
        Vector3 eulerAngle = new Vector3(0, angle + camTrasn.eulerAngles.y, 0);
        transform.localEulerAngles = eulerAngle;
    }

    protected virtual void SetMove()
    {
        ctrl.Move(transform.forward * timerSev.deltaTime * Constans.PlayerMoveSpeed);
    }

    protected virtual void SetSkillMove()
    {
        ctrl.Move(transform.forward * timerSev.deltaTime * skillMoveSpeed);
    }

    public virtual void SetBlend(float blend)
    {
        animator.SetFloat("Blend", blend);
    }

    public virtual void SetAction(int action)
    {
        animator.SetInteger("Action", action);
    }

    public virtual void SetFX(string name, float dur)
    {

    }

    public void SetSkillMoveState(bool move, float speed = 0f)
    {
        skillMove = move;
        skillMoveSpeed = speed;
    }

    public virtual void SetAtkDirCam(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(dir, Vector2.up);
        Vector3 eulerAngle = new Vector3(0, angle + camTrasn.eulerAngles.y, 0);
        transform.localEulerAngles = eulerAngle;
    }

    public virtual void SetAtkDirLocal(Vector2 dir)
    {
        Vector3 eulerAngle = new Vector3(0, Vector2.SignedAngle(dir, Vector2.up), 0);
        transform.localEulerAngles = eulerAngle;
    }

    public virtual void PlayAudio(string name, string swtich)
    {
        AkSoundEngine.SetSwitch(name, swtich, soundObj);
        sound.HandleEvent(soundObj);
    }
}
