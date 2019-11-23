using log4net;
using MiNET;
using MiNET.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

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

        /// <summary>
        /// OP 목록을 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public static List<string> OPList
        {
            get
            {
                List<string> list = new List<string>();

                if (!TryCreateOpsTxt())
                {
                    return list;
                }

                bool lockTaken = false;
                Monitor.Enter(LockOpsTxt, ref lockTaken);
                try
                {
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
                    Log.Warn(e.Message, e);
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

        internal static bool TryCreateOpsTxt()
        {
            bool lockTaken = false;
            Monitor.Enter(LockOpsTxt, ref lockTaken);
            try
            {
                if (!File.Exists(OpsPath))
                {
                    File.Create(OpsPath);
                }
            }
            catch (Exception e)
            {
                Log.Warn(e.Message, e);
                return false;
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(LockOpsTxt);
                }
            }

            return true;
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

                if (string.IsNullOrWhiteSpace(txt))
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
            // OP거나 Admin이면 추가 안됨.
            if (IsOP(name) || IsAdmin(name))
            {
                return false;
            }

            bool lockTaken = false;
            Monitor.Enter(LockOpsTxt, ref lockTaken);
            try
            {
                using (StreamWriter stream = new StreamWriter(OpsPath, true, Encoding.UTF8))
                {
                    stream.WriteLine(name);
                }
            }
            catch (Exception e)
            {
                Log.Warn(e.Message, e);
                return false;
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(LockOpsTxt);
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
            // OP가 아니므로 삭제 불가
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
            Monitor.Enter(LockOpsTxt, ref lockTaken);
            try
            {
                using (StreamWriter stream = new StreamWriter(OpsPath, false, Encoding.UTF8))
                {
                    stream.Write(sb.ToString());
                }
            }
            catch (Exception e)
            {
                Log.Warn(e.Message, e);
                return false;
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