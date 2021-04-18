﻿using System;
using System.Collections.Generic;
using TBAR.Components;
using TBAR.Enums;
using TBAR.Input;
using TBAR.Players;
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
            AddCombos();
            SortCombos(NormalCombos);
            SortCombos(GlobalCombos);
        }

        public abstract void TryActivate(Player player);

        public abstract void AddCombos();

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
            TBAR.Instance.Logger.Debug(name + " was subject to force");
            StandCombo tryGlobalCombo = GlobalCombos.Find(x => x.ComboName == name);
            TBAR.Instance.Logger.Debug(tryGlobalCombo + " is result of globalCombo search");

            StandCombo tryNormalCombo = NormalCombos.Find(x => x.ComboName == name);
            TBAR.Instance.Logger.Debug(tryNormalCombo + " is result of normalCombo search");

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

        public List<StandCombo> GlobalCombos { get; }

        public List<StandCombo> NormalCombos { get; }
        
        public string StandName { get; }

        public virtual bool CanAcquire(TBARPlayer player) => true;

        public abstract SpriteAnimation AlbumEntryAnimation();
    }
}
