using CalloutInterfaceAPI;
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
using System.Collections;
using Rage.Native;



namespace HypersCallouts.Callouts
{
    [CalloutInfo("[HC] Stolen vehicle reported", CalloutProbability.Medium)]
    [CalloutInterfaceAPI.CalloutInterface("[HC] stolen vehicle reported", CalloutProbability.Medium, "Report of a stolen vehicle", "Code 2", "LSPD")]
    public class StolenVehicle : Callout
    {
        private Ped person;
        private Ped Suspect;
        private Vehicle SuspectVehicle;
        private Blip SuspectBlip;
        private Blip VictimBlip;
        private LHandle Pursuit;
        private Vector3 Spawnpoint;
        private Vector3 Spawnpoint2;
        private float NpcSpawnpoint;
        private bool PursuitCreated;
        private int counter;
        private string MaleFemale;
        private bool Display;
        private bool Person;
        public Vector3 _searcharea;
        public Blip _Blip;
        private int timer = 0;
        private Vector3 areablip;
        private bool Dissionmade;
        private bool dialdone;
        private bool update;
        private int randomending;
        private string[] peds = new string[]
       {
            "a_f_m_beach_01",
            "a_f_m_bevhills_01",
            "a_f_m_bevhills_02",
            "a_f_m_bodybuild_01",
            "a_f_m_business_02",
            "a_f_m_downtown_0",
            "a_f_m_eastsa_01",
            "a_f_m_eastsa_02",
            "a_f_m_fatbla_01",
            "a_f_m_fatcult_01",
            "a_f_m_fatwhite_01",
            "a_f_m_ktown_01",
            "a_f_m_ktown_02",
            "a_f_m_prolhost_01",
            "a_f_m_salton_01",
            "a_f_m_skidrow_01",
            "a_f_m_soucent_01",
            "a_f_m_soucent_02",
            "a_f_m_soucentmc_01",
            "a_f_m_tourist_01",
            "a_f_m_tramp_01",
            "a_f_m_trampbeac_0",
            "a_f_o_genstreet_01",
            "a_f_o_indian_01",
            "a_f_y_beach_01",
            "a_f_y_bevhills_01",
            "a_f_y_bevhills_02",
            "a_f_y_bevhills_03",
            "a_f_y_hipster_02",
            "s_f_y_bartender_01",
            "s_f_y_baywatch_01",
            "s_f_y_shop_mid",
            "u_f_y_dancelthr_01",
            "ig_tracydisanto"
       };
        private float heading;
        private readonly LSPD_First_Response.Engine.Scripting.Entities.Persona persona;
        private bool atGetIn;
        private readonly List<string> SuicideDialog = new List<string>()
        {
            "~b~Player~w~: Excuse me mam, Did you call 911??",
            "~g~Victim~w~: Yes i called 911.",
            "~b~Player~w~:  can u tell me what happend? ",
            "~g~Victim~w~: So i was just shopping and i was going outside and i saw a person taking my car and he drove off. ",
            "~b~Player~w~: alright can you tell me what car it is and the licence plate?",
            "~g~Victim~w~: It is a Black Dukes. licence plate: ~b~85LZN448~w~",
            "~b~Player~w~: Oke we are gonna take care of it."

        };

