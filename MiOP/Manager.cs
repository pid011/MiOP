﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using log4net;
using MiNET;
using MiNET.Utils;

namespace MiOP
{
	/// <summary>
	/// 여러가지 op기능을 제공합니다.
	/// </summary>
	public class Manager
	{
		private ILog Log = LogManager.GetLogger("op Manager");
		private const string fileName = "op.txt";
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
			List<string> list = GetList();
			list.RemoveAll(x => x == name ? true : false);

			StringBuilder sb = new StringBuilder();
			foreach(var item in list)
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
		public List<string> GetList()
		{
			List<string> list = new List<string>();
			if(File.Exists(path))
			{
				StreamReader stream = new StreamReader(path, Encoding.UTF8);
				int counter = 0;
				string line;
				while((line = stream.ReadLine()) != null)
				{
					list.Add(line);
					counter++;
				}

				stream.Close();
			}
			else
			{
				File.Create(path);
			}

			return list;
		}

		/// <summary>
		/// 해당 플레이어가 op인지 확인합니다.
		/// </summary>
		/// <param name="name">Player's name</param>
		/// <returns></returns>
		public bool IsOP(string name) => GetList().Contains(name);

		/// <summary>
		/// 매개변수로 받은 플레이어의 퍼미션을 확인하고 op면 true,
		/// 아니면 false를 반환하고 메시지를 보냅니다.
		/// </summary>
		/// <param name="sender">Instance of player</param>
		/// <returns></returns>
		public bool CheckCurrentUserPermission(Player sender)
		{
			if(IsOP(sender.Username))
			{
				return true;
			}
			else
			{
				sender.SendMessage($"{ChatColors.Red}[Server] 명령어를 사용할 권한이 없습니다 !");
			}
			return false;
		}
	}
}