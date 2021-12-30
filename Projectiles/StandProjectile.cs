using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
        public Dictionary<string, Animation2D> Animations { get; private set; }

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
            Animations = new Dictionary<string, Animation2D>();
            States = new Dictionary<string, StandState>();

            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.netImportant = true;
            projectile.friendly = true;

            States.Clear();
            Animations.Clear();

            Scale = Vector2.One;

            if (projectile.active)
                InitializeStates(projectile);

            if (Animations.Count <= 0)
            {
                Animations.Add("Forgor", new Animation2D("Textures/Forgor", 1));
                CurrentAnimation = "Forgor";
            }

            SafeSetDefaults();
        }

        public virtual void SafeSetDefaults() { }

        public abstract void InitializeStates(Projectile projectile);

        public override void AI()
        {
            projectile.timeLeft = 60;
            projectile.netUpdate = true;


            if (Main.myPlayer == Owner.whoAmI)
                MousePosition = Main.MouseWorld;

            if (CurrentAnimation != null && Animations.Count > 0)
                Animations[CurrentAnimation].UpdateAnimation();

            if (States.Count > 0)
            {
                CurrentState.Update();
                
                if (!CurrentState.Active)
                {
                    CurrentState.EndState();
                }
            }
            else // some placeholder shit ?
                projectile.Kill();
        }

        public virtual void HandleImmediateInputs(ImmediateInput input)
        { }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(MousePosition.X);
            writer.Write(MousePosition.Y);

            writer.Write((string)State);
            writer.Write(CurrentAnimation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            MousePosition = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            string receivedState = reader.ReadString();
            string receivedAnimation = reader.ReadString();

            if (States.ContainsKey(receivedState))
            {
                SetState(receivedState, Animations.ContainsKey(receivedAnimation) ? receivedAnimation : "None");
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
            if (CurrentAnimation != null)
            {
                Animation2D animation = Animations[CurrentAnimation];

                spriteBatch.Draw(animation.SpriteSheet, position - Main.screenPosition, animation.FrameRect, color * Opacity, 0f, animation.FrameCenter, Scale, fx, 1f);
            }
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

                        case DamageType.Summoner:
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

        /// <summary>
        /// Sets Stand's AI state to the one with the provided key
        /// </summary>
        /// <param name="state">State key</param>
        /// <param name="animationKey">Animation key <para>If no animation key is provided, will look for animation with the same key as state.</para><para>In case no key is found, does not switch the animation</para></param>
        public void SetState(string state, string animationKey = "None")
        {
            if (State == state)
                return;

            if (animationKey == "None")
            {
                if(Animations.ContainsKey(state))
                    CurrentAnimation = state;
            }
            else
                CurrentAnimation = animationKey;

            State = state;

            CurrentState.BeginState();
        }

        public void SetState(Enum state, string animationkey = "None") => SetState(state.ToString(), animationkey);

        protected StandState AddState(string key, int duration = 0)
        {
            StandState output = new StandState(duration);
            States.Add(key, output);
            return output;
        }

        //lazy
        protected StandState AddState(Enum key, int duration = 0) => AddState(key.ToString(), duration);

        protected Animation2D AddAnimation(Enum key, string sheetPath, int frameCount, float fps = 10f, bool looping = false, string modName = "TBAR")
        {
            return AddAnimation(key.ToString(), sheetPath, frameCount, fps, looping, modName);
        }

        protected Animation2D AddAnimation(string key, string sheetPath, int frameCount, float fps = 10f, bool looping = false, string modName = "TBAR")
        {
            Animation2D result = new Animation2D(sheetPath, frameCount, fps, looping, modName);

            Animations.Add(key, result);

            return result;
        }
        
        protected Animation2D AddAnimation(Enum key, Animation2D animation)
        {
            return AddAnimation(key.ToString(), animation);
        }

        protected Animation2D AddAnimation(string key, Animation2D animation)
        {
            Animation2D result = animation;

            Animations.Add(key, result);

            return result;
        }

        private string curAnim;
        public string CurrentAnimation
        {
            get => curAnim;
            set
            {
                if(curAnim != null && curAnim != "" && Animations.ContainsKey(curAnim))
                    Animations[curAnim].Reset();

                curAnim = value;
                Animations[curAnim].Reset();
            }
        }

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
