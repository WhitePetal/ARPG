/*
* Tencent is pleased to support the open source community by making xLua available.
* Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
* Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
* http://opensource.org/licenses/MIT
* Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;

[System.Serializable]
public class Injection
{
    public string name;
    public GameObject value;
}

[LuaCallCSharp]
public class LuaBehaviour: MonoBehaviour
{
    public static Dictionary<string, object> componentDic = new Dictionary<string, object>();

    public TextAsset luaScript;
    public string luaScriptPath;
    public Injection[] injections;

    internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    protected Action luaStart;
    protected Action luaUpdate;
    protected Action luaOnDestroy;

    public LuaTable scriptEnv;

    public LuaTable GetScriptEnv()
    {
        return scriptEnv;
    }

    protected virtual void Awake()
    {
        scriptEnv = luaEnv.NewTable();

        // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", this);
        Debug.Log(this.name + "Set Self!!");
        if(injections != null)
        {
            foreach (var injection in injections)
            {
                scriptEnv.Set(injection.name, injection.value);
            }
        }

        if(!string.IsNullOrEmpty(luaScriptPath)) 
        {
            luaScript = Resources.Load<TextAsset>(luaScriptPath);
        }
        componentDic.Add(transform.name ,luaEnv.DoString(luaScript.text, luaScript.name, scriptEnv));
        Debug.Log(transform.name);
        luaEnv.Global.Set("ComponentTable", componentDic);

        Action luaAwake = scriptEnv.Get<Action>("awake");
        scriptEnv.Get("start", out luaStart);
        scriptEnv.Get("update", out luaUpdate);
        scriptEnv.Get("ondestroy", out luaOnDestroy);

        if (luaAwake != null)
        {
            luaAwake();
        }
    }

    // Use this for initialization
    protected virtual void Start()
    {
        if (luaStart != null)
        {
            luaStart();
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (luaUpdate != null)
        {
            luaUpdate();
        }
        if (Time.time - LuaBehaviour.lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            LuaBehaviour.lastGCTime = Time.time;
        }
    }

    void OnDestroy()
    {
        if (luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        scriptEnv.Dispose();
        injections = null;
    }
}
