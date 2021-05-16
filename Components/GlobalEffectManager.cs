using System;
using System.Collections.Generic;

namespace TBAR.Components
{
    public abstract class GlobalEffectManager<T>
        where T : IGlobalEffect 
    {
        public GlobalEffectManager()
        {
            effects = new List<T>();
        }

        /// <summary>
        /// Updates effects
        /// </summary>
        /// <param name="predicate">Criteria which determines which instances should be removed</param>
        public void Update(Predicate<T> predicate)
        {
            foreach(IGlobalEffect effect in effects)
            {
                effect.Update();
            }

            effects.RemoveAll(predicate);
        }

        public void AddEffect(T effect)
        {
            effects.Add(effect);
        }

        public IGlobalEffect GetEffect(Predicate<T> predicate)
        {
            return effects.Find(predicate);
        }

        public IGlobalEffect GetLastEffect(Predicate<T> predicate)
        {
            return effects.FindLast(predicate);
        }

        public void RemoveEffect(Predicate<T> predicate) => effects.Remove((T)GetEffect(predicate));

        public void RemoveEffectAt(int index) => effects.RemoveAt(index);

        public void ClearEffects() => effects.Clear();

        public int GetIndex(Predicate<T> predicate) => effects.FindIndex(predicate);

        public int EffectCount => effects.Count;

        protected readonly List<T> effects;
    }
}
