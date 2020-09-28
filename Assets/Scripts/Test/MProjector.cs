/********************************************************************
 FileName: ProjectorEffect.cs
 Description: Projector原理，自定义实现
 history: 4:1:2019 by puppet_master
 https://blog.csdn.net/puppet_master
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MProjector : MonoBehaviour
{

    private Camera projectorCam = null;
    public Material projectorMaterial = null;


    private void Awake()
    {
        projectorCam = GetComponent<Camera>();
    }

    private void Update()
    {
        var projectionMatrix = projectorCam.projectionMatrix;
        projectionMatrix = GL.GetGPUProjectionMatrix(projectionMatrix, false);
        var viewMatirx = projectorCam.worldToCameraMatrix;
        var vpMatrix = projectionMatrix * viewMatirx;
        projectorMaterial.SetMatrix("_ProjectorVPMatrix", vpMatrix);
    }


}