        public override bool OnBeforeCalloutDisplayed()
        {
            if (!Settings.StolenVehicle)
            {
                Game.LogTrivial("[LOG]: User has disabled this callout.");
                return false;
            }

            Random rand = new Random();

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

            Spawnpoint2 = spawnpoints.OrderBy(x => x.DistanceTo(Game.LocalPlayer.Character.Position)).FirstOrDefault();


            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(600f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint2, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint2);
            CalloutMessage = "[HC] Report of a stolen vehicle";
            CalloutPosition = Spawnpoint2;
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("WE_HAVE_A_CRIME_RESISTING_ARREST_02 IN_OR_ON_POSITION", Spawnpoint2);


            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            
            SuspectVehicle = new Vehicle("DUKES", Spawnpoint);
            SuspectVehicle.LicensePlate = "85LZN448";
            SuspectVehicle.PrimaryColor = Color.Black;
            SuspectVehicle.SecondaryColor = Color.Black;
            SuspectVehicle.PearlescentColor = Color.Black;
            SuspectVehicle.IsPersistent = true;
            SuspectVehicle.IsStolen = true;

            Suspect = new Ped(SuspectVehicle.GetOffsetPositionFront(5f));
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.WarpIntoVehicle(SuspectVehicle, -1);
            
            Random rand = new Random();
            person = new Ped(peds[rand.Next(peds.Length)], Spawnpoint2, NpcSpawnpoint);
            person.IsPersistent = true;
            person.BlockPermanentEvents = true;
            LSPD_First_Response.Mod.API.Functions.SetVehicleOwnerName(SuspectVehicle, LSPD_First_Response.Mod.API.Functions.GetPersonaForPed(person).FullName);
            Game.LogTrivial(LSPD_First_Response.Mod.API.Functions.GetVehicleOwnerName(SuspectVehicle));
            Game.LogTrivial(LSPD_First_Response.Mod.API.Functions.GetPersonaForPed(person).FullName.ToString());
            
            VictimBlip = person.AttachBlip();
            VictimBlip.Color = Color.Blue;
            VictimBlip.IsRouteEnabled = true;


            if (person.IsMale)
                MaleFemale = "sir";
            else
                MaleFemale = "mam";
            counter = 0;

            PursuitCreated = false;
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            try
            {
                if (Game.LocalPlayer.Character.DistanceTo(person) < 52f)
                {
                   
                    if (!Display)
                    {
                        Game.DisplayHelp("Press ~y~" + Settings.Dialog + " ~w~ To talk to the victim");

                        person.Face(Game.LocalPlayer.Character);
                        person.KeepTasks = true;
                        VictimBlip.IsRouteEnabled = false;
                        Display = true;
                    }
                    if (!dialdone)
                    {
                        if (Game.IsKeyDown(Settings.Dialog))
                        {
                            Handler.Dialogue(SuicideDialog);
                            VictimBlip.Delete();
                            Suspect.Tasks.CruiseWithVehicle(SuspectVehicle, 40, VehicleDrivingFlags.FollowTraffic);

                            dialdone = true;
                           
                        }
                        while (Game.LocalPlayer.Character.DistanceTo(SuspectVehicle) >10 && !update && dialdone == true)
                        {
                            Updateblip();
                            _Blip.IsRouteEnabled = true;
                            GameFiber.Sleep(5000);
                            _Blip.Delete();

                            if (Game.LocalPlayer.Character.DistanceTo(SuspectVehicle) <30f && !Dissionmade && !update)
                            {
                                
                                SuspectBlip = SuspectVehicle.AttachBlip();
                                SuspectBlip.Color = Color.Red;
                                update = true;
                                Dissionmade = true;
                                Randomend();


                            }
                        }
                       
                    }
                }
                if (Suspect.IsCuffed || Suspect.IsDead || Game.LocalPlayer.Character.IsDead || !Suspect.Exists())
                {
                    End();
                }
            }
            catch (Exception e)
            {
                Helper.CrashLog("Crashed in process", e);
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
            
            if (SuspectVehicle.Exists())
            {
                SuspectVehicle.Dismiss();
            }
            if (person.Exists())
            {
                person.Dismiss();
            }
            if (VictimBlip.Exists())
            {
                VictimBlip.Delete();
            }
            if (_Blip.Exists())
            {
                _Blip.Delete();
            }
            Game.LogTrivial("HypersCallouts: Stolen Vehicle has been cleaned up!");
        }
       public void Randomend()
        {
            Random r = new Random();
            randomending = r.Next(0, 100);
            

            
            if (randomending >50)
            {
                Game.LogTrivial(randomending + "0");
                if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(SuspectVehicle) <= 40f)
                {
                    if (Settings.AutomaticBackup)
                    {
                       LSPD_First_Response.Mod.API.Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
                       LSPD_First_Response.Mod.API.Functions.RequestBackup(Suspect.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.LocalUnit);
                    }
                    Pursuit = LSPD_First_Response.Mod.API.Functions.CreatePursuit();
                    LSPD_First_Response.Mod.API.Functions.AddPedToPursuit(Pursuit, Suspect);
                    LSPD_First_Response.Mod.API.Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                    LSPD_First_Response.Mod.API.Functions.SetVehicleOwnerName(SuspectVehicle, LSPD_First_Response.Mod.API.Functions.GetPersonaForPed(person).FullName);
                    PursuitCreated = true;
                }

                if (PursuitCreated && !LSPD_First_Response.Mod.API.Functions.IsPursuitStillRunning(Pursuit))
                {
                    End();
                }
            }
            else if (randomending <50)
            {
                Game.LogTrivial(randomending + "1");
                Game.DisplayHelp("Perform a ~y~Traffic stop~w~ on the suspect", 7500);
                StopThePed.API.Functions.setVehicleInsuranceStatus(SuspectVehicle, StopThePed.API.STPVehicleStatus.None);
                StopThePed.API.Functions.setVehicleRegistrationStatus(SuspectVehicle, StopThePed.API.STPVehicleStatus.Valid);
                LSPD_First_Response.Mod.API.Functions.SetVehicleOwnerName(SuspectVehicle, LSPD_First_Response.Mod.API.Functions.GetPersonaForPed(person).FullName);
                
                SuspectVehicle.IsStolen = true;

                if (Suspect.IsCuffed || Suspect.IsDead || Game.LocalPlayer.Character.IsDead || !Suspect.Exists())
                {
                    End();
                }
            }
        }
        public void Updateblip()
        {
            _Blip = new Blip(Suspect.Position, 100f);
            _Blip.Alpha = 0.5f;
            _Blip.Color = Color.Yellow;
        }
    }
}