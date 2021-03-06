using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    [Serializable]
    public class Block : IBlock {
        public string Id { get; set; }
        public Color Lightness { get; set; }
        public bool Explored { get; set; }
        public string Mtex { get; set; }

        public static string GetSmartActionName(SmartAction smartAction)
        {
            switch (smartAction) {
                case SmartAction.ActionOpenContainer:
                    return "��������� ����������";
                case SmartAction.ActionOpenClose:
                    return "�������/�������";
                default:
                    return "���������";
            }
        }

        public void SetLight(Color color) {
            Lightness = color;
        }

        public virtual void Draw(SpriteBatch sb, Dictionary<string, Texture2D> batlas, Vector2 vector2) {
            Color light = Lightness;
            if (Explored && light == Color.Black)
            {
                light = new Color(40, 40, 40);
            }

            sb.Draw(batlas[Mtex],vector2, light);
        }

        public virtual void Update(TimeSpan ts, Vector2 vector2)
        {
            
        }
    }
}