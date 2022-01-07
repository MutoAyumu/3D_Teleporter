using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class PortalCamera : MonoBehaviour
{
    [SerializeField] PortalScript[] _portals = new PortalScript[2];
    [SerializeField] Camera _portalCam = default;
    [SerializeField] int _iterations = 7;
    [SerializeField] float _near = 0.3f;
    [SerializeField] float _far = 1000f;

    RenderTexture _tempTexture1 = default;
    RenderTexture _tempTexture2 = default;
    Camera _mainCam = default;
    private void Awake()
    {
        _mainCam = GetComponent<Camera>();

        _tempTexture1 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        _tempTexture2 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    }
    private void Start()
    {
        _portals[0].Renderer.material.mainTexture = _tempTexture1;
        _portals[1].Renderer.material.mainTexture = _tempTexture2;
    }
    private void OnEnable()
    {
        RenderPipeline.beginCameraRendering += UpdateCamera;
    }
    private void OnDisable()
    {
        RenderPipeline.beginCameraRendering -= UpdateCamera;
    }
    void UpdateCamera(ScriptableRenderContext SRC, Camera cam)
    {
        if(!_portals[0].IsPlaced || !_portals[1].IsPlaced)
        {
            return;
        }

        if(_portals[0].Renderer.isVisible)
        {
            _portalCam.targetTexture = _tempTexture1;

            for(int i = _iterations - 1; i >= 0; --i)
            {
                RenderCamera(_portals[0], _portals[1], i, SRC);
            }
        }

        if (_portals[1].Renderer.isVisible)
        {
            _portalCam.targetTexture = _tempTexture2;

            for (int i = _iterations - 1; i >= 0; --i)
            {
                RenderCamera(_portals[1], _portals[0], i, SRC);
            }
        }
    }
    void RenderCamera(PortalScript inPortal, PortalScript outPortal, int iterationID, ScriptableRenderContext SRC)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        Transform cameraTransform = _portalCam.transform;
        cameraTransform.position = this.transform.position;
        cameraTransform.rotation = this.transform.rotation;

        for(int i = 0; i <= iterationID; ++i)
        {
            //もう片方のポータルの背後にカメラを設置させる
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            //もう一方ポータルのから覗くようにさせるためにカメラを回転させている
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        //カメラの視野角を作成して設定する
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlaneWorldSpase = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipCameraSpase = Matrix4x4.Transpose(Matrix4x4.Inverse(_portalCam.worldToCameraMatrix)) * clipPlaneWorldSpase;

        var newMatrix = _mainCam.CalculateObliqueMatrix(clipCameraSpase);
        _portalCam.projectionMatrix = newMatrix;

        _portalCam.nearClipPlane = _near;
        _portalCam.farClipPlane = _far;

        //カメラのrenderTargetにセット
        UniversalRenderPipeline.RenderSingleCamera(SRC, _portalCam);
    }
}
