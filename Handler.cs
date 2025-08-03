using Rage;
using LSPD_First_Response.Mod.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;


namespace HypersCallouts
{
    class Handler
    {
        private static int count;

        
        public static void Dialogue(List<string> dialogue, Ped animped = null, String animdict = "missfbi3_party_d", String animname = "stand_talk_loop_a_male1", float animspeed = -1, AnimationFlags animflag = AnimationFlags.Loop)
        {
            count = 0;
            while (count < dialogue.Count)
            {
                GameFiber.Yield();
                if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                {
                    if (animped != null && animped.Exists())
                    {
                        try
                        {
                            animped.Tasks.PlayAnimation(animdict, animname, animspeed, animflag);
                        }
                        catch (Exception) { }
                    }
                    Game.DisplaySubtitle(dialogue[count]);
                    count++;
                }
            }
        }
    }
}
