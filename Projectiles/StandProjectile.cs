using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using TBAR.Components;
using TBAR.Input;
using TBAR.Players;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands
{
    public abstract class StandProjectile : ModProjectile
    {
        public StandProjectile(string name)
        {
            StandName = name;
            States = new Dictionary<int, SpriteAnimation>();
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(StandName);
        }

        public sealed override void SetDefaults()
        {
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.netImportant = true;

            AddStates();
        }

        public abstract void AddStates();

        public override void AI()
        {
            projectile.timeLeft = 60;

            projectile.netUpdate = true;

            if (States.Count > 0)
            {
                CurrentState.UpdateEvent();
                CurrentState.Update();

                if (!CurrentState.Active)
                {
                    CurrentState.Reset();
                    State = LastState;
                }
            }
            else // some placeholder shit
                projectile.Kill();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(State);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            State = reader.ReadInt32();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false; // default drawing can suck my balls;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!TBARPlayer.Get().IsStandUser)
                return;

            if (States.Count > 0) 
                DefaultDrawStand(spriteBatch, projectile.Center, Color.White, SpriteFX);
        }

        public void DefaultDrawStand(SpriteBatch spriteBatch, Vector2 position, Color color, SpriteEffects fx = SpriteEffects.None)
        {
            spriteBatch.Draw(CurrentState.SpriteSheet, position - Main.screenPosition, CurrentState.FrameRect, color * Opacity, 0f, CurrentState.DrawOrigin, 1f, fx, 1f);
        }

        public Player Owner => Main.player[projectile.owner];

        public float Opacity { get; set; } = 1f;

        public SpriteEffects SpriteFX { get; set; }

        private int state;
        public int State 
        { 
            get => state; 
            set
            {
                LastState = state;
                state = value;
            }
        }

        public int LastState { get; set; }

        public string StandName { get; set; }

        public SpriteAnimation CurrentState => States[State];

        public Color AuraColor { get; set; } = new Color(255, 255, 255);

        /// <summary>
        /// Contains all the available states for the stand
        /// </summary>
        public Dictionary<int, SpriteAnimation> States { get; }

        public sealed override string Texture => "TBAR/Textures/EmptyPixel";
    }
}
