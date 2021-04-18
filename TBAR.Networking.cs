﻿using Microsoft.Xna.Framework;
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
        bool IsServer => Main.netMode == NetmodeID.Server;

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketType type = (PacketType)reader.ReadByte();

            switch (type)
            {
                case PacketType.SyncStand:
                    byte playerNumber = reader.ReadByte();
                    string name = reader.ReadString();
                    TBARPlayer plr = TBARPlayer.Get(Main.player[playerNumber]);

                    plr.PlayerStand = StandFactory.Instance.Get(name);

                    if(IsServer)
                        plr.PlayerStand = StandFactory.Instance.Get(name);

                    break;

                case PacketType.UsedCombo:
                    this.Logger.Debug("Combo packet received");
                    this.Logger.DebugFormat("State before packet {0}", TimeStopManager.Instance.IsTimeStopped);
                    playerNumber = reader.ReadByte();
                    string comboName = reader.ReadString();

                    this.Logger.Debug($"Combo Name {comboName}");
                    plr = TBARPlayer.Get(Main.player[playerNumber]);

                    plr.PlayerStand.ForceCombo(comboName, plr.player);

                    if (IsServer)
                        StandCombo.SendPacket(plr.player, comboName, whoAmI);

                    this.Logger.DebugFormat("State after packet {0}", TimeStopManager.Instance.IsTimeStopped);
                    break;

                case PacketType.StandChanged:
                    playerNumber = reader.ReadByte();
                    name = reader.ReadString();
                    plr = TBARPlayer.Get(Main.player[playerNumber]);

                    plr.PlayerStand = StandFactory.Instance.Get(name);

                    if (IsServer)
                    {
                        plr.PlayerStand = StandFactory.Instance.Get(name);
                        plr.SendStandChangedPacket(playerNumber);
                    }
                    break;
            }
        }
    }
}
