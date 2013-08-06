using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Particles;

namespace rglikeworknamelib.Creatures {
    [Serializable]
    public class Creature : ICreature {
        public string Id { get; set; }

        private Vector2 lastpos_;
        private Vector2 remPos;

        private Vector2 sectoroffset;
        internal Color col;

        internal float percenter = (float)Settings.rnd.NextDouble()/5f + 0.8f;

        public bool IsWarmCloth() {
            return true;
        }

        public Stat Hunger  = new Stat(100);

        public Stat Thirst = new Stat(100);

        public Stat Heat = new Stat(36);

        internal Vector2 position_;
        public Vector2 Position  {
            get { return position_; }
            set {
                lastpos_ = position_; 
                position_ = value;
            }
        }

        internal Stat hp_ = new Stat(200);
        public Stat Hp {
            get { return hp_; }
            set { hp_ = value; }
        }

        public bool isDead { get; internal set; }

        public void SetPositionInBlocks(int x, int y)
        {
            position_ = new Vector2((x + 0.5f) * 32, (y + 0.5f) * 32);
        }

        /// <summary>
        /// Returns creature position in game blocks
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPositionInBlocks() {
            var po = Position;
            po.X = po.X < 0 ? po.X / 32 - 1 : po.X / 32;
            po.Y = po.Y < 0 ? po.Y / 32 - 1 : po.Y / 32;
            return po;
        }

        /// <summary>
        /// Returns creature position in game blocks
        /// </summary>
        /// <returns></returns>
        public Vector2 GetWorldPositionInBlocks()
        {
            var po = WorldPosition();
            po.X = po.X < 0 ? po.X / 32 - 1 : po.X / 32;
            po.Y = po.Y < 0 ? po.Y / 32 - 1 : po.Y / 32;
            return po;
        }

        public event EventHandler onDamageRecieve;

        private List<IBuff> buffs_ = new List<IBuff>();
        public List<IBuff> buffs 
        {
            get
            {
                return buffs_;
            }
            set
            {
                buffs_ = value;
            }
        }

        public Vector2 LastPos
        {
            get { return lastpos_; }
            set { lastpos_ = value; }
        }

