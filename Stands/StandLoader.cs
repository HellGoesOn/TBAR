using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TBAR.Players;
using Terraria;

namespace TBAR.Stands
{
    public class StandLoader
    {
        private StandLoader()
        {
            Stands = new List<Stand>();
        }

        public void Load()
        {
            foreach (Type type in Assembly.GetAssembly(typeof(Stand)).GetTypes()
                .Where(myType => myType.IsClass
                && !myType.IsAbstract
                && myType.IsSubclassOf(typeof(Stand))))
            {
                Stands.Add((Stand)Activator.CreateInstance(type));
            }
        }

        public static void Unload()
        {
            Instance.Stands.Clear();
            Instance.Stands = null;
            _instance = null;

        }

        // Will keep rolling until it finds a diffrent stand
        public Stand GetNewRandom(TBARPlayer player)
        {
            Stand stand = player.PlayerStand;

            Stand resultStand = GetRandom();

            if (stand == null || StandCount <= 1)
                return resultStand;

            while (stand.GetType() == resultStand.GetType() || !stand.CanAcquire(player))
                resultStand = GetRandom();

            return resultStand;
        }

        public Stand GetRandom()
        {
            Type type = Stands[Main.rand.Next(StandCount)].GetType();

            return (Stand)Activator.CreateInstance(type);
        }

        public Stand Get(string name)
        {
            if (name == "None")
                return null;

            Stand attempt = Stands.Find(x => x.StandName == name);

            if (attempt != null)
                return (Stand)Activator.CreateInstance(attempt.GetType());

            foreach (Stand s in Stands)
            {
                if (s.GetType().Name == name)
                    return (Stand)Activator.CreateInstance(s.GetType());
            }

            throw new Exception("Invalid stand name");
        }

        public int StandCount => Stands.Count;

        public List<Stand> Stands { get; private set; }

        private static StandLoader _instance;
        public static StandLoader Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new StandLoader();

                return _instance;
            }
        }
    }
}
