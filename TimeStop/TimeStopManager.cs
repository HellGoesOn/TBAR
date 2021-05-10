using System;
using System.Collections.Generic;
using TBAR.Enums;
using TBAR.NPCs;
using Terraria;
using Terraria.ID;

namespace TBAR.TimeStop
{
    public class TimeStopManager
    {
        private TimeStopManager()
        {
            timeStops = new List<TimeStopInstance>();
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
                timeStops.Add(new TimeStopInstance(owner, duration, soundPath));
            else
            {
                FindAndRemoveInstance(owner);
            }
        }

        public void TryStopTime(TimeStopInstance instance)
        {
            Entity owner = instance.Owner;

            if (!HaveITimeStopped(owner))
                timeStops.Add(instance);
            else
            {
                FindAndRemoveInstance(owner);
            }
        }

        public void ForceStop(TimeStopInstance instance)
        {
            timeStops.Add(instance);
        }

        private void FindAndRemoveInstance(Entity owner)
        {
            int myTimeStopIndex = timeStops.FindIndex(x => x.Owner == owner);

            int preRemoveCount = timeStops.Count;

            if (timeStops.Count - 1 == 0)
            {
                HasOrderToRestore = true;
                TBAR.Instance.PlaySound(timeStops[0].EndSoundEffect);
            }

            if (timeStops.Count > 0)
                timeStops.RemoveAt(myTimeStopIndex);
        }

        public void Update()
        {
            if (Main.gameMenu && Main.netMode == NetmodeID.SinglePlayer)
                timeStops.Clear();

            if (HasOrderToRestore)
                RestoreOrder();

            if (IsTimeStopped)
            {
                Main.windSpeed = 0;
                Main.time--;
            }

            for (int i = timeStops.Count - 1; i >= 0; i--)
            {
                if (--timeStops[i].Duration <= 0)
                {
                    if (i == 0)
                    {
                        HasOrderToRestore = true;
                        TBAR.Instance.PlaySound(timeStops[0].EndSoundEffect);
                    }
                    timeStops.RemoveAt(i);
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

        private readonly List<TimeStopInstance> timeStops;

        public bool HaveITimeStopped(Entity e) => timeStops.Find(x => x.Owner == e) != null;

        public bool IsTimeStopped => timeStops.Count > 0;

        public bool HasOrderToRestore { get; set; }

        public static void Unload()
        {
            instance = null;
        }

        public TimeStopInstance FindInstance(Predicate<TimeStopInstance> predicate)
        {
            return timeStops.FindLast(predicate);
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
