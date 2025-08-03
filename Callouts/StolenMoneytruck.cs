using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;
using LSPD_First_Response.Mod.Menus;
using CalloutInterfaceAPI;
using LSPD_First_Response.Engine;


namespace HypersCallouts.Callouts
{
    [CalloutInfo("[HC] Stolen Moneytruck", CalloutProbability.High)]
    [CalloutInterfaceAPI.CalloutInterface("[HC]Stolen MoneyTruck", CalloutProbability.Medium, "Reports of a stolen Moneytruck", "Code 3", "LSPD")]
    internal class StolenMoneytruck : Callout
    {
        private Ped Suspect;
        private Vehicle SuspectVehicle;
        private Blip SuspectBlip;
        private LHandle Pursuit;
        private Vector3 Spawnpoint;
        private bool PursuitCreated;
        private float heading; 
        private string[] peds = new string[]
        {
            "g_m_m_chicold_01", "hc_gunman"
        };
        Vector3[] spawnpoints = new Vector3[]
            {
                new Vector3(86.728f, -1403.438f, 29.358f),
                new Vector3(416.429f, -806.130f, 29.375f),
                new Vector3(130.597f, -207.108f, 54.522f),
                new Vector3(-152.649f, -305.464f, 38.792f),
                new Vector3(-719.136f, -157.477f, 36.997f),
                new Vector3(-1456.986f, -228.923f, 49.235f),
                new Vector3(1197.331f, 2700.146f, 38.156f),
                new Vector3(617.831f, 2746.724f, 42.009f),
                new Vector3(-1.890f, 6518.620f, 31.478f),
                new Vector3(1684.345f, 4819.815f, 42.018f),
            };


        public override bool OnBeforeCalloutDisplayed()
        {
            if (!Settings.StolenMoneytruck)
            {
                Game.LogTrivial("[LOG]: User has disabled this callout.");
                return false;
            }
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "[HC] Stolen MoneyTruck Theft  ";
            CalloutPosition = Spawnpoint;
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("WE_HAVE_A_CRIME_RESISTING_ARREST_02 IN_OR_ON_POSITION", Spawnpoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Random rand = new Random();
            
;           SuspectVehicle = new Vehicle("STOCKADE", Spawnpoint);
            SuspectVehicle.IsPersistent = true;

            Suspect = new Ped("s_m_y_factory_01", Spawnpoint, heading);
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.WarpIntoVehicle(SuspectVehicle, -1);
            Suspect.Tasks.CruiseWithVehicle(50f, VehicleDrivingFlags.FollowTraffic);

            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.Color = Color.Red;
            SuspectBlip.IsRouteEnabled = true;

            PursuitCreated = false;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            try
            {
                if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(SuspectVehicle) <= 30f)
                {
                    SuspectBlip.IsRouteEnabled = false;
                    Pursuit = LSPD_First_Response.Mod.API.Functions.CreatePursuit();
                    LSPD_First_Response.Mod.API.Functions.AddPedToPursuit(Pursuit, Suspect);
                    LSPD_First_Response.Mod.API.Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                    PursuitCreated = true;
                    Suspect.Inventory.GiveNewWeapon(WeaponHash.CombatPistol, 50, true);
                    Suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    Suspect.KeepTasks = true;
                }
            }
            catch (Exception e)
            {
                Helper.CrashLog("Crashed in process", e);
                End();
            }

            if (PursuitCreated && !LSPD_First_Response.Mod.API.Functions.IsPursuitStillRunning(Pursuit))
            {
                End();
            }
        }

        public override void End()
        {
            base.End();

            if (Suspect.Exists())
            {
                Suspect.Dismiss();
            }
            if (SuspectBlip.Exists())
            {
                SuspectBlip.Delete();
            }
            if (SuspectVehicle.Exists())
            {
                SuspectVehicle.Dismiss();
            }
            Game.LogTrivial("HypersCallouts: Stolen Moneytruck Pursuit has been cleaned up!");
        }
    }
}