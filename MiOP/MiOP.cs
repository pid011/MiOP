using System.Collections.Generic;
using System.Text;
using log4net;
using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Utils;

namespace MiOP
{
	/// <summary>
	/// MiOP 메인 클래스 입니다.
	/// </summary>
	[Plugin]
	public class MiOP : Plugin
	{
		private static ILog Log = LogManager.GetLogger(typeof(MiOP));
		private List<string> ops = new List<string>();
		private Manager manager;

		private Dictionary<string, string> helpText = new Dictionary<string, string>
		{
			{ "add", $"{ChatColors.Yellow}/op add [ name ] - OP 추가" },
			{ "rm", $"{ChatColors.Yellow}/op rm [ name ] - op삭제" },
			{ "list", $"{ChatColors.Yellow}/op list - op 전체 목록 조회" }
		};

		private enum Commands
		{ add, rm, list }

		/// <summary>
		/// 시작시 실행될 함수.
		/// </summary>
		protected override void OnEnable()
		{
			Startup();
			base.OnEnable();
		}

		private void Startup()
		{
            this.manager = new Manager();
		}

		/// <summary>
		/// op commad.
		/// </summary>
		/// <param name="player"></param>
		[Command]
		public void Op(Player player)
		{
			if(!this.manager.CheckCurrentUserPermission(player))
            {
                return;
            }

            Utility.SendMsg(player, this.helpText["add"]);
			Utility.SendMsg(player, this.helpText["rm"]);
			Utility.SendMsg(player, this.helpText["list"]);
		}

		/// <summary>
		/// op command.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="args"></param>
		[Command]
		public void Op(Player player, string args)
		{
			if(!this.manager.CheckCurrentUserPermission(player))
            {
                return;
            }

            if (args == "list")
			{
				List<string> msgs = MakeupList(this.manager.GetOpList());
				foreach(var item in msgs)
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
						Utility.SendMsg(player, this.helpText["add"]);
						Utility.SendMsg(player, this.helpText["rm"]);
						Utility.SendMsg(player, this.helpText["list"]);
						break;
					}
				}
			}
		}

		/// <summary>
		/// op command.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="args1"></param>
		/// <param name="args2"></param>
		[Command]
		public void Op(Player player, string args1, string args2)
		{
			if(!this.manager.CheckCurrentUserPermission(player))
            {
                return;
            }

            string msg;
			if(args1 == "add")
			{
				if(this.manager.Add(args2))
				{
					msg = $"{args2}님을 성공적으로 추가 하였습니다!";
				}
				else
				{
					msg = "추가에 실패하였습니다. ";
					if(this.manager.IsOP(args2))
					{
						msg += $"{args2}님은 이미 op입니다.";
					}
					else if(this.manager.IsAdmin(args2))
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
				if(this.manager.IsAdmin(args2))
				{
					Utility.SendMsg(player, $"admin은 삭제가 불가능 합니다.");
					return;
				}
				if(this.manager.Remove(args2))
				{
					msg = $"{args2}님을 성공적으로 삭제 하였습니다!";
				}
				else
				{
					msg = "삭제에 실패하였습니다. ";
					if(!this.manager.IsOP(args2))
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
				Utility.SendMsg(player, this.helpText["add"]);
				Utility.SendMsg(player, this.helpText["rm"]);
				Utility.SendMsg(player, this.helpText["list"]);
			}
		}

		private List<string> MakeupList(List<string> list)
		{
			List<string> makeupText = new List<string>();
			List<string> op = this.manager.GetOpList();
			List<string> admin = this.manager.GetAdminList();

			StringBuilder sb = new StringBuilder();

			makeupText.Add("=== LIST ===");
			makeupText.Add($"{ChatColors.Red}RED{ChatColors.White} = admin, {ChatColors.Green}GREEN{ChatColors.White} = op");
			sb.Append(ChatColors.Gold);
			int i = 1;
			foreach(var item in admin)
			{
				sb.Append($"[{ChatColors.Red}{item}{ChatColors.Gold}] ");
				if(i % 5 == 0)
				{
					makeupText.Add(sb.ToString());
					sb.Clear();
					sb.Append(ChatColors.Gold);
				}
				i++;
			}
			foreach(var item in op)
			{
				sb.Append($"[{ChatColors.Green}{item}{ChatColors.Gold}] ");
				if(i % 5 == 0)
				{
					makeupText.Add(sb.ToString());
					sb.Clear();
					sb.Append(ChatColors.Gold);
				}
				i++;
			}
			makeupText.Add(sb.ToString());
			makeupText.Add($"총 {op.Count + admin.Count}명의 admin과 op가 있습니다.");

			return makeupText;
		}
	}
}