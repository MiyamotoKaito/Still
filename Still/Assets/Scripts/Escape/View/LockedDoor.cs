using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///ミッションで解錠されるまで開かないドア（出口・階段室・鍵の掛かった部屋）
    ///</summary>
    public class LockedDoor : DoorView
    {
        ///<summary>ミッションから指定するID</summary>
        public TargetId Id => new TargetId(_doorId);
        ///<summary>開けたときにプレイヤーを移動させる先（階段室用。無ければnull）</summary>
        public Transform WarpDestination => _warpDestination;

        [SerializeField, Tooltip("ミッションから指定するID")] private string _doorId = "Exit";
        [SerializeField, Tooltip("施錠中に出す操作ガイド")] private string _lockedLabel = "Locked\nCollect The Key!!";
        [SerializeField, Tooltip("開けたときに移動させる先（階段室用）")] private Transform _warpDestination;

        private bool _isLocked = true;

        ///<summary>
        ///施錠状態を表示に反映する
        ///</summary>
        public void SetLocked(bool isLocked)
        {
            _isLocked = isLocked;
        }

        ///<summary>
        ///施錠中は鍵を探すよう促す
        ///</summary>
        public override void ShowUI()
        {
            if (!_isLocked)
            {
                base.ShowUI();
                return;
            }

            _text.gameObject.SetActive(true);
            _text.text = _lockedLabel;
        }
    }
}
