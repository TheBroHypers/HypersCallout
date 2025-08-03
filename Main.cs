using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using System.Reflection;

namespace HypersCallouts.Callouts
{
    public class Main: Plugin
    {
        public override void Initialize()
        {
         LSPD_First_Response.Mod.API.Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Helper.log("Plugin HypersCallouts" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " by Hypers has been initiallised");
            Helper.log("Go on duty to fully load HypersCallouts");

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LSPDFRResolveEventHandler);
        }

        public override void Finally()
        {
            Helper.log("HypersCallouts has been cleaned up");
        }

        private static void OnOnDutyStateChangedHandler(bool OnDuty)
        {
            if (OnDuty)
            {
                
                RegisterCallouts();
                Settings.LoadSettings();
                Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept",  "HypersCallouts", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()+ " By TheBroHypers", " ~b~Has been loaded~w~ ~y~Succesfully~w~");
            }
        }
        private static void RegisterCallouts()
        {
           if (Settings.SuspiciousVehicle) Functions.RegisterCallout(typeof(SuspiciousVehicle));
           if (Settings.Highspeedchase) Functions.RegisterCallout(typeof(HighSpeedPursuit));
           if (Settings.PersonWithAKnife) Functions.RegisterCallout(typeof(PersonWithAKnife));
           if (Settings.StolenMoneytruck) Functions.RegisterCallout(typeof(StolenMoneytruck));
           if (Settings.StolenVehicle) Functions.RegisterCallout(typeof(StolenVehicle));
           if (Settings.unstableperson) Functions.RegisterCallout(typeof(UnstablePeron));
           if (Settings.SuicidelPerson) Functions.RegisterCallout(typeof(SuicidelPerson));
           if (Settings.GangDriveBy) Functions.RegisterCallout(typeof(GangDriveBy));
            
            
            
        }

        public static Assembly LSPDFRResolveEventHandler(object sender, ResolveEventArgs args)
        {
            foreach (Assembly assembly in Functions.GetAllUserPlugins())
            {
                if (args.Name.ToLower().Contains(assembly.GetName().Name.ToLower()))
                {
                    return assembly;
                }
            }
            return null;
        }
        public static bool IsLSPDFRPluginRunning(string Plugin, Version minversion = null)
        {
            foreach (Assembly assembly in Functions.GetAllUserPlugins())
            {
                AssemblyName an = assembly.GetName();
                if (an.Name.ToLower() == Plugin.ToLower())
                {
                    if (minversion == null || an.Version.CompareTo(minversion) >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    
    }
}
