using log4net;
using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Utils;
using System.Collections.Generic;
using System.Text;
using MiOP.API;
using MiOP.Box.Commands;

namespace MiOP.Box
{
    /// <summary>
    /// MiOP 메인 클래스.
    /// </summary>
    [Plugin(Author = "Sepi", Description = "MiNET에서 권한기능을 사용할 수 있습니다.",
        PluginName = "MiOP", PluginVersion = "v2.0")]
    public class MiOP : Plugin
    {
        /// <summary>
        /// 시작시 실행될 함수.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            PermissionManager.TryCreateOpsTxt();
            LoadCommands();
        }

        private void LoadCommands()
        {
            Context.PluginManager.LoadCommands(new OPCommand(Context));
            Context.PluginManager.LoadCommands(new PermissionCommand(Context));
        }
    }
}