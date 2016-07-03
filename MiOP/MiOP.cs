using System.Collections.Generic;
using System.Text;
using log4net;
using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Utils;

namespace MiOP
{
	[Plugin]
	public class MiOP : Plugin
	{
		private static ILog Log = LogManager.GetLogger(typeof(MiOP));
		private List<string> ops = new List<string>();
		private Manager manager;

		private Dictionary<string, string> helpText = new Dictionary<string, string>
		{
			{ "add", $"{ChatColors.Yellow}/op add [ name ] - op 추가" },
			{ "rm", $"{ChatColors.Yellow}/op rm [ name ] - op삭제" },
			{ "list", $"{ChatColors.Yellow}/op list - op 전체 목록 조회" }
		};

		protected override void OnEnable()
		{
			Startup();
			base.OnEnable();
		}

		private void Startup()
		{
			manager = new Manager();
		}

		[Command]
		public void op(Player player)
		{
			if(!manager.CheckCurrentUserPermission(player)) return;

			Utility.SendMsg(player, helpText["add"]);
			Utility.SendMsg(player, helpText["rm"]);
			Utility.SendMsg(player, helpText["list"]);
		}

		[Command]
		public void op(Player player, string args)
		{
			if(!manager.CheckCurrentUserPermission(player)) return;

			if(args == "list")
			{
				OP ops = manager.GetList();
				foreach(var item in ops.ops)
				{
					Utility.SendMsg(player, item);
				}
			}
			else
			{
				char[] texts = args.ToCharArray();
				foreach(var t in texts)
				{
					if("add".Contains(t.ToString()))
					{
						Utility.SendMsg(player, $"이 명령어를 찾나요? -> {ChatColors.Gold}{helpText["add"]}");
						break;
					}
					else if("rm".Contains(t.ToString()))
					{
						Utility.SendMsg(player, $"이 명령어를 찾나요? -> {ChatColors.Gold}{helpText["rm"]}");
						break;
					}
					else if("list".Contains(t.ToString()))
					{
						Utility.SendMsg(player, $"이 명령어를 찾나요? -> {ChatColors.Gold}{helpText["list"]}");
						break;
					}
					else
					{
						Utility.SendMsg(player, helpText["add"]);
						Utility.SendMsg(player, helpText["rm"]);
						Utility.SendMsg(player, helpText["list"]);
						break;
					}
				}
			}
		}

		[Command]
		public void op(Player player, string args1, string args2)
		{
			if(!manager.CheckCurrentUserPermission(player)) return;

			string msg;
			if(args1 == "add")
			{
				if(manager.Add(args2))
				{
					msg = $"{args2}님을 성공적으로 추가 하였습니다!";
				}
				else
				{
					msg = "추가에 실패하였습니다. ";
					if(manager.IsOP(args2))
					{
						msg += $"{args2}님은 이미 op입니다.";
					}
					else if(manager.IsAdmin(args2))
					{
						msg += $"{args2}님은 이미 admin입니다.";
					}
					else
					{
						msg += $"내부적 오류입니다.";
					}
				}
				Utility.SendMsg(player, msg);
			}
			else if(args1 == "rm")
			{
				if(manager.IsAdmin(args2))
				{
					Utility.SendMsg(player, $"{args2}님은 이미 admin입니다.");
					return;
				}

				if(manager.Remove(args2))
				{
					msg = $"{args2}님을 성공적으로 삭제 하였습니다!";
				}
				else
				{
					msg = "추가에 실패하였습니다. ";
					if(!manager.IsOP(args2))
					{
						msg += $"{args2}님은 op가 아닙니다.";
					}
					else
					{
						msg += $"내부적 오류입니다.";
					}
				}
				Utility.SendMsg(player, msg);
			}
			else
			{
				Utility.SendMsg(player, helpText["add"]);
				Utility.SendMsg(player, helpText["rm"]);
				Utility.SendMsg(player, helpText["list"]);
			}
		}

		private List<string> MakeupList(List<string> list)
		{
			List<string> makeupText = new List<string>();
			StringBuilder sb = new StringBuilder();

			makeupText.Add("=== LIST ===");
			makeupText.Add($"{ChatColors.Red}RED = admin{ChatColors.White}, {ChatColors.Green}GREEN = op");
			sb.Append(ChatColors.Gold);
			int i = 1;
			foreach(var item in list)
			{
				if(manager.IsAdmin(item))
				{
					sb.Append($"[{ChatColors.Red}{item}{ChatColors.Gold}] ");
				}
				else
				{
					sb.Append($"[{ChatColors.Green}{item}{ChatColors.Gold}] ");
				}

				if(i % 5 == 0)
				{
					makeupText.Add(sb.ToString());
					sb.Clear();
					sb.Append(ChatColors.Gold);
				}
				i++;
			}
			makeupText.Add(sb.ToString());
			makeupText.Add($"총 {list.Count}명의 op와 admin이 있습니다.");

			return makeupText;
		}
	}
}