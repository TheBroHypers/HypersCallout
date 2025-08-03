using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
namespace HypersCallouts
{
    internal class Helper
    {
        internal static void log(string message)
        {
            Game.LogTrivial($"[HypersCallouts] {message}");
        }
        internal static void CrashLog(string Message,Exception e)
        {
            log(Message);
            log(e.Message);
            log(e.StackTrace);
        }
    }
}
