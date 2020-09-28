/*********************************************************
	文件：IState
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/30 15:51:48
	功能：状态接口
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Enter(EntityBase entity, StateParam param = default);

    void Process(EntityBase entity, StateParam param = default);

    void Exit(EntityBase entity, StateParam param = default);
}

public enum AnimState
{
    None,
    Idle,
    Move,
    Attack,
    Born,
    Die,
    Hit
}

public struct StateParam
{
    public StateParamType type;
    public int _int;
    public float _float;
    public string _str;
    public bool _bool;

    public StateParam(int _int)
    {
        type = StateParamType.Int;
        this._int = _int;
        this._float = 0f;
        this._str = null;
        this._bool = false;
    }
    public StateParam(float _float)
    {
        type = StateParamType.Float;
        this._int = 0;
        this._float = _float;
        this._str = null;
        this._bool = false;
    }
    public StateParam(string _str)
    {
        type = StateParamType.String;
        this._int = 0;
        this._float = 0f;
        this._str = _str;
        this._bool = false;
    }
    public StateParam(bool _bool)
    {
        type = StateParamType.Bool;
        this._int = 0;
        this._float = 0f;
        this._str = null;
        this._bool = _bool;
    }
}
public enum StateParamType
{
    None,
    Int,
    Float,
    String,
    Bool
}
