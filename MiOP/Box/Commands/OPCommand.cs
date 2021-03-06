﻿using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Utils;
using MiOP.API;
using System.Collections.Generic;

namespace MiOP.Box.Commands
{
    public class OPCommand
    {
        private List<string> ops = new List<string>();

        private readonly PluginContext context;

        private Dictionary<string, string> helpText = new Dictionary<string, string>
        {
            { "add", $"{ChatColors.Yellow}/op add [ name ] - OP 추가" },
            { "rm", $"{ChatColors.Yellow}/op rm [ name ] - op삭제" }
        };

        private enum Commands
        { add, rm, list }

        public OPCommand(PluginContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Permission 명령어
        /// </summary>
        /// <param name="player"></param>
        [Command(Description = "OP 관련 명령어입니다. OP 또는 Admin만 사용가능합니다.")]
        public void OP(Player player)
        {
            if (!PermissionManager.CheckCurrentUserPermission(player))
            {
                return;
            }

            player.SendMessage(helpText["add"]);
            player.SendMessage(helpText["rm"]);
        }

        /// <summary>
        /// Permission 명령어
        /// </summary>
        /// <param name="player"></param>
        /// <param name="args"></param>
        [Command]
        public void OP(Player player, string args)
        {
            if (!PermissionManager.CheckCurrentUserPermission(player))
            {
                return;
            }

            char[] texts = args.ToCharArray();
            foreach (var t in texts)
            {
                if ("add".Contains(t.ToString()))
                {
                    player.SendMessage($"이 명령어를 찾나요? -> {ChatColors.Gold}{helpText["add"]}");
                    break;
                }
                else if ("rm".Contains(t.ToString()))
                {
                    player.SendMessage($"이 명령어를 찾나요? -> {ChatColors.Gold}{helpText["rm"]}");
                    break;
                }
                else
                {
                    player.SendMessage(helpText["add"]);
                    player.SendMessage(helpText["rm"]);
                    break;
                }
            }
        }

        /// <summary>
        /// Permission 명령어
        /// </summary>
        /// <param name="player"></param>
        /// <param name="args1"></param>
        /// <param name="args2"></param>
        [Command]
        public void OP(Player player, string args1, string args2)
        {
            if (!PermissionManager.CheckCurrentUserPermission(player))
            {
                return;
            }

            string msg;
            if (args1 == "add")
            {
                if (PermissionManager.Add(args2))
                {
                    msg = $"{args2}님을 성공적으로 추가 하였습니다!";
                }
                else
                {
                    msg = "추가에 실패하였습니다. ";
                    if (PermissionManager.IsOP(args2))
                    {
                        msg += $"{args2}님은 이미 op입니다.";
                    }
                    else if (PermissionManager.IsAdmin(args2))
                    {
                        msg += $"{args2}님은 이미 admin입니다.";
                    }
                    else
                    {
                        msg += $"내부적 오류입니다.";
                    }
                }
                player.SendMessage(msg);
            }
            else if (args1 == "rm")
            {
                if (PermissionManager.IsAdmin(args2))
                {
                    player.SendMessage($"admin은 삭제가 불가능 합니다.");
                    return;
                }
                if (PermissionManager.Remove(args2))
                {
                    msg = $"{args2}님을 성공적으로 삭제 하였습니다!";
                }
                else
                {
                    msg = "삭제에 실패하였습니다. ";
                    if (!PermissionManager.IsOP(args2))
                    {
                        msg += $"{args2}님은 op가 아닙니다.";
                    }
                    else
                    {
                        msg += $"내부적 오류입니다.";
                    }
                }
                player.SendMessage(msg);
            }
            else
            {
                player.SendMessage(helpText["add"]);
                player.SendMessage(helpText["rm"]);
            }
        }
    }
}