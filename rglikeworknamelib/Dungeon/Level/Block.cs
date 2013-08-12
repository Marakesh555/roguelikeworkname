using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class Block {
        public int Id;
        public Color Lightness;
        public bool Explored;

        public int Mtex;

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
    }
}