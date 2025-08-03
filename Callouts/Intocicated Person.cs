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
using StopThePed.API;


namespace HypersCallouts.Callouts
{

    [CalloutInfo("Intoxicatedperson", CalloutProbability.High)]
    public class Intoxicatedperson : Callout
    {
        private Ped Suspect;
        private Blip SuspectBlip;
        private float heading;
        private Vector3 Spawnpoint;
        private string Femalemale;
        private int counter;
        private bool SuspectDone;

        public AnimationDictionary New { get; private set; }

        public override bool OnBeforeCalloutDisplayed()
        {
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "Intoxicated Person in public";
            CalloutPosition = Spawnpoint;
            LSPD_First_Response.Mod.API. Functions.PlayScannerAudioUsingPosition("WE_HAVE_A_CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION", Spawnpoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Suspect = new Ped(Spawnpoint, heading);

            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;

            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.Color = Color.Blue;
            SuspectBlip.IsRouteEnabled = true;
           

            if (Suspect.IsMale)
                Femalemale = "sir";
            else
                Femalemale = "mam";
            counter = 0;


            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            base.Process();
            try
            {
                
                if (!SuspectDone)
                {
                    if (Game.LocalPlayer.Character.DistanceTo(Suspect) <= 20f)
                    {
                        Game.DisplayHelp("Press ~y~Y~w~ to talk to the suspect");
                        if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                        {
                            counter++;
                            if (counter == 1)
                            {
                                Helper.log("Counter 1");
                                Game.DisplaySubtitle("~b~Player~w~: Excuse me " + Femalemale + ", do you need any assistance?");
                            }
                            if (counter == 2)
                            {
                                Helper.log("Counter 2");
                                Game.DisplaySubtitle("~r~suspect~w~: Nahhh *hic* its all *hic* goood.");
                            }
                            if (counter == 3)
                            {
                                Helper.log("Counter 3");
                                Game.DisplaySubtitle("~b~Player~w~: Being intoxicated in public is not legal. I'm going to have to write you a citation");
                            }
                            if (counter == 4)
                            {
                                Helper.log("Counter 4");
                                Game.DisplaySubtitle("~r~Suspect~w~: A ciwhaat, *hic* Get outta my *hic* sight.");
                                Suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                                Suspect.KeepTasks = true;
                                SuspectDone = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Helper.CrashLog("Crashed in process", e);
                End();
            }
            if (Suspect.IsCuffed || Suspect.IsDead || Game.LocalPlayer.Character.IsDead || !Suspect.Exists())
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
            Game.LogTrivial("HypersCallouts: Intoxicated person  has been cleaned up!");
        }
    }
}