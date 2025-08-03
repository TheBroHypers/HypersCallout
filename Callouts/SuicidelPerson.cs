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
using CalloutInterfaceAPI;

namespace HypersCallouts.Callouts
{
    [CalloutInfo("[HC] Suicidal Person", CalloutProbability.High)]
    [CalloutInterfaceAPI.CalloutInterface("Suicidal Person", CalloutProbability.Medium, "Person threatening to do suicide ", "Code 2", "LSPD")]
    public class SuicidelPerson : Callout
    {
        private Ped suspect1;
        private Blip Suspectblip1;
        private Vector3 Spawnpoint;
        private Vector3 Rooftopspawn;
        private int rand;
        private Vector3 Spawnpoint3;
        private bool SuspectAttack;
        private string MaleFemale;
        private bool suspectdone;
        private bool surrendering;
        private bool dialdone;
        private bool Dissionmade;
        private float heading;
        private Blip DangerZone;
        private int dissicion;
        private int dissicionfailed;
        private bool RoofSpawn;
        private bool bridgespawn;
        private bool groundspawn;
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

        private readonly List<string> SuicideDialog = new List<string>()
        {
            "~r~Suspect~w~: Dont come any closer i swear!",
            "~b~Player~w~:  Mam drop the gun! ",
            "~r~Suspect~w~: No Stay away from me! I'm gonna fucking shoot if u come any closer i swear!",
            "~b~Player~w~:  mam Please just drop your gun. ",
            "~r~Suspect~w~: No i'm fucking gonna kill myself if u come any closer!",
            "~b~Player~w~:  Alright i'm gonna stay right here. What happend?",
            "~r~Suspect~w~: My boyfriend died...",
            "~b~Player~w~:  I know how u are feeling right know.. But u dont need to do this!",
            "~r~Suspect~w~  No u dont! and i need to do this!",
            "~b~Player~w~:  No u dont need to do this please let me help u"
        };
        private readonly List<string> suicidaldialog2 = new List<string>()
        {
            "~r~Suspect~w~: Don't come closer or I'll shoot my self or you!",
            "~b~Player~w~: Alright I'll stay here mam, but please don't do any harm!",
            "~r~Suspect~w~: If you come closer I'll do harm!",
            "~b~Player~w~: I said I will stay here. Can I ask why you wanna do harm to yourself or others?",
            "~r~Suspect~w~: A person from my family died but that's none of your business!",
            "~b~Player~w~: I'm sorry for your loss mam, I know what it feels like.",
            "~r~Suspect~w~: No you don't!",
            "~b~Player~w~: Yes i do, but please dont harm yourself, im here to help you."
        };

        enum CalloutStates { None = 0, EnRoute, OnScene, DecisionMade };

        CalloutStates CalloutState = CalloutStates.None;


