using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBAR.Enums;
using Terraria;
using Terraria.ModLoader;

namespace TBAR.Players
{
    public partial class TBARPlayer : ModPlayer
    {
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)PacketType.SyncStand);
            packet.Write((byte)player.whoAmI);
            packet.Write(PlayerStand == null ? "None" : PlayerStand.GetType().Name);
            packet.Send(toWho, fromWho);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            TBARPlayer clone = clientPlayer as TBARPlayer;

            if (clone.PlayerStand != PlayerStand)
            {
                SendStandChangedPacket();
            }
        }

        public override void clientClone(ModPlayer clientClone)
        {
            TBARPlayer clone = clientClone as TBARPlayer;

            clone.PlayerStand = PlayerStand;
        }

        public void SendStandChangedPacket(int fromWho = -1)
        {
            Main.NewText("Packet sent");
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)PacketType.StandChanged);
            packet.Write((byte)player.whoAmI);
            packet.Write(PlayerStand == null ? "None" : PlayerStand.GetType().Name);
            packet.Send(-1, fromWho);
        }
    }
}
