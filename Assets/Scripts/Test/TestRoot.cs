/*********************************************************
	文件：TestRoot
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/9/4 16:43:37
	功能：待定
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoot : MonoBehaviour
{
    Dictionary<int, TransformTest> tts = new Dictionary<int, TransformTest>();
    public float globalDeltaTime;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 100; ++i)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(transform);
            tts.Add(i, go.AddComponent<TransformTest>());
            tts[i].root = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        globalDeltaTime = Time.deltaTime;

        for(int i = 0; i < 10000; ++i)
        {
            foreach (KeyValuePair<int, TransformTest> pair in tts)
            {
                pair.Value.Trick();
            }
        }
    }
}
