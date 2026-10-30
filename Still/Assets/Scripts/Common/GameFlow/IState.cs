using System.Threading;
using Cysharp.Threading.Tasks;

namespace Still.Common
{
    ///<summary>
    ///StateMachineに登録するモード状態
    ///</summary>
    public interface IState
    {
        ///<summary>このStateが表すモード</summary>
        GameModeType GameModeType { get; }

        ///<summary>
        ///モードに入るときの処理（シーンのロードなど）
        ///</summary>
        UniTask EnterAsync(CancellationToken ct);

        ///<summary>
        ///モードから出るときの処理（シーンのアンロードなど）
        ///</summary>
        UniTask ExitAsync(CancellationToken ct);
    }
}
