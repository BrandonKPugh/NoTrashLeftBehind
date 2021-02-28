using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NoTrashLeftBehind
{
    public class SpriteAnimationData
    {
        private TimeSpan timer;
        public int Frame;
        private int frames;
        TimeSpan frameTime;
        public SpriteAnimationData(int Frames, int MillisPerFrame)
        {
            frames = Frames;
            frameTime = new TimeSpan(0, 0, 0, 0, MillisPerFrame);
            timer = new TimeSpan(0);
        }



        public void Update(GameTime gameTime)
        {
            // Could optionally account for gameTime delay here to potentially skip animation frames when stuttering
            timer = timer.Add(gameTime.ElapsedGameTime);
            if (timer > frameTime)
            {
                timer -= frameTime;
                Frame++;
            }
            if(Frame >= frames)
            {
                Frame = 0;
            }

        }

        public void ResetAnimation()
        {
            Frame = 0;
        }
    }
}
