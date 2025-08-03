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
using Rage.Native;

namespace HypersCallouts.Callouts
{
    [CalloutInfo("[HC] GangDriveBy", CalloutProbability.High)]
    [CalloutInterfaceAPI.CalloutInterface("[HC] GangDriveBy", CalloutProbability.Medium, "There are reports of a shooting going on", "Code 3", "LSPD")]
    internal class GangDriveBy : Callout
    {
        private bool Suspect1shooting = false; 
        private float heading;
        private Ped Suspect;
        private Vehicle SuspectVehicle;
        private Blip SuspectBlip;
        private LHandle Pursuit;
        private Vector3 Spawnpoint;
        private bool PursuitCreated;
        private Ped Suspect1;
        private Ped suspect2;
        private Ped suspect3;
        private Blip Suspect1blip;
        private Blip suspect2blip;
        private Blip suspect3blip;
        private bool suspectdone;
        private string[] peds = new string[] { "csb_ramp_gang", "g_m_y_ballaorig_01", "g_m_y_ballasout_01", "g_m_y_mexgoon_03", "g_m_y_famfor_01" };
        private readonly string[] WeaponList = new string[] { "weapon_ceramicpistol", "weapon_pistol", "weapon_combatpistol", "weapon_vintagepistol", "weapon_microsmg", "weapon_machinepistol", "weapon_minismg" };

        public override bool OnBeforeCalloutDisplayed()
        {
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage =  "[HC] GangDriveBy";
            CalloutPosition = Spawnpoint;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE_A_CRIME_RESISTING_ARREST_02 IN_OR_ON_POSITION", Spawnpoint);
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Model[] vehiclemodels = new Model[] { "Primo", "Baller2", "Baller4", "Cavalcade", " Emperor", "Emperor2", "Stanier" };
            SuspectVehicle = new Vehicle(vehiclemodels[new Random().Next(vehiclemodels.Length)], Spawnpoint, heading);
            SuspectVehicle.IsPersistent = true;

            Random rand = new Random();
            Suspect = new Ped(peds[rand.Next((int)peds.Length)], Spawnpoint, heading);
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.WarpIntoVehicle(SuspectVehicle, -1);
            Suspect.Tasks.CruiseWithVehicle(SuspectVehicle, 50f, VehicleDrivingFlags.FollowTraffic);

            

            suspect2 = new Ped(peds[rand.Next((int)peds.Length)], Spawnpoint, heading);
            suspect2.BlockPermanentEvents = true;
            suspect2.IsPersistent = true;
            suspect2.WarpIntoVehicle(SuspectVehicle, +1);
            suspect2.Inventory.GiveNewWeapon(new WeaponAsset(WeaponList[new Random().Next((int)WeaponList.Length)]), 500, true);
            NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(suspect2, 2, true);

            suspect3 = new Ped(peds[rand.Next((int)peds.Length)], Spawnpoint, heading);
            suspect3.BlockPermanentEvents = true;
            suspect3.IsPersistent = true; 
            suspect3.WarpIntoVehicle(SuspectVehicle, +2);
            suspect3.Inventory.GiveNewWeapon(new WeaponAsset(WeaponList[new Random().Next((int)WeaponList.Length)]), 500, true);
            NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(suspect3, 2, true);
            

            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.Color = Color.Red;
            SuspectBlip.IsRouteEnabled = true;

          

            suspect2blip = suspect2.AttachBlip();
            suspect2blip.Color = Color.Red;

            suspect3blip = suspect3.AttachBlip();
            suspect3blip.Color = Color.Red;


            PursuitCreated = false;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            try
            {
                    if (Game.LocalPlayer.Character.DistanceTo(SuspectVehicle) >1f)
                    {
                        foreach (Ped enemy in Suspect.GetNearbyPeds(16))
                        {
                           if (enemy.IsAlive && !enemy.IsInAnyVehicle(false) && !Suspect1shooting)
                           {
                            suspect2.Tasks.FightAgainst(enemy);
                            suspect2.KeepTasks = true; 
                             
                              break;
                                
                           }
                           if (enemy.IsAlive && !enemy.IsInAnyVehicle(false) && Suspect1shooting == true )
                           {
                               suspect3.Tasks.FightAgainst(enemy);
                               suspect3.KeepTasks = true;
                               
                                 
                           }
                                Suspect1shooting = true;
                        }
                    }

                    if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(SuspectVehicle) <= 30f)
                    {
                    if (Settings.AutomaticBackup)
                    {
                        Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
                        Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
                    }
                    SuspectBlip.IsRouteEnabled = false;
                    Pursuit = Functions.CreatePursuit();
                    Functions.AddPedToPursuit(Pursuit, Suspect);
                    Functions.AddPedToPursuit(Pursuit, suspect2);
                    Functions.AddPedToPursuit(Pursuit, suspect3);
                    Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                    PursuitCreated = true;
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
            if (suspect2.Exists())
            {
                suspect2.Dismiss();
            }
            if (suspect2blip.Exists())
            {
                suspect2blip.Delete();
            }
            
            if (suspect3.Exists())
            {
                suspect3.Dismiss();
            }
            if (suspect3blip.Exists())
            {
                suspect3blip.Delete();
            }
            Game.LogTrivial("HypersCallouts: Gangdriveby has been cleaned up!");
        }
    }
}