using System.Collections.Generic;
using TBAR.Components;
using TBAR.Enums;
using TBAR.Input;
using TBAR.Players;
using TBAR.UI.Elements;
using Terraria;

namespace TBAR.Stands
{
    public abstract class Stand
    {
        protected Stand(string name)
        {
            StandName = name;
            GlobalCombos = new List<StandCombo>();
            NormalCombos = new List<StandCombo>();
            InitializeCombos();
            SortCombos(NormalCombos);
            SortCombos(GlobalCombos);
        }

        public abstract void TryActivate(Player player);

        /// <summary>
        /// Use to initialize & add combos via AddNormalCombos & AddGlobalCombos
        /// </summary>
        public abstract void InitializeCombos();

        public abstract void HandleInputs(Player player, List<ComboInput> receivedInputs);

        public virtual void HandleImmediateInputs(Player player, ImmediateInput input)
        {

        }

        public abstract void KillStand();

        public abstract void Update();

        public void SortCombos(List<StandCombo> list)
        {
            if (list.Count <= 0)
                return;

            if (list.Count > 1)
            {
                list.Sort(
                    delegate (StandCombo x, StandCombo y)
                    {
                        if (x.RequiredInputs.Count > y.RequiredInputs.Count)
                            return -1;
                        else
                            return 1;
                    }
                );
            }
        }

        public bool IsActive { get; set; }

        public virtual void ForceCombo(string name, Player player)
        {
            StandCombo tryGlobalCombo = GlobalCombos.Find(x => x.ComboName == name);

            StandCombo tryNormalCombo = NormalCombos.Find(x => x.ComboName == name);

            if (tryNormalCombo != null)
            {
                tryNormalCombo.ForceActivate(player);
                return;
            }

            if (tryGlobalCombo != null)
            {
                tryGlobalCombo.ForceActivate(player);
                return;
            }
        }

        /// <summary>
        /// Adds combos that can be used any time.
        /// Normal combos take priority over Global
        /// </summary>
        /// <param name="combos"></param>
        public void AddGlobalCombos(params StandCombo[] combos)
        {
            foreach (StandCombo s in combos)
                GlobalCombos.Add(s);
        }

        /// <summary>
        /// Adds combos that can only be used when stand is active
        /// </summary>
        /// <param name="combos"></param>
        public void AddNormalCombos(params StandCombo[] combos)
        {
            foreach (StandCombo s in combos)
                NormalCombos.Add(s);
        }

        public virtual string GetDamageScalingText => "???";

        public virtual string GetEffectiveRangeText => "???";

        public List<StandCombo> GlobalCombos { get; }

        public List<StandCombo> NormalCombos { get; }
        
        public string StandName { get; }

        public virtual bool CanAcquire(TBARPlayer player) => true;

        public abstract SpriteAnimation AlbumEntryAnimation();

        public virtual DamageType StandDamageType => DamageType.Any;
    }
}
