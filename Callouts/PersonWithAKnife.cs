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
using StopThePed;
using UltimateBackup;

namespace HypersCallouts.Callouts
{
    [CalloutInfo("[HC] Person With a knife", CalloutProbability.Medium)]
    public class PersonWithAKnife : Callout
    {
        private Ped suspect1;
        private Blip Suspectblip1;
        private Vector3 Spawnpoint;
        private bool SuspectAttack;

        public override bool OnBeforeCalloutDisplayed()
        {
            if (!Settings.PersonWithAKnife)
            {
                Game.LogTrivial("[LOG]: User has disabled this callout.");
                return false;
            }
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "[HC] Person with a Knife";
            CalloutPosition = Spawnpoint;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE_A_CRIME_BRANDISHING_WEAPON_03 IN_OR_ON_POSITION", Spawnpoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            suspect1 = new Ped(Spawnpoint);
            suspect1.IsPersistent = true;
            suspect1.BlockPermanentEvents = true;
            Suspectblip1 = suspect1.AttachBlip();
            Suspectblip1.IsRouteEnabled = true;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            try
            {
                if (!SuspectAttack)
                {
                    if (Game.LocalPlayer.Character.DistanceTo(suspect1) <= 50f)
                    {
                        Suspectblip1.IsRouteEnabled = false;
                        suspect1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        suspect1.Inventory.GiveNewWeapon(WeaponHash.Knife, 2, true);
                        suspect1.KeepTasks = true;
                        SuspectAttack = true;
                    }
                }
            }
            catch (Exception e)
            {
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

            Game.LogTrivial("HypersCallouts: Person with a Knife Is cleaned up!");
        }
    }
}