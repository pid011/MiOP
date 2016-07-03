using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using log4net;
using MiNET;
using MiNET.Utils;

namespace MiOP
{
		public class OP
		{
			public List<string> ops { get; internal set; }
			public List<string> admins { get; internal set; }
		}
	/// <summary>
	/// 여러가지 op기능을 제공합니다.
	/// </summary>
	public class Manager : OP
	{

		private ILog Log = LogManager.GetLogger("op Manager");
		private const string fileName = "ops.txt";
		private static string assembly = Assembly.GetExecutingAssembly().GetName().CodeBase;
		private static string path = Path.Combine(new Uri(Path.GetDirectoryName(assembly)).LocalPath, fileName);
		

		/// <summary>
		/// op 인스턴스를 초기화합니다.
		/// </summary>
		public Manager()
		{
			if(!File.Exists(path))
			{
				File.Create(path);
			}
		}

		/// <summary>
		/// op를 추가하고 성공여부를 반환합니다.
		/// </summary>
		/// <param name="name">Player's name</param>
		public bool Add(string name)
		{
			if(!File.Exists(path))
			{
				File.Create(path);
			}
			if(IsOP(name))
			{
				return false;
			}
			else if(IsAdmin(name))
			{
				return false;
			}
			else
			{
				StreamWriter stream = new StreamWriter(path, true, Encoding.UTF8);
				stream.WriteLine(name);

				stream.Close();
			}
			return true;
		}

		/// <summary>
		/// op를 제거하고 성공여부를 반환합니다.
		/// </summary>
		/// <param name="name">Player's name</param>
		/// <returns></returns>
		public bool Remove(string name)
		{
			if(!IsOP(name))
			{
				return false;
			}
			ops.RemoveAll(x => x == name ? true : false);

			StringBuilder sb = new StringBuilder();
			foreach(var item in ops)
			{
				sb.AppendLine(item);
			}

			StreamWriter stream = new StreamWriter(path, false, Encoding.UTF8);
			stream.Write(sb.ToString());

			stream.Close();
			return true;
		}

		/// <summary>
		/// op 목록을 가져옵니다.
		/// </summary>
		/// <returns></returns>
		public OP GetList()
		{
			if(File.Exists(path))
			{
				StreamReader stream = new StreamReader(path, Encoding.UTF8);
				int counter = 0;
				string line;
				while((line = stream.ReadLine()) != null)
				{
					ops.Add(line.Replace(" ", string.Empty));
					counter++;
				}
				stream.Close();
			}
			else
			{
				File.Create(path);
			}

			string txt = Config.GetProperty("admins", string.Empty);
			if(txt == string.Empty) return this;
		
			txt = txt.Replace(" ", string.Empty);
			if(! txt.Contains(","))
			{
				admins.Add(txt);
			}
			else
			{
				string[] users = txt.Split(',');
			}
			return this;
		}

		/// <summary>
		/// 해당 플레이어가 op인지 확인합니다.
		/// </summary>
		/// <param name="name">플레이어의 이름</param>
		/// <returns></returns>
		public bool IsOP(string name) => ops.Contains(name);

		/// <summary>
		/// 해당 플레이어가 admin인지 확인합니다.
		/// </summary>
		/// <param name="name">플레이어의 이름</param>
		/// <returns></returns>
		public bool IsAdmin(string name) => admins.Contains(name);

		/// <summary>
		/// 매개변수로 받은 플레이어의 퍼미션을 확인하고 op면 true,
		/// 아니면 false를 반환하고 메시지를 보냅니다.
		/// </summary>
		/// <param name="player">플레이어의 인스턴스</param>
		/// <returns></returns>
		public bool CheckCurrentUserPermission(Player player)
		{

			if(IsOP(player.Username))
			{
				return true;
			}
			else
			{
				Utility.SendMsg(player, $"{ChatColors.Red}명령어를 사용할 권한이 없습니다 !");
			}
			return false;
		}
	}
}