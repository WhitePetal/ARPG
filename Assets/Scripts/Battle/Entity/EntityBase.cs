/*********************************************************
	文件：EntityBase
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/30 15:55:18
	功能：实体基类
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase
{
    public EntityType entityType;

    protected BattleProps props;
    public BattleProps Props { get => props; protected set => props = value; }
    protected int hp;
    public int Hp
    {
        get => hp;
        set
        {
            SetHp(hp, value);
            hp = value;
        }
    }
    protected int lv;
    public int LV { get => lv; private set => lv = value; }

    protected bool canControl = true;
    public bool CanControl
    {
        get { return canControl; }
        protected set { canControl = value; }
    }

    public bool canSkill = true;

    public AnimState curState = AnimState.None;
    public EntityState entityState = EntityState.None;

    protected BattleMgr battleMgr = null;
    public BattleMgr BattleMgr
    {
        get { return battleMgr; }
    }
    protected StateMgr stateMgr = null;
    protected SkillMgr skillMgr = null;
    protected Controller ctrl;

    public SkillCfg curSkillCfg;

    public Transform _transform { get => ctrl._transform; }
    public GameObject _gameObject { get => ctrl._gameObject; }

    public int nextCombo = 0;
    public Queue<int> toIdleSkillQue = new Queue<int>();

    public HashSet<int> sklMoveCBSet = new HashSet<int>();
    public HashSet<int> sklActCBSet = new HashSet<int>();

    public void SetLv(int lv)
    {
        this.lv = lv;
    }

    public void SetBattleMgr(BattleMgr mgr)
    {
        battleMgr = mgr;
    }
    public void SetStateMgr(StateMgr mgr)
    {
        this.stateMgr = mgr;
    }
    public void SetController(Controller ctrl)
    {
        this.ctrl = ctrl;
        ctrl.Init();
    }
    public void SetSkillMgr(SkillMgr mgr)
    {
        skillMgr = mgr;
    }
    public virtual void SetBattlePorps(BattleProps props)
    {
        hp = props.hp;
        this.props = props;
    }

    public virtual bool GetBreakState()
    {
        return true;
    }

    public void ReleaseControl()
    {
        canControl = false;
    }
    public void HandleControl()
    {
        canControl = true;
    }

    public virtual Vector2 GetDirInput()
    {
        return Vector2.zero;
    }

    public virtual void SetBlend(float blend)
    {
        if (ctrl != null) ctrl.SetBlend(blend);
    }

    public virtual void SetDir(Vector2 dir)
    {
        if (ctrl != null) ctrl.Dir = dir;
    }

    #region State
    public virtual void Move()
    {
        stateMgr.ChangeState(this, AnimState.Move);
    }
    public virtual void Idle()
    {
        stateMgr.ChangeState(this, AnimState.Idle);
    }
    public virtual void Attack(int skillId, bool normal = false)
    {
        StateParam param = new StateParam();
        param._int = skillId;
        param._bool = normal;
        stateMgr.ChangeState(this, AnimState.Attack, param);
    }
    public virtual void Born()
    {
        stateMgr.ChangeState(this, AnimState.Born);
    }
    public virtual void Die()
    {
        stateMgr.ChangeState(this, AnimState.Die);
    }
    public virtual void Hit()
    {
        stateMgr.ChangeState(this, AnimState.Hit);
    }
    #endregion

    #region Skill
    public virtual void SkillAttack(int skillId)
    {
        skillMgr.SkillAttack(this, skillId);
    }
    public virtual void SetAction(int action)
    {
        if (ctrl != null) ctrl.SetAction(action);
    }
    public virtual void SetFX(string name, float dur)
    {
        if (ctrl != null) ctrl.SetFX(name, dur);
    }
    public virtual void SetSkillMoveState(bool move, float moveSpeed = 0f)
    {
        if (ctrl != null) ctrl.SetSkillMoveState(move, moveSpeed);
    }

    public virtual void SetAtkDir(Vector2 dir, bool offset = false)
    {
        if (ctrl != null)
        {
            if (offset)
            {
                ctrl.SetAtkDirCam(dir);
            }
            else
            {
                ctrl.SetAtkDirLocal(dir);
            }
        }
    }

    public virtual Vector2 CalcTargetDir()
    {
        return Vector2.zero;
    }
    #endregion

    #region HeadUI
    public virtual Transform GetHpRoot()
    {
        return ctrl.hpRoot;
    }
    public virtual void SetDodge()
    {
        GameRoot.Instance.dynamicWindow.SetDodge(_gameObject.name);
    }
    public virtual void SetDamage(int damage)
    {
        GameRoot.Instance.dynamicWindow.SetDamage(_gameObject.name, damage);
    }
    public virtual void SetCritical(int critical)
    {
        GameRoot.Instance.dynamicWindow.SetCritical(_gameObject.name, critical);
    }
    public virtual void SetHp(int oldVal, int newVal)
    {
        GameRoot.Instance.dynamicWindow.SetHp(_gameObject.name, oldVal, newVal);
    }
    #endregion

    #region AI
    protected virtual void TickAILogic() { }
    public virtual void RegisterAILogic() { }
    #endregion

    #region Audio
    public virtual void PlayAudio(string name, string swtich)
    {
        ctrl.PlayAudio(name, swtich);
    }
    #endregion

    public Vector3 GetPos()
    {
        return _transform.position;
    }
    public Transform GetTransform()
    {
        return _transform;
    }
    public virtual AnimationClip[] GetAnimRunClips()
    {
        return ctrl.animator.runtimeAnimatorController.animationClips;
    }
}
