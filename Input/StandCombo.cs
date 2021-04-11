using System.Collections.Generic;
using System;
using Terraria;

namespace TBAR.Input
{
    public class StandCombo
    {
        public event ComboEventHandler OnActivate;

        public StandCombo(string Name, params ComboInput[] inputs)
        {
            ComboName = Name;
            
            foreach(ComboInput i in inputs)
                RequiredInputs.Add(i);
        }

        public bool TryActivate(List<ComboInput> inputs, bool force = false)
        {
            if (force)
            {
                OnActivate?.Invoke();
                return true;
            }
            // if received input count is lower, we won't be able to activate the combo
            if (inputs.Count < RequiredInputs.Count)
                return false;

            // find diffrence so we only check the last "answer"
            // e.g: we press A1-A1-Up-Down; combo is just Up-Down
            // it will skip over A1-A1 and only check the last 2 inputs
            int dif = Math.Abs(RequiredInputs.Count - inputs.Count);

            for (int i = 0 + dif; i < inputs.Count - 1; i++)
            {
                if (inputs[i] != RequiredInputs[i-dif])
                    return false;
            }

            OnActivate?.Invoke();

            return true;
        }

        public string ComboName { get; }

        public List<ComboInput> RequiredInputs { get; } = new List<ComboInput>();
    }
}
