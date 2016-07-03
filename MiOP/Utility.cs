using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiNET;

namespace MiOP
{
	public class Utility
	{
		/// <summary>
		/// 플레이어에게 메시지를 보냅니다.
		/// </summary>
		/// <typeparam name="T">보낼 메시지의 타입</typeparam>
		/// <param name="player">메시지를 보낼 플레이어의 인스턴스</param>
		/// <param name="msg">보낼 메시지</param>
		public static void SendMsg<T>(Player player, T msg) => player.SendMessage($"[ MiOP ] {msg}");
	}
}
