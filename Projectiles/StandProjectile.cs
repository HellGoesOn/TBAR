using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using TBAR.Components;
using TBAR.Enums;
using TBAR.Input;
using TBAR.Players;
using TBAR.Stands;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Projectiles.Stands
{
    public abstract class StandProjectile : ModProjectile
    {
        public StandProjectile(string name)
        {
            StandName = name;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(StandName);
        }

        public sealed override void SetDefaults()
        {
            States = new Dictionary<string, StandState>();

            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.netImportant = true;
            projectile.friendly = true;

            States.Clear();

            Scale = Vector2.One;

            if (projectile.active)
                AddStates(projectile);

            SafeSetDefaults();
        }

        public virtual void SafeSetDefaults() { }

        public abstract void AddStates(Projectile projectile);

        public override void AI()
        {
            projectile.timeLeft = 60;
            projectile.netUpdate = true;


            if (Main.myPlayer == Owner.whoAmI)
                MousePosition = Main.MouseWorld;

            if (States.Count > 0)
            {
                CurrentState.Update();
                
                if (!CurrentState.CurrentAnimation.Active)
                {
                    CurrentState.EndState();
                }
            }
            else // some placeholder shit
                projectile.Kill();
        }

        public virtual void HandleImmediateInputs(ImmediateInput input)
        { }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(MousePosition.X);
            writer.Write(MousePosition.Y);

            writer.Write((string)State);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            MousePosition = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            string receivedState = reader.ReadString();

            if (States.ContainsKey(receivedState))
            {
                SetState(receivedState);
            }
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
                DrawDefault(spriteBatch, projectile.Center, Color.White, SpriteFX);
        }

        public void DrawDefault(SpriteBatch spriteBatch, Vector2 position, Color color, SpriteEffects fx = SpriteEffects.None)
        {
            SpriteAnimation animation = CurrentState.AssignedAnimations[CurrentState.CurrentAnimationID];

            spriteBatch.Draw(animation.SpriteSheet, position - Main.screenPosition, animation.FrameRect, color * Opacity, 0f, animation.DrawOrigin, Scale, fx, 1f);
        }

        public Player Owner => Main.player[projectile.owner];

        public float Opacity { get; set; } = 1f;

        public SpriteEffects SpriteFX { get; set; }

        public int GetBaseDamage(DamageType type, Player player)
        {
            var currentDamage = 5;

            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];

                if (item.damage > currentDamage)
                {
                    switch (type)
                    {
                        case DamageType.Melee:
                            if (item.melee)
                                currentDamage = item.damage;
                            break;

                        case DamageType.Ranged:
                            if (item.ranged && item.useAmmo > 0)
                                currentDamage = item.damage;
                            break;

                        case DamageType.Magic:
                            if (item.magic)
                                currentDamage = item.damage;
                            break;

                        case DamageType.Summon:
                            if (item.summon)
                                currentDamage = item.damage;
                            break;

                        case DamageType.Any:
                            currentDamage = item.damage;
                            break;
                    }
                }
                else
                    continue;
            }

            return currentDamage;
        }

        public void SetState(string value)
        {
            if (State == value)
                return;

            LastState = State;

            State = value;

            CurrentState.BeginState();
        }

        public string LastState { get; private set; }

        public string State { get; private set; }

        public string StandName { get; set; }

        public Vector2 Scale { get; set; }

        public StandState CurrentState => States[State];

        public Color AuraColor { get; set; } = new Color(255, 255, 255);

        /// <summary>
        /// Contains all the available states for the stand
        /// </summary>
        public Dictionary<string, StandState> States { get; private set; }

        public TBARPlayer User => TBARPlayer.Get(Owner);

        public sealed override string Texture => "TBAR/Textures/EmptyPixel";

        public Vector2 MousePosition { get; set; }
    }
}
