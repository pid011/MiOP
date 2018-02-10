using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Utils;
using MiOP.API;
using System.Collections.Generic;
using System.Text;

namespace MiOP.Box.Commands
{
    public class PermissionCommand
    {
        private readonly PluginContext context;

        private Dictionary<string, string> helpText = new Dictionary<string, string>
        {
            { "list", $"{ChatColors.Yellow}/permission list - 권한을 가진 유저 전체 목록 조회" }
        };

        public PermissionCommand(PluginContext context)
        {
            this.context = context;
        }

        [Command(Description = "권한 관련 명령어입니다.")]
        public void Permission(Player player, string arg)
        {
            if (!PermissionManager.CheckCurrentUserPermission(player))
            {
                return;
            }

            if (arg == "list")
            {
                List<string> msgs = MakeupList();
                foreach (var item in msgs)
                {
                    player.SendMessage(item);
                }
            }
            else
            {
                player.SendMessage(helpText["list"]);
            }
        }

        private List<string> MakeupList()
        {
            List<string> makeupText = new List<string>();
            List<string> op = PermissionManager.OPList;
            List<string> admin = PermissionManager.AdminList;

            StringBuilder sb = new StringBuilder();

            makeupText.Add("=== LIST ===");
            makeupText.Add($"{ChatColors.Red}RED{ChatColors.White} = Admin, {ChatColors.Green}GREEN{ChatColors.White} = OP");
            sb.Append(ChatColors.Gold);
            int i = 1;
            foreach (var item in admin)
            {
                sb.Append($"[{ChatColors.Red}{item}{ChatColors.Gold}] ");
                if (i % 5 == 0)
                {
                    makeupText.Add(sb.ToString());
                    sb.Clear();
                    sb.Append(ChatColors.Gold);
                }
                i++;
            }
            foreach (var item in op)
            {
                sb.Append($"[{ChatColors.Green}{item}{ChatColors.Gold}] ");
                if (i % 5 == 0)
                {
                    makeupText.Add(sb.ToString());
                    sb.Clear();
                    sb.Append(ChatColors.Gold);
                }
                i++;
            }
            makeupText.Add(sb.ToString());
            makeupText.Add($"총 {op.Count + admin.Count}명의 Admin과 OP가 있습니다.");

            return makeupText;
        }
    }
}