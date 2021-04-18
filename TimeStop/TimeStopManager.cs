using System;
using System.Collections.Generic;
using TBAR.Enums;
using TBAR.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TBAR.TimeStop
{
    public class TimeStopManager
    {
        private TimeStopManager()
        {
            TimeStops = new List<TimeStopInstance>();
        }

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
                TimeStops.Add(new TimeStopInstance(owner, duration, soundPath));
            else
            {
                FindAndRemoveInstance(owner);
            }
        }

        public void TryStopTime(TimeStopInstance instance)
        {
            Entity owner = instance.Owner;

            if (!HaveITimeStopped(owner))
                TimeStops.Add(instance);
            else
            {
                FindAndRemoveInstance(owner);
            }
        }

        private void FindAndRemoveInstance(Entity owner)
        {
            int myTimeStopIndex = TimeStops.FindIndex(x => x.Owner == owner);

            if (TimeStops.Count == 1)
            {
                HasOrderToRestore = true;
                TBAR.Instance.PlaySound(TimeStops[0].EndSoundEffect);
            }

            TimeStops.RemoveAt(myTimeStopIndex);
        }

        public void Update()
        {
            if (Main.gameMenu && Main.netMode == NetmodeID.SinglePlayer)
                TimeStops.Clear();

            if (HasOrderToRestore)
                RestoreOrder();

            for (int i = TimeStops.Count - 1; i >= 0; i--)
            {
                if (--TimeStops[i].Duration <= 0)
                {
                    if (i == 0)
                    {
                        HasOrderToRestore = true;
                        TBAR.Instance.PlaySound(TimeStops[0].EndSoundEffect);
                    }
                    TimeStops.RemoveAt(i);
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

        public List<TimeStopInstance> TimeStops { get; }

        public bool HaveITimeStopped(Entity e) => TimeStops.Find(x => x.Owner == e) != null;

        public bool IsTimeStopped => TimeStops.Count > 0;

        public bool HasOrderToRestore { get; set; }

        public static void Unload()
        {
            instance = null;
        }

        private static TimeStopManager instance;
        public static TimeStopManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new TimeStopManager();

                return instance;
            }
        }
    }
}
