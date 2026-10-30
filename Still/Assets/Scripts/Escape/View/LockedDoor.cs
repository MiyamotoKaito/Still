namespace Still.Escape
{
    ///<summary>
    ///鍵が無いと開かない出口の扉
    ///</summary>
    public class LockedDoor : DoorView
    {
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
            _text.text = "Locked\nCollect The Key!!";
        }
    }
}
