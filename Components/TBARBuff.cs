using System;
using TBAR.NPCs;
using Terraria;

namespace TBAR.Components
{
    public abstract class TBARBuff
    {
        public static TBARBuff AddToNpc<T>(NPC target, int duration, Entity inflictor = null, BuffAddStyle style = BuffAddStyle.Default) where T : TBARBuff
        {
            TBARGlobalNPC globalNPC = TBARGlobalNPC.Get(target);

            TBARBuff buff = (TBARBuff)Activator.CreateInstance(typeof(T));

            if (inflictor != null)
                buff.Owner = inflictor;

            buff.Initialize();
            buff.Duration = duration;

            TBARBuff temp = null;

            switch (style)
            {
                case BuffAddStyle.Default:
                    globalNPC.CustomBuffs.Add(buff);
                    break;

                case BuffAddStyle.Replace:
                    temp = globalNPC.CustomBuffs.Find(x => x is T);

                    if(temp != null)
                        globalNPC.CustomBuffs.Remove(temp);

                    globalNPC.CustomBuffs.Add(buff);
                    break;

                case BuffAddStyle.Extend:
                    temp = globalNPC.CustomBuffs.Find(x => x is T);

                    if (temp != null)
                    {
                        temp.Duration += duration;
                        return temp;
                    }
                    else
                        globalNPC.CustomBuffs.Add(buff);
                    break;
            }

            return buff;
        }

        public static TBARBuff AddToNpcIf<T>(NPC target, int duration, Predicate<TBARBuff> predicate, Entity inflictor = null, BuffAddStyle style = BuffAddStyle.Default) where T : TBARBuff
        {
            TBARGlobalNPC globalNPC = TBARGlobalNPC.Get(target);

            TBARBuff buff = (TBARBuff)Activator.CreateInstance(typeof(T));

            predicate += x => x is T;

            if (inflictor != null)
                buff.Owner = inflictor;

            buff.Initialize();
            buff.Duration = duration;

            TBARBuff temp = null;

            switch (style)
            {
                case BuffAddStyle.Default:
                    globalNPC.CustomBuffs.Add(buff);
                    break;

                case BuffAddStyle.Replace:
                    temp = globalNPC.CustomBuffs.Find(predicate);

                    if (temp != null)
                        globalNPC.CustomBuffs.Remove(temp);

                    globalNPC.CustomBuffs.Add(buff);
                    break;

                case BuffAddStyle.Extend:
                    temp = globalNPC.CustomBuffs.Find(predicate);

                    if (temp != null)
                    {
                        temp.Duration += duration;
                        return temp;
                    }
                    globalNPC.CustomBuffs.Add(buff);
                    break;
            }

            return buff;
        }

        public static TBARBuff ExtendForNpc<T>(NPC target, int duration) where T : TBARBuff
        {
            TBARGlobalNPC globalNPC = TBARGlobalNPC.Get(target);

            TBARBuff buff = globalNPC.CustomBuffs.Find(x => x is T);

            if(buff != null)
                buff.Duration += duration;

            return buff;
        }

        public static TBARBuff ExtendForNpcIf<T>(NPC target, int duration, Predicate<TBARBuff> predicate) where T : TBARBuff
        {
            TBARGlobalNPC globalNPC = TBARGlobalNPC.Get(target);

            predicate += x => x is T;

            TBARBuff buff = globalNPC.CustomBuffs.Find(predicate);

            if (buff != null)
                buff.Duration += duration;

            return buff;
        }

        public abstract void Initialize();

        public virtual string Name => GetType().Name;

        public virtual void UpdateNPC(NPC n) { }

        public virtual void UpdatePlayer(Player p) { }

        public int Duration { get; set; }

        public Entity Owner { get; set; }
    }

    public enum BuffAddStyle
    {
        /// <summary>
        /// Adds buff with no additional logic
        /// </summary>
        Default,
        /// <summary>
        /// Replaces existing buff with a new instance
        /// </summary>
        Replace,
        /// <summary>
        /// Increases the duration of the buff if the target already has it
        /// </summary>
        Extend
    }
}
