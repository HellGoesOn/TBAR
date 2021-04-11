using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using TBAR.Components;
using TBAR.Input;
using TBAR.Players;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Stands
{
    public abstract class Stand : ModProjectile
    {
        public Stand(string name)
        {
            StandName = name;
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
            AddCombos();

            if (Combos.Count > 1)
            {
                Combos.Sort(
                    delegate (StandCombo x, StandCombo y)
                    {
                        if (x.RequiredInputs.Count > y.RequiredInputs.Count)
                            return -1;
                        else
                            return 1;
                    }
                );
            }
        }

        public abstract void AddStates();

        public abstract void AddCombos();

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

                if (HasReceivedInputs)
                {
                    foreach (StandCombo combo in Combos)
                    {
                        if (combo.TryActivate(ReceivedInputs))
                            ReceivedInputs.Clear();
                    }

                    if (State != LastState && States.ContainsKey(LastState))
                        States[LastState].Reset();

                    ReceivedInputs.Clear();
                }
            }
            else // some placeholder shit
                projectile.Kill();
        }

        public override bool PreKill(int timeLeft)
        {
            TBARPlayer.Get(Owner).KillStand();

            return base.PreKill(timeLeft);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(State);

            writer.Write(HasReceivedInputs);

            if (HasReceivedInputs)
            {
                writer.Write(ReceivedInputs.Count - 1);

                for (int i = 0; i < ReceivedInputs.Count - 1; i++)
                    writer.Write((int)ReceivedInputs[i]);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            State = reader.ReadInt32();

            bool hasReceivedInputs = reader.ReadBoolean();

            if (hasReceivedInputs)
            {
                int count = reader.ReadInt32();

                for (int i = 0; i < count; i++)
                    ReceivedInputs.Add((ComboInput)reader.ReadInt32());
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

        public List<StandCombo> Combos { get; } = new List<StandCombo>();

        public bool HasReceivedInputs => ReceivedInputs.Count > 0;

        public List<ComboInput> ReceivedInputs { get; } = new List<ComboInput>(10);

        public SpriteAnimation CurrentState => States[State];

        /// <summary>
        /// Contains all the available states for the stand
        /// </summary>
        public readonly Dictionary<int, SpriteAnimation> States = new Dictionary<int, SpriteAnimation>();
    }
}
