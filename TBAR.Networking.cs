using Microsoft.Xna.Framework;
using System.IO;
using TBAR.Enums;
using TBAR.Input;
using TBAR.Players;
using TBAR.Stands;
using TBAR.TimeStop;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR
{
    public partial class TBAR : Mod
    {
        bool IsSender(int whoAmI) => Main.myPlayer != whoAmI;

        bool IsServer => Main.netMode == NetmodeID.Server;

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketType type = (PacketType)reader.ReadByte();

            switch (type)
            {
                case PacketType.TimeStopServer:
                    EntityType eType = (EntityType)reader.ReadByte();
                    int index = reader.ReadInt32();
                    int duration = reader.ReadInt32();

                    if (!IsSender(whoAmI))
                        TimeStopManager.Instance.TryStopTime(eType, index, duration);

                    if (IsServer)
                        TimeStopManager.Instance.SendPacket(eType, index, duration, whoAmI);
                    break;

                case PacketType.SyncStand:
                    byte playerNumber = reader.ReadByte();
                    string name = reader.ReadString();
                    TBARPlayer plr = TBARPlayer.Get(Main.player[playerNumber]);

                    if (!IsSender(whoAmI))
                        plr.PlayerStand = StandFactory.Instance.Get(name);

                    break;

                case PacketType.UsedCombo:
                    playerNumber = reader.ReadByte();
                    string comboName = reader.ReadString();

                    plr = TBARPlayer.Get(Main.player[playerNumber]);

                    if (!IsSender(whoAmI))
                        plr.PlayerStand.ForceCombo(comboName, plr.player);

                    if (IsServer)
                        StandCombo.SendPacket(plr.player, comboName, whoAmI);

                    break;

                case PacketType.StandChanged:
                    playerNumber = reader.ReadByte();
                    name = reader.ReadString();
                    plr = TBARPlayer.Get(Main.player[playerNumber]);

                    if (!IsSender(whoAmI))
                        plr.PlayerStand = StandFactory.Instance.Get(name);

                    if (IsServer)
                        plr.SendStandChangedPacket(playerNumber);
                    break;
            }
        }
    }
}
