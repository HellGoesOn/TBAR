using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TBAR.Projectiles.Stands.Crusaders.Chariot
{
    public class ChariotProjectile : PunchGhostProjectile
    {
        public ChariotProjectile() : base("Silver Chariot")
        {
        }

        protected override string PunchState => throw new NotImplementedException();

        public override void InitializeStates(Projectile projectile)
        {
        }
    }
}