        private TimeSpan sec = TimeSpan.Zero;
        private TimeSpan reactionT = TimeSpan.Zero;
        public virtual void Update(GameTime gt, MapSector ms, Player hero) {
            var time = gt.ElapsedGameTime.TotalSeconds;
            reactionT += gt.ElapsedGameTime;
            sec += gt.ElapsedGameTime;
            if (!Skipp) {
                sectoroffset = new Vector2(ms.SectorOffsetX, ms.SectorOffsetY);
                if (ms.Parent.GetBlock((int) GetWorldPositionInBlocks().X, (int) GetWorldPositionInBlocks().Y).Lightness ==
                    Color.White && reactionT.TotalMilliseconds > MonsterDataBase.Data[Id].ReactionTime) {
                    remPos = hero.Position - WorldPosition() + Position;
                    MoveByMover(ms, time);

                    col = Color.White;
                    if(sec.TotalSeconds > 1 && ms.Parent.IsCreatureMeele(hero, this)) {
                        hero.GiveDamage(MonsterDataBase.Data[Id].Damage, DamageType.Default, ms);
                        sec = TimeSpan.Zero;
                    }
                }
                else {
                    col = Color.Black;
                    MoveByMover(ms, time);
                }

                if (Position.Y >= 1024) {
                    var t = ms.Parent.GetDownN(ms.SectorOffsetX, ms.SectorOffsetY);
                    if (t != null) {
                        ms.creatures.Remove(this);
                        t.creatures.Add(this);
                        position_.Y = position_.Y - 1024;
                        sectoroffset = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else {
                        position_.Y = 1023;
                    }
                }
                else if (Position.Y < 0) {
                    var t = ms.Parent.GetUpN(ms.SectorOffsetX, ms.SectorOffsetY);
                    if (t != null) {
                        ms.creatures.Remove(this);
                        t.creatures.Add(this); 
                        position_.Y = position_.Y + 1024;
                        sectoroffset = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else {
                        position_.Y = 0;
                    }
                }
                if (Position.X >= 1024) {
                    var t = ms.Parent.GetRightN(ms.SectorOffsetX, ms.SectorOffsetY);
                    if (t != null) {
                        ms.creatures.Remove(this);
                        t.creatures.Add(this);
                        position_.X = position_.X - 1024;
                        sectoroffset = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else {
                        position_.X = 1023;
                    }
                }
                else if (Position.X < 0) {
                    
                    var t = ms.Parent.GetLeftN(ms.SectorOffsetX, ms.SectorOffsetY);
                    if (t != null) {
                        ms.creatures.Remove(this);
                        t.creatures.Add(this);
                        position_.X = position_.X + 1024;
                        sectoroffset = new Vector2(t.SectorOffsetX, t.SectorOffsetY);
                    }
                    else {
                        position_.X = 0;
                    }
                }
            }

            foreach (var buff in buffs) {
                buff.Update(gt);
            }
        }

        private void MoveByMover(MapSector ms, double time) {
            if (remPos != Vector2.Zero && ((int)remPos.X != (int)position_.X || (int)remPos.Y != (int)position_.Y))
            {
                var mover = - position_ + remPos;
                var percenterMax = MonsterDataBase.Data[Id].Speed*percenter;

                if (mover.Length() > time * percenterMax)
                {
                    mover.Normalize();
                    mover *= (float)time * percenterMax;
                }

                var newwposx = GetWorldPositionInBlocks() + new Vector2(mover.X, 0);
                var newwposy = GetWorldPositionInBlocks() + new Vector2(0, mover.Y);

                if (BlockDataBase.Data[ms.Parent.GetBlock((int) newwposx.X, (int) newwposx.Y).Id].IsWalkable) {
                    position_.X += mover.X;
                }
                if (BlockDataBase.Data[ms.Parent.GetBlock((int) newwposy.X, (int) newwposy.Y).Id].IsWalkable) {
                    position_.Y += mover.Y;
                }
            }
        }


        public Vector2 ShootPoint { get; set; }

        public Vector2 Velocity;
        public bool Skipp { get; set; }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 camera, MapSector ms) {
            var a = GetPositionInBlocks();
            var p = WorldPosition() - camera;
            spriteBatch.Draw(Atlases.CreatureAtlas[MonsterDataBase.Data[Id].MTex], p+new Vector2(16,-32), col);
            if (Settings.DebugInfo) {
                spriteBatch.DrawString(Settings.Font, position_.ToString(), p, Color.White);
            }
        }

        public Vector2 WorldPosition() {
            return Position + new Vector2(-16, -32) +
                   new Vector2(sectoroffset.X*MapSector.Rx*32, sectoroffset.Y*MapSector.Ry*32);
        }

        public virtual void Kill(MapSector ms) {
            Hp = new Stat(0,0);
            isDead = true;
            ms.AddDecal(new Particle(WorldPosition(), 3) { Rotation = -3.14f/2, Life = new TimeSpan(0, 0, 1, 0) });
        }

        public void GiveDamage(float value, DamageType type, MapSector ms) {

            hp_.Current -= value;
            EventLog.Add(string.Format("{0} �������� {1} �����", MonsterDataBase.Data[Id].Name, value), GlobalWorldLogic.CurrentTime, Color.Pink, LogEntityType.Damage);
            if(Hp.Current <= 0 ) {
                Kill(ms);
                EventLog.Add(string.Format("{0} �������!", MonsterDataBase.Data[Id].Name.ToUpper()), GlobalWorldLogic.CurrentTime, Color.Pink, LogEntityType.Dies);
            }
            Vector2 adder = new Vector2(Settings.rnd.Next(-10, 10), Settings.rnd.Next(-10, 10));

            ms.AddDecal(new Particle(WorldPosition() + adder, 3)
                        {Rotation = Settings.rnd.Next()%360, Life = new TimeSpan(0, 0, 1, 0)});

            if(onDamageRecieve != null) {
                onDamageRecieve(null, null);
            }
        }

        public void GiveDamage(float value, MapSector ms)
        {
            GiveDamage(value, DamageType.Default, ms);
        }
    }
}