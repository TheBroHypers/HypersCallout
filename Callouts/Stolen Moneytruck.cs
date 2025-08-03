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


namespace HypersCallouts.Callouts
{

    [CalloutInfo("Stolen Moneytruck Pursuit", CalloutProbability.High)]
    internal class StolenMoneytruck : Callout
    {
        private Ped Suspect;
        private Vehicle SuspectVehicle;
        private Blip SuspectBlip;
        private LHandle Pursuit;
        private Vector3 Spawnpoint;
        private bool PursuitCreated;



        public override bool OnBeforeCalloutDisplayed()
        {
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "Stolen MoneyTruck Theft  ";
            CalloutPosition = Spawnpoint;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE_A_CRIME_RESISTING_ARREST_02 IN_OR_ON_POSITION", Spawnpoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {

            SuspectVehicle = new Vehicle("STOCKADE", Spawnpoint);
            SuspectVehicle.IsPersistent = true;

            Suspect = new Ped(SuspectVehicle.GetOffsetPositionFront(5f));
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
                    Pursuit = Functions.CreatePursuit();
                    Functions.AddPedToPursuit(Pursuit, Suspect);
                    Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
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

            if (PursuitCreated && !Functions.IsPursuitStillRunning(Pursuit))
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