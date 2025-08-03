using System.Windows.Forms;
using Rage;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypersCallouts
{
    internal static class Settings
    {
        internal static InitializationFile ini;
        internal static string inipath = "Plugins/LSPDFR/HypersCallouts.ini";
        internal static bool GangDriveBy = true;
        internal static bool StolenVehicle = true;
        internal static bool SuspiciousVehicle = true; 
        internal static bool StolenMoneytruck = true;
        internal static bool SuicidelPerson = true;
        internal static bool unstableperson = true;
        internal static bool Highspeedchase = true;
        internal static bool PersonWithAKnife = true;
        internal static bool AutomaticBackup = true;
        internal static bool LeaveCalloutsRunning = false;
        internal static bool HelpMessages = true;
        internal static bool DisableNotifician;
        internal static Keys EndCall = Keys.End;
        internal static Keys Dialog = Keys.Y;
        internal static Keys Menu = Keys.F9;
        internal static Keys InteractionKey1 = Keys.K;
        internal static Keys InteractionKey2 = Keys.L;
        

        internal static void LoadSettings()
        {
            Game.LogTrivial("[LOG]: Loading config file from HypersCallouts.");
            ini = new InitializationFile(inipath);
            ini.Create();

            EndCall = ini.ReadEnum("Keys", "EndCall", Keys.End);
            Dialog = ini.ReadEnum("Keys", "Dialog", Keys.Y);
            Menu = ini.ReadEnum("Keys", "Menu", Keys.F9);
            InteractionKey1 = ini.ReadEnum("Keys", "InteractionKey1", Keys.K);
            InteractionKey2 = ini.ReadEnum("Keys", "InteractionKey2", Keys.L);
            HelpMessages = ini.ReadBoolean("Miscellaneous", "HelpMessages", true);
            LeaveCalloutsRunning = ini.ReadBoolean("Miscellaneous", "LeaveCalloutsRunning", false);
            AutomaticBackup = ini.ReadBoolean("Miscellaneous", "AutomaticBackup", true);
            DisableNotifician = ini.ReadBoolean("Miscellaneous", "CalloutNotification", true);

            GangDriveBy = ini.ReadBoolean("Callouts", "GangDriveBy", true);
            StolenVehicle = ini.ReadBoolean("Callouts", "StolenVehicle", true);
            StolenMoneytruck = ini.ReadBoolean("Callouts", "StolenMoneytruck", true);
            SuicidelPerson = ini.ReadBoolean("Callouts", "SuicidelPerson", true);
            Highspeedchase = ini.ReadBoolean("Callouts", "Highspeedchase", true);
            PersonWithAKnife = ini.ReadBoolean("Callouts", "PersonWithAKnife", true);
            unstableperson = ini.ReadBoolean("Callouts", "unstableperson", true);
            SuspiciousVehicle = ini.ReadBoolean("Callouts", "SuspiciousVehicle", true);



            
        }
        public static readonly string PluginVersion = "1.0.0.0";
    }
}