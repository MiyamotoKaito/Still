using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Still.Common
{
    ///<summary>
    ///各モジュールのInstallerの基底。LifetimeScopeはこれを並べて回すだけにする
    ///</summary>
    public abstract class InstallerObject : ScriptableObject, IInstaller
    {
        ///<summary>
        ///自モジュールの登録だけを書く
        ///</summary>
        public abstract void Install(IContainerBuilder builder);
    }
}
