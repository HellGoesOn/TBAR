using Microsoft.Xna.Framework;
using TBAR.Components;
using TBAR.Extensions;
using TBAR.Input;
using TBAR.Projectiles.Stands;
using TBAR.UI;
using Terraria;

namespace TBAR.Projectiles.Stands
{
    public enum TestStandStates : int
    {
        None,
        Summon,
        Idle,
        Sugma
    }

    public class TestStand : StandProjectile
    {
        public TestStand() : base("Test Stand")
        {
            Opacity = 0f;
        }

        public override void AI()
        {
            base.AI();

            if (State == (int)TestStandStates.Idle && TBARInputs.SummonStand.JustPressed)
                States.Clear();
        }

        public override void AddStates()
        {
            State = (int)TestStandStates.Summon;
            States.Add((int)TestStandStates.Summon, new SpriteAnimation("Stands/TestStand", 3, 1f));

            States[(int)TestStandStates.Summon].AnimationPlay += delegate { if (Opacity < 1) Opacity += 0.01f; projectile.Center = Owner.Center + new Vector2(0, -80); };
            States[(int)TestStandStates.Summon].OnAnimationEnd += delegate { State = (int)TestStandStates.Idle; };

            States.Add((int)TestStandStates.Idle, new SpriteAnimation("Stands/TestStand", 3, 8f, true));
            States.Add((int)TestStandStates.Sugma, new SpriteAnimation("Stands/TestStand", 3, 1f, true, 180));

            States[(int)TestStandStates.Idle].AnimationPlay += delegate { projectile.Center = Owner.Center + new Vector2(0, -80); };

            States[(int)TestStandStates.Sugma].AnimationPlay += Sugma;
            States[(int)TestStandStates.Sugma].AnimationPlay += OnSugmaBegin;
            States[(int)TestStandStates.Sugma].OnAnimationEnd += OnSugmaEnd; 
        }

        private void Sugma(SpriteAnimation sender)
        {
            projectile.Center = Owner.Center + new Vector2(40 * Owner.direction, -20);
        }

        private void OnSugmaBegin(SpriteAnimation sender)
        {
            TimeSkipManager.Instance.VFX.Animation.Active = true;

            sender.AnimationPlay -= OnSugmaBegin;

        }

        private void OnSugmaEnd(SpriteAnimation sender)
        {
            sender.AnimationPlay += OnSugmaBegin;
        }
    }
}
