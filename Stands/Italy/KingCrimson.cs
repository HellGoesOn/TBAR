using TBAR.Components;
using TBAR.Input;
using TBAR.Projectiles.Stands.Italy.KingCrimson;
using TBAR.Projectiles.Visual;
using TBAR.TimeSkip;
using TBAR.UI.ScreenEffects.TimeSkip;
using Terraria;

namespace TBAR.Stands.Italy
{
    public class KingCrimson : SingleEntityStand
    {
        public KingCrimson() : base(new KingCrimsonProjectile(), "King Crimson")
        {
        }

        public override SpriteAnimation AlbumEntryAnimation()
        {
            return new SpriteAnimation("Projectiles/Stands/Italy/KingCrimson/KCIdle", 5, 5, true);
        }

        public override void InitializeCombos()
        {
            StandCombo timeErase = new StandCombo("Court of the Crimson King", ComboInput.Action1, ComboInput.Action2, ComboInput.Action2, ComboInput.Action1);
            timeErase.OnActivate += TimeErase_OnActivate;

            AddGlobalCombos(timeErase);
        }

        private void TimeErase_OnActivate(Player player)
        {
            TBARMusic.AddTrackToQueue("Sounds/Music/KingCrimsonMusic", 600);
            FakeTilesProjectile.Create(player.Center);
            TimeSkipVisual vs = TimeSkipVisual.Start();
            vs.Animation.AnimationPlay += Animation_AnimationPlay;
            TBAR.TimeSkipManager.AddEffect(new TimeSkipInstance(player, 600));
        }

        private void Animation_AnimationPlay(SpriteAnimation sender)
        {
            if (sender.CurrentFrame == sender.FrameCount / 3)
                TBAR.Instance.PlaySound("Sounds/StandAbilityEffects/BigTimeSkip");
        }
    }
}
