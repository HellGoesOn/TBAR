using System;
using System.Collections.Generic;
using System.Linq;
using TBAR.Components;
using TBAR.Enums;
using TBAR.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;

namespace TBAR.TimeStop
{
    public class TimeStopManager : GlobalEffectManager<TimeStopInstance>
    {
        private Entity GetEntity(EntityType type, int index)
        {
            switch(type)
            {
                case EntityType.Player:
                    return Main.player[index];

                case EntityType.Item:
                    return Main.item[index];

                case EntityType.Npc:
                    return Main.npc[index];

                case EntityType.Projectile:
                    return Main.projectile[index];
                default:
                    throw new System.Exception("Unknown entity type passed");
            }
        }

        public void TryStopTime(EntityType type, int index, int duration, string soundPath = "")
        {
            Entity owner = GetEntity(type, index);

            if (!HaveITimeStopped(owner))
                AddEffect(new TimeStopInstance(owner, duration, soundPath));
        }

        public void TryStopTime(TimeStopInstance instance)
        {
            Entity owner = instance.Owner();

            if (!HaveITimeStopped(owner))
                AddEffect(instance);
        }

        public void ForceStop(TimeStopInstance instance)
        {
            AddEffect(instance);
        }

        private void FindAndRemoveInstance(Entity owner)
        {
            int myTimeStopIndex = GetIndex(x => x.Owner() == owner);

            int preRemoveCount = EffectCount;

            if (EffectCount - 1 == 0)
            {
                HasOrderToRestore = true;
                TBAR.Instance.PlaySound(effects[0].EndSoundEffect);
            }

            if (EffectCount > 0)
                RemoveEffectAt(myTimeStopIndex);
        }

        public void Update()
        {
            if (Main.gameMenu && Main.netMode == NetmodeID.SinglePlayer)
                ClearEffects();

            if (HasOrderToRestore)
                RestoreOrder();

            if (IsTimeStopped)
            {
                Main.windSpeed = 0;
                Main.time--;
            }

            for (int i = EffectCount - 1; i >= 0; i--)
            {
                if (--effects[i].Duration <= 0)
                {
                    if (i == 0)
                    {
                        HasOrderToRestore = true;
                        TBAR.Instance.PlaySound(effects[0].EndSoundEffect);
                    }

                    if(Main.dedServ)
                    {
                        ModPacket packet = TBAR.Instance.GetPacket();
                        packet.Write((byte)PacketType.RemoveTimeStopInstance);
                        packet.Write((int)i);
                        packet.Send();
                    }

                    if(Main.netMode != NetmodeID.MultiplayerClient)
                        RemoveEffectAt(i);
                }
            }

            /*if (Main.dedServ)
                TBAR.Instance.Logger.Debug($"IsTimeStopped: {IsTimeStopped}");*/
        }

        public void RestoreOrder()
        {
            // if we are playing as a client, then only the server should handle this
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active)
                    continue;

                TBARGlobalNPC.RestoreAction(npc);
            }

            HasOrderToRestore = false;
        }

        /*public void SendPacket(EntityType myType, int myIndex, int duration, int ignore = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            ModPacket packet = TBAR.Instance.GetPacket();

            packet.Write((byte)PacketType.TimeStopServer);
            packet.Write((byte)myType);
            packet.Write((int)myIndex);
            packet.Write((int)duration);

            packet.Send(-1, ignore);
        }*/

        public bool IsMyTeamImmune(Player player)
        {
            if (effects.Count(x => x.Owner() is Player plr && plr.team == player.team && plr.team != (int)Team.None) > 0)
                return true;

            return false;
        }

        public bool HaveITimeStopped(Entity e) => GetEffect(x => x.Owner() == e) != null;

        public bool IsTimeStopped => EffectCount > 0;

        public bool HasOrderToRestore { get; set; }

        public TimeStopInstance FindInstance(Predicate<TimeStopInstance> predicate)
        {
            return (TimeStopInstance)GetLastEffect(predicate);
        }
    }
}
