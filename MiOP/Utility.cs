using MiNET;

namespace MiOP
{
	/// <summary>
	/// MiOP에 대한 각종 유틸리티를 제공합니다.
	/// </summary>
	public class Utility
	{
        /// <summary>
        /// 플레이어에게 메시지를 보냅니다.
        /// </summary>
        /// <typeparam name="T">보낼 메시지의 타입</typeparam>
        /// <param name="player">메시지를 보낼 플레이어의 인스턴스</param>
        /// <param name="msg">보낼 메시지</param>
        public static void SendMsg<T>(Player player, T msg)
        {
            player.SendMessage($"[ MiOP ] {msg}");
        }
    }
}