using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;

namespace TBAR.Stands
{
    public class StandFactory
    {
        private StandFactory() { }

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

        public void Unload()
        {
            Stands.Clear();
        }

        // Will keep rolling until it finds a diffrent stand
        public Stand GetNewRandom(Stand stand)
        {
            Stand resultStand = GetRandom();

            if (stand == null || StandCount <= 1)
                return resultStand;

            while (stand.GetType() == resultStand.GetType())
                resultStand = GetRandom();

            return resultStand;
        }

        public Stand GetRandom()
        {
            return (Stand)Stands[Main.rand.Next(StandCount)].Clone();
        }

        public int StandCount => Stands.Count;

        public List<Stand> Stands { get; } = new List<Stand>();

        private static StandFactory _instance;
        public static StandFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new StandFactory();

                return _instance;
            }
        }
    }
}
