/*********************************************************
	文件：TransformTest
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/9/4 12:19:36
	功能：待定
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTest : MonoBehaviour
{
    private Transform _transform;
    private Vector3 _localPosition;
    private Vector3 dir = new Vector3(0.1f, 0.1f, 0.1f);
    public TestRoot root;
    //public Vector3 Dir { get => dir; set => dir = value; }
    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;
        _localPosition = _transform.localPosition;
    }

    public void Trick()
    {
        _localPosition.x += dir.x * root.globalDeltaTime;
        _localPosition.y += dir.y * root.globalDeltaTime;
        _localPosition.z += dir.z * root.globalDeltaTime;
        _transform.localPosition = _localPosition;
    }
}
