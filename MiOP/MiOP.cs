using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
		Dictionary<string, string> helpText = new Dictionary<string, string>
		{
			{ "add", $"{ChatColors.Yellow}/op add [ name ] - OP 추가" },
			{ "rm", $"{ChatColors.Yellow}/op rm [ name ] - op삭제" },
			{ "list", $"{ChatColors.Yellow}/op list - op 전체 목록 조회" }
		};
		enum commands{ add, rm, list }

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
			Utility.SendMsg(player, helpText[commands.add.ToString()]);
			Utility.SendMsg(player, helpText[commands.rm.ToString()]);
			Utility.SendMsg(player, helpText[commands.list.ToString()]);
		}

		[Command]
		public void op(Player player, string args)
		{
			if(args == commands.list.ToString())
			{
				List<string> msgs = MakeupList(manager.GetList());
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
					if(commands.add.ToString().Contains(t.ToString()))
					{
						Utility.SendMsg(player, $"이 명령어를 찾나요? -> {ChatColors.Gold}{helpText[commands.add.ToString()]}");
						break;
					}
					else if(commands.rm.ToString().Contains(t.ToString()))
					{
						Utility.SendMsg(player, $"이 명령어를 찾나요? -> {ChatColors.Gold}{helpText[commands.rm.ToString()]}");
						break;
					}
					else if(commands.list.ToString().Contains(t.ToString()))
					{
						Utility.SendMsg(player, $"이 명령어를 찾나요? -> {ChatColors.Gold}{helpText[commands.list.ToString()]}");
						break;
					}
					else
					{
						Utility.SendMsg(player, helpText[commands.add.ToString()]);
						Utility.SendMsg(player, helpText[commands.rm.ToString()]);
						Utility.SendMsg(player, helpText[commands.list.ToString()]);
						break;
					}
				}
			}
		}
		[Command]
		public void op(Player player, string args1, string args2)
		{
			string msg;
			if(args1 == commands.add.ToString())
			{
				if(manager.Add(args2))
				{
					msg = $"{args2}님을 성공적으로 추가 하였습니다!";
				}
				else
				{
					if(manager.IsOP(args2))
					{
						msg = $"추가에 실패하였습니다. {args2}님은 이미 op입니다.";
					}
					else
					{
						msg = $"추가에 실패하였습니다. 내부적 오류입니다.";
					}
				}
				Utility.SendMsg(player, msg);
			}
			else if(args1 == commands.rm.ToString())
			{
				if(manager.Remove(args2))
				{
					msg = $"{args2}님을 성공적으로 삭제 하였습니다!";
				}
				else
				{
					if(! manager.IsOP(args2))
					{
						msg = $"추가에 실패하였습니다. {args2}님은 op가 아닙니다.";
					}
					else
					{
						msg = $"추가에 실패하였습니다. 내부적 오류입니다.";
					}
				}
				Utility.SendMsg(player, msg);
			}
			else
			{
				Utility.SendMsg(player, helpText[commands.add.ToString()]);
				Utility.SendMsg(player, helpText[commands.rm.ToString()]);
				Utility.SendMsg(player, helpText[commands.list.ToString()]);
			}
		}

		private List<string> MakeupList(List<string> list)
		{
			List<string> makeupText = new List<string>();
			StringBuilder sb = new StringBuilder();

			makeupText.Add("=== LIST ===");
			sb.Append(ChatColors.Gold);
			int i = 1;
			foreach(var item in list)
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
			makeupText.Add($"총 {list.Count}명의 op가 있습니다.");

			return makeupText;
		}
	}
}