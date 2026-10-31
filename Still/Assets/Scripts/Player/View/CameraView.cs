using Unity.Cinemachine;
using UnityEngine;

namespace Still.Player
{
    ///<summary>
    ///プレイヤーの一人称カメラ（CinemachineCameraと同じGameObjectに付ける）
    ///</summary>
    public class CameraView : MonoBehaviour
    {
        ///<summary>レンズ設定を持つカメラ</summary>
        public CinemachineCamera CinemachineCamera => _cinemachineCamera != null ? _cinemachineCamera : _cinemachineCamera = GetComponent<CinemachineCamera>();

        // PresenterはスコープのAwake（このViewのAwakeより前）で購読するため、取得を遅らせる
        private CinemachineInputAxisController InputAxisController => _inputAxisController != null ? _inputAxisController : _inputAxisController = GetComponent<CinemachineInputAxisController>();

        private CinemachineCamera _cinemachineCamera;
        private CinemachineInputAxisController _inputAxisController;

        ///<summary>
        ///視野角を反映する
        ///</summary>
        public void SetFieldOfView(float fov)
        {
            CinemachineCamera.Lens.FieldOfView = fov;
        }

        ///<summary>
        ///カメラを前後軸まわりに傾ける（めまい・揺れの演出用）
        ///</summary>
        public void SetDutch(float degrees)
        {
            CinemachineCamera.Lens.Dutch = degrees;
        }

        ///<summary>
        ///マウスによる視点操作の有効／無効を切り替える
        ///</summary>
        public void SetLookInputEnabled(bool isEnabled)
        {
            InputAxisController.enabled = isEnabled;
        }
    }
}