        public override bool OnBeforeCalloutDisplayed()
        {
            if (!Settings.SuicidelPerson)
            {
                Game.LogTrivial("[LOG]: User has disabled this callout.");
                return false;
            }
            Random r = new Random();
            rand = r.Next(1, 100);
            //for random spawns 
            Vector3[] Bridges = new Vector3[]
            {
                //Locations for the bridge sides
                new Vector3(597.002f, -835.697f, 42.508f),
                new Vector3(583.515f, -867.250f, 42.279f),
                new Vector3(-962.559f, -2735.258f, 35.024f),
                new Vector3(734.001f, 1193.950f, 349.145f)

            };
            Vector3[] Rooftops = new Vector3[]
            {
                //locations for the rooftops sides 
                new Vector3(398.310f, -1657.755f, 50.130f),
                new Vector3(66.453f, -1007.535f, 80.728f),
                new Vector3(15.482f, -1017.388f, 84.309f),
                new Vector3(-616.450f, -222.435f, 56.628f),
                new Vector3(302.451f, -1467.722f, 46.509f),

            };
            if (rand < 50 & !bridgespawn & !groundspawn & !RoofSpawn)
            {
                Rooftopspawn = Rooftops.OrderBy(x => x.DistanceTo(Game.LocalPlayer.Character.Position)).FirstOrDefault();
                CalloutPosition = Rooftopspawn;
                Random randd = new Random();
                suspect1 = new Ped(peds[randd.Next(peds.Length)], Rooftopspawn, heading);
                RoofSpawn = true;
            }
            else if (rand > 25 & !RoofSpawn & !groundspawn & !bridgespawn)
            {
                Spawnpoint3 = Bridges.OrderBy(x => x.DistanceTo(Game.LocalPlayer.Character.Position)).FirstOrDefault();
                CalloutPosition = Spawnpoint3;
                Random randd = new Random();
                suspect1 = new Ped(peds[randd.Next(peds.Length)], Spawnpoint3, heading);
                bridgespawn = true;
            }
            else if (rand < 25 & rand > 50 & !bridgespawn & !RoofSpawn & !groundspawn)
            {
                Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));
                CalloutPosition = Spawnpoint;
                Random randd = new Random();
                suspect1 = new Ped(peds[randd.Next(peds.Length)], Spawnpoint, heading);
                groundspawn = true;
            }

            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "[HC] Person threatening to do suicide";
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("WE_HAVE_A_CRIME_BRANDISHING_WEAPON_03 IN_OR_ON_POSITION", Spawnpoint);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            CalloutState = CalloutStates.EnRoute;
            var num = Tuple.Create(324.266);
           


            
            suspect1.Inventory.GiveNewWeapon(WeaponHash.Pistol, 2, true);
            suspect1.IsPersistent = true;
            suspect1.BlockPermanentEvents = true;
            Suspectblip1 = suspect1.AttachBlip();
            Suspectblip1.IsRouteEnabled = true;

            Game.DisplayHelp("~r~Suspect~w~ said to atleast stay away 10 meters from her.");
            GameFiber.Wait(5000);
            Game.DisplayHelp("Stay out of the ~r~red circle~w~ located on your GPS!");

            DangerZone = new Blip(suspect1.Position, 10);
            DangerZone.Alpha = 0.48f;
            DangerZone.Color = Color.Red;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            try
            {
                GameFiber.Yield();
                if (RoofSpawn == true)
                {
                    if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 50f)
                    {

                        if (CalloutState == CalloutStates.EnRoute)
                        {
                            CalloutState = CalloutStates.OnScene;
                        }
                        if (!suspectdone & !RoofSpawn & !bridgespawn)
                        {

                            suspect1.Tasks.AimWeaponAt(Game.LocalPlayer.Character, -1);
                            Game.DisplayHelp("Press ~y~ " + Settings.Dialog + " ~w~ to talk to the suspect");
                            suspectdone = true;
                        }
                      
                       
                        
                        if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 10f && !surrendering)
                        {
                            DangerzoneEndRoof();
                        }
                        if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 10f && dissicion != 0 && dissicion != 1 && dissicion != 2)
                        {
                            DangerzoneEndRoof();
                        }


