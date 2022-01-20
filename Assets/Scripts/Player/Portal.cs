using UnityEngine;

public class Portal : MonoBehaviour
{
    public Camera MainCamera;
    // One to render to texture, and another to render normally to switch between (preview)
    public Camera[] PortalCameras;
    public Transform Source;
    public Transform Destination;

    private void LateUpdate()
    {
        foreach (var portalCamera in PortalCameras)
        {
            // PortalCameraがMainCameraの鏡像となるようにSourceを180度回転させる。
            Matrix4x4 destinationFlipRotation =
                Matrix4x4.TRS(MathUtil.ZeroV3, Quaternion.AngleAxis(180.0f, Vector3.up), MathUtil.OneV3);
            Matrix4x4 sourceInvMat = destinationFlipRotation * Source.worldToLocalMatrix;

            // ソース空間におけるMainCameraの移動と回転を計算する。
            Vector3 cameraPositionInSourceSpace =
                MathUtil.ToV3(sourceInvMat * MathUtil.PosToV4(MainCamera.transform.position));
            Quaternion cameraRotationInSourceSpace =
                MathUtil.QuaternionFromMatrix(sourceInvMat) * MainCamera.transform.rotation;

            // PortalカメラをDestination transformを基準にWorld Spaceにトランスフォームする。
            // メインカメラの位置・向きに合わせる
            portalCamera.transform.position = Destination.TransformPoint(cameraPositionInSourceSpace);
            portalCamera.transform.rotation = Destination.rotation * cameraRotationInSourceSpace;

            // ポータルのクリッププレーンを計算（デスティネーションカメラとポータルの間にあるオブジェクトをカリングするため）
            Vector4 clipPlaneWorldSpace =
                new Vector4(
                    Destination.forward.x,
                    Destination.forward.y,
                    Destination.forward.z,
                    Vector3.Dot(Destination.position, -Destination.forward));

            Vector4 clipPlaneCameraSpace =
                Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

            // 新しいクリッププレーンに基づく投影の更新
            // Note: http://aras-p.info/texts/obliqueortho.html and http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
            portalCamera.projectionMatrix = MainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
    }
}