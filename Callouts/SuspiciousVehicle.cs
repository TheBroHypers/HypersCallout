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
    [CalloutInfo("[HC] SuspiciousVehicle", CalloutProbability.Low)]
    [CalloutInterfaceAPI.CalloutInterface("[HC] SuspiciousVehicle", CalloutProbability.Medium, "Reports of a suspicious vehicle ", "Code 2", "LSPD")]
    internal class SuspiciousVehicle : Callout
    {
        private float heading;
        private Ped Suspect;
        private Ped guard;
        private bool suspectdone;
        private Vehicle SuspectVehicle;
        private Blip SuspectBlip;
        private Blip guardblip;
        private Blip vehicleblip;
        private LHandle Pursuit;
        private Vector3 Spawnpoint;
        private bool PursuitCreated;
        private string[] peds = new string[] { "a_m_m_hillbilly_01", "a_m_m_og_boss_01", "a_m_m_hillbilly_02", "a_m_m_salton_03", "g_m_m_chiboss_01" };
        private int randomending;
        private string[] DrugList = new string[] { "Heroin", "Cocain", "Ketamine", "LSD (D-Lysergic Acid Diethylamide)", "Psilocybin" };
        private string drug;

        public override bool OnBeforeCalloutDisplayed()
        {
            if (!Settings.SuspiciousVehicle)
            {
                Game.LogTrivial("[LOG]: User has disabled this callout.");
                return false;
            }
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "[HC]Reports of a Suspicious vehicle in the area ";
            CalloutPosition = Spawnpoint;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE_A_CRIME_RESISTING_ARREST_02 IN_OR_ON_POSITION", Spawnpoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Model[] modell = new Model[] { "Journey", "Speedo", "Rumpo", "Surfer2", "mule", "cavalcade", "primo", "emperor" };
            SuspectVehicle = new Vehicle(modell[new Random().Next(modell.Length)], Spawnpoint);
            SuspectVehicle.IsPersistent = true;
            StopThePed.API.Functions.injectVehicleSearchItems(SuspectVehicle);
            drug = DrugList[new Random().Next(DrugList.Length)];
            SuspectVehicle.Metadata.searchTrunk = "~r~ lots of " + drug + "~s~";
            Random rand = new Random();
            Suspect = new Ped(peds[rand.Next((int)peds.Length)], Spawnpoint, heading);
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.WarpIntoVehicle(SuspectVehicle, -1);
            Suspect.Tasks.CruiseWithVehicle(20f, VehicleDrivingFlags.FollowTraffic);
            Suspect.Inventory.GiveNewWeapon(WeaponHash.Pistol, 50, true);
            Suspect.KeepTasks = true;


            guard = new Ped(peds[rand.Next((int)peds.Length)], Spawnpoint, heading);

            guard.IsPersistent = true;
            guard.BlockPermanentEvents = true;
            guard.WarpIntoVehicle(SuspectVehicle, 0);
            guard.Inventory.GiveNewWeapon(WeaponHash.Pistol, 50, true);
            StopThePed.API.Functions.setPedUnderDrugsInfluence(guard, true);


            vehicleblip = SuspectVehicle.AttachBlip();
            vehicleblip.Color = Color.Red;
            vehicleblip.IsRouteEnabled = true;

            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.Color = Color.Red;

            guardblip = guard.AttachBlip();
            guardblip.Color = Color.Red;

            PursuitCreated = false;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            try
            {
                if (Game.LocalPlayer.Character.DistanceTo(SuspectVehicle) < 50f && !suspectdone)
                {
                    Ending();

                    suspectdone = true;
                }

            }
            catch (Exception e)
            {
                Helper.CrashLog("Crashed in process", e);
                End();
            }


            if (Suspect.IsCuffed || Suspect.IsDead || Game.LocalPlayer.Character.IsDead || !Suspect.Exists() && guard.IsCuffed || guard.IsDead || !guard.Exists())
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
            if (guard.Exists())
            {
                guard.Dismiss();
            }
            if (vehicleblip.Exists())
            {
                vehicleblip.Delete();
            }
            if (guardblip.Exists())
            {
                guardblip.Delete();
            }
            Game.LogTrivial("HypersCallouts: Drugs Smugglers has been cleaned up!");
        }

        public void Ending()
        {
            Random r = new Random();
            randomending = r.Next(0, 4);



            if (randomending == 1)
            {
                Game.LogTrivial(randomending + "0");
                if (!PursuitCreated)
                {
                    if (Settings.AutomaticBackup)
                    {
                        Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
                        Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
                    }
                    Pursuit = Functions.CreatePursuit();
                    Functions.AddPedToPursuit(Pursuit, Suspect);
                    Functions.AddPedToPursuit(Pursuit, guard);
                    Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                    Suspect.RelationshipGroup = "red";
                    guard.RelationshipGroup = "red";
                    Game.SetRelationshipBetweenRelationshipGroups("red", "COP", Relationship.Hate);
                    Suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    guard.Tasks.FightAgainst(Game.LocalPlayer.Character);

                    PursuitCreated = true;
                }

                if (PursuitCreated && !Functions.IsPursuitStillRunning(Pursuit))
                {
                    End();
                }
            }
            else if (randomending == 2)
            {
                Game.LogTrivial(randomending + "1");
                Game.DisplayHelp("Perform a ~y~Traffic stop~w~ on the suspect", 7500);


                if (Suspect.IsCuffed || Suspect.IsDead || Game.LocalPlayer.Character.IsDead || !Suspect.Exists())
                {
                    End();
                }
            }

            else if (randomending == 3 )
            {
                Game.LogTrivial(randomending + "2");
                Game.DisplayHelp("Perform a ~y~Traffic stop~w~ on the suspect", 7500);
                
                if (Game.LocalPlayer.Character.CurrentVehicle.IsSirenOn == true)
                {
                    GameFiber.Sleep(5000);
                    if (!PursuitCreated)
                    {
                        if (Settings.AutomaticBackup)
                        {
                            Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
                            Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
                        }
                        Pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(Pursuit, Suspect);
                        Functions.AddPedToPursuit(Pursuit, guard);
                        Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                        Suspect.RelationshipGroup = "red";
                        guard.RelationshipGroup = "red";
                        Game.SetRelationshipBetweenRelationshipGroups("red", "COP", Relationship.Hate);
                        Suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        guard.Tasks.FightAgainst(Game.LocalPlayer.Character);


                        PursuitCreated = true;
                    }

                    if (PursuitCreated && !Functions.IsPursuitStillRunning(Pursuit))
                    {
                        End();
                    }
                }
            }

            else
            {
                guard.Tasks.FightAgainst(Game.LocalPlayer.Character);
                Suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
            }
        }
    }
}