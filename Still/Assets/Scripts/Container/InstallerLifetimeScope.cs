using System.Collections.Generic;
using Still.Common;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Still.Container
{
    ///<summary>
    ///Installerを集めて回すだけのLifetimeScope
    ///</summary>
    public abstract class InstallerLifetimeScope : LifetimeScope
    {
        [SerializeField] private List<InstallerObject> _installers = new();

        protected override void Configure(IContainerBuilder builder)
        {
            foreach (var installer in _installers) installer.Install(builder);
        }
    }
}
