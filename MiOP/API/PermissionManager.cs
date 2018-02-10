using log4net;
using MiNET;
using MiNET.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using MiOP.Box;

namespace MiOP.API
{
    /// <summary>
    /// 여러가지 권한기능을 제공합니다.
    /// </summary>
    public static class PermissionManager
    {
        private static readonly ILog Log = LogManager.GetLogger("op Manager");
        private const string fileName = "ops.txt";
        private static string assembly = Assembly.GetExecutingAssembly().GetName().CodeBase;
        private static string OpsPath = Path.Combine(new Uri(Path.GetDirectoryName(assembly)).LocalPath, fileName);

        private static object LockOpsTxt = new object();
        /*
        /// <summary>
        /// MiOP의 API기능을 사용할 수 있습니다.
        /// </summary>
        public static PermissionManager Manager { get; } = new PermissionManager();
        */

        /// <summary>
        /// OP 목록을 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public static List<string> OPList
        {
            get
            {
                List<string> list = new List<string>();
                TryCreateOpsTxt();
                bool lockTaken = false;
                try
                {
                    Monitor.Enter(LockOpsTxt, ref lockTaken);
                    using (StreamReader stream = new StreamReader(OpsPath, Encoding.UTF8))
                    {
                        int counter = 0;
                        string line;
                        while ((line = stream.ReadLine()) != null)
                        {
                            list.Add(line);
                            counter++;
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(LockOpsTxt);
                    }
                }

                return list;
            }
        }

        internal static void TryCreateOpsTxt()
        {
            if (!File.Exists(OpsPath))
            {
                File.Create(OpsPath);
            }
        }

        /// <summary>
        /// Admin목록을 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public static List<string> AdminList
        {
            get
            {
                List<string> list = new List<string>();
                string txt = Config.GetProperty("admins", string.Empty);
                if (txt == string.Empty)
                {
                    return list;
                }

                txt = txt.Replace(" ", string.Empty);
                if (!txt.Contains(","))
                {
                    list.Add(txt);
                }
                else
                {
                    string[] users = txt.Split(',');
                    foreach (var item in users)
                    {
                        list.Add(item);
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// OP를 추가하고 성공여부를 반환합니다.
        /// </summary>
        /// <param name="name">타겟 플레이어의 이름</param>
        public static bool Add(string name)
        {
            if (IsOP(name) || IsAdmin(name))
            {
                return false;
            }
            else
            {
                bool lockTaken = false;
                try
                {
                    Monitor.Enter(LockOpsTxt, ref lockTaken);
                    using (StreamWriter stream = new StreamWriter(OpsPath, true, Encoding.UTF8))
                    {
                        stream.WriteLine(name);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(LockOpsTxt);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// OP를 제거하고 성공여부를 반환합니다.
        /// </summary>
        /// <param name="name">타겟 플레이어의 이름</param>
        /// <returns></returns>
        public static bool Remove(string name)
        {
            if (!IsOP(name))
            {
                return false;
            }
            List<string> list = OPList;
            list.RemoveAll(x => x == name ? true : false);

            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.AppendLine(item);
            }

            bool lockTaken = false;
            try
            {
                Monitor.Enter(LockOpsTxt, ref lockTaken);
                using(StreamWriter stream = new StreamWriter(OpsPath, false, Encoding.UTF8))
                {
                    stream.Write(sb.ToString());
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Monitor.Exit(LockOpsTxt);
            }
            return true;
        }

        /// <summary>
        /// 해당 플레이어가 OP인지 확인합니다.
        /// </summary>
        /// <param name="name">타겟 플레이어의 이름</param>
        /// <returns></returns>
        public static bool IsOP(string name)
        {
            return OPList.Contains(name);
        }

        /// <summary>
        /// 해당 플레이어가 Admin인지 확인합니다.
        /// </summary>
        /// <param name="name">플레이어의 이름</param>
        /// <returns></returns>
        public static bool IsAdmin(string name)
        {
            return AdminList.Contains(name);
        }

        /// <summary>
        /// 매개변수로 받은 플레이어의 권한을 확인하고 OP면 true,
        /// 아니면 false를 반환하고 메시지를 보냅니다.
        /// </summary>
        /// <param name="player">타겟 플레이어</param>
        /// <returns></returns>
        public static bool CheckCurrentUserPermission(Player player)
        {
            if (IsOP(player.Username) || IsAdmin(player.Username))
            {
                return true;
            }
            else
            {
                player.SendMessage($"{ChatColors.Red}명령어를 사용할 권한이 없습니다 !");
            }
            return false;
        }
    }
}