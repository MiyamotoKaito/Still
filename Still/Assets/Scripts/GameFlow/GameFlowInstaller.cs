using Still.Common;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Still.GameFlow
{
    ///<summary>
    ///モード（タイトル／インゲーム）とシーンの対応を登録する。Rootスコープで使う
    ///</summary>
    [CreateAssetMenu(fileName = "GameFlowInstaller", menuName = "Installer/GameFlow")]
    public sealed class GameFlowInstaller : InstallerObject
    {
        [SerializeField] private string _titleSceneName = "Title";
        [SerializeField] private string _inGameSceneName = "MasterScene";

        ///<summary>
        ///登録する
        ///</summary>
        public override void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameBoot>()
                .WithParameter("titleSceneName", _titleSceneName)
                .WithParameter("inGameSceneName", _inGameSceneName);
        }
    }
}
