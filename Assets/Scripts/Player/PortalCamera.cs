using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    [SerializeField] PortalScript[] _portals = new PortalScript[2];
    [SerializeField] Camera _portalCamera = default;
        [SerializeField] int _iterations = 7;

    RenderTexture _texture1;
    RenderTexture _texture2;
    Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = GetComponent<Camera>();

        _texture1 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        _texture2 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    }

    private void Start()
    {
        _portals[0].SetTexture(_texture1);
        _portals[1].SetTexture(_texture2);
    }

    private void OnPreRender()
    {
        //この辺は無駄なレンダリングをしないため
        if (!_portals[0].IsPlaced() || !_portals[1].IsPlaced())
        {
            return;
        }

        if (_portals[0].IsRendererVisible())
        {
            _portalCamera.targetTexture = _texture1;
            for (int i = _iterations - 1; i >= 0; i--)
            {
                RenderCamera(_portals[0], _portals[1], i);
            }
        }

        if (_portals[1].IsRendererVisible())
        {
            _portalCamera.targetTexture = _texture2;
            for (int i = _iterations - 1; i >= 0; i--)
            {
                RenderCamera(_portals[1], _portals[0], i);
            }
        }
    }

    private void RenderCamera(PortalScript inPortal, PortalScript outPortal, int iterationID)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        Transform cameraTransform = _portalCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for (int i = 0; i <= iterationID; i++)
        {
            // もう一方のポータルの後ろにカメラを配置
            //InverseTransformPoint(位置をワールド→ローカルへと変える)
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            // カメラを回転させて、もう一方のポータルを覗く
            //Quaternion.Inverse(逆のQuaternionを作る)
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        // Set the camera's oblique view frustum.
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(_portalCamera.worldToCameraMatrix)) * clipPlane;

        var newMatrix = _mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        _portalCamera.projectionMatrix = newMatrix;

        // Render the camera to its render target.
        _portalCamera.Render();
    }
}