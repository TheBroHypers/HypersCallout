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


namespace HypersCallouts.Callouts
{

    [CalloutInfo("[HC] Grand Theft Auto", CalloutProbability.VeryHigh)]
    [CalloutInterfaceAPI.CalloutInterface("[HC] Grand Theft Auto", CalloutProbability.Medium, "Theres a persuit going on", "Code 3", "LSPD")]
    internal class HighSpeedPursuit : Callout
    {
        private float heading;
        private Ped Suspect;
        private Vehicle SuspectVehicle;
        private Blip SuspectBlip;
        private LHandle Pursuit;
        private Vector3 Spawnpoint;
        private bool PursuitCreated;

        public override bool OnBeforeCalloutDisplayed()
        {
            if (!Settings.Highspeedchase)
            {
                Game.LogTrivial("[LOG]: User has disabled this callout.");
                return false;
            }
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "[HC] HighSpeed Pursuit in Progress";
            CalloutPosition = Spawnpoint;
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("WE_HAVE_A_CRIME_RESISTING_ARREST_02 IN_OR_ON_POSITION", Spawnpoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Model[] vehiclemodels = new Model[] { "Adder", "Banshee 900R", "Bullet", "Cheetah", "Cyclone", "Entity XF", "FMJ", "Grotti Furia",
                "Infernus", "Itali GTB Custom", "Osiris", "Penetrator", "RE-7B", "SC1", "Sultan RS", "T20",
                "Taipan", "Tempesta", "Turismo R", "Tyrant", "Vacca", "Vagner", "Visione", "Voltic",
                "X80 Proto", "Zentorno", "Progen Emerus",  "9F", "Alpha", "Banshee", "Bestia GTS", "Buffalo", "Carbonizzare", "Comet", "Coquette",
                "Elegy RH8", "Elegy Retro Custom", "Feltzer", "Furore GT", "Futo", "Jester", "Jester (Racecar)",
                "Khamelion", "Kuruma", "Lynx", "Massacro", "Neon", "Pariah", "Penumbra", "Raiden", "Rapid GT",
                "Schafter V12", "Specter Custom", "Sultan", "Surano", "Tampa", "Tropos Rallye", "Verlierer", "Blade", "Buccaneer", "Chino", "Dominator", "Dominator GTX", "Dukes", "Ellie", "Gauntlet",
                "Hermes", "Hotknife", "Impaler", "Lurcher", "Phoenix", "Picador", "Sabre Turbo", "Slamvan",
                "Stallion", "Vigero", "Virgo", "Yosemite", "Bifta", "Blazer", "Bodhi", "Brawler", "Dune Buggy", "Injection", "Kamacho", "Mesa",
                "Rancher XL", "Rebel", "Sandking XL", "Trophy Truck", "Asea", "Asterope", "Emperor", "Fugitive", "Glendale", "Ingot", "Intruder", "Premier",
                "Primo", "Regina", "Schafter", "Stanier", "Stratum", "Surge", "Tailgater", "Warrener", "Washington", "Baller", "BeeJay XL", "Cavalcade", "Dubsta", "FQ 2", "Granger", "Gresley", "Habanero",
                "Huntley S", "Landstalker", "Mesa", "Patriot", "Radius", "Rocoto", "Seminole", "Serrano", "XLS", "Cognoscenti Cabrio", "Exemplar", "F620", "Felon", "Felon GT", "Jackal", "Oracle", "Sentinel",
                "Windsor", "Zion",  "Akuma", "Bagger", "Bati 801", "BF400", "Carbon RS", "Cliffhanger", "Double-T", "Faggio",
                "Gargoyle", "Hakuchou", "Innovation", "Lectro", "Manchez", "Nemesis", "Nightblade", "PCJ-600",
                "Ruffian", "Sanchez", "Thrust", "Vindicator", "Bison", "Bobcat XL", "Boxville", "Burrito", "Journey", "Minivan", "Paradise", "Pony",
                "Rumpo", "Speedo", "Surfer", "Youga", "Benson", "Brickade", "Hauler", "Mule", "Phantom", "Pounder", "Rubble", "Tow Truck", "Wastelander", "Casco", "Coquette Classic", "JB 700", "Manana", "Monroe", "Peyote", "Pigalle", "Roosevelt",
                "Stinger", "Tornado", "Z-Type",  };
            SuspectVehicle = new Vehicle(vehiclemodels[new Random().Next(vehiclemodels.Length)], Spawnpoint, heading);
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
                    Pursuit = LSPD_First_Response.Mod.API.Functions.CreatePursuit();
                    LSPD_First_Response.Mod.API.Functions.AddPedToPursuit(Pursuit, Suspect);
                    LSPD_First_Response.Mod.API.Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                    PursuitCreated = true;
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
            Game.LogTrivial("HypersCallouts: HighSpeed Pursuit has been cleaned up!");
        }
    }
}