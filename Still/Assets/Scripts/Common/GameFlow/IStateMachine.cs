using Cysharp.Threading.Tasks;
using R3;

namespace Still.Common
{
    ///<summary>
    ///モード遷移を管理する
    ///</summary>
    public interface IStateMachine
    {
        ///<summary>現在のモード</summary>
        ReadOnlyReactiveProperty<GameModeType> CurrentMode { get; }

        ///<summary>
        ///Stateを登録する
        ///</summary>
        void AddState(IState state);

        ///<summary>
        ///Stateの登録を解除する（スコープ破棄時に必ず呼ぶ）
        ///</summary>
        void RemoveState(IState state);

        ///<summary>
        ///指定したモードへ遷移する。遷移中の要求は無視する
        ///</summary>
        UniTask ChangeStateAsync(GameModeType gameModeType);
    }
}
