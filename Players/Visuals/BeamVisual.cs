using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TBAR.Players.Visuals
{
    public class BeamVisual
    {
        public static BeamVisual AddBeamVisual(Player player, int width = 20, int height = 1200, int duration = 60, int openTime = 20, Color color = default)
        {
            TBARPlayer tbr = TBARPlayer.Get(player);

            if (color == default)
                color = Color.PaleVioletRed;

            var beam = new BeamVisual(width, height, duration, openTime, color);

            tbr.BeamVisuals.Add(beam);

            return beam;
        }

        protected BeamVisual(int width, int height, int duration, int openTime, Color color)
        {
            Width = width;
            Height = height;
            TimeLeft = duration;
            OpenTimeMax = openTime;
            BeamColor = color;

            OpenTime = OpenTimeMax;
        }

        public void Update()
        {
            if (TimeLeft <= 0)
                return;

            if (OpenTime > 0 && TimeLeft > OpenTimeMax)
                OpenTime--;
            else if (TimeLeft <= OpenTimeMax)
                OpenTime++;

            TimeLeft--;
        }

        public int OpenTime { get; set; }

        public Color BeamColor { get; }

        public int Width { get; }

        public int Height { get; }

        public int TimeLeft { get; private set; }

        public int OpenTimeMax { get; }
    }
}
