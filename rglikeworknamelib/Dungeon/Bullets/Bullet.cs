﻿using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Particles;

namespace rglikeworknamelib.Dungeon.Bullets {
    public class Bullet {
        internal int Mtex;

        internal Vector2 Pos;
        internal Vector2 Start;
        internal float Vel;
        internal float Angle, ASpeed;
        internal TimeSpan Life;
        private ParticleSystem ps;
        private Random rnd = new Random();

        internal Vector3 Velocity;

        public Bullet(Vector2 pos, float vel, float an, float asp, int texn, TimeSpan ts) {
            Pos = pos;
            Vel = vel;
            ASpeed = asp;
            Angle = an;
            Mtex = texn;
            Life = ts;
            Start = pos;
        }

        /// <summary>
        /// Get position in blocks
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPositionInBlocks()
        {
            var po = Pos;
            po.X = po.X < 0 ? po.X / 32 - 1 : po.X / 32;
            po.Y = po.Y < 0 ? po.Y / 32 - 1 : po.Y / 32;
            return po;
        }

        public void Kill() {}

        public void Update(GameTime gameTime) {
            
        }
    }
}