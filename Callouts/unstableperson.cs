using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;
using LSPD_First_Response.Mod.Menus;
using StopThePed;
using UltimateBackup;


namespace HypersCallouts.Callouts
{
    [CalloutInfo("[HC] Unstable Person", CalloutProbability.Low)]
    [CalloutInterfaceAPI.CalloutInterface("Mentally unstable person", CalloutProbability.Medium, "Person threatening to harm other persons", "Code 2", "LSPD")]
    public class UnstablePeron : Callout
    {
        private Ped suspect1;
        private Blip Suspectblip1;
        private Vector3 Spawnpoint;
        private bool SuspectAttack;
        private int counter;
        private string MaleFemale;
        private bool suspectdone;
        private float heading;
        private readonly List<string> SuicideDialog = new List<string>()
        {
            //put here the dialoge 

        };

        public override bool OnBeforeCalloutDisplayed()
        {
            if (!Settings.unstableperson)
            {
                Game.LogTrivial("[LOG]: User has disabled this callout.");
                return false;
            }
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "[HC] Unstable Individual Spotted With A Bladed Weapon";
            CalloutPosition = Spawnpoint;
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("WE_HAVE_A_CRIME_BRANDISHING_WEAPON_03 IN_OR_ON_POSITION", Spawnpoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            suspect1 = new Ped("s_m_y_factory_01", Spawnpoint, heading);
            suspect1.IsPersistent = true;
            suspect1.BlockPermanentEvents = true;
            suspect1.Inventory.GiveNewWeapon(WeaponHash.Knife, 2, true);


            Suspectblip1 = suspect1.AttachBlip();
            Suspectblip1.IsRouteEnabled = true;

            if (suspect1.IsMale)
                MaleFemale = "sir";
            else
                MaleFemale = "mam";
            counter = 0;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            try
            {
                if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 50f)
                {
                    Game.DisplayHelp("Press ~y~" + Settings.Dialog + " ~w~ to talk to the suspect");
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    {
                        counter++;
                        if (counter == 1)
                        {
                            Game.DisplaySubtitle("~b~Player~w~: " + MaleFemale + " You can't be walking on open roads like that, please walk towards a footpath!");
                        }
                        if (counter == 2)
                        {
                            Game.DisplaySubtitle("~r~suspect~w~: *uhghgh* Get out of my face you damn moron!");
                        }
                        if (counter == 3)
                        {
                            Game.DisplaySubtitle("~b~Player~w~: " + MaleFemale + " I am not going to repeat myself, this is an order!");
                        }
                        if (counter == 4)
                        {
                            Game.DisplaySubtitle("~r~suspect~w~: *uhgh* Who are you and why would I even listen to you?!");
                        }
                        if (counter == 5)
                        {
                            Game.DisplaySubtitle("~b~Player~w~: " + MaleFemale + " I'm a Law Representative, in other words a Police Officer for this State. PLease walk towards a footpatch" + MaleFemale);
                        }
                        if (counter == 6)
                        {
                            Game.DisplaySubtitle("~r~suspect~w~: I don't care who you are. No one gives me orders. BE CARFUL WHO IS THAT BEHIND YOU?");
                            if (!SuspectAttack)
                            {
                                if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 40f)
                                {
                                    GameFiber.Sleep(2000);
                                    suspectdone = true;
                                    Suspectblip1.IsRouteEnabled = false;
                                    suspect1.Tasks.FightAgainst(Game.LocalPlayer.Character);

                                    suspect1.KeepTasks = true;
                                    SuspectAttack = true;
                                }
                            }
                        }
                        if (counter == 7)
                        {
                            Game.DisplaySubtitle("~b~Player~w~: " + MaleFemale + " What do you mean, there is no one behind me!");
                            
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Game.LogTrivial("Error in OnCalloutAccepted: " + e.Message);
                Helper.CrashLog("Crashed in process", e);
                End();
            }
            if (suspect1.IsCuffed || suspect1.IsDead || Game.LocalPlayer.Character.IsDead || !suspect1.Exists())
            {
                End();
            }
        }
        public override void End()
        {
            base.End();

            if (suspect1.Exists())
            {
                suspect1.Dismiss();
            }
            if (Suspectblip1.Exists())
            {
                Suspectblip1.Delete();
            }

            Game.LogTrivial("HypersCallouts: UnstablePerson Is cleaned up!");
        }
    }
}