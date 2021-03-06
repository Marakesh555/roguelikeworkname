using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using jarg;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Particles;

namespace rglikeworknamelib.Dungeon.Bullets {
    public class BulletSystem {
        private readonly Collection<Bullet> bullet_;
        private readonly Collection<Texture2D> _bulletAtlas;
        private readonly SpriteBatch _spriteBatch;
        private ParticleSystem p;
        private GameLevel level;
        private SpriteFont font;
        private LineBatch lb ;

        public BulletSystem(SpriteBatch spr, Collection<Texture2D> atlas, GameLevel gl, SpriteFont fnt, LineBatch l) {
            _bulletAtlas = atlas;
            _spriteBatch = spr;
            bullet_ = new Collection<Bullet>();
            level = gl;
            font = fnt;
            lb = l;
        }

        public void AddBullet(Vector2 pos, float vel, float an) {
            bullet_.Add(new Bullet(pos, vel, an, 0, 1, TimeSpan.FromSeconds(1)));
        }

        public void AddBullet(Creature who, float vel, float an)
        {
            bullet_.Add(new Bullet(who.Position, vel, an, 0, 1, TimeSpan.FromSeconds(1)));
        }

        public void Update(GameTime gameTime) {
            for (int i = 0; i < bullet_.Count; i++) {
                var bullet = bullet_[i];
                var a = bullet;
                bullet.Update(gameTime);
                float f = a.Vel * (float)Math.Cos(a.Angle);
                float f1 = a.Vel * (float)Math.Sin(a.Angle);

                bullet.Pos += new Vector2(f, f1);

                bullet.Angle += bullet.ASpeed;

                bullet.Life -= gameTime.ElapsedGameTime;

                bullet.Start += new Vector2(f/2, f1/2);

                var bl = level.GetBlock((int) bullet.GetPositionInBlocks().X, (int) bullet.GetPositionInBlocks().Y);
                if(!BlockDataBase.Data[bl.Id].IsWalkable) {
                    bullet.Life = TimeSpan.Zero;
                }
               
                var sect = level.GetCreatureAtCoord(bullet.Pos, bullet.Start);
                var crea = level.GetCreatureSector(bullet.Pos, bullet.Start);

                if (sect != null) {
                    sect.GiveDamage(20, DamageType.Default, crea);
                    if (sect.isDead) {

                    }

                    bullet.Life = TimeSpan.Zero;
                }

                if (bullet.Life <= TimeSpan.Zero)
                {
                    bullet_.Remove(bullet);
                    continue;
                }
            }
        }


        public void Draw(GameTime gameTime, Vector2 cam) {
            bool b = Settings.DebugInfo;
            foreach (Bullet bullet in bullet_) {
                _spriteBatch.Draw(_bulletAtlas[bullet.Mtex],
                                  bullet.Pos - cam, null,
                                  Color.White, 0,
                                  new Vector2(_bulletAtlas[bullet.Mtex].Width / 2,
                                              _bulletAtlas[bullet.Mtex].Height / 2), 1, SpriteEffects.None, 0);

                 lb.AddLine(bullet.Start-cam, bullet.Pos-cam, Color.Yellow, 2);
                
                if (b) {
                    _spriteBatch.DrawString(font, bullet.Pos.ToString(), bullet.Pos - cam, Color.White);
                }
            }
        }

        public int GetCount() {
            return bullet_.Count;
        }
    }
}