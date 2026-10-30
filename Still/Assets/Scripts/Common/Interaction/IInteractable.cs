namespace Still.Common
{
    ///<summary>
    ///プレイヤーが調べられるオブジェクト
    ///</summary>
    public interface IInteractable
    {
        ///<summary>
        ///調べる
        ///</summary>
        void Interact();

        ///<summary>
        ///操作ガイドを表示する
        ///</summary>
        void ShowUI();

        ///<summary>
        ///操作ガイドを隠す
        ///</summary>
        void HideUI();
    }
}
