using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TBAR.Projectiles.Stands;
using TBAR.TimeSkip;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.NPCs
{
    public partial class TBARGlobalNPC : GlobalNPC
    {
        private int elapsedTime;

        public void PreTimeSkipAI(NPC npc)
        {
            if(npc.type == NPCID.CultistBoss)
            {
                foreach (TimeSkipData dt in TimeSkipStates)
                    Main.NewText(dt);
            }

            var IsTimeSkipped = TBAR.TimeSkipManager.IsTimeSkipped;

            if (IsTimeSkipped)
            {
                elapsedTime++;
                if (elapsedTime == 6)
                {
                    TimeSkipStates.Add
                    (
                        new TimeSkipData(npc.Center, npc.velocity, npc.scale, npc.rotation, npc.frame, npc.direction, npc.ai)
                    );

                    elapsedTime = 0;
                }

                if (TimeSkipStates.Count > 12)
                    TimeSkipStates.RemoveAt(0);

                if (TimeSkipStates.Count <= 0)
                    for (int i = 0; i < 13; i++)
                        TimeSkipStates.Add
                        (
                            new TimeSkipData(npc.Center, npc.velocity, npc.scale, npc.rotation, npc.frame, npc.direction, npc.ai)
                        );
            }
            else
                elapsedTime = 0;

            if (!IsTimeSkipped && TimeSkipStates.Count > 0)
            {
                /*
                npc.ai = TimeSkipStates[0].AI;
                npc.Center = TimeSkipStates[0].Position;
                npc.scale = TimeSkipStates[0].Scale;
                npc.direction = TimeSkipStates[0].Direction;
                */

                TimeSkipStates.Clear();
            }
        }

        // terrible implementantion but will do for now?
        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (TBAR.TimeSkipManager.IsTimeSkipped)
            {
                int halfWidth = (int)(npc.Hitbox.Width * 0.5f);
                int halfHeight = (int)(npc.Hitbox.Height * 0.5f);

                int x = (int)TimeSkipStates[0].Position.X - halfWidth;
                int y = (int)TimeSkipStates[0].Position.Y - halfHeight;

                Rectangle rect = new Rectangle(x, y, npc.Hitbox.Width, npc.Hitbox.Height);

                if (rect.Intersects(projectile.Hitbox))
                {
                    npc.immune[projectile.owner] += projectile.penetrate != 1 ? 20 : 0;

                    if (projectile.modProjectile is PunchGhostProjectile stand)
                        npc.immune[projectile.owner] = stand.AttackSpeed;
                    
                    if (projectile.penetrate > 0 && !(projectile.modProjectile is PunchGhostProjectile))
                        projectile.penetrate--;

                    Main.LocalPlayer.ApplyDamageToNPC(npc, projectile.damage, projectile.knockBack, projectile.direction, false);
                    
                }
            }

            return base.CanBeHitByProjectile(npc, projectile);
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (TBAR.TimeSkipManager.IsTimeSkipped)
                drawColor = Color.Red * 0.5f;
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            if (TBAR.TimeSkipManager.IsTimeSkipped)
            {
                Texture2D texture = Main.npcTexture[npc.type];
                int frameCount = Main.npcFrameCount[npc.type];
                int frameHeight = texture.Height / frameCount;

                Vector2 drawOrig = new Vector2(texture.Width * 0.5f, frameHeight * 0.5f);

                for (int i = TimeSkipStates.Count - 1; i > 0; i--)
                {
                    SpriteEffects spriteEffects = TimeSkipStates[i].Direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    spriteBatch.Draw(texture, TimeSkipStates[i].Position - Main.screenPosition, TimeSkipStates[i].Frame, (i == 1 ? Color.White : Color.Red * 0.5f), TimeSkipStates[i].Rotation, drawOrig, TimeSkipStates[i].Scale, spriteEffects, 1f);
                }
            }

            return base.PreDraw(npc, spriteBatch, drawColor);
        }

        public void PostKingDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            if (TBAR.TimeSkipManager.IsTimeSkipped)
            {
                Texture2D texture = Main.npcTexture[npc.type];
                int frameCount = Main.npcFrameCount[npc.type];
                int frameHeight = texture.Height / frameCount;

                Vector2 drawOrig = new Vector2(texture.Width * 0.5f, frameHeight * 0.5f);

                for (int i = TimeSkipStates.Count - 1; i > 0; i--)
                {
                    SpriteEffects spriteEffects = TimeSkipStates[i].Direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    spriteBatch.Draw(texture, TimeSkipStates[i].Position - Main.screenPosition, TimeSkipStates[i].Frame, (i == 1 ? Color.White : Color.Red * 0.5f), TimeSkipStates[i].Rotation, drawOrig, TimeSkipStates[i].Scale, spriteEffects, 1f);
                }
            }
        }

        public List<TimeSkipData> TimeSkipStates { get; private set; }
    }
}
