/*********************************************************
	文件：Test
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/23 13:44:57
	功能：待定
***********************************************************/
using BJTimer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;

public class Test : MonoBehaviour
{
    private TimerSev timeSys;
    private Queue<IDPack> queue = new Queue<IDPack>();
    private float timer;
    Stopwatch watch;
    // Start is called before the first frame update
    void Start()
    {
        watch = new Stopwatch();
        watch.Start();
        //timeSys = TimerSystem.Instance;
        //timeSys.Init();

        //timeSys.StartTimer();

        //for(int i = 0; i < 50; i++)
        //{
        //    IDPack id = timeSys.AddTimerTask((tid) => {  }, 2, 0, TimeUnit.Secound);
        //    queue.Enqueue(id);
        //}
        //for(int i = 0; i < 50; i++)
        //{
        //    IDPack id = timeSys.AddFrameTask((tid) => {  }, 10, 5);
        //    queue.Enqueue(id);
        //}

        // 计时任务：10s
        // 创建任务时，记录任务创建时的时间 createTime 
        // 在 Update 中，获取当前时间 curTime：Time.realtimeSinceStartup
        // 计算 createTime + 10 * 1000 > cutTime  => 执行计时任务回调
        // UTC 世界时间


    }

    private void Update()
    {
        UnityEngine.Debug.Log(watch.Elapsed.TotalMilliseconds);
        watch.Restart();
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    while(queue.Count > 0)
        //    {
        //        IDPack tid = queue.Dequeue();
        //        timeSys.ReplaceTimeTask(tid.id, (_id) => {  }, 500, 2);
        //        if (tid.type == TaskType.TimeTask) timeSys.DeleteTimeTask(tid.id);
        //        else timeSys.DeleteFrameTask(tid.id);
        //    }
        //}
    }

}