                        if (Game.LocalPlayer.Character.DistanceTo(suspect1) > 10)
                        {
                            if (!dialdone)
                            {
                                if (Game.IsKeyDown(Settings.Dialog) && !Dissionmade)
                                {
                                    Random r = new Random();
                                    int Openingsdialog = r.Next(0, 100);
                                    if (Openingsdialog < 50)
                                    {
                                        Handler.Dialogue(SuicideDialog);

                                        Game.LogTrivial("Playing Suicidal dialog");
                                        surrendering = true;
                                    }
                                    else
                                    {
                                        Handler.Dialogue(suicidaldialog2);
                                        Game.LogTrivial("Playing Suicidal dialog");
                                        surrendering = true;
                                    }
                                    CalloutState = CalloutStates.DecisionMade;
                                    Ending3();
                                    Game.LogTrivial("random ending started");
                                    Dissionmade = true;
                                    dialdone = true;
                                }
                            }
                        }

                    }
                }
               else if (groundspawn == true)
                {
                    if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 50f)
                    {

                        if (CalloutState == CalloutStates.EnRoute)
                        {
                            CalloutState = CalloutStates.OnScene;
                        }
                        if (!suspectdone & !RoofSpawn & !bridgespawn)
                        {

                            suspect1.Tasks.AimWeaponAt(Game.LocalPlayer.Character, -1);
                            Game.DisplayHelp("Press ~y~ " + Settings.Dialog + " ~w~ to talk to the suspect");
                            suspectdone = true;
                        }
                       
                        if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 10f && !surrendering)
                        {
                            DangerzoneEnd();
                        }
                        if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 10f && dissicion != 0 && dissicion != 1 && dissicion != 2)
                        {
                            DangerzoneEnd();
                        }


                        if (Game.LocalPlayer.Character.DistanceTo(suspect1) > 10)
                        {
                            if (!dialdone)
                            {
                                if (Game.IsKeyDown(Settings.Dialog) && !Dissionmade)
                                {
                                    Random r = new Random();
                                    int Openingsdialog = r.Next(0, 100);
                                    if (Openingsdialog < 50)
                                    {
                                        Handler.Dialogue(SuicideDialog);

                                        Game.LogTrivial("Playing Suicidal dialog");
                                        surrendering = true;
                                    }
                                    else
                                    {
                                        Handler.Dialogue(suicidaldialog2);
                                        Game.LogTrivial("Playing Suicidal dialog");
                                        surrendering = true;
                                    }
                                    CalloutState = CalloutStates.DecisionMade;
                                   Startschooting();
                                    Game.LogTrivial("random ending started");
                                    Dissionmade = true;
                                    dialdone = true;
                                }
                            }
                        }

                    }
                }
                else if (bridgespawn == true)
                {
                    if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 50f)
                    {

                        if (CalloutState == CalloutStates.EnRoute)
                        {
                            CalloutState = CalloutStates.OnScene;
                        }
                        if (!suspectdone & !RoofSpawn & bridgespawn == true)
                        {

                            suspect1.Tasks.AimWeaponAt(Game.LocalPlayer.Character, -1);
                            Game.DisplayHelp("Press ~y~ " + Settings.Dialog + " ~w~ to talk to the suspect");
                            suspectdone = true;
                        }
                        
                        if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 10f && !surrendering)
                        {
                            DangerzoneEndBridge();
                        }
                        if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 10f && dissicion != 0 && dissicion != 1 && dissicion != 2)
                        {
                            DangerzoneEndBridge();
                        }


                        if (Game.LocalPlayer.Character.DistanceTo(suspect1) > 10)
                        {
                            if (!dialdone)
                            {
                                if (Game.IsKeyDown(Settings.Dialog) && !Dissionmade)
                                {
                                    Random r = new Random();
                                    int Openingsdialog = r.Next(0, 100);
                                    if (Openingsdialog < 50)
                                    {
                                        Handler.Dialogue(SuicideDialog);

                                        Game.LogTrivial("Playing Suicidal dialog");
                                        surrendering = true;
                                    }
                                    else
                                    {
                                        Handler.Dialogue(suicidaldialog2);
                                        Game.LogTrivial("Playing Suicidal dialog");
                                        surrendering = true;
                                    }
                                    CalloutState = CalloutStates.DecisionMade;
                                    Ending3();
                                    Game.LogTrivial("random ending started");
                                    Dissionmade = true;
                                    dialdone = true;
                                }
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
            if (DangerZone) DangerZone.Delete();


            Game.LogTrivial("HypersCallouts: Suicidal Is cleaned up!");
        }
        public void Startschooting()
        {
            Random e = new Random();
            dissicion = e.Next(0, 2);

            CalloutState = CalloutStates.DecisionMade;
            Game.HideHelp();

            GameFiber.Yield();
            if (dissicion == 0)
            {
                Game.DisplaySubtitle("~r~Suspect~w~ Alright i surrender");

                suspect1.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                DangerZone.Delete();
                surrendering = true;
                Game.LogTrivial("suspect surrendring");

            }
            else if (dissicion == 1)
            {
                suspect1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                Game.LogTrivial("suspect is shooting at " + Game.LocalPlayer.Character);
            }
            else
            {
                Game.DisplaySubtitle("~r~Suspect~w~: Im sorry i cant help it anymore");
                suspect1.Tasks.ClearImmediately();
                suspect1.Tasks.PlayAnimation("mp_suicide", "pistol", 8f, AnimationFlags.None).WaitForCompletion();
                suspect1.Kill();
                suspect1.DropsCurrentWeaponOnDeath = true;
                Game.LogTrivial("suspect killed herself");
            }
        }
        public void DangerzoneEnd()
        {

            Random a = new Random();
            dissicionfailed = a.Next(0, 100);

            CalloutState = CalloutStates.DecisionMade;
            Game.HideHelp();

            GameFiber.Yield();
            if (dissicionfailed < 50)
            {
                Game.DisplaySubtitle("~r~Suspect~w~: Im sorry i cant help it anymore");
                suspect1.Tasks.ClearImmediately();
                suspect1.Tasks.PlayAnimation("mp_suicide", "pistol", 8f, AnimationFlags.None).WaitForCompletion();
                suspect1.Kill();
                suspect1.DropsCurrentWeaponOnDeath = true;
            }
            else if(dissicionfailed > 50 && !SuspectAttack)
            {
                suspect1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                SuspectAttack = true;
            }

        }
        public void Ending3()
        {
            Random e = new Random();
            dissicion = e.Next(0, 2);

            CalloutState = CalloutStates.DecisionMade;
            Game.HideHelp();

            GameFiber.Yield();
            if (dissicion == 0)
            {
                Game.DisplaySubtitle("~r~Suspect~w~ Alright i surrender");

                suspect1.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                suspect1.Inventory.Weapons.Clear();
                DangerZone.Delete();
                surrendering = true;
                Game.LogTrivial("suspect surrendring");

            }
            else if (dissicion == 1 && !SuspectAttack)
            {
                suspect1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                Game.LogTrivial("suspect is shooting at " + Game.LocalPlayer.Character);
                SuspectAttack = true;
            }
            else
            {
                Game.DisplaySubtitle("~r~Suspect~w~: Im sorry i cant help it anymore");
               ;
                suspect1.Tasks.Jump();
                suspect1.Tasks.ClearImmediately();
                suspect1.DropsCurrentWeaponOnDeath = true;
                Game.LogTrivial("suspect killed herself");
            }
        }
        public void DangerzoneEndRoof()
        {

            Random a = new Random();
            dissicionfailed = a.Next(0, 100);

            CalloutState = CalloutStates.DecisionMade;
            Game.HideHelp();

            GameFiber.Yield();
            if (dissicionfailed < 50)
            {
                Game.DisplaySubtitle("~r~Suspect~w~: Im sorry i cant help it anymore");
                
                suspect1.Tasks.Jump();
                suspect1.Tasks.ClearImmediately();
                suspect1.DropsCurrentWeaponOnDeath = true;
            }
            else if (dissicionfailed >50 && !SuspectAttack)
            {
                suspect1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                SuspectAttack = true;
            }

        }
        public void DangerzoneEndBridge()
        {

            Random a = new Random();
            dissicionfailed = a.Next(0, 2);

            CalloutState = CalloutStates.DecisionMade;
            Game.HideHelp();

            GameFiber.Yield();
            if (dissicionfailed == 1)
            {
                Game.DisplaySubtitle("~r~Suspect~w~: Im sorry i cant help it anymore");
                
                suspect1.Tasks.Jump();
                suspect1.Tasks.ClearImmediately();
                suspect1.DropsCurrentWeaponOnDeath = true;
            }
            else if (dissicionfailed == 2 && !SuspectAttack)
            {
                suspect1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                SuspectAttack = true;
            }

        }
    }
}